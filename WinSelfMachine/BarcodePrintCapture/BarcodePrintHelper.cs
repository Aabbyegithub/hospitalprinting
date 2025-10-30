using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace BarcodePrintCapture
{
    /// <summary>
    /// 条码打印辅助类
    /// </summary>
    public class BarcodePrintHelper
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public class PrintConfig
        {
            public string HospitalName { get; set; }
            public string PrinterName { get; set; }
            public bool PrintRegistrationTime { get; set; }
            public string TitleTemplate { get; set; }
            public string IdLabel { get; set; }
            public string NameLabel { get; set; }
            public string TimeLabel { get; set; }
        }

        private PrintConfig _config;
        private string _id;
        private string _name;
        private string _gender;
        private string _age;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BarcodePrintHelper(PrintConfig config, string id, string name, string gender = "", string age = "")
        {
            _config = config;
            _id = id;
            _name = name;
            _gender = gender;
            _age = age;
        }

        /// <summary>
        /// 执行打印
        /// </summary>
        public void Print()
        {
            try
            {
                using (PrintDocument printDoc = new PrintDocument())
                {
                    if (!string.IsNullOrEmpty(_config.PrinterName))
                    {
                        // 检查打印机是否存在
                        bool printerExists = false;
                        foreach (string printer in PrinterSettings.InstalledPrinters)
                        {
                            if (printer == _config.PrinterName)
                            {
                                printerExists = true;
                                break;
                            }
                        }

                        if (printerExists)
                        {
                            printDoc.PrinterSettings.PrinterName = _config.PrinterName;
                        }
                    }

                    printDoc.PrintPage += PrintDoc_PrintPage;
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成Code 39条形码图像
        /// </summary>
        private Bitmap GenerateBarcode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;

            // Code 39参数
            int narrowWidth = 2;  // 窄条宽度（像素）
            int wideWidth = 5;    // 宽条宽度（像素）
            int height = 100;     // 条码高度
            int quietZone = 30;   // 静默区

            // 转换为大写（Code 39只支持大写）
            code = code.ToUpper();

            // 计算总宽度：起始符 + 字符 + 间隔 + 结束符
            int totalWidth = quietZone * 2; // 两端静默区
            
            // 起始符 *
            totalWidth += GetPatternWidth("*", narrowWidth, wideWidth);
            totalWidth += narrowWidth; // 字符间隔
            
            // 所有字符
            foreach (char c in code)
            {
                totalWidth += GetPatternWidth(c.ToString(), narrowWidth, wideWidth);
                totalWidth += narrowWidth; // 字符间隔
            }
            
            // 结束符 *
            totalWidth += GetPatternWidth("*", narrowWidth, wideWidth);

            // 创建位图
            Bitmap bmp = new Bitmap(totalWidth, height + 35);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                int x = quietZone;
                
                // 绘制起始符 *
                x = DrawCharacter(g, "*", x, height, narrowWidth, wideWidth);
                x += narrowWidth; // 字符间隔
                
                // 绘制每个字符
                foreach (char c in code)
                {
                    x = DrawCharacter(g, c.ToString(), x, height, narrowWidth, wideWidth);
                    x += narrowWidth; // 字符间隔
                }
                
                // 绘制结束符 *
                DrawCharacter(g, "*", x, height, narrowWidth, wideWidth);

                // 添加文本
                using (Font font = new Font("Arial", 14, FontStyle.Regular))
                {
                    SizeF textSize = g.MeasureString(code, font);
                    float textX = (totalWidth - textSize.Width) / 2;
                    g.DrawString(code, font, Brushes.Black, textX, height + 8);
                }
            }

            return bmp;
        }

        /// <summary>
        /// 绘制单个字符
        /// </summary>
        private int DrawCharacter(Graphics g, string c, int x, int height, int narrowWidth, int wideWidth)
        {
            string pattern = GetCode39Pattern(c);
            if (string.IsNullOrEmpty(pattern))
                pattern = "010010100"; // 默认空格模式

            for (int i = 0; i < pattern.Length; i++)
            {
                int width = (pattern[i] == '1') ? wideWidth : narrowWidth;
                
                // 奇数位是条（黑色），偶数位是空（白色）
                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, width, height);
                }
                
                x += width;
            }
            
            return x;
        }

        /// <summary>
        /// 计算字符宽度
        /// </summary>
        private int GetPatternWidth(string c, int narrowWidth, int wideWidth)
        {
            string pattern = GetCode39Pattern(c);
            if (string.IsNullOrEmpty(pattern))
                pattern = "010010100";

            int width = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                width += (pattern[i] == '1') ? wideWidth : narrowWidth;
            }
            return width;
        }

        /// <summary>
        /// 获取Code 39字符模式（标准编码）
        /// 格式：9位字符串，1=宽，0=窄，位置：条-空-条-空-条-空-条-空-条
        /// </summary>
        private string GetCode39Pattern(string c)
        {
            switch (c)
            {
                case "0": return "000110100";
                case "1": return "100100001";
                case "2": return "001100001";
                case "3": return "101100000";
                case "4": return "000110001";
                case "5": return "100110000";
                case "6": return "001110000";
                case "7": return "000100101";
                case "8": return "100100100";
                case "9": return "001100100";
                case "A": return "100001001";
                case "B": return "001001001";
                case "C": return "101001000";
                case "D": return "000011001";
                case "E": return "100011000";
                case "F": return "001011000";
                case "G": return "000001101";
                case "H": return "100001100";
                case "I": return "001001100";
                case "J": return "000011100";
                case "K": return "100000011";
                case "L": return "001000011";
                case "M": return "101000010";
                case "N": return "000010011";
                case "O": return "100010010";
                case "P": return "001010010";
                case "Q": return "000000111";
                case "R": return "100000110";
                case "S": return "001000110";
                case "T": return "000010110";
                case "U": return "110000001";
                case "V": return "011000001";
                case "W": return "111000000";
                case "X": return "010010001";
                case "Y": return "110010000";
                case "Z": return "011010000";
                case "-": return "010000101";
                case ".": return "110000100";
                case " ": return "011000100";
                case "*": return "010010100"; // 起始/结束符
                case "$": return "010101000";
                case "/": return "010100010";
                case "+": return "010001010";
                case "%": return "000101010";
                default:  return "011000100"; // 空格
            }
        }


        /// <summary>
        /// 打印页面事件
        /// </summary>
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font idFont = new Font("Arial", 14, FontStyle.Bold);
            Font titleFont = new Font("宋体", 16, FontStyle.Bold);
            Font contentFont = new Font("宋体", 14);
            Font smallFont = new Font("宋体", 11);

            float x = 40;
            float y = 30;
            float leftMargin = x;

            try
            {
                // 1. 打印条形码（如果有ID）
                if (!string.IsNullOrEmpty(_id))
                {
                    Bitmap barcode = GenerateBarcode(_id);
                    if (barcode != null)
                    {
                        try
                        {
                            // 左对齐绘制条形码
                            graphics.DrawImage(barcode, leftMargin, y, barcode.Width, barcode.Height);
                            
                            // 条形码内部已包含文字，不需要再绘制
                            // 更新Y坐标，条形码高度包含文字区域
                            y += barcode.Height + 10;
                        }
                        finally
                        {
                            barcode.Dispose();
                        }
                    }
                }

                // 2. 打印患者姓名和基本信息（姓名、性别、年龄）
                if (!string.IsNullOrEmpty(_name))
                {
                    string nameLabel = string.IsNullOrEmpty(_config.NameLabel) 
                        ? "" 
                        : _config.NameLabel;
                    
                    // 如果有性别和年龄，一起显示
                    string info = nameLabel + _name;
                    if (!string.IsNullOrEmpty(_gender) || !string.IsNullOrEmpty(_age))
                    {
                        info += " ";
                        if (!string.IsNullOrEmpty(_gender)) info += _gender;
                        if (!string.IsNullOrEmpty(_age)) info += " " + _age + "岁";
                    }
                    
                    graphics.DrawString(info, contentFont, Brushes.Black, new PointF(leftMargin, y));
                    y += 28;
                }

                // 3. 打印医院名称（标题）
                string title = string.IsNullOrEmpty(_config.TitleTemplate) 
                    ? _config.HospitalName 
                    : ReplacePlaceholders(_config.TitleTemplate);
                
                if (!string.IsNullOrEmpty(title))
                {
                    graphics.DrawString(title, titleFont, Brushes.Black, new PointF(leftMargin, y));
                    y += 28;
                }

                // 4. 打印登记时间
                if (_config.PrintRegistrationTime)
                {
                    string timeLabel = string.IsNullOrEmpty(_config.TimeLabel) 
                        ? "登记时间：" 
                        : _config.TimeLabel;
                    string regTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    graphics.DrawString($"{timeLabel}{regTime}", smallFont, Brushes.Black, new PointF(leftMargin, y));
                }

                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.HasMorePages = false;
            }
        }

        /// <summary>
        /// 处理占位符替换
        /// </summary>
        private string ReplacePlaceholders(string template)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            return template
                .Replace("{HOSPITAL_NAME}", _config.HospitalName ?? "")
                .Replace("{PATIENT_NAME}", _name ?? "")
                .Replace("{PATIENT_ID}", _id ?? "")
                .Replace("{REG_TIME}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}

