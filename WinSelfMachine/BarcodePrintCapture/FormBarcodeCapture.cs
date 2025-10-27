using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BarcodePrintCapture
{
    public partial class FormBarcodeCapture : Form
    {
        private System.Windows.Forms.Timer _autoPrintTimer;
        private FormHoverWindow _hoverWindow;
        private string _lastId = "";
        private string _lastName = "";
        private bool _isMonitoring = false;
        private string _configPath;

        public FormBarcodeCapture()
        {
            InitializeComponent();
            InitializeConfig();
            LoadInstalledPrinters();
            LoadConfig();
        }

        /// <summary>
        /// 初始化配置
        /// </summary>
        private void InitializeConfig()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BarcodePrint.ini");
            if (!File.Exists(_configPath))
            {
                File.Create(_configPath).Close();
            }
        }

        /// <summary>
        /// 加载打印机列表
        /// </summary>
        private void LoadInstalledPrinters()
        {
            comboBoxPrinter.Items.Clear();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                comboBoxPrinter.Items.Add(printer);
            }
            if (comboBoxPrinter.Items.Count > 0)
            {
                comboBoxPrinter.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void FormBarcodeCapture_Load(object sender, EventArgs e)
        {
            LoadConfig();
            
            // 如果配置中启用了悬停窗口，自动显示
            if (checkBoxHoverWindow.Checked)
            {
                ShowHoverWindow();
            }
            
            // 如果配置中启用了自动定时打印，自动启动
            if (checkBoxAutoPrint.Checked)
            {
                StartAutoPrint();
            }
        }

        /// <summary>
        /// 获取信息按钮点击事件
        /// </summary>
        private void btnGetInfo_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResults.Clear();
                comboBoxId.Items.Clear();
                comboBoxName.Items.Clear();

                // 获取搜索参数
                string processName = textBoxProcessName.Text.Trim();
                string windowTitle = textBoxWindowTitle.Text.Trim();
                string className = textBoxClassName.Text.Trim();

                // 搜索窗口
                var windows = WindowCaptureHelper.SearchWindows(processName, windowTitle, className);

                if (windows.Count == 0)
                {
                    textBoxResults.Text = "未找到匹配的窗口";
                    return;
                }

                // 显示找到的窗口信息
                foreach (var window in windows)
                {
                    textBoxResults.AppendText($"{window.ProcessName} - {window.WindowTitle}\r\n");
                    
                    // 获取子控件信息
                    var controls = WindowCaptureHelper.GetChildControls(window.Handle);
                    
                    // 添加到下拉框
                    foreach (var control in controls)
                    {
                        if (!string.IsNullOrEmpty(control.Text))
                        {
                            string item = $"{control.FullInfo}";
                            if (!comboBoxId.Items.Contains(item))
                            {
                                comboBoxId.Items.Add(item);
                            }
                            if (!comboBoxName.Items.Contains(item))
                            {
                                comboBoxName.Items.Add(item);
                            }
                        }
                        textBoxResults.AppendText($"  {control.FullInfo}\r\n");
                    }
                }

                // 如果指定了ID内容和姓名内容，自动选择
                if (!string.IsNullOrEmpty(textBoxIdContent.Text))
                {
                    for (int i = 0; i < comboBoxId.Items.Count; i++)
                    {
                        if (comboBoxId.Items[i].ToString().Contains($"Txt=[{textBoxIdContent.Text}]"))
                        {
                            comboBoxId.SelectedIndex = i;
                            // 提取坐标
                            if (ExtractCoordinates(comboBoxId.Text, out int x, out int y))
                            {
                                textBoxIdCoords.Text = $"{x},{y}";
                            }
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(textBoxNameContent.Text))
                {
                    for (int i = 0; i < comboBoxName.Items.Count; i++)
                    {
                        if (comboBoxName.Items[i].ToString().Contains($"Txt=[{textBoxNameContent.Text}]"))
                        {
                            comboBoxName.SelectedIndex = i;
                            // 提取坐标
                            if (ExtractCoordinates(comboBoxName.Text, out int x, out int y))
                            {
                                textBoxNameCoords.Text = $"{x},{y}";
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取信息失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存配置按钮点击事件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证必填项
                if (string.IsNullOrEmpty(textBoxConfigProcessName.Text))
                {
                    MessageBox.Show("请输入进程名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 使用简单的方式保存配置
                SaveConfigToFile();

                MessageBox.Show("配置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 如果启用了悬停窗口，显示悬停窗口
                if (checkBoxHoverWindow.Checked)
                {
                    ShowHoverWindow();
                }

                // 如果启用了自动定时打印，启动定时器
                if (checkBoxAutoPrint.Checked)
                {
                    StartAutoPrint();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                using (var iniHelper = new IniFileHelper(_configPath))
                {
                    textBoxConfigProcessName.Text = iniHelper.Read("Config", "ProcessName", "");
                    textBoxConfigWindowTitle.Text = iniHelper.Read("Config", "WindowTitle", "");
                    textBoxConfigClassName.Text = iniHelper.Read("Config", "ClassName", "");
                    textBoxIdCoords.Text = iniHelper.Read("Config", "IdCoords", "");
                    textBoxNameCoords.Text = iniHelper.Read("Config", "NameCoords", "");
                    checkBoxAutoPrint.Checked = iniHelper.ReadInt("Config", "AutoPrint", 0) == 1;
                    numericUpDownInterval.Value = iniHelper.ReadInt("Config", "Interval", 10);
                    checkBoxPrintRegTime.Checked = iniHelper.ReadInt("Config", "PrintRegTime", 1) == 1;
                    checkBoxHoverWindow.Checked = iniHelper.ReadInt("Config", "HoverWindow", 1) == 1;
                    textBoxHospitalName.Text = iniHelper.Read("Config", "HospitalName", "中国大医院");
                    
                    string printerName = iniHelper.Read("Config", "PrinterName", "");
                    if (!string.IsNullOrEmpty(printerName))
                    {
                        int index = comboBoxPrinter.Items.IndexOf(printerName);
                        if (index >= 0)
                        {
                            comboBoxPrinter.SelectedIndex = index;
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 保存配置到文件
        /// </summary>
        private void SaveConfigToFile()
        {
            try
            {
                using (var iniHelper = new IniFileHelper(_configPath))
                {
                    iniHelper.Write("Config", "ProcessName", textBoxConfigProcessName.Text);
                    iniHelper.Write("Config", "WindowTitle", textBoxConfigWindowTitle.Text);
                    iniHelper.Write("Config", "ClassName", textBoxConfigClassName.Text);
                    iniHelper.Write("Config", "IdCoords", textBoxIdCoords.Text);
                    iniHelper.Write("Config", "NameCoords", textBoxNameCoords.Text);
                    iniHelper.Write("Config", "AutoPrint", checkBoxAutoPrint.Checked ? "1" : "0");
                    iniHelper.Write("Config", "Interval", numericUpDownInterval.Value.ToString());
                    iniHelper.Write("Config", "PrintRegTime", checkBoxPrintRegTime.Checked ? "1" : "0");
                    iniHelper.Write("Config", "HoverWindow", checkBoxHoverWindow.Checked ? "1" : "0");
                    iniHelper.Write("Config", "HospitalName", textBoxHospitalName.Text);
                    iniHelper.Write("Config", "PrinterName", comboBoxPrinter.SelectedItem?.ToString() ?? "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 显示悬停窗口
        /// </summary>
        private void ShowHoverWindow()
        {
            if (_hoverWindow == null || _hoverWindow.IsDisposed)
            {
                _hoverWindow = new FormHoverWindow(this);
                _hoverWindow.Show();
                _hoverWindow.FormClosed += HoverWindow_FormClosed;
            }
        }

        /// <summary>
        /// 悬停窗口关闭事件
        /// </summary>
        private void HoverWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            _hoverWindow = null;
        }

        /// <summary>
        /// 启动自动打印
        /// </summary>
        private void StartAutoPrint()
        {
            if (_autoPrintTimer == null)
            {
                _autoPrintTimer = new System.Windows.Forms.Timer();
                _autoPrintTimer.Interval = (int)(numericUpDownInterval.Value * 1000);
                _autoPrintTimer.Tick += AutoPrintTimer_Tick;
            }
            else
            {
                _autoPrintTimer.Stop();
                _autoPrintTimer.Interval = (int)(numericUpDownInterval.Value * 1000);
            }

            _autoPrintTimer.Start();
            _isMonitoring = true;
        }

        /// <summary>
        /// 自动打印定时器事件
        /// </summary>
        private void AutoPrintTimer_Tick(object sender, EventArgs e)
        {
            if (!_isMonitoring)
                return;

            try
            {
                // 搜索目标窗口
                string processName = "";
                try
                {
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        processName = iniHelper.Read("Config", "ProcessName", "");
                    }
                }
                catch { }

                if (string.IsNullOrEmpty(processName))
                    return;

                var windows = WindowCaptureHelper.SearchWindows(processName, "", "");
                if (windows.Count == 0)
                    return;

                IntPtr targetWindow = windows[0].Handle;

                // 获取坐标
                string idCoords = "";
                string nameCoords = "";
                try
                {
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        idCoords = iniHelper.Read("Config", "IdCoords", "");
                        nameCoords = iniHelper.Read("Config", "NameCoords", "");
                    }
                }
                catch { }

                // 解析坐标
                if (!string.IsNullOrEmpty(idCoords) && !string.IsNullOrEmpty(nameCoords))
                {
                    ExtractCoordinates(idCoords, out int idX, out int idY);
                    ExtractCoordinates(nameCoords, out int nameX, out int nameY);

                    // 获取控件内容
                    var controls = WindowCaptureHelper.GetChildControls(targetWindow);
                    
                    string currentId = "";
                    string currentName = "";

                    foreach (var control in controls)
                    {
                        if (!string.IsNullOrEmpty(control.Text))
                        {
                            if (!string.IsNullOrEmpty(idCoords) && 
                                ExtractCoordinates(control.FullInfo, out int x, out int y) &&
                                x == idX && y == idY)
                            {
                                currentId = control.Text.Trim();
                            }

                            if (!string.IsNullOrEmpty(nameCoords) &&
                                ExtractCoordinates(control.FullInfo, out int x2, out int y2) &&
                                x2 == nameX && y2 == nameY)
                            {
                                currentName = control.Text.Trim();
                            }
                        }
                    }

                    // 如果内容发生变化，执行打印
                    if (!string.IsNullOrEmpty(currentId) && !string.IsNullOrEmpty(currentName))
                    {
                        if (currentId != _lastId || currentName != _lastName)
                        {
                            _lastId = currentId;
                            _lastName = currentName;

                            // 更新悬停窗口
                            if (_hoverWindow != null && !_hoverWindow.IsDisposed)
                            {
                                _hoverWindow.UpdateName(currentName);
                            }

                            // 执行打印
                            ExecutePrint(currentId, currentName);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 执行打印
        /// </summary>
        private void ExecutePrint(string id, string name)
        {
            try
            {
                var printConfig = new BarcodePrintHelper.PrintConfig
                {
                    HospitalName = "中国大医院",
                    PrinterName = "",
                    PrintRegistrationTime = true
                };

                try
                {
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        printConfig.HospitalName = iniHelper.Read("Config", "HospitalName", "中国大医院");
                        printConfig.PrinterName = iniHelper.Read("Config", "PrinterName", "");
                        printConfig.PrintRegistrationTime = iniHelper.ReadInt("Config", "PrintRegTime", 1) == 1;
                    }
                }
                catch { }

                var helper = new BarcodePrintHelper(printConfig, id, name);
                helper.Print();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"打印失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 覆盖ExtractCoordinates方法，支持坐标解析
        /// </summary>
        private bool ExtractCoordinates(string text, out int x, out int y)
        {
            x = 0;
            y = 0;
            
            if (string.IsNullOrEmpty(text))
                return false;

            // 支持两种格式："Rect=260, 324" 或 "260, 324"
            string coords = text;
            int rectIndex = text.IndexOf("Rect=");
            if (rectIndex >= 0)
            {
                coords = text.Substring(rectIndex + 5);
            }

            string[] parts = coords.Split(',');
            if (parts.Length >= 2)
            {
                if (int.TryParse(parts[0].Trim(), out x) && int.TryParse(parts[1].Trim(), out y))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

