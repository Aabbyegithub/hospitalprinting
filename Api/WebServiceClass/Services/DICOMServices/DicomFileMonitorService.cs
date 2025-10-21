using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebServiceClass.Helper;

namespace WebServiceClass.Services.DICOMServices
{
    /// <summary>
    /// DICOM文件监控服务
    /// 监控接收到的DICOM文件，进行后处理操作
    /// </summary>
    public class DicomFileMonitorService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DicomFileMonitorService> _logger;
        private readonly DICOMHelper _dicomHelper;
        private readonly string _monitorDirectory;
        private readonly int _monitorInterval;
        private readonly long _maxFileSize;
        private readonly bool _enabled;

        public DicomFileMonitorService(IConfiguration configuration, ILogger<DicomFileMonitorService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _dicomHelper = new DICOMHelper();
            
            _enabled = _configuration.GetValue<bool>("SCPService:Enabled");
            _monitorDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                _configuration.GetValue<string>("SCPService:OutputDirectory") ?? "ReceivedDICOMs");
            _monitorInterval = _configuration.GetValue<int>("SCPService:MonitorInterval") * 1000; // 转换为毫秒
            _maxFileSize = _configuration.GetValue<long>("SCPService:MaxFileSize");
            
            // 设置默认值
            if (_monitorInterval == 0)
                _monitorInterval = 30000; // 30秒
            if (_maxFileSize == 0)
                _maxFileSize = 100 * 1024 * 1024; // 100MB
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enabled)
            {
                _logger.LogInformation("DICOM文件监控服务已禁用");
                return;
            }

            _logger.LogInformation($"DICOM文件监控服务已启动 - 监控目录: {_monitorDirectory}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDicomFiles();
                    await Task.Delay(_monitorInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("DICOM文件监控服务正在停止");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "DICOM文件监控服务处理文件时发生错误");
                    await Task.Delay(5000, stoppingToken); // 错误后等待5秒再重试
                }
            }
        }

        private async Task ProcessDicomFiles()
        {
            if (!Directory.Exists(_monitorDirectory))
            {
                Directory.CreateDirectory(_monitorDirectory);
                return;
            }

            var dicomFiles = Directory.GetFiles(_monitorDirectory, "*.dcm", SearchOption.TopDirectoryOnly);
            
            foreach (var filePath in dicomFiles)
            {
                try
                {
                    await ProcessDicomFile(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"处理DICOM文件时发生错误: {filePath}");
                }
            }
        }

        private async Task ProcessDicomFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            
            // 检查文件大小
            if (fileInfo.Length > _maxFileSize)
            {
                _logger.LogWarning($"DICOM文件过大，跳过处理: {filePath} (大小: {fileInfo.Length} bytes)");
                return;
            }

            // 检查文件是否正在被写入（文件大小在最近5秒内没有变化）
            if (!IsFileReady(filePath))
            {
                return;
            }

            try
            {
                // 解析DICOM文件信息
                var dicomFile = FellowOakDicom.DicomFile.Open(filePath);
                var dataset = dicomFile.Dataset;

                // 提取关键信息
                var dicomInfo = ExtractDicomInfo(dataset, filePath);
                
                // 记录到日志
                _logger.LogInformation($"处理DICOM文件: {System.Text.Json.JsonSerializer.Serialize(dicomInfo)}");

                // 这里可以添加更多的后处理逻辑，例如：
                // 1. 保存到数据库
                // 2. 生成缩略图
                // 3. 发送到其他系统
                // 4. 文件归档等

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"解析DICOM文件失败: {filePath}");
            }
        }

        private bool IsFileReady(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var currentSize = fileInfo.Length;
                
                // 等待1秒后再次检查文件大小
                Thread.Sleep(1000);
                
                fileInfo.Refresh();
                return fileInfo.Length == currentSize;
            }
            catch
            {
                return false;
            }
        }

        private object ExtractDicomInfo(FellowOakDicom.DicomDataset dataset, string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            
            return new
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                CreatedTime = fileInfo.CreationTime,
                ModifiedTime = fileInfo.LastWriteTime,
                SOPInstanceUID = dataset.GetString(FellowOakDicom.DicomTag.SOPInstanceUID),
                SOPClassUID = dataset.GetString(FellowOakDicom.DicomTag.SOPClassUID),
                StudyInstanceUID = dataset.GetString(FellowOakDicom.DicomTag.StudyInstanceUID),
                SeriesInstanceUID = dataset.GetString(FellowOakDicom.DicomTag.SeriesInstanceUID),
                PatientID = dataset.GetString(FellowOakDicom.DicomTag.PatientID),
                PatientName = dataset.GetString(FellowOakDicom.DicomTag.PatientName),
                StudyDate = dataset.GetString(FellowOakDicom.DicomTag.StudyDate),
                StudyTime = dataset.GetString(FellowOakDicom.DicomTag.StudyTime),
                Modality = dataset.GetString(FellowOakDicom.DicomTag.Modality),
                StudyDescription = dataset.GetString(FellowOakDicom.DicomTag.StudyDescription),
                SeriesDescription = dataset.GetString(FellowOakDicom.DicomTag.SeriesDescription),
                InstitutionName = dataset.GetString(FellowOakDicom.DicomTag.InstitutionName),
                Manufacturer = dataset.GetString(FellowOakDicom.DicomTag.Manufacturer),
                ModelName = dataset.GetString(FellowOakDicom.DicomTag.ManufacturerModelName),
                ProcessedTime = DateTime.Now
            };
        }
    }
}
