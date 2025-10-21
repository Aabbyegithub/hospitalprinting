using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Memory;
using FellowOakDicom.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServiceClass.Helper;

namespace WebServiceClass.Services.DICOMServices
{
    public class DicomSCPServices : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DicomSCPServices> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IDicomServer? _server;
        private readonly string _outputDirectory;
        private readonly string _aeTitle;
        private readonly int _port;
        private readonly string _ipAddress;
        private readonly bool _enabled;
        
        private static IServiceProvider? _staticServiceProvider;

        public DicomSCPServices(IConfiguration configuration, ILogger<DicomSCPServices> logger, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _staticServiceProvider = serviceProvider;
            
            // 从配置文件读取设置
            _enabled = _configuration.GetValue<bool>("SCPService:Enabled");
            _aeTitle = _configuration.GetValue<string>("SCPService:AETitle") ?? "HOSPITAL_SCP_AE";
            _port = _configuration.GetValue<int>("SCPService:Port");
            _ipAddress = _configuration.GetValue<string>("SCPService:IPAddress") ?? "0.0.0.0";
            
            // 设置默认端口
            if (_port == 0)
                _port = 104;
            
            // 初始化输出目录
            _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReceivedDICOMs");
            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_enabled)
            {
                _logger.LogInformation("DICOM SCP服务已禁用");
                return Task.CompletedTask;
            }

            try
            {
                // 使用简化的DicomServer.Create方法
                _server = DicomServerFactory.Create<DicomScp>(_ipAddress, _port);
                
                // 更新服务状态
                DicomServiceStatus.Instance.IsServiceRunning = true;
                DicomServiceStatus.Instance.ServiceStartTime = DateTime.Now;
                
                _logger.LogInformation($"DICOM SCP服务已启动 - AE Title: {_aeTitle}, Port: {_port}, IP: {_ipAddress}");
                _logger.LogInformation($"DICOM文件保存目录: {_outputDirectory}");
                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动DICOM SCP服务时发生错误");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_server != null)
            {
                _server.Stop();
                
                // 更新服务状态
                DicomServiceStatus.Instance.IsServiceRunning = false;
                
                _logger.LogInformation("DICOM SCP服务已停止");
            }
        }

        // 自定义 SCP 处理类，继承 DicomService 并实现 DICOM 服务接口
        public class DicomScp : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
        {
            private static string _outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReceivedDICOMs");
            
            public DicomScp(INetworkStream stream, Encoding fallbackEncoding, ILogger log, DicomServiceDependencies? dependencies = null)
                : base(stream, fallbackEncoding, log, dependencies ?? GetDefaultDependencies())
            {
                // 初始化保存目录（不存在则创建）
                if (!Directory.Exists(_outputDirectory))
                    Directory.CreateDirectory(_outputDirectory);
            }
            
            private static DicomServiceDependencies GetDefaultDependencies()
            {
                if (_staticServiceProvider != null)
                {
                    var loggerFactory = _staticServiceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    var networkManager = _staticServiceProvider.GetService(typeof(INetworkManager)) as INetworkManager;
                    var transcoderManager = _staticServiceProvider.GetService(typeof(ITranscoderManager)) as ITranscoderManager;
                    var memoryProvider = _staticServiceProvider.GetService(typeof(IMemoryProvider)) as IMemoryProvider;
                    
                    if (loggerFactory != null && networkManager != null && transcoderManager != null && memoryProvider != null)
                    {
                        return new DicomServiceDependencies(loggerFactory, networkManager, transcoderManager, memoryProvider, _staticServiceProvider);
                    }
                }
                
                // 如果无法从DI容器获取，使用null作为fallback（DicomService会使用内部默认值）
                return null!;
            }

            // 处理 C-ECHO 请求（验证连接是否正常）
            public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
            {
                try
                {
                    Logger.LogInformation($"收到 C-ECHO 请求，来自: {Association.CallingAE}");
                    return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
                }
                catch (Exception ex)
                {
                    Logger.LogError($"处理 C-ECHO 请求时发生错误: {ex.Message}");
                    return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.ProcessingFailure));
                }
            }

            // 处理 C-STORE 请求（接收 DICOM 文件）
            public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
            {
                string? sopInstanceUid = null;
                try
                {
                    // 获取 DICOM 文件信息
                    var dicomFile = request.File;
                    sopInstanceUid = request.SOPInstanceUID.UID;
                    
                    Logger.LogInformation($"开始接收 DICOM 文件 - SOP Instance UID: {sopInstanceUid}");
                    
                    // 验证DICOM文件完整性
                    if (!ValidateDicomFile(dicomFile))
                    {
                        Logger.LogWarning($"DICOM 文件验证失败 - SOP Instance UID: {sopInstanceUid}");
                        return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.ProcessingFailure));
                    }

                    // 生成文件名（包含时间戳避免冲突）
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = $"{sopInstanceUid}_{timestamp}.dcm";
                    var filePath = Path.Combine(_outputDirectory, fileName);

                    // 保存 DICOM 文件到本地
                    dicomFile.Save(filePath);
                    
                    // 记录文件到状态管理
                    var fileInfo = new FileInfo(filePath);
                    DicomServiceStatus.Instance.RecordFileReceived(filePath, fileInfo.Length, sopInstanceUid);
                    
                    // 记录DICOM文件信息到日志
                    LogDicomFileInfo(dicomFile, filePath);
                    
                    Logger.LogInformation($"DICOM 文件接收成功：{filePath}");

                    // 向发送方返回成功响应
                    return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.Success));
                }
                catch (Exception ex)
                {
                    Logger.LogError($"接收 DICOM 文件失败 - SOP Instance UID: {sopInstanceUid}, 错误: {ex.Message}");
                    return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.ProcessingFailure));
                }
            }
            
            public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
            {
                Logger.LogError($"C-STORE 请求处理异常 - 临时文件: {tempFileName}, 错误: {e.Message}");
                return Task.CompletedTask;
            }

            // 验证DICOM文件完整性
            private bool ValidateDicomFile(DicomFile dicomFile)
            {
                try
                {
                    // 检查必要的DICOM标签
                    var requiredTags = new[]
                    {
                        DicomTag.SOPInstanceUID,
                        DicomTag.SOPClassUID,
                        DicomTag.StudyInstanceUID,
                        DicomTag.SeriesInstanceUID
                    };

                    foreach (var tag in requiredTags)
                    {
                        if (!dicomFile.Dataset.Contains(tag))
                        {
                            Logger.LogWarning($"DICOM 文件缺少必要标签: {tag}");
                            return false;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"验证 DICOM 文件时发生错误: {ex.Message}");
                    return false;
                }
            }

            // 记录DICOM文件信息
            private void LogDicomFileInfo(DicomFile dicomFile, string filePath)
            {
                try
                {
                    var dataset = dicomFile.Dataset;
                    var fileInfo = new
                    {
                        FilePath = filePath,
                        SOPInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty),
                        SOPClassUID = dataset.GetSingleValueOrDefault(DicomTag.SOPClassUID, string.Empty),
                        StudyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                        SeriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
                        PatientID = dataset.GetSingleValueOrDefault(DicomTag.PatientID, string.Empty),
                        PatientName = dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty),
                        StudyDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, string.Empty),
                        StudyTime = dataset.GetSingleValueOrDefault(DicomTag.StudyTime, string.Empty),
                        Modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, string.Empty),
                        FileSize = new FileInfo(filePath).Length,
                        ReceivedTime = DateTime.Now
                    };

                    Logger.LogInformation($"DICOM 文件信息: {System.Text.Json.JsonSerializer.Serialize(fileInfo)}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"记录 DICOM 文件信息时发生错误: {ex.Message}");
                }
            }

            // 其他接口实现
            public void OnConnectionClosed(Exception? exception)
            {
                if (exception != null)
                {
                    Logger.LogWarning($"DICOM 连接关闭，异常: {exception.Message}");
                }
                else
                {
                    Logger.LogInformation("DICOM 连接正常关闭");
                }
            }

            public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
            {
                Logger.LogWarning($"收到 DICOM 中止请求 - 来源: {source}, 原因: {reason}");
            }

            public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
            {
                Logger.LogInformation($"收到关联请求 - 调用方 AE: {association.CallingAE}, 被调用方 AE: {association.CalledAE}");
                
                try
                {
                    // 验证调用方AE标题（可选的安全检查）
                    if (ValidateCallingAE(association.CallingAE))
                    {
                        // 接受所有提议的presentation contexts
                        foreach (var pc in association.PresentationContexts)
                        {
                            pc.SetResult(DicomPresentationContextResult.Accept);
                        }
                        
                        // 记录连接信息
                        var connectionId = $"{association.CallingAE}_{DateTime.Now:yyyyMMddHHmmss}";
                        DicomServiceStatus.Instance.AddConnection(connectionId, association.CallingAE, association.CalledAE);
                        
                        Logger.LogInformation("关联请求已接受");
                    }
                    else
                    {
                        Logger.LogWarning($"拒绝关联请求 - 未识别的调用方 AE: {association.CallingAE}");
                        return SendAssociationRejectAsync(DicomRejectResult.Permanent, DicomRejectSource.ServiceUser, DicomRejectReason.CallingAENotRecognized);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"处理关联请求时发生错误: {ex.Message}");
                    return SendAssociationRejectAsync(DicomRejectResult.Permanent, DicomRejectSource.ServiceUser, DicomRejectReason.NoReasonGiven);
                }
                
                return SendAssociationAcceptAsync(association);
            }

            public Task OnReceiveAssociationReleaseRequestAsync()
            {
                Logger.LogInformation("收到关联释放请求");
                return SendAssociationReleaseResponseAsync();
            }

            // 验证调用方AE标题
            private bool ValidateCallingAE(string callingAE)
            {
                if (string.IsNullOrEmpty(callingAE))
                    return false;
                return true;
            }
        }
    }
}