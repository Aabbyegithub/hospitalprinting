using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ModelClassLibrary.Model.Dto.HolDto;
using PaddleOCRSharp;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.OCRIServices;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WebServiceClass.Services.OCRServices
{
    /// <summary>
    /// OCR识别服务实现
    /// 参考WinFormsOCR项目，使用PaddleOCR进行识别
    /// </summary>
    public class OCRRecognitionService : IBaseService, IOCRRecognitionService
    {
        private readonly PaddleOCREngine _ocrEngine;
        private readonly HttpClient _httpClient;

        public OCRRecognitionService()
        {
            _httpClient = new HttpClient();
            
            try
            {
                // 初始化PaddleOCR引擎
                _ocrEngine = new PaddleOCREngine();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 识别图片或PDF文件
        /// 支持本地文件路径、URL链接、共享文件夹路径（UNC路径）
        /// </summary>
        public async Task<ApiResponse<OcrRecognitionDto>> RecognizeFileAsync(string filePath)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return Error<OcrRecognitionDto>("文件路径不能为空");
                }


                // 获取文件字节数组（支持多种路径类型）
                byte[] fileBytes;
                string actualFilePath = filePath;

                // 判断路径类型并获取文件
                if (IsUrl(filePath))
                {
                    // URL路径：从URL下载文件
                    fileBytes = await DownloadFileFromUrlAsync(filePath);
                    actualFilePath = filePath; // 保留原始URL
                }
                else if (IsUncPath(filePath))
                {
                    // UNC路径（共享文件夹）：直接读取网络路径文件
                    fileBytes = await ReadFileFromUncPathAsync(filePath);
                    actualFilePath = filePath;
                }
                else
                {
                    // 本地文件路径
                    if (!File.Exists(filePath))
                    {
                        return Error<OcrRecognitionDto>($"文件不存在: {filePath}");
                    }
                    fileBytes = await File.ReadAllBytesAsync(filePath);
                    actualFilePath = filePath;
                }

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return Error<OcrRecognitionDto>("无法读取文件内容");
                }

                // 根据文件扩展名判断处理方式
                var extension = Path.GetExtension(filePath).ToLower();
                OcrRecognitionDto result;

                if (extension == ".pdf")
                {
                    result = await RecognizePdfAsync(fileBytes, actualFilePath);
                }
                else if (new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" }.Contains(extension))
                {
                    result = await RecognizeImageAsync(fileBytes, actualFilePath);
                }
                else
                {
                    return Error<OcrRecognitionDto>($"不支持的文件格式: {extension}");
                }

                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                return Success(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                return Error<OcrRecognitionDto>($"识别失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 识别上传的文件
        /// </summary>
        public async Task<ApiResponse<OcrRecognitionDto>> RecognizeUploadedFileAsync(byte[] fileBytes, string fileName)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return Error<OcrRecognitionDto>("文件内容不能为空");
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return Error<OcrRecognitionDto>("文件名不能为空");
                }

                // 根据文件扩展名判断处理方式
                var extension = Path.GetExtension(fileName).ToLower();
                OcrRecognitionDto result;

                if (extension == ".pdf")
                {
                    result = await RecognizePdfAsync(fileBytes, fileName);
                }
                else if (new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif" }.Contains(extension))
                {
                    result = await RecognizeImageAsync(fileBytes, fileName);
                }
                else
                {
                    return Error<OcrRecognitionDto>($"不支持的文件格式: {extension}");
                }

                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

                return Success(result);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                return Error<OcrRecognitionDto>($"识别失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 识别PDF文件
        /// </summary>
        private async Task<OcrRecognitionDto> RecognizePdfAsync(byte[] pdfBytes, string filePath)
        {
            try
            {
                // 使用PaddleOCRSharp.PDF的DetectTextPDF方法处理PDF
                dynamic pdfOcrResult = await Task.Run(() =>
                {
                    // DetectTextPDF可以接受字节数组或文件路径
                    // 这里使用字节数组
                    return _ocrEngine.DetectTextPDF(pdfBytes);
                });

                return ParsePdfOcrResult(pdfOcrResult, filePath);
            }
            catch (Exception ex)
            {
                return new OcrRecognitionDto
                {
                    Success = false,
                    FilePath = filePath,
                    ErrorMessage = $"PDF识别失败: {ex.Message}",
                    TextBlocks = new List<TextBlockDto>()
                };
            }
        }

        /// <summary>
        /// 识别图片文件
        /// </summary>
        private async Task<OcrRecognitionDto> RecognizeImageAsync(byte[] imageBytes, string filePath)
        {
            try
            {
                // 预处理图片以提高识别率（参考WinFormsOCR的实现）
                Bitmap processedBitmap = null;
                try
                {
                    using var ms = new MemoryStream(imageBytes);
                    var originalBitmap = new Bitmap(ms);
                    processedBitmap = PreprocessImageForPaddleOCR(originalBitmap);
                    originalBitmap.Dispose();

                    // 将处理后的Bitmap转换为字节数组
                    using var processedMs = new MemoryStream();
                    processedBitmap.Save(processedMs, ImageFormat.Png);
                    imageBytes = processedMs.ToArray();
                }
                catch (Exception ex)
                {
                    // 如果预处理失败，使用原始图片
                }

                // 执行PaddleOCR识别
                var ocrResult = await Task.Run(() =>
                {
                    return _ocrEngine.DetectText(imageBytes);
                });

                if (processedBitmap != null)
                {
                    processedBitmap.Dispose();
                }

                return ParseOcrResult(ocrResult, filePath);
            }
            catch (Exception ex)
            {
                return new OcrRecognitionDto
                {
                    Success = false,
                    FilePath = filePath,
                    ErrorMessage = $"图片识别失败: {ex.Message}",
                    TextBlocks = new List<TextBlockDto>()
                };
            }
        }

        /// <summary>
        /// 解析OCR识别结果
        /// </summary>
        private OcrRecognitionDto ParseOcrResult(dynamic ocrResult, string filePath)
        {
            var result = new OcrRecognitionDto
            {
                FilePath = filePath,
                TextBlocks = new List<TextBlockDto>()
            };

            try
            {
                if (ocrResult == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "OCR识别结果为空";
                    result.Text = "";
                    result.Confidence = 0;
                    result.TextBlockCount = 0;
                    return result;
                }

                // 使用反射或dynamic访问TextBlocks属性
                var textBlocksProperty = ocrResult.GetType().GetProperty("TextBlocks");
                if (textBlocksProperty == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "OCR结果格式不正确：缺少TextBlocks属性";
                    return result;
                }

                var textBlocks = textBlocksProperty.GetValue(ocrResult) as System.Collections.IEnumerable;
                if (textBlocks == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "未检测到任何文本";
                    result.Text = "";
                    result.Confidence = 0;
                    result.TextBlockCount = 0;
                    return result;
                }

                var allText = new StringBuilder();
                var totalConfidence = 0.0f;
                int validBlockCount = 0;

                foreach (dynamic textBlock in textBlocks)
                {
                    try
                    {
                        var textProperty = textBlock.GetType().GetProperty("Text");
                        var scoreProperty = textBlock.GetType().GetProperty("Score");
                        var boundingBoxProperty = textBlock.GetType().GetProperty("BoundingBox");

                        if (textProperty == null || scoreProperty == null)
                            continue;

                        var text = textProperty.GetValue(textBlock) as string;
                        var score = scoreProperty?.GetValue(textBlock);
                        float scoreValue = 0f;

                        if (score != null)
                        {
                            if (float.TryParse(score.ToString() ?? "0", out float f))
                                scoreValue = f;
                            else if (score is float fScore)
                                scoreValue = fScore;
                        }

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            allText.AppendLine(text);

                            var textBlockDto = new TextBlockDto
                            {
                                Text = text,
                                Score = scoreValue
                            };

                            // 处理边界框
                            if (boundingBoxProperty != null)
                            {
                                var boundingBox = boundingBoxProperty.GetValue(textBlock);
                                if (boundingBox != null)
                                {
                                    var xProp = boundingBox.GetType().GetProperty("X");
                                    var yProp = boundingBox.GetType().GetProperty("Y");
                                    var widthProp = boundingBox.GetType().GetProperty("Width");
                                    var heightProp = boundingBox.GetType().GetProperty("Height");

                                    textBlockDto.BoundingBox = new BoundingBoxDto
                                    {
                                        X = GetIntValue(xProp?.GetValue(boundingBox)),
                                        Y = GetIntValue(yProp?.GetValue(boundingBox)),
                                        Width = GetIntValue(widthProp?.GetValue(boundingBox)),
                                        Height = GetIntValue(heightProp?.GetValue(boundingBox))
                                    };
                                }
                            }

                            if (textBlockDto.BoundingBox == null)
                            {
                                textBlockDto.BoundingBox = new BoundingBoxDto();
                            }

                            result.TextBlocks.Add(textBlockDto);
                            totalConfidence += scoreValue;
                            validBlockCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                result.Success = validBlockCount > 0;
                result.Text = allText.ToString().TrimEnd('\r', '\n');
                result.Confidence = validBlockCount > 0 ? totalConfidence / validBlockCount : 0;
                result.TextBlockCount = validBlockCount;
                result.ErrorMessage = result.Success ? "" : "未检测到有效文本";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"解析OCR结果失败: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// 解析PDF OCR识别结果
        /// </summary>
        private OcrRecognitionDto ParsePdfOcrResult(dynamic pdfOcrResult, string filePath)
        {
            var result = new OcrRecognitionDto
            {
                FilePath = filePath,
                TextBlocks = new List<TextBlockDto>()
            };

            try
            {
                if (pdfOcrResult == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "PDF OCR识别结果为空";
                    result.Text = "";
                    result.Confidence = 0;
                    result.TextBlockCount = 0;
                    return result;
                }

                // 使用反射访问Pages属性（PDF结果包含多个页面）
                var pagesProperty = pdfOcrResult.GetType().GetProperty("Pages");
                if (pagesProperty == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "PDF OCR结果格式不正确：缺少Pages属性";
                    return result;
                }

                var pages = pagesProperty.GetValue(pdfOcrResult) as System.Collections.IEnumerable;
                if (pages == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "PDF中没有检测到任何页面";
                    result.Text = "";
                    result.Confidence = 0;
                    result.TextBlockCount = 0;
                    return result;
                }

                var allText = new StringBuilder();
                var totalConfidence = 0.0f;
                int validBlockCount = 0;
                int pageNumber = 0;

                // 遍历所有页面
                foreach (dynamic page in pages)
                {
                    pageNumber++;
                    
                    try
                    {
                        // 获取页面的TextBlocks属性
                        var textBlocksProperty = page.GetType().GetProperty("TextBlocks");
                        if (textBlocksProperty == null)
                        {
                            // 如果没有TextBlocks，尝试直接获取Text属性
                            var textProperty = page.GetType().GetProperty("Text");
                            if (textProperty != null)
                            {
                                var pageText = textProperty.GetValue(page) as string;
                                if (!string.IsNullOrWhiteSpace(pageText))
                                {
                                    allText.AppendLine($"=== 第{pageNumber}页 ===");
                                    allText.AppendLine(pageText);
                                    allText.AppendLine();
                                }
                            }
                            continue;
                        }

                        var textBlocks = textBlocksProperty.GetValue(page) as System.Collections.IEnumerable;
                        if (textBlocks != null)
                        {
                            allText.AppendLine($"=== 第{pageNumber}页 ===");
                            
                            foreach (dynamic textBlock in textBlocks)
                            {
                                try
                                {
                                    var textProperty = textBlock.GetType().GetProperty("Text");
                                    var scoreProperty = textBlock.GetType().GetProperty("Score");
                                    var boundingBoxProperty = textBlock.GetType().GetProperty("BoundingBox");

                                    if (textProperty == null || scoreProperty == null)
                                        continue;

                                    var text = textProperty.GetValue(textBlock) as string;
                                    var score = scoreProperty?.GetValue(textBlock);
                                    float scoreValue = 0f;

                                    if (score != null)
                                    {
                                        if (float.TryParse(score.ToString() ?? "0", out float f))
                                            scoreValue = f;
                                        else if (score is float fScore)
                                            scoreValue = fScore;
                                    }

                                    if (!string.IsNullOrWhiteSpace(text))
                                    {
                                        allText.AppendLine(text);

                                        var textBlockDto = new TextBlockDto
                                        {
                                            Text = text,
                                            Score = scoreValue,
                                            PageNumber = pageNumber
                                        };

                                        // 处理边界框
                                        if (boundingBoxProperty != null)
                                        {
                                            var boundingBox = boundingBoxProperty.GetValue(textBlock);
                                            if (boundingBox != null)
                                            {
                                                var xProp = boundingBox.GetType().GetProperty("X");
                                                var yProp = boundingBox.GetType().GetProperty("Y");
                                                var widthProp = boundingBox.GetType().GetProperty("Width");
                                                var heightProp = boundingBox.GetType().GetProperty("Height");

                                                textBlockDto.BoundingBox = new BoundingBoxDto
                                                {
                                                    X = GetIntValue(xProp?.GetValue(boundingBox)),
                                                    Y = GetIntValue(yProp?.GetValue(boundingBox)),
                                                    Width = GetIntValue(widthProp?.GetValue(boundingBox)),
                                                    Height = GetIntValue(heightProp?.GetValue(boundingBox))
                                                };
                                            }
                                        }

                                        if (textBlockDto.BoundingBox == null)
                                        {
                                            textBlockDto.BoundingBox = new BoundingBoxDto();
                                        }

                                        result.TextBlocks.Add(textBlockDto);
                                        totalConfidence += scoreValue;
                                        validBlockCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                            
                            allText.AppendLine();
                        }
                        else
                        {
                            // 如果页面没有TextBlocks，尝试直接获取Text
                            var textProperty = page.GetType().GetProperty("Text");
                            if (textProperty != null)
                            {
                                var pageText = textProperty.GetValue(page) as string;
                                if (!string.IsNullOrWhiteSpace(pageText))
                                {
                                    allText.AppendLine($"=== 第{pageNumber}页 ===");
                                    allText.AppendLine(pageText);
                                    allText.AppendLine();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }

                result.Success = validBlockCount > 0 || allText.Length > 0;
                result.Text = allText.ToString().TrimEnd('\r', '\n');
                result.Confidence = validBlockCount > 0 ? totalConfidence / validBlockCount : 0;
                result.TextBlockCount = validBlockCount;
                result.ErrorMessage = result.Success ? "" : "PDF中未检测到有效文本";

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"解析PDF OCR结果失败: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// 获取整数值
        /// </summary>
        private int GetIntValue(object value)
        {
            if (value == null) return 0;
            if (value is int i) return i;
            if (int.TryParse(value.ToString(), out int result)) return result;
            return 0;
        }

        /// <summary>
        /// 预处理图片以提高识别率（参考WinFormsOCR的实现）
        /// </summary>
        private Bitmap PreprocessImageForPaddleOCR(Bitmap originalBitmap)
        {
            var processedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
            
            using (var g = Graphics.FromImage(processedBitmap))
            {
                // 设置高质量渲染
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                
                // 绘制原图
                g.DrawImage(originalBitmap, 0, 0, originalBitmap.Width, originalBitmap.Height);
            }

            // 轻微增强对比度
            var enhancedBitmap = EnhanceContrast(processedBitmap, 1.2f);
            
            // 清理资源
            processedBitmap.Dispose();
            
            return enhancedBitmap;
        }

        /// <summary>
        /// 增强对比度
        /// </summary>
        private Bitmap EnhanceContrast(Bitmap original, float contrast)
        {
            var enhancedBitmap = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    var pixel = original.GetPixel(x, y);
                    var r = Math.Max(0, Math.Min(255, (int)((pixel.R - 128) * contrast + 128)));
                    var g = Math.Max(0, Math.Min(255, (int)((pixel.G - 128) * contrast + 128)));
                    var b = Math.Max(0, Math.Min(255, (int)((pixel.B - 128) * contrast + 128)));
                    enhancedBitmap.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            
            return enhancedBitmap;
        }

        /// <summary>
        /// 判断是否为URL路径
        /// </summary>
        private bool IsUrl(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && 
                   (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    path.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 判断是否为UNC路径（共享文件夹路径）
        /// </summary>
        private bool IsUncPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && 
                   (path.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 从URL下载文件
        /// </summary>
        private async Task<byte[]> DownloadFileFromUrlAsync(string url)
        {
            try
            {                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
              
                
                return fileBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"从URL下载文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从UNC路径读取文件（共享文件夹）
        /// </summary>
        private async Task<byte[]> ReadFileFromUncPathAsync(string uncPath)
        {
            try
            {                
                // UNC路径可以直接使用File.ReadAllBytesAsync读取
                // 但需要确保运行服务的账户有访问共享文件夹的权限
                if (!File.Exists(uncPath))
                {
                    throw new FileNotFoundException($"UNC路径文件不存在: {uncPath}");
                }
                
                var fileBytes = await File.ReadAllBytesAsync(uncPath);
                
                return fileBytes;
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception($"访问共享文件夹权限不足，请确保运行服务的账户有访问权限: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"从共享文件夹读取文件失败: {ex.Message}", ex);
            }
        }
    }
}

