using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WindowsOCR
{
    public partial class Form1 : Form
    {
        private Image originalImage = null;
        private bool isSelecting = false;
        private Point selectionStart;
        private Rectangle selectionRect = Rectangle.Empty;
        
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开图片
        /// </summary>
        private void 打开图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.tiff;*.gif|所有文件|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    originalImage = Image.FromFile(openFileDialog.FileName);
                    pictureBox1.Image = (Image)originalImage.Clone();
                    selectionRect = Rectangle.Empty;
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    
                    toolStripStatusLabel1.Text = $"已加载图片: {Path.GetFileName(openFileDialog.FileName)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"无法加载图片: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 鼠标按下开始选择
        /// </summary>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null)
                return;

            isSelecting = true;
            selectionStart = e.Location;
            selectionRect = Rectangle.Empty;
        }

        /// <summary>
        /// 鼠标移动更新选择区域
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting || pictureBox1.Image == null)
                return;

            int x = Math.Min(selectionStart.X, e.X);
            int y = Math.Min(selectionStart.Y, e.Y);
            int width = Math.Abs(e.X - selectionStart.X);
            int height = Math.Abs(e.Y - selectionStart.Y);

            selectionRect = new Rectangle(x, y, width, height);

            // 重绘显示选择框
            pictureBox1.Invalidate();

            // 更新状态栏显示坐标
            toolStripStatusLabel1.Text = $"选择区域: ({x}, {y}) 大小: {width}x{height}";
        }

        /// <summary>
        /// 鼠标抬起完成选择
        /// </summary>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isSelecting = false;
        }

        /// <summary>
        /// 绘制选择框
        /// </summary>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectionRect != Rectangle.Empty && !selectionRect.IsEmpty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, selectionRect);
                }
            }
        }

        /// <summary>
        /// 识别选中区域
        /// </summary>
        private async void 识别选中区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("请先打开图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (selectionRect == Rectangle.Empty || selectionRect.Width == 0 || selectionRect.Height == 0)
            {
                MessageBox.Show("请先选择要识别的区域", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            toolStripStatusLabel1.Text = "正在识别...";
            textBox1.Text = "正在识别中...";
            Application.DoEvents();

            try
            {
                // 获取选中的图片区域
                Bitmap selectedImage = GetSelectedImage();

                // 进行OCR识别
                string result = await PerformOCR(selectedImage);

                textBox1.Text = result;
                toolStripStatusLabel1.Text = "识别完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"识别失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "识别失败";
            }
        }

        /// <summary>
        /// 识别全图
        /// </summary>
        private async void 识别全图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("请先打开图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            toolStripStatusLabel1.Text = "正在识别...";
            textBox1.Text = "正在识别中...";
            Application.DoEvents();

            try
            {
                // 使用全图进行OCR识别
                string result = await PerformOCR((Bitmap)pictureBox1.Image);

                textBox1.Text = result;
                toolStripStatusLabel1.Text = "识别完成";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"识别失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.Text = "识别失败";
            }
        }

        /// <summary>
        /// 清除选区
        /// </summary>
        private void 清除选区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionRect = Rectangle.Empty;
            pictureBox1.Invalidate();
            toolStripStatusLabel1.Text = "已清除选区";
        }

        /// <summary>
        /// 复制结果
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Clipboard.SetText(textBox1.Text);
                toolStripStatusLabel1.Text = "已复制到剪贴板";
            }
        }

        /// <summary>
        /// 获取选中区域的图片
        /// </summary>
        private Bitmap GetSelectedImage()
        {
            Bitmap source = (Bitmap)pictureBox1.Image;
            if (selectionRect.X < 0 || selectionRect.Y < 0 || 
                selectionRect.X + selectionRect.Width > source.Width || 
                selectionRect.Y + selectionRect.Height > source.Height)
            {
                // 边界检查
                Rectangle safeRect = Rectangle.Intersect(
                    new Rectangle(0, 0, source.Width, source.Height),
                    selectionRect);
                
                Bitmap cropped = new Bitmap(safeRect.Width, safeRect.Height);
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(source, 
                        new Rectangle(0, 0, safeRect.Width, safeRect.Height),
                        safeRect, GraphicsUnit.Pixel);
                }
                return cropped;
            }
            else
            {
                Bitmap cropped = new Bitmap(selectionRect.Width, selectionRect.Height);
                using (Graphics g = Graphics.FromImage(cropped))
                {
                    g.DrawImage(source, 
                        new Rectangle(0, 0, selectionRect.Width, selectionRect.Height),
                        selectionRect, GraphicsUnit.Pixel);
                }
                return cropped;
            }
        }

        /// <summary>
        /// 执行OCR识别
        /// </summary>
        private Task<string> PerformOCR(Bitmap image)
        {
            return Task.Run(() =>
            {
                StringBuilder result = new StringBuilder();
                
                try
                {
                    // 获取Tesseract数据文件夹路径
                    string tesseractDataPath = GetTesseractDataPath();
                    string tessdataDir = Path.Combine(tesseractDataPath, "tessdata");
                    
                    result.AppendLine($"调试信息：查找路径 - {tesseractDataPath}");
                    result.AppendLine($"调试信息：tessdata目录 - {tessdataDir}");
                    
                    if (!Directory.Exists(tessdataDir))
                    {
                        result.AppendLine("");
                        result.AppendLine("错误：未找到tessdata文件夹。");
                        result.AppendLine("");
                        result.AppendLine("请将tessdata文件夹放置在以下任一位置：");
                        result.AppendLine($"1. {Application.StartupPath}\\tessdata");
                        result.AppendLine($"2. C:\\Program Files\\Tesseract-OCR\\tessdata");
                        result.AppendLine("");
                        result.AppendLine("下载地址：https://github.com/tesseract-ocr/tessdata");
                        return result.ToString();
                    }
                    
                    // 检查语言文件
                    string chiSimFile = Path.Combine(tessdataDir, "chi_sim.traineddata");
                    string engFile = Path.Combine(tessdataDir, "eng.traineddata");
                    
                    result.AppendLine($"调试信息：chi_sim文件存在 - {File.Exists(chiSimFile)}");
                    result.AppendLine($"调试信息：eng文件存在 - {File.Exists(engFile)}");
                    
                    // 检查本机库
                    CheckNativeLibraries(result, tesseractDataPath);

                    // 进行OCR识别
                    using (var engine = new TesseractEngine(tesseractDataPath, "chi_sim+eng", EngineMode.Default))
                    {
                        engine.SetVariable("tessedit_char_whitelist", "");  // 可以设置允许的字符集
                        engine.SetVariable("preserve_interword_spaces", "1");
                        
                        // 转换为Pix图像格式
                        Pix pix = ConvertBitmapToPix(image);
                        
                        using (var img = pix)
                        {
                            using (var page = engine.Process(img))
                            {
                                string text = page.GetText();
                                float confidence = page.GetMeanConfidence();
                                
                                // 清空之前的调试信息，显示识别结果
                                result.Clear();
                                result.AppendLine("OCR识别结果：");
                                result.AppendLine($"识别置信度: {confidence:F2}%");
                                result.AppendLine($"图片大小: {image.Width}x{image.Height}");
                                result.AppendLine("");
                                result.AppendLine("=== 识别文本内容 ===");
                                result.AppendLine(text);
                                result.AppendLine("=== 文本内容结束 ===");
                            }
                        }
                    }

                    // 保存截取的图片用于调试
                    try
                    {
                        string debugPath = Path.Combine(Application.StartupPath, "ocr_temp.png");
                        image.Save(debugPath, ImageFormat.Png);
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    result.AppendLine("❌ OCR识别出错！");
                    result.AppendLine("");
                    result.AppendLine("错误信息：" + ex.Message);
                    result.AppendLine("");
                    
                    // 提供明确的解决方案
                    result.AppendLine("💡 解决方案：");
                    result.AppendLine("");
                    result.AppendLine("您已安装 Tesseract-OCR，但需要复制本机库 DLL 到程序目录。");
                    result.AppendLine("");
                    result.AppendLine("【步骤1】复制 DLL 文件");
                    result.AppendLine("从以下源文件夹复制所有 DLL：");
                    result.AppendLine(@"  源: C:\Program Files\Tesseract-OCR");
                    result.AppendLine($"  到: {Application.StartupPath}");
                    result.AppendLine("");
                    result.AppendLine("需要复制的文件：");
                    result.AppendLine("  - tesseract*.dll");
                    result.AppendLine("  - liblept*.dll");
                    result.AppendLine("  - 其他 .dll 文件");
                    result.AppendLine("");
                    result.AppendLine("【步骤2】复制 tessdata 文件");
                    result.AppendLine($"将以下 tessdata 文件复制到: {Application.StartupPath}\\tessdata");
                    result.AppendLine("  - chi_sim.traineddata");
                    result.AppendLine("  - eng.traineddata");
                    result.AppendLine("");
                    result.AppendLine("【步骤3】重启程序");
                    result.AppendLine("");
                    result.AppendLine("或者使用 NuGet 重新安装 Tesseract 包：");
                    result.AppendLine("1. 右键项目 -> 管理 NuGet 程序包");
                    result.AppendLine("2. 卸载 Tesseract");
                    result.AppendLine("3. 重新安装 Tesseract");
                }

                return result.ToString();
            });
        }

        /// <summary>
        /// 获取Tesseract数据路径（返回tessdata的父目录）
        /// </summary>
        private string GetTesseractDataPath()
        {
            // 首先尝试在程序目录下的tessdata文件夹
            //string appDirTessdata = Path.Combine(Application.StartupPath, "tessdata");
            //if (Directory.Exists(appDirTessdata))
            //{
            //    string parentDir = Path.GetDirectoryName(appDirTessdata);
            //    // 验证路径是否正确
            //    if (Directory.Exists(Path.Combine(parentDir, "tessdata")))
            //    {
            //        return parentDir;
            //    }
            //}

            // 尝试常见的Tesseract安装路径
            string[] possiblePaths = new string[]
            {
                @"C:\Program Files\Tesseract-OCR\tessdata",
                @"C:\Program Files (x86)\Tesseract-OCR\tessdata",
                @"C:\ProgramData\Tesseract-OCR\tessdata",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Tesseract-OCR", "tessdata"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Tesseract-OCR", "tessdata"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata")
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    return Path.GetDirectoryName(path);
                }
            }

            // 返回应用程序目录（tessdata的父目录）
            return Application.StartupPath;
        }

        /// <summary>
        /// 检查本机库
        /// </summary>
        private void CheckNativeLibraries(StringBuilder result, string basePath)
        {
            // 检查常见的本机库位置
            string[] x86Paths = new string[]
            {
                Path.Combine(basePath, "x86"),
                Path.Combine(Application.StartupPath, "x86"),
                @"C:\Program Files\Tesseract-OCR",
                @"C:\Program Files (x86)\Tesseract-OCR"
            };
            
            result.AppendLine("本机库检查：");
            foreach (var path in x86Paths)
            {
                string dllPath = Path.Combine(path, "tesseract53.dll");
                if (File.Exists(dllPath))
                {
                    result.AppendLine($"  找到 tesseract53.dll: {dllPath}");
                    break;
                }
            }
            
            // 检查是否安装了完整的Tesseract
            string[] tesseractPaths = new string[]
            {
                @"C:\Program Files\Tesseract-OCR\tesseract.exe",
                @"C:\Program Files (x86)\Tesseract-OCR\tesseract.exe"
            };
            
            bool tesseractInstalled = false;
            foreach (var exePath in tesseractPaths)
            {
                if (File.Exists(exePath))
                {
                    tesseractInstalled = true;
                    result.AppendLine($"  Tesseract-OCR已安装: {exePath}");
                    break;
                }
            }
            
            if (!tesseractInstalled)
            {
                result.AppendLine("警告：未检测到Tesseract-OCR安装，可能需要本机库");
            }
            result.AppendLine("");
        }

        /// <summary>
        /// 将Bitmap转换为Tesseract的Pix格式
        /// </summary>
        private Pix ConvertBitmapToPix(Bitmap bitmap)
        {
            // 将Bitmap保存为字节数组
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                
                // 创建Pix对象
                return Pix.LoadFromMemory(imageBytes);
            }
        }
    }
}
