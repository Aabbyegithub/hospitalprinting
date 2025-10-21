using Microsoft.AspNetCore.Mvc;
using WebServiceClass.Services.OCRServices;
using WebProjectTest.Common;
using static WebProjectTest.Common.Message;
using WebIServices.IServices.OCRIServices;

namespace WebProjectTest.Controllers
{
    /// <summary>
    /// OCR识别控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OCRController : ControllerBase
    {
        private readonly ILogger<OCRController> _logger;
        private readonly IOCRService _ocrService;

        public OCRController(ILogger<OCRController> logger, IOCRService ocrService)
        {
            _logger = logger;
            _ocrService = ocrService;
        }

        /// <summary>
        /// 识别图片文件中的文字
        /// </summary>
        /// <param name="filePath">图片文件路径</param>
        /// <param name="language">识别语言 (chi_sim=简体中文, eng=英文)</param>
        /// <returns></returns>
        [HttpPost("recognize")]
        public async Task<ApiResponse<OCRResult>> RecognizeImage([FromQuery] string filePath, [FromQuery] string language = "chi_sim+eng")
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return Fail<OCRResult>("请提供图片文件路径");
                }

                _logger.LogInformation($"开始OCR识别: {filePath}");
                var result = await _ocrService.RecognizeTextAsync(filePath, language);
                
                if (result.Success)
                {
                    _logger.LogInformation($"OCR识别成功: {filePath}, 置信度: {result.Confidence:F2}%");
                    return Success(result);
                }
                else
                {
                    _logger.LogWarning($"OCR识别失败: {filePath}, 错误: {result.ErrorMessage}");
                    return Fail<OCRResult>(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OCR识别异常: {filePath}");
                return Fail<OCRResult>("OCR识别失败");
            }
        }

        /// <summary>
        /// 上传图片并识别
        /// </summary>
        /// <param name="file">图片文件</param>
        /// <param name="language">识别语言</param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ApiResponse<OCRResult>> UploadAndRecognize(IFormFile file, [FromQuery] string language = "chi_sim+eng")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Fail<OCRResult>("请选择要上传的图片文件");
                }

                // 检查文件类型
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return Fail<OCRResult>("不支持的图片格式");
                }

                // 检查文件大小 (限制10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return Fail<OCRResult>("图片文件过大，请选择小于10MB的文件");
                }

                _logger.LogInformation($"开始上传并识别图片: {file.FileName}");

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();

                var result = await _ocrService.RecognizeTextAsync(imageBytes, language);
                result.FilePath = file.FileName;

                if (result.Success)
                {
                    _logger.LogInformation($"上传OCR识别成功: {file.FileName}, 置信度: {result.Confidence:F2}%");
                    return Success(result);
                }
                else
                {
                    _logger.LogWarning($"上传OCR识别失败: {file.FileName}, 错误: {result.ErrorMessage}");
                    return Fail<OCRResult>(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"上传OCR识别异常: {file?.FileName}");
                return Fail<OCRResult>("上传OCR识别失败");
            }
        }

        /// <summary>
        /// 批量识别文件夹中的图片
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="language">识别语言</param>
        /// <returns></returns>
        [HttpPost("batch")]
        public async Task<ApiResponse<List<OCRResult>>> BatchRecognize([FromQuery] string folderPath, [FromQuery] string language = "chi_sim+eng")
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return Fail<List<OCRResult>>("请提供文件夹路径");
                }

                _logger.LogInformation($"开始批量OCR识别: {folderPath}");
                var results = await _ocrService.RecognizeFolderAsync(folderPath, language);
                
                var successCount = results.Count(r => r.Success);
                _logger.LogInformation($"批量OCR识别完成: {successCount}/{results.Count} 成功");

                return Success(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"批量OCR识别异常: {folderPath}");
                return Fail<List<OCRResult>>("批量OCR识别失败");
            }
        }

        /// <summary>
        /// 获取支持的图片格式
        /// </summary>
        /// <returns></returns>
        [HttpGet("formats")]
        public ApiResponse<List<string>> GetSupportedFormats()
        {
            var formats = new List<string> { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" };
            return Success(formats);
        }

        /// <summary>
        /// 获取支持的语言
        /// </summary>
        /// <returns></returns>
        [HttpGet("languages")]
        public ApiResponse<List<string>> GetSupportedLanguages()
        {
            var languages = new List<string>
            {
                "chi_sim",      // 简体中文
                "chi_tra",      // 繁体中文
                "eng",          // 英文
                "chi_sim+eng",  // 中英文混合
                "chi_tra+eng"   // 繁英文混合
            };
            return Success(languages);
        }
    }
}
