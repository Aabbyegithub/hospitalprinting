using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Text;
using PaddleOCRSharp;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WinFormsOCR
{
    public partial class Form1 : Form
    {
        private string? _currentImagePath;
        private Bitmap? _currentBitmap;
        private Rectangle _selectionRect;
        private bool _isSelecting;
        private Point _startPoint;
        private bool _hasValidSelection;

        private PaddleOCREngine? _ocrEngine;
        private int _zoomFactor = 3; // 默认放大倍数

        public Form1()
        {
            InitializeComponent();
            
            InitializePaddleOCR();
        }

        private void InitializePaddleOCR()
        {
            try
            {
                // 初始化PaddleOCR引擎
                //var config = new OCRConfig()
                //{
                //    // 使用中英文混合识别
                //    use_angle_cls = true,
                //    lang = "ch",
                //    det_model_dir = "",
                //    rec_model_dir = "",
                //    cls_model_dir = "",
                //    use_gpu = false, // 根据需要设置是否使用GPU
                //    gpu_id = 0,
                //    gpu_mem = 500,
                //    cpu_math_library_num_threads = 4,
                //    enable_mkldnn = true,
                //    det_db_thresh = 0.3f,
                //    det_db_box_thresh = 0.5f,
                //    det_db_unclip_ratio = 1.6f,
                //    det_db_score_mode = "slow",
                //    rec_batch_num = 6,
                //    max_text_length = 25,
                //    rec_char_dict_path = "",
                //    use_space_char = true,
                //    drop_score = 0.5f,
                //    use_tensorrt = false,
                //    use_fp16 = false,
                //    cls_thresh = 0.9f,
                //    cls_batch_num = 6,
                //    cls_image_shape = "3, 48, 192",
                //    label_list = new List<string> { "0", "180" },
                //    cls_image_shape = "3, 48, 192",
                //    cls_batch_num = 6,
                //    cls_thresh = 0.9f,
                //    cls_image_shape = "3, 48, 192",
                //    label_list = new List<string> { "0", "180" }
                //};
                
                _ocrEngine = new PaddleOCREngine();
                toolStripStatusLabel1.Text = "PaddleOCR引擎初始化成功";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PaddleOCR初始化失败: {ex.Message}\n请确保已正确安装PaddleOCRSharp依赖。", 
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "PaddleOCR初始化失败";
            }
        }

        private void 打开图片ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|DICOM文件 (*.dcm;*.dicom)|*.dcm;*.dicom|DICOM UID文件 (无扩展名)|*|所有文件 (*.*)|*.*",
                Title = "选择图片或DICOM文件（支持UID格式）"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                LoadFile(openDialog.FileName);
            }
        }

        private void LoadFile(string filePath)
        {
            try
            {
                this.Text = "正在加载文件...";
                toolStripStatusLabel1.Text = "正在加载文件...";

                var extension = Path.GetExtension(filePath).ToLower();
                Bitmap? loadedBitmap = null;
                string displayInfo = "";

                // 检测是否为DICOM文件（包括UID格式）
                bool isDicomFile = IsDicomFile(filePath, extension);

                // 处理DICOM文件
                if (isDicomFile)
                {
                    toolStripStatusLabel1.Text = "正在转换DICOM文件...";
                    
                    // 验证DICOM文件
                    if (!DicomConverter.IsValidDicomFile(filePath))
                    {
                        // 显示详细的诊断信息
                        var diagnosticInfo = DicomConverter.GetDicomDiagnosticInfo(filePath);
                        MessageBox.Show($"无效的DICOM文件格式\n\n{diagnosticInfo}", 
                            "DICOM文件错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        // 转换DICOM为Bitmap
                        loadedBitmap = DicomConverter.ConvertDicomToBitmap(filePath);
                        
                        // 获取DICOM信息
                        var dicomInfo = DicomConverter.GetDicomInfo(filePath);
                        var patientName = dicomInfo.ContainsKey("患者姓名") ? dicomInfo["患者姓名"] : "未知";
                        var patientId = dicomInfo.ContainsKey("患者ID") ? dicomInfo["患者ID"] : "未知";
                        var modality = dicomInfo.ContainsKey("检查类型") ? dicomInfo["检查类型"] : "未知";
                        
                        displayInfo = $"DICOM文件 | 患者: {patientName} | ID: {patientId} | 类型: {modality} | 尺寸: {loadedBitmap?.Width}x{loadedBitmap?.Height}";
                    }
                    catch (Exception dicomEx)
                    {
                        // 显示详细的错误信息和诊断
                        var diagnosticInfo = DicomConverter.GetDicomDiagnosticInfo(filePath);
                        MessageBox.Show($"DICOM文件转换失败\n\n错误信息: {dicomEx.Message}\n\n{diagnosticInfo}", 
                            "DICOM转换错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // 处理普通图片文件
                    loadedBitmap = new Bitmap(filePath);
                    var fileInfo = new FileInfo(filePath);
                    displayInfo = $"文件: {fileInfo.Name} | 大小: {FormatFileSize(fileInfo.Length)} | 尺寸: {loadedBitmap.Width}x{loadedBitmap.Height}";
                }

                // 释放之前的图片
                if (_currentBitmap != null)
                {
                    _currentBitmap.Dispose();
                }

                _currentBitmap = loadedBitmap;
                
                // 设置图片显示模式为自适应缩放
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = _currentBitmap;
                
                // 强制刷新显示
                pictureBox1.Refresh();
                
                _currentImagePath = filePath;
                _hasValidSelection = false;

                // 更新状态信息
                toolStripStatusLabel1.Text = displayInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "加载失败";
            }
            finally
            {
                this.Text = "OCR图片识别工具";
            }
        }

        private async void 识别全图ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap == null)
            {
                MessageBox.Show("请先打开文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await PerformOCR(null);
        }

        private async void 识别选中区域ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap == null)
            {
                MessageBox.Show("请先打开文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_hasValidSelection)
            {
                MessageBox.Show("请先在图片上拖拽选择要识别的区域", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 截取选区并放大显示
            var croppedBitmap = CropAndEnhanceSelection(_selectionRect);
            if (croppedBitmap != null)
            {
                // 显示放大的选区
                pictureBox2.Image = croppedBitmap;
                pictureBox2.Refresh();
                
                // 更新状态栏显示放大倍数
                toolStripStatusLabel1.Text = $"选区已放大{_zoomFactor}倍并增强";
                
                // 对放大的选区进行OCR识别
                await PerformOCROnCroppedImage(croppedBitmap);
            }
        }

        private async Task PerformOCR(Rectangle? selectionRect)
        {
            try
            {
                toolStripStatusLabel1.Text = "正在识别...";

                Bitmap? imageToProcess = null;

                // 如果指定了区域，裁剪图片
                if (selectionRect.HasValue && _currentBitmap != null)
                {
                    var rect = selectionRect.Value;
                    // 确保区域在图片范围内
                    rect.X = Math.Max(0, Math.Min(rect.X, _currentBitmap.Width));
                    rect.Y = Math.Max(0, Math.Min(rect.Y, _currentBitmap.Height));
                    rect.Width = Math.Min(rect.Width, _currentBitmap.Width - rect.X);
                    rect.Height = Math.Min(rect.Height, _currentBitmap.Height - rect.Y);

                    imageToProcess = _currentBitmap.Clone(rect, _currentBitmap.PixelFormat);
                }
                else
                {
                    imageToProcess = _currentBitmap;
                }

                if (imageToProcess == null)
                {
                    throw new InvalidOperationException("没有可处理的图片");
                }

                // 执行OCR识别
                var result = await RecognizeText(imageToProcess);

                // 显示结果
                textBox1.Text = result;

                // 如果是裁剪的图片，释放资源
                if (selectionRect.HasValue && imageToProcess != _currentBitmap)
                {
                    imageToProcess.Dispose();
                }

                toolStripStatusLabel1.Text = "识别完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"OCR识别失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "识别失败";
            }
        }

        private Task<string> RecognizeText(Bitmap bitmap)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (_ocrEngine == null)
                    {
                        return "OCR引擎未初始化";
                    }

                    // 预处理图片以提高识别率
                    var processedBitmap = PreprocessImageForPaddleOCR(bitmap);
                    
                    // 将Bitmap转换为字节数组
                    using var ms = new MemoryStream();
                    processedBitmap.Save(ms, ImageFormat.Png);
                    var imageBytes = ms.ToArray();

                    // 执行PaddleOCR识别
                    var ocrResult = _ocrEngine.DetectText(imageBytes);
                    
                    // 处理识别结果
                    var sb = new StringBuilder();
                    sb.AppendLine("=== PaddleOCR识别结果 ===");
                    sb.AppendLine($"检测到 {ocrResult.TextBlocks.Count} 个文本块");
                    sb.AppendLine();
                    sb.AppendLine("识别内容:");
                    
                    if (ocrResult.TextBlocks != null && ocrResult.TextBlocks.Count > 0)
                    {
                        var allText = new StringBuilder();
                        var totalConfidence = 0.0f;
                        
                        foreach (var textBlock in ocrResult.TextBlocks)
                        {
                            if (textBlock.Text != null && !string.IsNullOrWhiteSpace(textBlock.Text))
                            {
                                allText.AppendLine(textBlock.Text);
                                totalConfidence += textBlock.Score;
                                
                                // 显示每个文本块的详细信息
                                sb.AppendLine($"[置信度: {textBlock.Score:F2}] {textBlock.Text}");
                            }
                        }
                        
                        var avgConfidence = ocrResult.TextBlocks.Count > 0 ? totalConfidence / ocrResult.TextBlocks.Count : 0;
                        sb.AppendLine();
                        sb.AppendLine($"平均置信度: {avgConfidence:F2}%");
                        sb.AppendLine();
                        sb.AppendLine("完整文本:");
                        sb.AppendLine(allText.ToString());
                    }
                    else
                    {
                        sb.AppendLine("未检测到任何文本");
                    }

                    // 释放处理后的图片
                    if (processedBitmap != bitmap)
                    {
                        processedBitmap.Dispose();
                    }

                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return $"PaddleOCR识别错误: {ex.Message}";
                }
            });
        }

        private Bitmap PreprocessImageForPaddleOCR(Bitmap originalBitmap)
        {
            // PaddleOCR对图像预处理的要求相对较低，主要保持原始图像质量
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

            // 轻微增强对比度，但不过度处理
            var enhancedBitmap = EnhanceContrast(processedBitmap, 1.2f);
            
            // 清理资源
            processedBitmap.Dispose();
            
            return enhancedBitmap;
        }

        private Bitmap ApplyAlternativePreprocessing(Bitmap originalBitmap)
        {
            // 备用预处理方法：更激进的图像增强
            var processedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
            
            using (var g = Graphics.FromImage(processedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                
                g.DrawImage(originalBitmap, 0, 0, originalBitmap.Width, originalBitmap.Height);
            }

            // 转换为灰度图
            var grayBitmap = ConvertToGrayscale(processedBitmap);
            
            // 应用自适应二值化
            var binaryBitmap = ApplyAdaptiveBinaryThreshold(grayBitmap);
            
            // 应用更强的对比度增强
            var enhancedBitmap = EnhanceContrast(binaryBitmap, 1.5f);
            
            // 应用轻微锐化
            var sharpenedBitmap = ApplyGentleSharpen(enhancedBitmap);
            
            // 清理资源
            processedBitmap.Dispose();
            grayBitmap.Dispose();
            binaryBitmap.Dispose();
            enhancedBitmap.Dispose();
            
            return sharpenedBitmap;
        }

        private Bitmap ApplyBinaryThreshold(Bitmap original)
        {
            var binaryBitmap = new Bitmap(original.Width, original.Height);
            
            // 计算自适应阈值
            int threshold = CalculateAdaptiveThreshold(original);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    var pixel = original.GetPixel(x, y);
                    var gray = pixel.R; // 已经是灰度图，RGB值相同
                    
                    // 二值化：大于阈值设为白色(255)，否则设为黑色(0)
                    var binaryValue = gray > threshold ? 255 : 0;
                    binaryBitmap.SetPixel(x, y, Color.FromArgb(binaryValue, binaryValue, binaryValue));
                }
            }
            
            return binaryBitmap;
        }

        private int CalculateAdaptiveThreshold(Bitmap image)
        {
            // 使用Otsu方法计算最佳阈值
            int[] histogram = new int[256];
            
            // 计算直方图
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    var pixel = image.GetPixel(x, y);
                    histogram[pixel.R]++;
                }
            }
            
            // Otsu算法计算最佳阈值
            int totalPixels = image.Width * image.Height;
            float sum = 0;
            for (int i = 0; i < 256; i++)
            {
                sum += i * histogram[i];
            }
            
            float sumB = 0;
            int wB = 0;
            int wF = 0;
            float varMax = 0;
            int threshold = 0;
            
            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t];
                if (wB == 0) continue;
                
                wF = totalPixels - wB;
                if (wF == 0) break;
                
                sumB += (float)(t * histogram[t]);
                float mB = sumB / wB;
                float mF = (sum - sumB) / wF;
                
                float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);
                
                if (varBetween > varMax)
                {
                    varMax = varBetween;
                    threshold = t;
                }
            }
            
            return threshold;
        }

        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            var grayBitmap = new Bitmap(original.Width, original.Height);
            
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    var pixel = original.GetPixel(x, y);
                    var gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    grayBitmap.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            
            return grayBitmap;
        }

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

        private Bitmap ApplySharpen(Bitmap original)
        {
            var sharpenedBitmap = new Bitmap(original.Width, original.Height);
            
            // 锐化核
            float[,] kernel = {
                { 0, -1, 0 },
                { -1, 5, -1 },
                { 0, -1, 0 }
            };
            
            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    float r = 0, g = 0, b = 0;
                    
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            var pixel = original.GetPixel(x + kx, y + ky);
                            var weight = kernel[kx + 1, ky + 1];
                            r += pixel.R * weight;
                            g += pixel.G * weight;
                            b += pixel.B * weight;
                        }
                    }
                    
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));
                    
                    sharpenedBitmap.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
                }
            }
            
            // 复制边缘像素
            for (int x = 0; x < original.Width; x++)
            {
                sharpenedBitmap.SetPixel(x, 0, original.GetPixel(x, 0));
                sharpenedBitmap.SetPixel(x, original.Height - 1, original.GetPixel(x, original.Height - 1));
            }
            for (int y = 0; y < original.Height; y++)
            {
                sharpenedBitmap.SetPixel(0, y, original.GetPixel(0, y));
                sharpenedBitmap.SetPixel(original.Width - 1, y, original.GetPixel(original.Width - 1, y));
            }
            
            return sharpenedBitmap;
        }

        private string TryAlternativeOCR(Bitmap processedBitmap)
        {
            try
            {
                if (_ocrEngine == null)
                {
                    return "OCR引擎未初始化";
                }

                // 尝试不同的图像预处理方法
                var alternativeBitmap = ApplyAlternativePreprocessing(processedBitmap);
                
                // 将Bitmap转换为字节数组
                using var ms = new MemoryStream();
                alternativeBitmap.Save(ms, ImageFormat.Png);
                var imageBytes = ms.ToArray();

                // 执行PaddleOCR识别
                var ocrResult = _ocrEngine.DetectText(imageBytes);
                
                var result = new StringBuilder();
                if (ocrResult.TextBlocks != null && ocrResult.TextBlocks.Count > 0)
                {
                    foreach (var textBlock in ocrResult.TextBlocks)
                    {
                        if (textBlock.Text != null && !string.IsNullOrWhiteSpace(textBlock.Text))
                        {
                            result.AppendLine(textBlock.Text);
                        }
                    }
                }
                
                alternativeBitmap.Dispose();
                return CleanupText(result.ToString());
            }
            catch (Exception ex)
            {
                return $"备用OCR识别失败: {ex.Message}";
            }
        }

        private Bitmap? CropAndEnhanceSelection(Rectangle selectionRect)
        {
            try
            {
                if (_currentBitmap == null) return null;

                // 确保选区在图片范围内
                var rect = new Rectangle(
                    Math.Max(0, selectionRect.X),
                    Math.Max(0, selectionRect.Y),
                    Math.Min(selectionRect.Width, _currentBitmap.Width - Math.Max(0, selectionRect.X)),
                    Math.Min(selectionRect.Height, _currentBitmap.Height - Math.Max(0, selectionRect.Y))
                );

                if (rect.Width <= 0 || rect.Height <= 0) return null;

                // 截取选区
                var croppedBitmap = _currentBitmap.Clone(rect, _currentBitmap.PixelFormat);

                // 使用更高质量的放大算法
                var enlargedBitmap = HighQualityUpscale(croppedBitmap, _zoomFactor);

                // 应用图像增强（但保持原始细节）
                var enhancedBitmap = EnhanceImageForOCRPreservingDetails(enlargedBitmap);

                croppedBitmap.Dispose();
                enlargedBitmap.Dispose();

                return enhancedBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"选区处理失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private Bitmap HighQualityUpscale(Bitmap original, int scaleFactor)
        {
            // 使用Lanczos插值算法进行高质量放大
            var newWidth = original.Width * scaleFactor;
            var newHeight = original.Height * scaleFactor;
            
            var enlargedBitmap = new Bitmap(newWidth, newHeight);
            
            using (var g = Graphics.FromImage(enlargedBitmap))
            {
                // 设置最高质量的插值模式
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                
                // 使用高质量重采样
                var destRect = new Rectangle(0, 0, newWidth, newHeight);
                var srcRect = new Rectangle(0, 0, original.Width, original.Height);
                
                g.DrawImage(original, destRect, srcRect, GraphicsUnit.Pixel);
            }
            
            return enlargedBitmap;
        }

        private Bitmap EnhanceImageForOCRPreservingDetails(Bitmap original)
        {
            // 根据图像大小选择不同的处理策略
            if (original.Width < 50 || original.Height < 50)
            {
                // 对于很小的图像，只进行轻微的对比度增强
                return EnhanceContrast(original, 1.2f);
            }
            else if (original.Width < 150 || original.Height < 150)
            {
                // 对于中等大小的图像，进行轻微的对比度增强和锐化
                var contrastBitmap = EnhanceContrast(original, 1.2f);
                var sharpenedBitmap = ApplyGentleSharpen(contrastBitmap);
                contrastBitmap.Dispose();
                return sharpenedBitmap;
            }
            else
            {
                // 对于较大的图像，可以应用完整的处理流程
                var contrastBitmap = EnhanceContrast(original, 1.3f);
                var sharpenedBitmap = ApplyGentleSharpen(contrastBitmap);
                
                // 转换为灰度图
                var grayBitmap = ConvertToGrayscale(sharpenedBitmap);
                
                // 应用软二值化，保持更多细节
                var binaryBitmap = ApplyAdaptiveBinaryThreshold(grayBitmap);
                
                contrastBitmap.Dispose();
                sharpenedBitmap.Dispose();
                grayBitmap.Dispose();
                
                return binaryBitmap;
            }
        }

        private Bitmap ApplyGentleSharpen(Bitmap original)
        {
            var sharpenedBitmap = new Bitmap(original.Width, original.Height);
            
            // 使用更温和的锐化核
            float[,] kernel = {
                { 0, -0.5f, 0 },
                { -0.5f, 3, -0.5f },
                { 0, -0.5f, 0 }
            };
            
            for (int x = 1; x < original.Width - 1; x++)
            {
                for (int y = 1; y < original.Height - 1; y++)
                {
                    float r = 0, g = 0, b = 0;
                    
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            var pixel = original.GetPixel(x + kx, y + ky);
                            var weight = kernel[kx + 1, ky + 1];
                            r += pixel.R * weight;
                            g += pixel.G * weight;
                            b += pixel.B * weight;
                        }
                    }
                    
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));
                    
                    sharpenedBitmap.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
                }
            }
            
            // 复制边缘像素
            for (int x = 0; x < original.Width; x++)
            {
                sharpenedBitmap.SetPixel(x, 0, original.GetPixel(x, 0));
                sharpenedBitmap.SetPixel(x, original.Height - 1, original.GetPixel(x, original.Height - 1));
            }
            for (int y = 0; y < original.Height; y++)
            {
                sharpenedBitmap.SetPixel(0, y, original.GetPixel(0, y));
                sharpenedBitmap.SetPixel(original.Width - 1, y, original.GetPixel(original.Width - 1, y));
            }
            
            return sharpenedBitmap;
        }

        private Bitmap ApplyAdaptiveBinaryThreshold(Bitmap original)
        {
            var binaryBitmap = new Bitmap(original.Width, original.Height);
            
            // 使用自适应阈值，但保持更多细节
            int threshold = CalculateAdaptiveThreshold(original);
            
            // 使用软阈值，不完全二值化
            for (int x = 0; x < original.Width; x++)
            {
                for (int y = 0; y < original.Height; y++)
                {
                    var pixel = original.GetPixel(x, y);
                    var gray = pixel.R;
                    
                    // 软阈值处理，保持边缘细节
                    int binaryValue;
                    if (gray > threshold + 20)
                        binaryValue = 255;
                    else if (gray < threshold - 20)
                        binaryValue = 0;
                    else
                        binaryValue = (int)((gray - threshold + 20) * 255.0 / 40.0);
                    
                    binaryValue = Math.Max(0, Math.Min(255, binaryValue));
                    binaryBitmap.SetPixel(x, y, Color.FromArgb(binaryValue, binaryValue, binaryValue));
                }
            }
            
            return binaryBitmap;
        }

        private Bitmap EnhanceImageForOCR(Bitmap original)
        {
            // 转换为灰度图
            var grayBitmap = ConvertToGrayscale(original);
            
            // 应用二值化处理
            var binaryBitmap = ApplyBinaryThreshold(grayBitmap);
            
            // 应用对比度增强
            var enhancedBitmap = EnhanceContrast(binaryBitmap, 2.5f);
            
            // 应用锐化
            var sharpenedBitmap = ApplySharpen(enhancedBitmap);
            
            // 清理资源
            grayBitmap.Dispose();
            binaryBitmap.Dispose();
            enhancedBitmap.Dispose();
            
            return sharpenedBitmap;
        }

        private async Task PerformOCROnCroppedImage(Bitmap croppedBitmap)
        {
            try
            {
                toolStripStatusLabel1.Text = "正在识别放大的选区...";

                // 执行OCR识别
                var result = await RecognizeText(croppedBitmap);

                // 显示结果
                textBox1.Text = result;
                toolStripStatusLabel1.Text = "选区识别完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"OCR识别失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "识别失败";
            }
        }

        private string CleanupText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
                
            // 保持原始换行，只清理每行的首尾空白
            var lines = text.Split('\n');
            var cleanedLines = new List<string>();
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    cleanedLines.Add(trimmedLine);
                }
            }
            
            // 如果所有行都被移除了，返回原始文本
            if (cleanedLines.Count == 0)
            {
                return text.Trim();
            }
            
            return string.Join("\n", cleanedLines);
        }

        // 鼠标事件处理 - 支持区域选择
        private void pictureBox1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_currentBitmap == null) return;

            if (e.Button == MouseButtons.Left)
            {
                _isSelecting = true;
                _startPoint = e.Location;
                _hasValidSelection = false;
            }
        }

        private void pictureBox1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_currentBitmap == null || !_isSelecting) return;

            var currentPoint = e.Location;
            var x = Math.Min(_startPoint.X, currentPoint.X);
            var y = Math.Min(_startPoint.Y, currentPoint.Y);
            var width = Math.Abs(currentPoint.X - _startPoint.X);
            var height = Math.Abs(currentPoint.Y - _startPoint.Y);

            _selectionRect = new Rectangle(x, y, width, height);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_currentBitmap == null) return;

            if (e.Button == MouseButtons.Left && _isSelecting)
            {
                _isSelecting = false;
                
                // 检查选择区域是否有效
                if (_selectionRect.Width > 10 && _selectionRect.Height > 10)
                {
                    _hasValidSelection = true;
                    toolStripStatusLabel1.Text = $"已选择区域: {_selectionRect.Width}x{_selectionRect.Height}";
                }
                else
                {
                    _hasValidSelection = false;
                    toolStripStatusLabel1.Text = "选择区域太小，请重新选择";
                }
            }
        }

        private void pictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            // 绘制选择框
            if (_isSelecting || _hasValidSelection)
            {
                using var pen = new Pen(Color.Red, 2);
                e.Graphics.DrawRectangle(pen, _selectionRect);
            }
        }

        /// <summary>
        /// 检测文件是否为DICOM文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="extension">文件扩展名</param>
        /// <returns>是否为DICOM文件</returns>
        private bool IsDicomFile(string filePath, string extension)
        {
            // 检查标准DICOM扩展名
            if (extension == ".dcm" || extension == ".dicom")
            {
                return true;
            }

            // 检查文件名是否为UID格式（无扩展名）
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (IsDicomUidFormat(fileName))
            {
                return true;
            }

            // 检查文件内容是否为DICOM格式
            try
            {
                return DicomConverter.IsValidDicomFile(filePath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文件名是否为DICOM UID格式
        /// </summary>
        /// <param name="fileName">文件名（不含扩展名）</param>
        /// <returns>是否为UID格式</returns>
        private bool IsDicomUidFormat(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // DICOM UID格式：数字和点组成，通常很长
            // 例如：1.2.826.0.1.3680043.6.625.28974.20250802183439.328.26511.2025.08.02.18.34.43.2076
            var uidPattern = @"^\d+(\.\d+)+$";
            return System.Text.RegularExpressions.Regex.IsMatch(fileName, uidPattern) && fileName.Length > 20;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void 显示DICOM信息ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentImagePath == null)
            {
                MessageBox.Show("请先打开文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var extension = Path.GetExtension(_currentImagePath).ToLower();
            bool isDicomFile = IsDicomFile(_currentImagePath, extension);
            if (!isDicomFile)
            {
                MessageBox.Show("当前文件不是DICOM文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var dicomInfo = DicomConverter.GetDicomInfo(_currentImagePath);
                
                var infoText = new StringBuilder();
                infoText.AppendLine("=== DICOM文件信息 ===");
                infoText.AppendLine();
                
                foreach (var kvp in dicomInfo)
                {
                    infoText.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
                
                // 显示在识别结果区域
                textBox1.Text = infoText.ToString();
                toolStripStatusLabel1.Text = "DICOM信息已显示";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取DICOM信息失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 显示DICOM诊断ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentImagePath == null)
            {
                MessageBox.Show("请先打开文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var extension = Path.GetExtension(_currentImagePath).ToLower();
            if (extension != ".dcm" && extension != ".dicom")
            {
                MessageBox.Show("当前文件不是DICOM文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var diagnosticInfo = DicomConverter.GetDicomDiagnosticInfo(_currentImagePath);
                
                // 显示在识别结果区域
                textBox1.Text = diagnosticInfo;
                toolStripStatusLabel1.Text = "DICOM诊断信息已显示";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取DICOM诊断信息失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 退出ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void 清除选区ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            _hasValidSelection = false;
            _isSelecting = false;
            pictureBox1.Invalidate();
            
            // 清除放大显示
            pictureBox2.Image = null;
            pictureBox2.Refresh();
            
            toolStripStatusLabel1.Text = "选区已清除";
        }

        private void btnCopy_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Clipboard.SetText(textBox1.Text);
                toolStripStatusLabel1.Text = "文本已复制到剪贴板";
            }
        }

        private void 适应窗口ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Refresh();
                toolStripStatusLabel1.Text = "图片已适应窗口大小";
            }
        }

        private void 原始大小ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Refresh();
                toolStripStatusLabel1.Text = "显示原始大小";
            }
        }

        private void 适应宽度ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Refresh();
                toolStripStatusLabel1.Text = "图片已适应宽度";
            }
        }

        private void 适应高度ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (_currentBitmap != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox1.Refresh();
                toolStripStatusLabel1.Text = "图片已适应高度";
            }
        }

        // 动态调整放大倍数的方法
        public void SetZoomFactor(int factor)
        {
            _zoomFactor = Math.Max(1, Math.Min(6, factor)); // 限制在1-6倍之间
            toolStripStatusLabel1.Text = $"放大倍数已设置为: {_zoomFactor}倍";
        }

        // 获取当前放大倍数
        public int GetZoomFactor()
        {
            return _zoomFactor;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 清理资源
            _currentBitmap?.Dispose();
            _ocrEngine?.Dispose();
            
            base.OnFormClosing(e);
        }
    }
}