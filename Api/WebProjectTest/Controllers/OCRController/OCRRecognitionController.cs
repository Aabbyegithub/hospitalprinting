using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WebIServices.IServices.OCRIServices;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using static ModelClassLibrary.Model.CommonEnmFixts;

namespace WebProjectTest.Controllers
{
    /// <summary>
    /// OCR识别控制器
    /// 支持识别PDF和图片文件，支持本地文件路径、URL链接、共享文件夹路径
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OCRRecognitionController : AutherController
    {
        private readonly IOCRRecognitionService _ocrRecognitionService;

        public OCRRecognitionController(
            IOCRRecognitionService ocrRecognitionService,
            WebIServices.IBase.IRedisCacheService redisCacheService)
            : base(redisCacheService)
        {
            _ocrRecognitionService = ocrRecognitionService;
        }

        /// <summary>
        /// 识别文件（支持本地路径、URL、共享文件夹路径）
        /// </summary>
        /// <param name="filePath">
        /// 文件路径，支持：
        /// 1. 本地文件路径：C:\path\to\file.jpg 或 /path/to/file.jpg
        /// 2. URL链接：http://example.com/image.jpg 或 https://example.com/image.jpg
        /// 3. 共享文件夹路径（UNC路径）：\\server\share\file.jpg
        /// </param>
        /// <returns>识别结果</returns>
        /// <remarks>
        /// 示例请求：
        /// POST /api/OCRRecognition/RecognizeFile?filePath=C:\images\test.jpg
        /// POST /api/OCRRecognition/RecognizeFile?filePath=http://example.com/image.jpg
        /// POST /api/OCRRecognition/RecognizeFile?filePath=\\server\share\document.pdf
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> RecognizeFile([FromQuery] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest(new { success = false, message = "文件路径不能为空" });
            }

            var result = await _ocrRecognitionService.RecognizeFileAsync(filePath);
            return Ok(result);
        }

        /// <summary>
        /// 识别上传的文件
        /// </summary>
        /// <param name="file">上传的文件（支持图片和PDF）</param>
        /// <returns>识别结果</returns>
        /// <remarks>
        /// 支持的文件格式：
        /// - 图片：jpg, jpeg, png, bmp, tiff, tif
        /// - PDF：pdf
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> RecognizeUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "请选择要上传的文件" });
            }

            // 验证文件大小（最大50MB）
            const long maxFileSize = 50 * 1024 * 1024; // 50MB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new { success = false, message = $"文件大小不能超过 {maxFileSize / 1024 / 1024}MB" });
            }

            // 验证文件格式
            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".pdf" };
            if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
            {
                return BadRequest(new { success = false, message = $"不支持的文件格式: {extension}，支持格式: {string.Join(", ", allowedExtensions)}" });
            }

            try
            {
                // 读取文件内容
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // 调用识别服务
                var result = await _ocrRecognitionService.RecognizeUploadedFileAsync(fileBytes, file.FileName);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"处理文件失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 批量识别文件
        /// </summary>
        /// <param name="filePaths">文件路径数组</param>
        /// <returns>识别结果列表</returns>
        [HttpPost]
        public async Task<IActionResult> RecognizeBatchFiles([FromBody] string[] filePaths)
        {
            if (filePaths == null || filePaths.Length == 0)
            {
                return BadRequest(new { success = false, message = "文件路径数组不能为空" });
            }

            var results = new System.Collections.Generic.List<object>();
            
            foreach (var filePath in filePaths)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    results.Add(new { filePath = filePath, success = false, message = "文件路径为空" });
                    continue;
                }

                var result = await _ocrRecognitionService.RecognizeFileAsync(filePath);
                results.Add(new 
                { 
                    filePath = filePath, 
                    success = result.success, 
                    message = result.Message,
                    data = result.Response 
                });
            }

            return Ok(new { success = true, message = "批量识别完成", data = results });
        }
    }
}

