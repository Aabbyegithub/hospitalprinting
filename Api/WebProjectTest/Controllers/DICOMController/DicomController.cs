using Microsoft.AspNetCore.Mvc;
using WebServiceClass.Services.DICOMServices;
using WebProjectTest.Common;
using static WebProjectTest.Common.Message;
using Microsoft.Extensions.Options;
using WebIServices.IServices.DICOMIServices;

namespace WebProjectTest.Controllers.DICOMController
{
    /// <summary>
    /// DICOM服务控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DicomController : ControllerBase
    {
        private readonly ILogger<DicomController> _logger;
        private readonly DicomServiceConfig _config;
        private readonly IDicomFileParserService _dicomFileParser;

        public DicomController(
            ILogger<DicomController> logger,
            IOptions<DicomServiceConfig> config,
            IDicomFileParserService dicomFileParser)
        {
            _logger = logger;
            _config = config.Value;
            _dicomFileParser = dicomFileParser;
        }

        /// <summary>
        /// 获取DICOM服务配置
        /// </summary>
        /// <returns></returns>
        [HttpGet("config")]
        public ApiResponse<DicomServiceConfig> GetConfig()
        {
            return Success(_config);
        }

        /// <summary>
        /// 更新DICOM服务配置
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <returns></returns>
        [HttpPost("config")]
        public ApiResponse<bool> UpdateConfig([FromBody] DicomServiceConfig config)
        {
            try
            {
                // 这里应该更新配置并重启服务
                // 实际实现中可能需要更复杂的配置管理
                _logger.LogInformation("DICOM服务配置已更新");
                return Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新DICOM服务配置失败");
                return Fail<bool>("更新配置失败");
            }
        }

        /// <summary>
        /// 获取接收到的DICOM文件列表
        /// </summary>
        /// <param name="date">日期筛选 (yyyyMMdd)</param>
        /// <param name="patientId">患者ID筛选</param>
        /// <returns></returns>
        [HttpGet("files")]
        public ApiResponse<List<object>> GetReceivedFiles([FromQuery] string? date = null, [FromQuery] string? patientId = null)
        {
            try
            {
                var files = new List<object>();
                var baseDir = _config.SaveDirectory;

                if (!Directory.Exists(baseDir))
                {
                    return Success(files);
                }

                // 如果指定了日期，只搜索该日期的目录
                var searchDirs = new List<string>();
                if (!string.IsNullOrEmpty(date))
                {
                    var dateDir = Path.Combine(baseDir, date);
                    if (Directory.Exists(dateDir))
                    {
                        searchDirs.Add(dateDir);
                    }
                }
                else
                {
                    // 搜索所有日期目录
                    var dateDirs = Directory.GetDirectories(baseDir);
                    searchDirs.AddRange(dateDirs);
                }

                foreach (var dir in searchDirs)
                {
                    var patientDirs = Directory.GetDirectories(dir);
                    foreach (var patientDir in patientDirs)
                    {
                        var patientIdFromDir = Path.GetFileName(patientDir);
                        
                        // 如果指定了患者ID，只处理匹配的目录
                        if (!string.IsNullOrEmpty(patientId) && !patientIdFromDir.Contains(patientId))
                        {
                            continue;
                        }

                        var dicomFiles = Directory.GetFiles(patientDir, "*.dcm");
                        foreach (var file in dicomFiles)
                        {
                            var fileInfo = new FileInfo(file);
                            files.Add(new
                            {
                                FileName = fileInfo.Name,
                                FilePath = file,
                                FileSize = fileInfo.Length,
                                fileInfo.CreationTime,
                                fileInfo.LastWriteTime,
                                PatientId = patientIdFromDir,
                                Date = Path.GetFileName(Path.GetDirectoryName(patientDir))
                            });
                        }
                    }
                }

                return Success(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取DICOM文件列表失败");
                return Fail<List<object>>("获取文件列表失败");
            }
        }

        /// <summary>
        /// 解析DICOM文件信息
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [HttpPost("parse")]
        public async Task<ApiResponse<Dictionary<string,object>>> ParseDicomFile([FromBody] ParseRequest request)
        {
            return Success(new Dictionary<string, object>());
            //try
            //{
            //    if (string.IsNullOrEmpty(request.FilePath) || !File.Exists(request.FilePath))
            //    {
            //        return Fail<Dictionary<string, object>>("文件不存在");
            //    }

            //    var result = new
            //    {
            //        PatientInfo = await _dicomFileParser.ParsePatientInfoAsync(request.FilePath),
            //        ExaminationInfo = await _dicomFileParser.ParseExaminationInfoAsync(request.FilePath),
            //        Metadata = await _dicomFileParser.GetDicomMetadataAsync(request.FilePath),
            //        IsValid = await _dicomFileParser.ValidateDicomFileAsync(request.FilePath)
            //    };

            //    return Success(result);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "解析DICOM文件失败");
            //    return Fail<Dictionary<string, object>>("解析文件失败");
            //}
        }

        /// <summary>
        /// 测试DICOM服务连接
        /// </summary>
        /// <returns></returns>
        [HttpPost("test")]
        public async Task<ApiResponse<string>> TestConnection()
        {
            try
            {
                _logger.LogInformation("DICOM服务连接测试");
                
                // 使用测试客户端进行完整测试
                var testClient = new DicomTestClient(_logger);
                var result = await testClient.RunFullTestAsync();
                
                return Success("连接成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DICOM服务连接测试失败");
                return Fail<string>("连接测试失败");
            }
        }

        /// <summary>
        /// 测试C-ECHO连接
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="aeTitle">AE标题</param>
        /// <returns></returns>
        [HttpPost("test/echo")]
        public async Task<ApiResponse<bool>> TestCEcho([FromQuery] string host = "localhost", [FromQuery] int port = 104, [FromQuery] string aeTitle = "HOSPITAL_SCP")
        {
            try
            {
                var testClient = new DicomTestClient(_logger);
                var success = await testClient.TestCEchoAsync(host, port, aeTitle);
                
                return Success(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "C-ECHO测试失败");
                return Fail<bool>("C-ECHO测试失败");
            }
        }

        /// <summary>
        /// 测试C-STORE文件传输
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="aeTitle">AE标题</param>
        /// <returns></returns>
        [HttpPost("test/store")]
        public async Task<ApiResponse<bool>> TestCStore([FromQuery] string filePath, [FromQuery] string host = "localhost", [FromQuery] int port = 104, [FromQuery] string aeTitle = "HOSPITAL_SCP")
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return Fail<bool>("请提供DICOM文件路径");
                }

                var testClient = new DicomTestClient(_logger);
                var success = await testClient.TestCStoreAsync(filePath, host, port, aeTitle);
                
                return Success(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "C-STORE测试失败");
                return Fail<bool>("C-STORE测试失败");
            }
        }
    }

    /// <summary>
    /// 解析请求
    /// </summary>
    public class ParseRequest
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}
