using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;
using Microsoft.Extensions.Logging;
using WebIServices.IBase;
using WebIServices.IServices.OCRIServices;

namespace WebServiceClass.Services.OCRServices
{
    /// <summary>
    /// 本地OCR识别服务实现
    /// </summary>
    public class LocalOCRService : IOCRService,IBaseService
    {
        private readonly ILogger<LocalOCRService> _logger;
        private readonly ISqlHelper _dal;
        private readonly string _tessDataPath;

        public LocalOCRService(ILogger<LocalOCRService> logger, ISqlHelper dal)
        {
            _logger = logger;
            _dal = dal;
            
            // Tesseract数据文件路径
            _tessDataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
            
            // 确保tessdata目录存在
            if (!Directory.Exists(_tessDataPath))
            {
                Directory.CreateDirectory(_tessDataPath);
                _logger.LogInformation($"创建Tesseract数据目录: {_tessDataPath}");
            }
        }

        public async Task<OCRResult> RecognizeTextAsync(string imagePath, string language = "chi_sim+eng")
        {
            var result = new OCRResult
            {
                FilePath = imagePath,
                Success = false
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (!File.Exists(imagePath))
                {
                    result.ErrorMessage = "图片文件不存在";
                    return result;
                }

                // 检查文件格式
                var extension = Path.GetExtension(imagePath).ToLower();
                if (!IsSupportedImageFormat(extension))
                {
                    result.ErrorMessage = "不支持的图片格式";
                    return result;
                }

                _logger.LogInformation($"开始OCR识别: {imagePath}");

                // 使用Tesseract进行OCR识别
                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                
                // 设置OCR参数
                engine.SetVariable("tessedit_char_whitelist", "");
                engine.SetVariable("tessedit_pageseg_mode", "3"); // 自动页面分割

                using var img = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(img);

                var text = page.GetText();
                var confidence = page.GetMeanConfidence();

                result.Text = text?.Trim() ?? string.Empty;
                result.Confidence = confidence * 100; // 转换为0-100
                result.Success = !string.IsNullOrEmpty(result.Text);
                result.TextBlocks = ExtractTextBlocks(page);

                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation($"OCR识别完成: {imagePath}, 置信度: {result.Confidence:F2}%, 耗时: {result.ProcessingTimeMs}ms");

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, $"OCR识别失败: {imagePath}");
                return result;
            }
        }

        public async Task<OCRResult> RecognizeTextAsync(byte[] imageBytes, string language = "chi_sim+eng")
        {
            var result = new OCRResult
            {
                FilePath = "内存图片",
                Success = false
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("开始OCR识别内存图片");

                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                engine.SetVariable("tessedit_char_whitelist", "");
                engine.SetVariable("tessedit_pageseg_mode", "3");

                using var img = Pix.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);

                var text = page.GetText();
                var confidence = page.GetMeanConfidence();

                result.Text = text?.Trim() ?? string.Empty;
                result.Confidence = confidence * 100;
                result.Success = !string.IsNullOrEmpty(result.Text);
                result.TextBlocks = ExtractTextBlocks(page);

                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation($"OCR识别完成: 内存图片, 置信度: {result.Confidence:F2}%, 耗时: {result.ProcessingTimeMs}ms");

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.Message;
                _logger.LogError(ex, "OCR识别内存图片失败");
                return result;
            }
        }

        public async Task<List<OCRResult>> RecognizeFolderAsync(string folderPath, string language = "chi_sim+eng")
        {
            var results = new List<OCRResult>();

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    _logger.LogWarning($"文件夹不存在: {folderPath}");
                    return results;
                }

                var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" };
                var imageFiles = Directory.GetFiles(folderPath)
                    .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToArray();

                _logger.LogInformation($"发现 {imageFiles.Length} 个图片文件");

                foreach (var imageFile in imageFiles)
                {
                    var result = await RecognizeTextAsync(imageFile, language);
                    results.Add(result);
                }

                var successCount = results.Count(r => r.Success);
                _logger.LogInformation($"批量OCR识别完成: {successCount}/{imageFiles.Length} 成功");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"批量OCR识别失败: {folderPath}");
                return results;
            }
        }

        /// <summary>
        /// 检查是否支持该图片格式
        /// </summary>
        private bool IsSupportedImageFormat(string extension)
        {
            var supportedFormats = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" };
            return supportedFormats.Contains(extension);
        }

        /// <summary>
        /// 提取文字块信息
        /// </summary>
        private List<OCRTextBlock> ExtractTextBlocks(Page page)
        {
            var textBlocks = new List<OCRTextBlock>();

            try
            {
                using var iter = page.GetIterator();
                iter.Begin();

                do
                {
                    var text = iter.GetText(PageIteratorLevel.Word);
                    if (!string.IsNullOrEmpty(text))
                    {
                        iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect);
                        
                        textBlocks.Add(new OCRTextBlock
                        {
                            Text = text,
                            Confidence = iter.GetConfidence(PageIteratorLevel.Word),
                            //BoundingBox = rect,
                            PageNumber = 1
                        });
                    }
                } while (iter.Next(PageIteratorLevel.Word));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "提取文字块信息失败");
            }

            return textBlocks;
        }
    }
}
