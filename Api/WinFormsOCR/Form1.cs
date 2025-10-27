using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Text;
using Tesseract;
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

        private readonly string _tessDataPath;
        private int _zoomFactor = 3; // 默认放大倍数

        public Form1()
        {
            InitializeComponent();
            
            // 设置Tesseract数据路径
            _tessDataPath = Path.Combine(Application.StartupPath, "tessdata");
            
            InitializeTesseract();
        }

        private void InitializeTesseract()
        {
            // 检查tessdata目录是否存在
            if (!Directory.Exists(_tessDataPath))
            {
                MessageBox.Show($"Tesseract数据目录不存在: {_tessDataPath}\n请确保有中英文训练数据文件。", 
                    "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void 打开图片ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "图片文件 (*.jpg;*.jpeg;*.png;*.bmp;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|DICOM文件 (*.dcm;*.dicom)|*.dcm;*.dicom|所有文件 (*.*)|*.*",
                Title = "选择图片或DICOM文件"
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

                // 处理DICOM文件
                if (extension == ".dcm" || extension == ".dicom")
                {
                    toolStripStatusLabel1.Text = "正在转换DICOM文件...";
                    
                    // 验证DICOM文件
                    if (!DicomConverter.IsValidDicomFile(filePath))
                    {
                        MessageBox.Show("无效的DICOM文件格式", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 转换DICOM为Bitmap
                    loadedBitmap = DicomConverter.ConvertDicomToBitmap(filePath);
                    
                    // 获取DICOM信息
                    var dicomInfo = DicomConverter.GetDicomInfo(filePath);
                    var patientName = dicomInfo.ContainsKey("患者姓名") ? dicomInfo["患者姓名"] : "未知";
                    var patientId = dicomInfo.ContainsKey("患者ID") ? dicomInfo["患者ID"] : "未知";
                    var modality = dicomInfo.ContainsKey("检查类型") ? dicomInfo["检查类型"] : "未知";
                    
                    displayInfo = $"DICOM文件 | 患者: {patientName} | ID: {patientId} | 类型: {modality} | 尺寸: {loadedBitmap?.Width}x{loadedBitmap?.Height}";
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
                    // 预处理图片以提高识别率
                    var processedBitmap = PreprocessImage(bitmap);
                    
                    using var engine = new TesseractEngine(_tessDataPath, "chi_sim+eng", EngineMode.Default);
                    
                    // 优化OCR引擎配置 - 针对中英文混合医疗文本
                    engine.SetVariable("tessedit_pageseg_mode", "1"); // 自动页面分割
                    engine.SetVariable("tessedit_ocr_engine_mode", "1"); // LSTM OCR引擎
                    
                    // 启用所有词典
                    engine.SetVariable("load_system_dawg", "1");
                    engine.SetVariable("load_freq_dawg", "1");
                    engine.SetVariable("load_punc_dawg", "1");
                    engine.SetVariable("load_number_dawg", "1");
                    engine.SetVariable("load_unambig_dawg", "1");
                    engine.SetVariable("load_bigram_dawg", "1");
                    engine.SetVariable("load_fixed_length_dawg", "1");
                    
                    // 移除字符白名单限制，允许识别所有字符包括中文
                    // engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.:/-+()[]{} ");
                    
                    // 提高识别精度的参数
                    engine.SetVariable("classify_bln_numeric_mode", "1");
                    engine.SetVariable("textord_min_linesize", "2.0");
                    engine.SetVariable("textord_old_baselines", "0");
                    engine.SetVariable("textord_old_xheight", "0");
                    engine.SetVariable("textord_min_xheight", "6");
                    engine.SetVariable("textord_force_make_prop_words", "F");
                    
                    // 针对医疗文本的特殊配置
                    engine.SetVariable("tessedit_char_blacklist", ""); // 不排除任何字符
                    engine.SetVariable("tessedit_enable_doc_dict", "1"); // 启用文档词典
                    engine.SetVariable("tessedit_enable_bigram_correction", "1"); // 启用双字符校正
                    
                    // 提高中文字符识别
                    engine.SetVariable("tessedit_do_invert", "0"); // 不反转图像
                    engine.SetVariable("textord_really_old_xheight", "1"); // 使用旧版xheight计算
                    engine.SetVariable("textord_min_xheight", "6"); // 降低最小字符高度
                    
                    // 保持换行和格式
                    engine.SetVariable("preserve_interword_spaces", "1"); // 保持单词间空格
                    engine.SetVariable("tessedit_create_hocr", "0"); // 不使用hOCR格式

                    // 将处理后的Bitmap转换为Pix
                    using var ms = new MemoryStream();
                    processedBitmap.Save(ms, ImageFormat.Png);
                    using var pix = Pix.LoadFromMemory(ms.ToArray());

                    // 执行识别
                    using var page = engine.Process(pix);
                    var text = page.GetText();
                    var confidence = page.GetMeanConfidence();

                    // 如果置信度很低，尝试使用不同的参数重新识别
                    if (confidence < 0.3f)
                    {
                        text = TryAlternativeOCR(processedBitmap);
                    }

                    // 清理识别结果
                    text = CleanupText(text);

                    var sb = new StringBuilder();
                    sb.AppendLine("=== OCR识别结果 ===");
                    sb.AppendLine($"置信度: {confidence:F2}%");
                    sb.AppendLine();
                    sb.AppendLine("识别内容:");
                    sb.AppendLine(text);

                    // 释放处理后的图片
                    if (processedBitmap != bitmap)
                    {
                        processedBitmap.Dispose();
                    }

                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    return $"OCR识别错误: {ex.Message}";
                }
            });
        }

        private Bitmap PreprocessImage(Bitmap originalBitmap)
        {
            // 创建处理后的图片
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

            // 转换为灰度图
            var grayBitmap = ConvertToGrayscale(processedBitmap);
            
            // 应用二值化处理（针对黑白对比强烈的医疗影像）
            var binaryBitmap = ApplyBinaryThreshold(grayBitmap);
            
            // 应用对比度增强
            var enhancedBitmap = EnhanceContrast(binaryBitmap, 2.0f);
            
            // 应用锐化
            var sharpenedBitmap = ApplySharpen(enhancedBitmap);
            
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
                using var engine = new TesseractEngine(_tessDataPath, "chi_sim+eng", EngineMode.Default);
                
                // 使用更宽松的参数配置
                engine.SetVariable("tessedit_pageseg_mode", "3"); // 完全自动页面分割
                engine.SetVariable("tessedit_ocr_engine_mode", "1");
                
                // 启用所有词典
                engine.SetVariable("load_system_dawg", "1");
                engine.SetVariable("load_freq_dawg", "1");
                engine.SetVariable("load_punc_dawg", "1");
                engine.SetVariable("load_number_dawg", "1");
                
                // 更宽松的文本检测参数
                engine.SetVariable("textord_min_linesize", "1.5");
                engine.SetVariable("textord_min_xheight", "4");
                engine.SetVariable("textord_force_make_prop_words", "T");
                
                // 保持空格和换行
                engine.SetVariable("preserve_interword_spaces", "1");
                
                // 将处理后的Bitmap转换为Pix
                using var ms = new MemoryStream();
                processedBitmap.Save(ms, ImageFormat.Png);
                using var pix = Pix.LoadFromMemory(ms.ToArray());

                // 执行识别
                using var page = engine.Process(pix);
                var text = page.GetText();
                
                return CleanupText(text);
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
            if (extension != ".dcm" && extension != ".dicom")
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
            
            base.OnFormClosing(e);
        }
    }
}