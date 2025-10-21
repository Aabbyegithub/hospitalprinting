using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebServiceClass.Services.DICOMServices;
using Microsoft.Extensions.Options;
using WebIServices.IServices.DICOMIServices;

namespace WebServiceClass.Services.DICOMServices
{
    public class HospitalDicomScp : DicomService, IDicomServiceProvider, IDicomCStoreProvider, IDicomCEchoProvider
    {
        private readonly ILogger _logger;
        private readonly IDicomFileParserService _dicomFileParser;
        private readonly DicomServiceConfig _config;

        public HospitalDicomScp(
            INetworkStream stream, 
            Encoding fallbackEncoding, 
            ILogger logger,
            DicomServiceDependencies dependencies,
            IDicomFileParserService dicomFileParser,
            IOptions<DicomServiceConfig> options) 
            : base(stream, fallbackEncoding, logger, dependencies)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dicomFileParser = dicomFileParser ?? throw new ArgumentNullException(nameof(dicomFileParser));
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _config = options.Value;

            // 确保保存目录存在
            if (_config.AutoCreateDirectory && !Directory.Exists(_config.SaveDirectory))
            {
                Directory.CreateDirectory(_config.SaveDirectory);
                _logger.LogInformation($"创建DICOM文件保存目录: {_config.SaveDirectory}");
            }
        }

        // ✅ 关联请求（异步）
        public async Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            _logger.LogInformation($"收到DICOM关联请求：Calling AE={association.CallingAE}, Called AE={association.CalledAE}");

            // 接受所有存储类型请求
            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax.IsImageStorage)
                {
                    // 接受常见传输语法：Explicit VR Little/Big Endian 与 Implicit VR Little Endian
                    pc.AcceptTransferSyntaxes(
                        DicomTransferSyntax.ExplicitVRLittleEndian,
                        DicomTransferSyntax.ExplicitVRBigEndian,
                        DicomTransferSyntax.ImplicitVRLittleEndian
                    );
                    _logger.LogInformation($"接受图像存储请求: {pc.AbstractSyntax} - 支持EVR LE/BE与IVR LE");
                }
                else if (pc.AbstractSyntax == DicomUID.Verification)
                {
                    pc.AcceptTransferSyntaxes(DicomTransferSyntax.ImplicitVRLittleEndian);
                    _logger.LogInformation($"接受验证请求: {pc.AbstractSyntax}");
                }
                else
                {
                    _logger.LogWarning($"拒绝不支持的抽象语法: {pc.AbstractSyntax}");
                }
            }

            await SendAssociationAcceptAsync(association);
            _logger.LogInformation("DICOM关联已建立");
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            _logger.LogInformation("客户端请求释放DICOM连接");
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            _logger.LogWarning($"DICOM连接中止: {reason}");
        }

        public void OnConnectionClosed(Exception exception)
        {
            _logger.LogInformation("DICOM连接已关闭");
        }

        // ✅ 接收DICOM文件（C-STORE）
        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            try
            {
                var dataset = request.File.Dataset;
                var sopInstanceUid = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, Guid.NewGuid().ToString());
                var patientId = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "Unknown");
                var studyDate = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));
                var modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "Unknown");

                // 创建按日期和患者ID组织的目录结构
                var patientDir = Path.Combine(_config.SaveDirectory, studyDate, patientId);
                if (!Directory.Exists(patientDir))
                {
                    Directory.CreateDirectory(patientDir);
                }

                var fileName = $"{sopInstanceUid}_{modality}_{DateTime.Now:HHmmss}.dcm";
                var filePath = Path.Combine(patientDir, fileName);

                // 保存DICOM文件
                request.File.Save(filePath);

                _logger.LogInformation($"DICOM文件保存成功: {filePath}");
                _logger.LogInformation($"患者ID: {patientId}, 检查日期: {studyDate}, 设备类型: {modality}");

                // 异步解析DICOM文件信息
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // 验证文件
                        var isValid = await _dicomFileParser.ValidateDicomFileAsync(filePath);
                        if (isValid)
                        {
                            // 解析患者信息
                            var patientInfo = await _dicomFileParser.ParsePatientInfoAsync(filePath);
                            if (patientInfo != null)
                            {
                                //_logger.LogInformation($"解析患者信息: {patientInfo.patient_name}");
                            }

                            // 解析检查信息
                            var examInfo = await _dicomFileParser.ParseExaminationInfoAsync(filePath);
                            if (examInfo != null)
                            {
                                //_logger.LogInformation($"解析检查信息: {examInfo.study_description}");
                            }

                            // 获取元数据
                            var metadata = await _dicomFileParser.GetDicomMetadataAsync(filePath);
                            _logger.LogInformation($"DICOM元数据字段数: {metadata.Count}");
                        }
                        else
                        {
                            _logger.LogWarning($"DICOM文件验证失败: {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"异步处理DICOM文件失败: {filePath}");
                    }
                });

                return new DicomCStoreResponse(request, DicomStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"保存DICOM文件失败");
                return new DicomCStoreResponse(request, DicomStatus.ProcessingFailure);
            }
        }

        public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
            => Task.FromResult(OnCStoreRequest(request));

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
        {
            _logger.LogError(e, $"C-STORE 异常: {tempFileName}");
            return Task.CompletedTask;
        }

        // ✅ C-ECHO 处理（用于连接测试）
        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            //_logger.LogInformation($"收到C-ECHO请求: {request.MessageId}");
            return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
        }
    }
}
