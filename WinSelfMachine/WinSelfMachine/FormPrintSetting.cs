using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Drawing.Printing.PrinterSettings;

namespace WinSelfMachine
{
    public partial class FormPrintSetting : Form
    {
        public FormPrintSetting()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取打印机列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPrintSetting_Load(object sender, EventArgs e)
        {
            try
            {
                StringCollection allPrinters = InstalledPrinters;

                if (allPrinters.Count == 0)
                {
                    CbmPrinter.Items.Add("未检测到打印机");
                    CbmPrinter.Enabled = false;
                    return;
                }
                string defaultPrinterName = new PrinterSettings().PrinterName; // 获取默认打印机名称
                foreach (string printerName in allPrinters)
                {
                    // 若为默认打印机，添加标识
                    string displayName = printerName == defaultPrinterName
                        ? $"{printerName}（默认）"
                        : printerName;

                    CbmPrinter.Items.Add(displayName);
                }

                // 4. 默认选中默认打印机
                CbmPrinter.Text = $"{defaultPrinterName}（默认）";
                CbmPrinter.Enabled = true;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"获取打印机失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 设置打印机纸张大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbmPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbmPaper.Items.Clear();
            // 获取选中的打印机名称（移除可能的"（默认）"标记）
            string selectedPrinter = CbmPrinter.Text.Replace("（默认）", "").Trim();
            if (string.IsNullOrEmpty(selectedPrinter))
            {
                MessageBox.Show("请选择有效的打印机", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 实例化打印机设置对象，绑定选中的打印机
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = selectedPrinter;

                // 检查打印机是否有效
                if (printerSettings.IsValid)
                {
                    MessageBox.Show($"打印机【{selectedPrinter}】无效，请检查驱动或连接", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 存储已添加的纸张名称（避免重复）
                HashSet<string> addedPaperNames = new HashSet<string>();

                // 遍历该打印机支持的所有纸张大小
                foreach (PaperSize paperSize in printerSettings.PaperSizes)
                {
                    // 过滤自定义纸张（可选，根据需求决定是否保留）
                    // if (paperSize.Kind == PaperKind.Custom) continue;

                    // 避免重复添加相同名称的纸张
                    if (!addedPaperNames.Contains(paperSize.PaperName))
                    {
                        // 将纸张信息添加到下拉框（格式：名称 (宽度x高度 毫米)）
                        string displayText = $"{paperSize.PaperName} ({paperSize.Width / 10}x{paperSize.Height / 10}mm)";
                        CbmPaper.Items.Add(new PaperSizeItem
                        {
                            DisplayText = displayText,
                            PaperSize = paperSize
                        });

                        addedPaperNames.Add(paperSize.PaperName);
                    }
                }

                // 设置默认选中项（优先选择A4纸）
                if (CbmPaper.Items.Count > 0)
                {
                    // 尝试找到A4纸并选中
                    var a4Item = CbmPaper.Items.Cast<PaperSizeItem>()
                        .FirstOrDefault(item => item.PaperSize.Kind == PaperKind.A4);

                    CbmPaper.SelectedItem = a4Item ?? CbmPaper.Items[0];
                    CbmPaper.Enabled = true;
                }
                else
                {
                    CbmPaper.Items.Add("未检测到支持的纸张大小");
                    CbmPaper.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取纸张大小失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // 辅助类：用于存储纸张大小信息和显示文本
        public class PaperSizeItem
        {
            public string DisplayText { get; set; }
            public PaperSize PaperSize { get; set; }

            // 重写ToString，使下拉框显示自定义文本
            public override string ToString()
            {
                return DisplayText;
            }
        }

        // 使用示例：在打印时应用选中的纸张大小
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // 获取选中的纸张大小
            if (CbmPaper.SelectedItem is PaperSizeItem selectedPaper)
            {
                // 设置打印文档的纸张大小
                e.PageSettings.PaperSize = selectedPaper.PaperSize;
            }

            // 执行打印逻辑...
        }
    }
}
