using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using Microsoft.Extensions.Logging;
using System.Net;

namespace WebServiceClass.Services.DICOMServices
{
    /// <summary>
    /// DICOM SCP服务测试客户端
    /// </summary>
    public class DicomTestClient
    {
        private readonly ILogger _logger;

        public DicomTestClient(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 测试C-ECHO连接
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="aeTitle">AE标题</param>
        /// <returns>是否成功</returns>
        public async Task<bool> TestCEchoAsync(string host = "localhost", int port = 104, string aeTitle = "HOSPITAL_SCP")
        {
            try
            {
                _logger.LogInformation($"测试C-ECHO连接到 {host}:{port} (AE: {aeTitle})");

                var client = DicomClientFactory.Create(host, port, false, "TEST_SCU", aeTitle);

                DicomCEchoResponse? response = null;
                await client.AddRequestAsync(new DicomCEchoRequest 
                { 
                    OnResponseReceived = (req, resp) => response = resp 
                });

                await client.SendAsync();

                if (response != null && response.Status == DicomStatus.Success)
                {
                    _logger.LogInformation("✅ C-ECHO测试成功!");
                    return true;
                }
                else
                {
                    _logger.LogError($"❌ C-ECHO测试失败: {response?.Status}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ C-ECHO测试异常");
                return false;
            }
        }

        /// <summary>
        /// 测试C-STORE文件传输
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="aeTitle">AE标题</param>
        /// <returns>是否成功</returns>
        public async Task<bool> TestCStoreAsync(string filePath, string host = "localhost", int port = 104, string aeTitle = "HOSPITAL_SCP")
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"❌ DICOM文件不存在: {filePath}");
                    return false;
                }

                _logger.LogInformation($"测试C-STORE文件传输: {filePath}");

                var client = DicomClientFactory.Create(host, port, false, "TEST_SCU", aeTitle);
                var dicomFile = await DicomFile.OpenAsync(filePath);

                DicomCStoreResponse? response = null;
                await client.AddRequestAsync(new DicomCStoreRequest(dicomFile)
                {
                    OnResponseReceived = (req, resp) => response = resp
                });

                await client.SendAsync();

                if (response != null && response.Status == DicomStatus.Success)
                {
                    _logger.LogInformation("✅ C-STORE测试成功!");
                    return true;
                }
                else
                {
                    _logger.LogError($"❌ C-STORE测试失败: {response?.Status}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ C-STORE测试异常");
                return false;
            }
        }

        /// <summary>
        /// 创建测试用的DICOM文件
        /// </summary>
        /// <param name="outputPath">输出路径</param>
        /// <returns>是否成功</returns>
        public async Task<bool> CreateTestDicomFileAsync(string outputPath = "test_dicom.dcm")
        {
            try
            {
                _logger.LogInformation("创建测试DICOM文件...");

                var dataset = new DicomDataset();
                dataset.Add(DicomTag.PatientName, "Test^Patient");
                dataset.Add(DicomTag.PatientID, "TEST001");
                dataset.Add(DicomTag.PatientSex, "M");
                // AS VR must be 4 chars like 003Y / 030Y
                dataset.Add(DicomTag.PatientAge, "030Y");
                dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
                dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
                dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
                dataset.Add(DicomTag.StudyDate, DateTime.Now.ToString("yyyyMMdd"));
                dataset.Add(DicomTag.StudyTime, DateTime.Now.ToString("HHmmss"));
                dataset.Add(DicomTag.Modality, "CT");
                dataset.Add(DicomTag.StudyDescription, "Test Study");
                dataset.Add(DicomTag.SeriesDescription, "Test Series");
                dataset.Add(DicomTag.InstitutionName, "Test Hospital");
                
                // 添加必要的SOP Class UID (CT Image Storage)
                dataset.Add(DicomTag.SOPClassUID, DicomUID.CTImageStorage);

                var dicomFile = new DicomFile(dataset);
                await dicomFile.SaveAsync(outputPath);

                _logger.LogInformation($"✅ 测试DICOM文件已创建: {outputPath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 创建测试DICOM文件失败");
                return false;
            }
        }

        /// <summary>
        /// 运行完整测试
        /// </summary>
        /// <param name="host">主机地址</param>
        /// <param name="port">端口</param>
        /// <param name="aeTitle">AE标题</param>
        /// <returns>测试结果</returns>
        public async Task<TestResult> RunFullTestAsync(string host = "localhost", int port = 104, string aeTitle = "HOSPITAL_SCP")
        {
            var result = new TestResult();

            _logger.LogInformation("=== DICOM SCP 服务测试开始 ===");
            _logger.LogInformation($"测试目标: {host}:{port} (AE: {aeTitle})");

            // 测试1: C-ECHO
            _logger.LogInformation("1. 测试C-ECHO连接...");
            result.CEchoSuccess = await TestCEchoAsync(host, port, aeTitle);

            // 测试2: C-STORE
            _logger.LogInformation("2. 测试C-STORE文件传输...");
            
            var testFile = "test_dicom.dcm";
            if (!File.Exists(testFile))
            {
                await CreateTestDicomFileAsync(testFile);
            }

            result.CStoreSuccess = await TestCStoreAsync(testFile, host, port, aeTitle);

            // 清理测试文件
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
                _logger.LogInformation("🧹 测试文件已清理");
            }

            // 汇总结果
            result.OverallSuccess = result.CEchoSuccess && result.CStoreSuccess;
            
            _logger.LogInformation("=== 测试结果汇总 ===");
            _logger.LogInformation($"C-ECHO测试: {(result.CEchoSuccess ? "✅ 通过" : "❌ 失败")}");
            _logger.LogInformation($"C-STORE测试: {(result.CStoreSuccess ? "✅ 通过" : "❌ 失败")}");
            
            if (result.OverallSuccess)
            {
                _logger.LogInformation("🎉 所有测试通过! DICOM SCP服务运行正常!");
            }
            else
            {
                _logger.LogWarning("⚠️ 部分测试失败，请检查服务配置和网络连接");
            }

            return result;
        }
    }

    /// <summary>
    /// 测试结果
    /// </summary>
    public class TestResult
    {
        public bool CEchoSuccess { get; set; }
        public bool CStoreSuccess { get; set; }
        public bool OverallSuccess { get; set; }
    }
}
