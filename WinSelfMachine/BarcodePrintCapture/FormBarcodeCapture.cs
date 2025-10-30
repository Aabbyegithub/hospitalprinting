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

                // 额外探测：如果配置了进程名和坐标，尝试直接命中DataGridView行，展示读取结果，方便校准
                try
                {
                    string cfgProcess = textBoxConfigProcessName.Text.Trim();
                    if (!string.IsNullOrEmpty(cfgProcess) &&
                        ExtractCoordinates(textBoxIdCoords.Text.Trim(), out int cfgIdX, out int cfgIdY) &&
                        ExtractCoordinates(textBoxNameCoords.Text.Trim(), out int cfgNameX, out int cfgNameY))
                    {
                        var wins = WindowCaptureHelper.SearchWindows(cfgProcess, string.Empty, string.Empty);
                        if (wins.Count > 0)
                        {
                            var hTarget = wins[0].Handle;
                            if (TryReadByCoords(hTarget, cfgIdX, cfgIdY, cfgNameX, cfgNameY, out string rid, out string rname))
                            {
                                textBoxResults.AppendText($"[探测] 命中行 → ID:{rid} 姓名:{rname}\r\n");
                                if (!string.IsNullOrEmpty(rid)) textBoxIdContent.Text = rid;
                                if (!string.IsNullOrEmpty(rname)) textBoxNameContent.Text = rname;
                            }
                            else
                            {
                                textBoxResults.AppendText("[探测] 未命中任何文本，请微调‘ID/姓名’坐标并确保落在同一行单元格内部\r\n");
                            }
                        }
                    }
                }
                catch { }

                // 显示找到的窗口信息
                foreach (var window in windows)
                {
                    textBoxResults.AppendText($"{window.ProcessName} - {window.WindowTitle}\r\n");
                    
                    // 获取子控件信息（先Win32，再UIA补充）
                    var controls = WindowCaptureHelper.GetChildControls(window.Handle);

                    // 如果Win32抓不到文字，尝试UI Automation增强
                    bool hasAnyText = false;
                    foreach (var c in controls)
                    {
                        if (!string.IsNullOrEmpty(c.Text)) { hasAnyText = true; break; }
                    }
                    if (!hasAnyText)
                    {
                        var uiaControls = UIAutomationHelper.GetChildControls(window.Handle);
                        if (uiaControls.Count > 0)
                        {
                            textBoxResults.AppendText("  [UIA] 尝试使用UI Automation读取...\r\n");
                            controls = uiaControls;
                        }
                    }
                    
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
        /// 打印模板配置按钮点击事件
        /// </summary>
        private void btnTemplateConfig_Click(object sender, EventArgs e)
        {
            try
            {
                using (var templateForm = new FormBarcodeTemplate())
                {
                    if (templateForm.ShowDialog(this) == DialogResult.OK)
                    {
                        MessageBox.Show("打印模板配置已更新", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开打印模板配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 测试打印按钮点击事件
        /// </summary>
        private void btnPrintOptions_Click(object sender, EventArgs e)
        {
            try
            {
                // 优先：如果手动输入区有值，则直接按手动输入打印
                string manualId = textBoxManualId != null ? textBoxManualId.Text.Trim() : string.Empty;
                string manualName = textBoxManualName != null ? textBoxManualName.Text.Trim() : string.Empty;
                if (!string.IsNullOrEmpty(manualId) && !string.IsNullOrEmpty(manualName))
                {
                    ExecutePrint(manualId, manualName, "", "");
                    MessageBox.Show("已按手动输入打印", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 其次：尝试按当前配置，从目标窗口读取ID/姓名
                string processName = textBoxConfigProcessName.Text.Trim();
                string idCoordsText = textBoxIdCoords.Text.Trim();
                string nameCoordsText = textBoxNameCoords.Text.Trim();

                if (!string.IsNullOrEmpty(processName) &&
                    ExtractCoordinates(idCoordsText, out int idX, out int idY) &&
                    ExtractCoordinates(nameCoordsText, out int nameX, out int nameY))
                {
                    var windows = WindowCaptureHelper.SearchWindows(processName, string.Empty, string.Empty);
                    if (windows.Count > 0)
                    {
                        var target = windows[0].Handle;

                        // 先用Win32，找不到再用UIA
                        if (TryReadByCoords(target, idX, idY, nameX, nameY, out string idVal, out string nameVal))
                        {
                            ExecutePrint(idVal, nameVal, "", "");
                            MessageBox.Show("已按窗口读取打印", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                // 最后：回退到内置测试数据
                ExecutePrint("D049887", "张三", "男", "58");
                MessageBox.Show("未获取到数据，已使用测试数据打印", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 手动输入打印按钮点击事件
        /// </summary>
        private void btnManualPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string id = textBoxManualId.Text.Trim();
                string name = textBoxManualName.Text.Trim();

                // 验证输入
                if (string.IsNullOrEmpty(id))
                {
                    MessageBox.Show("请输入员工ID", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxManualId.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("请输入员工姓名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxManualName.Focus();
                    return;
                }

                // 打印条码
                ExecutePrint(id, name, "", "");
                
                MessageBox.Show($"打印完成！\n\n员工ID：{id}\n员工姓名：{name}", "打印成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // 清空输入框，方便下一次输入
                textBoxManualId.Clear();
                textBoxManualName.Clear();
                textBoxManualId.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private void ExecutePrint(string id, string name, string gender = "", string age = "")
        {
            try
            {
                var printConfig = new BarcodePrintHelper.PrintConfig
                {
                    HospitalName = "中国大医院",
                    PrinterName = "",
                    PrintRegistrationTime = true,
                    TitleTemplate = "",
                    IdLabel = "编号：",
                    NameLabel = "姓名：",
                    TimeLabel = "登记时间："
                };

                try
                {
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        printConfig.HospitalName = iniHelper.Read("Config", "HospitalName", "中国大医院");
                        printConfig.PrinterName = iniHelper.Read("Config", "PrinterName", "");
                        printConfig.PrintRegistrationTime = iniHelper.ReadInt("Config", "PrintRegTime", 1) == 1;
                        
                        // 加载模板配置
                        printConfig.TitleTemplate = iniHelper.Read("Template", "Title", "");
                        printConfig.IdLabel = iniHelper.Read("Template", "IdLabel", "编号：");
                        printConfig.NameLabel = iniHelper.Read("Template", "NameLabel", "姓名：");
                        printConfig.TimeLabel = iniHelper.Read("Template", "TimeLabel", "登记时间：");
                    }
                }
                catch { }

                var helper = new BarcodePrintHelper(printConfig, id, name, gender, age);
                helper.Print();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"打印失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 手动打印方法（供悬停窗口调用）
        /// </summary>
        public void ManualPrint(string name)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ManualPrint被调用，name={name}");
                
                // 如果是测试数据，直接使用测试数据
                if (name.Contains("ceshi") || name.Contains("患者"))
                {
                    ExecutePrint("D049887", "张三", "男", "58");
                    System.Diagnostics.Debug.WriteLine("使用测试数据打印");
                    return;
                }
                
                // 尝试从目标窗口获取真实数据
                string id = "";
                string actualName = name;
                string gender = "";
                string age = "";
                
                try
                {
                    // 读取配置
                    string processName = "";
                    string idCoords = "";
                    string nameCoords = "";
                    
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        processName = iniHelper.Read("Config", "ProcessName", "");
                        idCoords = iniHelper.Read("Config", "IdCoords", "");
                        nameCoords = iniHelper.Read("Config", "NameCoords", "");
                    }

                    if (!string.IsNullOrEmpty(processName) && 
                        !string.IsNullOrEmpty(idCoords) && 
                        !idCoords.Equals("0,0") &&
                        !idCoords.Equals("0, 0"))
                    {
                        // 搜索窗口
                        var windows = WindowCaptureHelper.SearchWindows(processName, "", "");
                        if (windows.Count > 0)
                        {
                            IntPtr targetWindow = windows[0].Handle;
                            var controls = WindowCaptureHelper.GetChildControls(targetWindow);
                            
                            // 解析坐标
                            ExtractCoordinates(idCoords, out int idX, out int idY);
                            ExtractCoordinates(nameCoords, out int nameX, out int nameY);
                            
                            // 获取ID
                            foreach (var control in controls)
                            {
                                if (!string.IsNullOrEmpty(control.Text))
                                {
                                    if (ExtractCoordinates(control.FullInfo, out int x, out int y) &&
                                        x == idX && y == idY)
                                    {
                                        id = control.Text.Trim();
                                        break;
                                    }
                                }
                            }
                            
                            // 获取姓名
                            foreach (var control in controls)
                            {
                                if (!string.IsNullOrEmpty(control.Text))
                                {
                                    if (ExtractCoordinates(control.FullInfo, out int x, out int y) &&
                                        x == nameX && y == nameY)
                                    {
                                        actualName = control.Text.Trim();
                                        break;
                                    }
                                }
                            }
                            
                            System.Diagnostics.Debug.WriteLine($"从窗口读取：ID={id}, Name={actualName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"读取窗口数据失败：{ex.Message}");
                }
                
                // 如果获取不到ID，使用一个默认ID
                if (string.IsNullOrEmpty(id))
                {
                    id = DateTime.Now.ToString("HHmmss");
                    System.Diagnostics.Debug.WriteLine($"使用临时ID：{id}");
                }

                System.Diagnostics.Debug.WriteLine($"开始打印，ID={id}, Name={actualName}");
                
                // 调用打印
                ExecutePrint(id, actualName, gender, age);
                
                System.Diagnostics.Debug.WriteLine("打印完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"打印异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 从目标窗口获取ID
        /// </summary>
        private string GetIdFromWindow(string name)
        {
            try
            {
                // 读取配置
                string processName = "";
                string idCoords = "";
                try
                {
                    using (var iniHelper = new IniFileHelper(_configPath))
                    {
                        processName = iniHelper.Read("Config", "ProcessName", "");
                        idCoords = iniHelper.Read("Config", "IdCoords", "");
                    }
                }
                catch { }

                if (string.IsNullOrEmpty(processName) || string.IsNullOrEmpty(idCoords))
                    return "";

                // 搜索窗口
                var windows = WindowCaptureHelper.SearchWindows(processName, "", "");
                if (windows.Count == 0)
                    return "";

                IntPtr targetWindow = windows[0].Handle;

                // 解析坐标
                ExtractCoordinates(idCoords, out int idX, out int idY);

                // 获取控件内容
                var controls = WindowCaptureHelper.GetChildControls(targetWindow);
                
                foreach (var control in controls)
                {
                    if (!string.IsNullOrEmpty(control.Text))
                    {
                        if (ExtractCoordinates(control.FullInfo, out int x, out int y) &&
                            x == idX && y == idY)
                        {
                            return control.Text.Trim();
                        }
                    }
                }
            }
            catch { }

            return "";
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

        /// <summary>
        /// 按坐标从目标窗口读取ID/姓名文本，先Win32后UIA，坐标允许±5像素误差
        /// </summary>
        private bool TryReadByCoords(
            IntPtr targetWindow,
            int idX, int idY,
            int nameX, int nameY,
            out string id,
            out string name)
        {
            id = string.Empty;
            name = string.Empty;

            const int TOLERANCE = 5; // 坐标容差 ±5 像素

            // ① MSAA命中优先（更稳）
            string msaaId = MSAAHelper.GetNameAtWindowRelativePoint(targetWindow, idX, idY);
            string msaaName = MSAAHelper.GetNameAtWindowRelativePoint(targetWindow, nameX, nameY);
            if (!string.IsNullOrWhiteSpace(msaaId) && !string.IsNullOrWhiteSpace(msaaName))
            {
                id = msaaId.Trim();
                name = msaaName.Trim();
                return true;
            }

            // ② Win32方式
            var controls = WindowCaptureHelper.GetChildControls(targetWindow);
            foreach (var c in controls)
            {
                if (string.IsNullOrEmpty(c.Text)) continue;

                if (ExtractCoordinates(c.FullInfo, out int x, out int y))
                {
                    // ID 匹配
                    if (Math.Abs(x - idX) <= TOLERANCE && Math.Abs(y - idY) <= TOLERANCE)
                        id = c.Text.Trim();

                    // 姓名匹配
                    if (Math.Abs(x - nameX) <= TOLERANCE && Math.Abs(y - nameY) <= TOLERANCE)
                        name = c.Text.Trim();
                }
            }

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                return true;

            // ③ 若 Win32 读取失败 → UIAutomation 补充
            var uiaControls = UIAutomationHelper.GetChildControls(targetWindow);
            foreach (var c in uiaControls)
            {
                if (string.IsNullOrEmpty(c.Text)) continue;

                if (ExtractCoordinates(c.FullInfo, out int x, out int y))
                {
                    if (Math.Abs(x - idX) <= TOLERANCE && Math.Abs(y - idY) <= TOLERANCE)
                        id = c.Text.Trim();

                    if (Math.Abs(x - nameX) <= TOLERANCE && Math.Abs(y - nameY) <= TOLERANCE)
                        name = c.Text.Trim();
                }
            }

            return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name);
        }
    }
}

