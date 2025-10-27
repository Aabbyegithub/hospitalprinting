using System;
using System.Drawing;
using System.Drawing.Printing;
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
        }

        private PrintConfig _config;
        private string _id;
        private string _name;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BarcodePrintHelper(PrintConfig config, string id, string name)
        {
            _config = config;
            _id = id;
            _name = name;
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
        /// 打印页面事件
        /// </summary>
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font titleFont = new Font("宋体", 18, FontStyle.Bold);
            Font contentFont = new Font("宋体", 14);
            Font barcodeFont = new Font("Code39", 36);

            float x = 50;
            float y = 50;
            float lineHeight = 30;

            // 打印医院名称
            if (!string.IsNullOrEmpty(_config.HospitalName))
            {
                graphics.DrawString(_config.HospitalName, titleFont, Brushes.Black, new PointF(x, y));
                y += lineHeight * 2;
            }

            // 打印患者姓名
            if (!string.IsNullOrEmpty(_name))
            {
                graphics.DrawString($"姓名：{_name}", contentFont, Brushes.Black, new PointF(x, y));
                y += lineHeight;
            }

            // 打印ID
            if (!string.IsNullOrEmpty(_id))
            {
                graphics.DrawString($"编号：{_id}", contentFont, Brushes.Black, new PointF(x, y));
                y += lineHeight * 2;

                // 绘制条码（使用文本模拟）
                string barcodeText = $"*{_id}*";
                graphics.DrawString(barcodeText, barcodeFont, Brushes.Black, new PointF(x, y));
                y += lineHeight;
                graphics.DrawString(_id, contentFont, Brushes.Black, new PointF(x + 50, y));
            }

            y += lineHeight * 2;

            // 打印登记时间
            if (_config.PrintRegistrationTime)
            {
                string regTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                graphics.DrawString($"登记时间：{regTime}", contentFont, Brushes.Black, new PointF(x, y));
            }

            e.HasMorePages = false;
        }
    }
}

