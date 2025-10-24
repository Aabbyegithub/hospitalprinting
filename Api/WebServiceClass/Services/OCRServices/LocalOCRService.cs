using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;
using Microsoft.Extensions.Logging;
using WebIServices.IBase;
using WebIServices.IServices.OCRIServices;
using System.Text;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

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

                // 预处理图片（针对电子胶片优化）
                var processedImagePath = await PreprocessImageForMedicalFilmAsync(imagePath);

                // 使用Tesseract进行OCR识别
                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                
                // 针对医疗影像优化的OCR参数
                ConfigureEngineForMedicalText(engine);

                using var img = Pix.LoadFromFile(processedImagePath);
                using var page = engine.Process(img);

                var text = page.GetText();
                var confidence = page.GetMeanConfidence();

                // 清理和标准化文本
                result.Text = CleanAndNormalizeText(text?.Trim() ?? string.Empty);
                result.Confidence = confidence * 100; // 转换为0-100
                result.Success = !string.IsNullOrEmpty(result.Text);
                result.TextBlocks = ExtractTextBlocks(page);

                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation($"OCR识别完成: {imagePath}, 置信度: {result.Confidence:F2}%, 耗时: {result.ProcessingTimeMs}ms");

                // 清理临时文件
                if (processedImagePath != imagePath && File.Exists(processedImagePath))
                {
                    File.Delete(processedImagePath);
                }

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

                // 预处理图片
                var processedBytes = await PreprocessImageBytesForMedicalFilmAsync(imageBytes);

                using var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                ConfigureEngineForMedicalText(engine);

                using var img = Pix.LoadFromMemory(processedBytes);
                using var page = engine.Process(img);

                var text = page.GetText();
                var confidence = page.GetMeanConfidence();

                result.Text = CleanAndNormalizeText(text?.Trim() ?? string.Empty);
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
        /// 针对医疗影像优化OCR引擎参数
        /// </summary>
        private void ConfigureEngineForMedicalText(TesseractEngine engine)
        {
            try
            {
                // 设置页面分割模式为自动
                engine.SetVariable("tessedit_pageseg_mode", "3");
                
                // 设置OCR引擎模式为LSTM（更准确）
                engine.SetVariable("tessedit_ocr_engine_mode", "1");
                
                // 启用字符白名单（包含中文字符和数字）
                engine.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz一二三四五六七八九十百千万亿年月日时分秒左右上下前后内外大小高低长短粗细厚薄深浅明暗红橙黄绿青蓝紫黑白灰");
                
                // 设置最小字符置信度
                engine.SetVariable("tessedit_min_char_confidence", "30");
                
                // 启用字典查找
                engine.SetVariable("load_system_dawg", "1");
                engine.SetVariable("load_freq_dawg", "1");
                engine.SetVariable("load_punc_dawg", "1");
                engine.SetVariable("load_number_dawg", "1");
                
                // 设置字符间距
                engine.SetVariable("tessedit_char_blacklist", "");
                
                // 启用文本行检测
                engine.SetVariable("textord_min_linesize", "2.0");
                
                // 设置文本方向检测
                engine.SetVariable("textord_old_baselines", "0");
                engine.SetVariable("textord_old_xheight", "0");
                
                _logger.LogInformation("OCR引擎参数配置完成（医疗影像优化）");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "配置OCR引擎参数时出现警告");
            }
        }

        /// <summary>
        /// 针对电子胶片预处理图片
        /// </summary>
        private async Task<string> PreprocessImageForMedicalFilmAsync(string imagePath)
        {
            try
            {
                using var originalImage = Image.FromFile(imagePath);
                
                // 检查是否需要预处理
                if (originalImage.Width <= 2000 && originalImage.Height <= 2000)
                {
                    return imagePath; // 图片尺寸合适，直接返回原路径
                }

                var tempPath = Path.Combine(Path.GetTempPath(), $"preprocessed_{Guid.NewGuid()}.png");
                
                // 调整图片尺寸
                var newWidth = Math.Min(originalImage.Width, 2000);
                var newHeight = Math.Min(originalImage.Height, 2000);
                
                using var resizedImage = new Bitmap(newWidth, newHeight);
                using var graphics = Graphics.FromImage(resizedImage);
                
                // 设置高质量缩放
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                
                // 增强对比度
                var enhancedImage = EnhanceContrast(resizedImage);
                
                // 保存预处理后的图片
                enhancedImage.Save(tempPath, System.Drawing.Imaging.ImageFormat.Png);
                enhancedImage.Dispose();
                
                _logger.LogInformation($"图片预处理完成: {imagePath} -> {tempPath}");
                return tempPath;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"图片预处理失败，使用原图: {imagePath}");
                return imagePath;
            }
        }

        /// <summary>
        /// 预处理内存中的图片字节
        /// </summary>
        private async Task<byte[]> PreprocessImageBytesForMedicalFilmAsync(byte[] imageBytes)
        {
            try
            {
                using var ms = new MemoryStream(imageBytes);
                using var originalImage = Image.FromStream(ms);
                
                // 检查是否需要预处理
                if (originalImage.Width <= 2000 && originalImage.Height <= 2000)
                {
                    return imageBytes; // 图片尺寸合适，直接返回原字节
                }

                // 调整图片尺寸
                var newWidth = Math.Min(originalImage.Width, 2000);
                var newHeight = Math.Min(originalImage.Height, 2000);
                
                using var resizedImage = new Bitmap(newWidth, newHeight);
                using var graphics = Graphics.FromImage(resizedImage);
                
                // 设置高质量缩放
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                
                // 增强对比度
                var enhancedImage = EnhanceContrast(resizedImage);
                
                // 转换为字节数组
                using var resultMs = new MemoryStream();
                enhancedImage.Save(resultMs, ImageFormat.Png);
                enhancedImage.Dispose();
                
                _logger.LogInformation("内存图片预处理完成");
                return resultMs.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "内存图片预处理失败，使用原图");
                return imageBytes;
            }
        }

        /// <summary>
        /// 增强图片对比度
        /// </summary>
        private Bitmap EnhanceContrast(Bitmap image)
        {
            try
            {
                var enhancedImage = new Bitmap(image.Width, image.Height);
                
                // 使用直方图均衡化增强对比度
                using var graphics = Graphics.FromImage(enhancedImage);
                graphics.DrawImage(image, 0, 0);
                
                // 应用对比度增强
                var contrastMatrix = new float[][]
                {
                    new float[] { 1.2f, 0, 0, 0, 0 },
                    new float[] { 0, 1.2f, 0, 0, 0 },
                    new float[] { 0, 0, 1.2f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };
                
                var imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(new ColorMatrix(contrastMatrix), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                
                graphics.DrawImage(enhancedImage, 
                    new Rectangle(0, 0, enhancedImage.Width, enhancedImage.Height),
                    0, 0, enhancedImage.Width, enhancedImage.Height,
                    GraphicsUnit.Pixel, imageAttributes);
                
                return enhancedImage;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "对比度增强失败，返回原图");
                return image;
            }
        }

        /// <summary>
        /// 清理和标准化文本
        /// </summary>
        private string CleanAndNormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            try
            {
                // 移除多余的空白字符
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
                
                // 移除特殊字符但保留中文、英文、数字和常用标点
                text = System.Text.RegularExpressions.Regex.Replace(text, @"[^\u4e00-\u9fa5a-zA-Z0-9\s\.\,\;\:\!\?\(\)\[\]\{\}\-\+\=\/\*\%\&\#\@\$\^\~\`\|\<\>]", "");
                
                // 标准化数字格式
                text = System.Text.RegularExpressions.Regex.Replace(text, @"(\d+)\s*\.\s*(\d+)", "$1.$2");
                
                // 移除行首行尾空白
                text = text.Trim();
                
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "文本清理失败，返回原文本");
                return text;
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
                            Text = CleanAndNormalizeText(text),
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
