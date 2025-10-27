using Common;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine.Common;
using WinSelfMachine.Controls;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
using static Common.Response;
using MyNamespace;
using static WinSelfMachine.Model.HolModel;
using System.Diagnostics;

namespace WinSelfMachine
{
    public partial class Form1 : Form
    {
		// 自适应布局：记录设计时窗体大小与控件基准信息
		private Size _designClientSize;
		private Rectangle _label1Bounds;
		private Rectangle _roundTextBox1Bounds;
		private Rectangle _label2Bounds;
		private float _label1FontSize;
		private float _roundTextBox1FontSize;
		private float _label2FontSize;

        private IniFileHelper _iniConfig;
        private string _configFilePath;
        private readonly ApiCommon _apiCommon;
        
        // 防误触机制：需要双击五次才能触发
        private int doubleClickCount = 0;
        private const int requiredDoubleClicks = 5;
        private Timer resetDoubleClickTimer;
        private const int resetDoubleClickInterval = 10000; // 3秒后重置计数


        public Form1()
        {
            InitializeComponent();
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniConfig = new IniFileHelper(_configFilePath);
             _apiCommon = new ApiCommon();
            this.Resize += Form1_Resize;
           

            // 启用双缓冲，减少闪烁
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint | 
                         ControlStyles.DoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);
            
            // 绑定顶部区域双击事件（窗体与顶部控件）
            this.MouseDoubleClick += Form1_MouseDoubleClick;
            this.TxtTitle.MouseDoubleClick += Form1_MouseDoubleClick;
			this.pictureBox1.MouseDoubleClick += Form1_MouseDoubleClick;

			// 记录设计时的基准值（用于后续缩放）
			_captureDesignMetrics();
			ApplyResponsiveLayout();
            Txtbr.Focus();
            
            // 初始化双击防误触计时器
            resetDoubleClickTimer = new Timer();
            resetDoubleClickTimer.Interval = resetDoubleClickInterval;
            resetDoubleClickTimer.Tick += ResetDoubleClickTimer_Tick;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var IsStart = _iniConfig.ReadInt("EquipmentIsStart", "IsStart", 0);
            if (IsStart == 0)
            {
                var Setting = new FormProInitialize();
                Setting.ShowDialog();
            }
            var SerUrl = _iniConfig.Read("EquipmentUrl", "SerUrl", "");
            var PrinterId = _iniConfig.ReadInt("Printer", "PrinterId", -1);

            if (!string.IsNullOrEmpty(SerUrl) && PrinterId != -1)
            {
                await _apiCommon.UpdatePrinterStatus(PrinterId, 1);
            }

        }
        /// <summary>
        ///主界面控件
        /// </summary>
        /// <param name="visible"></param>
        private void SettingControl(bool visible)
        {
            // 暂停重绘，避免闪烁
            this.SuspendLayout();
            
            try
            {
                BtnPrintSetting.Visible = visible;
                BtnWaitTime.Visible = visible;
                BtnAvailableFilm.Visible = visible;
                BtnClose.Visible = visible;
                BtnSetting.Visible = visible;
            }
            finally
            {
                // 恢复重绘，一次性更新
                this.ResumeLayout(true);
            }
        }
        #region 控制窗体适应不同的分辨率
        /// <summary>
        /// 窗体大小变化时调整控件布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Form1_Resize(object sender, EventArgs e)
        {
			ApplyResponsiveLayout();
        }




        #endregion

        /// <summary>
        /// 顶部区域双击统一入口
        /// </summary>
        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // 增加双击计数
            doubleClickCount++;
            
            // 重置计时器
            resetDoubleClickTimer.Stop();
            resetDoubleClickTimer.Start();
            
            // 显示当前点击进度
            this.Text = $"医院自助一体机 - 双击进度: {doubleClickCount}/{requiredDoubleClicks}";
            
            // 只有达到要求的双击次数才执行操作
            if (doubleClickCount < requiredDoubleClicks)
            {
                return; // 未达到要求次数，直接返回
            }
            
            // 重置计数
            doubleClickCount = 0;
            resetDoubleClickTimer.Stop();
            this.Text = "医院自助一体机"; // 恢复标题
            
            // 仅响应靠近顶部的双击（阈值像素）
            const int topThreshold = 40; // 可根据实际 UI 调整
            const int leftThreshold = 40; // 左侧阈值

            // 当事件来自子控件时，e.Location 是相对该控件，需要转换到窗体坐标
            Point clientPoint;
            if (sender is Control ctrl && ctrl != this)
            {
                clientPoint = this.PointToClient(ctrl.PointToScreen(e.Location));
            }
            else
            {
                clientPoint = e.Location;
            }
            //顶部双击：关闭窗体
            if (clientPoint.Y <= topThreshold)
            {
                var res = MessageBox.Show("是否退出系统", "提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
                if(res == DialogResult.OK)
                    Close();
                return;
            }

            // 左侧区域双击：返回主界面
            if (clientPoint.X <= leftThreshold)
            {
                SettingControl(true);
                return;
            }
        }

        /// <summary>
        /// 打印设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrintSetting_Click(object sender, EventArgs e)
        {
            var pringSetting = new FormPrintSetting();
            pringSetting.ShowDialog();
        }

        /// <summary>
        /// 等待时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnWaitTime_Click(object sender, EventArgs e)
        {
            var waittime = new FormWaitTime();
            waittime.ShowDialog();
        }

        /// <summary>
        /// 可用胶片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAvailableFilm_Click(object sender, EventArgs e)
        {
            var AvailableFilm = new FormAvailableFilm();
            AvailableFilm.ShowDialog();
        }

        /// <summary>
        /// 启动设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetting_Click(object sender, EventArgs e)
        {
            var ProInitialize = new FormProInitialize();
            ProInitialize.ShowDialog();
        }

        /// <summary>
        /// 用户按下F11后显示设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F11)
            {
                 SettingControl(true);
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnClose_Click(object sender, EventArgs e)
        {
            var SerUrl = _iniConfig.Read("EquipmentUrl", "SerUrl", "");
            var PrinterId =  _iniConfig.ReadInt("Printer", "PrinterId", -1);

            if (!string.IsNullOrEmpty(SerUrl) && PrinterId != -1)
            {
                await _apiCommon.UpdatePrinterStatus(PrinterId,0);
            }
            Close();
        }

		// ======= 自适应布局实现 =======
		private void _captureDesignMetrics()
		{
			_designClientSize = this.ClientSize;
			_label1Bounds = label1.Bounds;
			_roundTextBox1Bounds = Txtbr.Bounds;
			_label2Bounds = label2.Bounds;
			_label1FontSize = label1.Font.Size;
			_roundTextBox1FontSize = Txtbr.Font.Size;
			_label2FontSize = label2.Font.Size;
		}

		private void ApplyResponsiveLayout()
		{
			if (_designClientSize.Width == 0 || _designClientSize.Height == 0)
				return;

			float scaleX = (float)this.ClientSize.Width / _designClientSize.Width;
			float scaleY = (float)this.ClientSize.Height / _designClientSize.Height;
			float fontScale = Math.Min(scaleX, scaleY);

			// label1
			label1.Location = new Point(
				(int)Math.Round(_label1Bounds.X * scaleX),
				(int)Math.Round(_label1Bounds.Y * scaleY));
			label1.Size = new Size(
				(int)Math.Round(_label1Bounds.Width * scaleX),
				(int)Math.Round(_label1Bounds.Height * scaleY));
			label1.Font = new Font(label1.Font.FontFamily, Math.Max(1f, _label1FontSize * fontScale), label1.Font.Style, label1.Font.Unit);
			// 让 AutoSize 文本按照当前字体取得恰当宽高
			Size label1Preferred = TextRenderer.MeasureText(label1.Text, label1.Font);
			label1.Size = label1Preferred;

			// roundTextBox1
			Txtbr.Location = new Point(
				(int)Math.Round(_roundTextBox1Bounds.X * scaleX),
				(int)Math.Round(_roundTextBox1Bounds.Y * scaleY));
			Txtbr.Size = new Size(
				(int)Math.Round(_roundTextBox1Bounds.Width * scaleX),
				(int)Math.Round(_roundTextBox1Bounds.Height * scaleY));
			Txtbr.Font = new Font(Txtbr.Font.FontFamily, Math.Max(1f, _roundTextBox1FontSize * fontScale), Txtbr.Font.Style, Txtbr.Font.Unit);

			// 将 label1 与 roundTextBox1 紧挨并整体水平居中
			int totalWidth = label1.Width + Txtbr.Width; // 无间距
			int groupLeft = Math.Max(0, (this.ClientSize.Width - totalWidth) / 2);
			label1.Left = groupLeft;
			Txtbr.Left = label1.Right; // 紧挨
			// 垂直居中对齐两者（以 label1 的 Y 为基准）
			Txtbr.Top = label1.Top + (label1.Height - Txtbr.Height) / 2;

			// label2
			label2.Location = new Point(
				(int)Math.Round(_label2Bounds.X * scaleX),
				(int)Math.Round(_label2Bounds.Y * scaleY));
			label2.Size = new Size(
				(int)Math.Round(_label2Bounds.Width * scaleX),
				(int)Math.Round(_label2Bounds.Height * scaleY));
			label2.Font = new Font(label2.Font.FontFamily, Math.Max(1f, _label2FontSize * fontScale), label2.Font.Style, label2.Font.Unit);

			// 始终水平居中
			label2.Left = Math.Max(0, (this.ClientSize.Width - label2.Width) / 2);
		}

        /// <summary>
        /// 触发打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Txtbr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r' || Txtbr.Text.Trim() == "") return;

            string examNo = Txtbr.Text.Trim();
            Txtbr.Text = "";

            try
            {
                // 显示加载提示
                this.Text = "正在查询检查数据...";
                this.Refresh();

                // 获取检查数据
                var response = await _apiCommon.GetExaminationByNo(examNo);
                if (string.IsNullOrEmpty(response))
                {
                    MessageBox.Show("未找到该检查号的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Text = "医院自助一体机";
                    return;
                }

                var responseData = JsonConvert.DeserializeObject<ApiResponse<HolExamination>>(response);
                if (responseData?.Response == null)
                {
                    MessageBox.Show("检查数据格式错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Text = "医院自助一体机";
                    return;
                }

                var examination = responseData.Response;
                
                // 检查是否已打印
                if (examination.is_printed == 1)
                {
                    var result = MessageBox.Show("该检查报告已打印过，是否重新打印？", "提示", 
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                    {
                        this.Text = "医院自助一体机";
                        return;
                    }
                }

                // 获取打印机配置
                var printerConfig = await GetPrinterConfiguration();
                if (printerConfig == null)
                {
                    MessageBox.Show("未找到可用的打印机配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Text = "医院自助一体机";
                    return;
                }

                // 执行打印
                await PrintReport(examination, printerConfig);

                // 保存打印记录
                await SavePrintRecord(examination);

                MessageBox.Show("打印完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Text = "医院自助一体机";
            }
        }
        
        /// <summary>
        /// 重置双击计数计时器事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetDoubleClickTimer_Tick(object sender, EventArgs e)
        {
            doubleClickCount = 0; // 重置双击计数
            resetDoubleClickTimer.Stop();
        }

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <returns></returns>
        private async Task<PrinterConfiguration> GetPrinterConfiguration()
        {
            try
            {
                // 从INI文件读取打印机配置
                var printerName = _iniConfig.Read("Printer", "PrinterOrdinary", "");
                var paperSize = _iniConfig.Read("Printer", "PrinterOrdinaryPage", "");

                if (string.IsNullOrEmpty(printerName))
                {
                    return null;
                }

                // 获取打印机配置
                var response = await _apiCommon.GetPrinterConfig();
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolPrinterConfig>>>(response);
                var configs = responseData?.Response;

                return new PrinterConfiguration
                {
                    PrinterName = printerName,
                    PaperSize = paperSize,
                    Configs = configs ?? new List<HolPrinterConfig>()
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 打印报告
        /// </summary>
        /// <param name="examination">检查数据</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintReport(HolExamination examination, PrinterConfiguration printerConfig)
        {
            try
            {
                // 根据检查类型确定打印方式
                if (examination.exam_type?.ToUpper() == "胶片" || !string.IsNullOrEmpty(examination.image_path))
                {
                    // 胶片打印
                    await PrintFilm(examination, printerConfig);
                }
                else
                {
                    // 报告打印
                    await PrintDocument(examination, printerConfig);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"打印过程中发生错误：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印胶片
        /// </summary>
        /// <param name="examination">检查数据</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintFilm(HolExamination examination, PrinterConfiguration printerConfig)
        {
            try
            {
                if (string.IsNullOrEmpty(examination.image_path))
                {
                    throw new Exception("未找到胶片文件路径");
                }

                // 获取胶片尺寸对应的激光相机配置
                var filmConfig = GetFilmPrinterConfig(examination, printerConfig);
                if (filmConfig == null)
                {
                    throw new Exception($"未找到胶片尺寸 {examination.exam_type} 对应的激光相机配置");
                }

                // 获取激光相机信息
                var laserCamera = await GetLaserCameraInfo(filmConfig.laser_printer_id);
                if (laserCamera == null)
                {
                    throw new Exception("未找到激光相机信息");
                }

                // 执行DICOM打印
                await PrintDicomFilm(examination.image_path, laserCamera, examination);
            }
            catch (Exception ex)
            {
                throw new Exception($"胶片打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取胶片尺寸对应的激光相机配置
        /// </summary>
        /// <param name="examination">检查数据</param>
        /// <param name="printerConfig">打印机配置</param>
        /// <returns></returns>
        private HolPrinterConfig GetFilmPrinterConfig(HolExamination examination, PrinterConfiguration printerConfig)
        {
            // 根据检查类型或胶片尺寸匹配配置
            var examType = examination.exam_type?.ToUpper();
            
            // 尝试匹配胶片尺寸
            foreach (var config in printerConfig.Configs)
            {
                if (!string.IsNullOrEmpty(config.film_size) && 
                    config.film_size.Equals(examType, StringComparison.OrdinalIgnoreCase))
                {
                    return config;
                }
            }

            // 如果没有精确匹配，尝试模糊匹配
            foreach (var config in printerConfig.Configs)
            {
                if (!string.IsNullOrEmpty(config.film_size) && 
                    config.film_size.Contains(examType))
                {
                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取激光相机信息
        /// </summary>
        /// <param name="laserCameraId">激光相机ID</param>
        /// <returns></returns>
        private async Task<HolPrinter> GetLaserCameraInfo(int laserCameraId)
        {
            try
            {
                var response = await _apiCommon.GetPrinter();
                if (string.IsNullOrEmpty(response))
                {
                    return null;
                }

                var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolPrinter>>>(response);
                var printers = responseData?.Response;

                if (printers != null)
                {
                    return printers.FirstOrDefault(p => p.id == laserCameraId && p.type == 4); // type=4 表示激光相机
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 执行DICOM胶片打印
        /// </summary>
        /// <param name="imagePath">DICOM图像路径</param>
        /// <param name="laserCamera">激光相机信息</param>
        /// <param name="examination">检查数据</param>
        private async Task PrintDicomFilm(string imagePath, HolPrinter laserCamera, HolExamination examination)
        {
            try
            {
                // 验证DICOM文件
                if (!File.Exists(imagePath))
                {
                    throw new Exception($"DICOM文件不存在：{imagePath}");
                }

                // 构建DICOM打印参数
                var dicomPrintParams = new DicomPrintParameters
                {
                    ImagePath = imagePath,
                    AETitle = laserCamera.name ?? "LASER_CAMERA",
                    IPAddress = laserCamera.ip_address,
                    Port = laserCamera.port ?? 104,
                    PatientName = examination.patient?.name ?? "Unknown",
                    PatientID = examination.patient_id.ToString(),
                    StudyInstanceUID = examination.exam_no,
                    SeriesInstanceUID = examination.exam_no + "_SERIES",
                    SOPInstanceUID = examination.exam_no + "_INSTANCE"
                };

                // 执行DICOM打印
                await ExecuteDicomPrint(dicomPrintParams);

                // 记录打印日志
                LogDicomPrint(dicomPrintParams, examination);
            }
            catch (Exception ex)
            {
                throw new Exception($"DICOM打印执行失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 执行DICOM打印
        /// </summary>
        /// <param name="parameters">DICOM打印参数</param>
        private async Task ExecuteDicomPrint(DicomPrintParameters parameters)
        {
            try
            {
                // 方法1：使用DICOM工具（如dcmtk）进行打印
                await PrintWithDcmtk(parameters);
                
                // 方法2：如果dcmtk不可用，使用自定义DICOM客户端
                await PrintWithCustomDicomClient(parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"DICOM打印执行失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 使用dcmtk工具进行DICOM打印
        /// </summary>
        /// <param name="parameters">DICOM打印参数</param>
        private async Task PrintWithDcmtk(DicomPrintParameters parameters)
        {
            try
            {
                // 构建dcmtk命令行参数
                var arguments = $"-aet {parameters.AETitle} " +
                               $"-aec LASER_CAMERA " +
                               $"-aes {parameters.AETitle} " +
                               $"-d {parameters.IPAddress} " +
                               $"-p {parameters.Port} " +
                               $"-m +P " +
                               $"\"{parameters.ImagePath}\"";

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "dcmsend", // dcmtk工具
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                
                // 等待打印完成
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"dcmtk打印失败：{error}");
                }

                // 等待打印完成（根据胶片尺寸调整等待时间）
                await Task.Delay(GetPrintWaitTime(parameters.ImagePath));
            }
            catch (Exception ex)
            {
                throw new Exception($"dcmtk打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 使用自定义DICOM客户端进行打印
        /// </summary>
        /// <param name="parameters">DICOM打印参数</param>
        private async Task PrintWithCustomDicomClient(DicomPrintParameters parameters)
        {
            try
            {
                // 这里可以实现自定义的DICOM客户端
                // 使用TCP连接发送DICOM打印请求
                
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    await client.ConnectAsync(parameters.IPAddress, parameters.Port);
                    
                    using (var stream = client.GetStream())
                    {
                        // 构建DICOM打印请求
                        var dicomRequest = BuildDicomPrintRequest(parameters);
                        
                        // 发送DICOM数据
                        await stream.WriteAsync(dicomRequest, 0, dicomRequest.Length);
                        
                        // 读取响应
                        var response = new byte[1024];
                        var bytesRead = await stream.ReadAsync(response, 0, response.Length);
                        
                        // 验证响应
                        if (bytesRead == 0)
                        {
                            throw new Exception("DICOM打印请求无响应");
                        }
                    }
                }

                // 等待打印完成
                await Task.Delay(GetPrintWaitTime(parameters.ImagePath));
            }
            catch (Exception ex)
            {
                throw new Exception($"自定义DICOM客户端打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 构建DICOM打印请求
        /// </summary>
        /// <param name="parameters">DICOM打印参数</param>
        /// <returns></returns>
        private byte[] BuildDicomPrintRequest(DicomPrintParameters parameters)
        {
            // 这里需要实现DICOM协议的C-STORE请求构建
            // 简化实现，实际需要完整的DICOM协议支持
            
            var request = new List<byte>();
            
            // DICOM PDU Header
            request.AddRange(BitConverter.GetBytes((uint)0x00000000)); // Length
            request.AddRange(BitConverter.GetBytes((uint)0x00000001)); // Type: C-STORE-RQ
            
            // 这里需要添加完整的DICOM数据集
            // 包括Patient信息、Study信息、Series信息等
            
            return request.ToArray();
        }

        /// <summary>
        /// 获取打印等待时间
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <returns></returns>
        private int GetPrintWaitTime(string imagePath)
        {
            try
            {
                // 根据文件大小估算打印时间
                var fileInfo = new FileInfo(imagePath);
                var fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);
                
                // 基础等待时间 + 文件大小相关时间
                var baseTime = 3000; // 3秒基础时间
                var sizeTime = (int)(fileSizeMB * 1000); // 每MB增加1秒
                
                return Math.Min(baseTime + sizeTime, 30000); // 最多等待30秒
            }
            catch
            {
                return 5000; // 默认等待5秒
            }
        }

        /// <summary>
        /// 记录DICOM打印日志
        /// </summary>
        /// <param name="parameters">DICOM打印参数</param>
        /// <param name="examination">检查数据</param>
        private void LogDicomPrint(DicomPrintParameters parameters, HolExamination examination)
        {
            try
            {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DICOM打印完成 - " +
                               $"患者：{examination.patient?.name}，" +
                               $"检查号：{examination.exam_no}，" +
                               $"激光相机：{parameters.AETitle}@{parameters.IPAddress}:{parameters.Port}，" +
                               $"文件：{parameters.ImagePath}";
                
                System.Diagnostics.Debug.WriteLine(logMessage);
                
                // 可以写入日志文件
                // File.AppendAllText("dicom_print.log", logMessage + Environment.NewLine);
            }
            catch
            {
                // 日志记录失败不影响打印流程
            }
        }

        /// <summary>
        /// DICOM打印参数类
        /// </summary>
        public class DicomPrintParameters
        {
            public string ImagePath { get; set; }
            public string AETitle { get; set; }
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public string PatientName { get; set; }
            public string PatientID { get; set; }
            public string StudyInstanceUID { get; set; }
            public string SeriesInstanceUID { get; set; }
            public string SOPInstanceUID { get; set; }
        }

        /// <summary>
        /// 打印文档报告
        /// </summary>
        /// <param name="examination">检查数据</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintDocument(HolExamination examination, PrinterConfiguration printerConfig)
        {
            try
            {
                // 优先打印已有文件
                if (!string.IsNullOrEmpty(examination.report_path) && File.Exists(examination.report_path))
                {
                    await PrintExistingFile(examination.report_path, printerConfig);
                }
                else
                {
                    // 如果没有文件，则生成打印内容
                    await PrintGeneratedContent(examination, printerConfig);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"文档打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印已有文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintExistingFile(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                var fileExtension = Path.GetExtension(filePath).ToLower();
                
                switch (fileExtension)
                {
                    case ".pdf":
                        await PrintPdfFile(filePath, printerConfig);
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".bmp":
                    case ".gif":
                        await PrintImageFile(filePath, printerConfig);
                        break;
                    case ".txt":
                        await PrintTextFile(filePath, printerConfig);
                        break;
                    case ".doc":
                    case ".docx":
                        await PrintWordFile(filePath, printerConfig);
                        break;
                    default:
                        // 尝试使用系统默认程序打印
                        await PrintWithDefaultProgram(filePath, printerConfig);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"打印文件失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印PDF文件
        /// </summary>
        /// <param name="filePath">PDF文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintPdfFile(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                // 使用Adobe Reader或其他PDF阅读器打印
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = $"/p /h /t \"{filePath}\" \"{printerConfig.PrinterName}\"",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                await Task.Delay(3000); // 等待打印完成
            }
            catch (Exception ex)
            {
                throw new Exception($"PDF打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印图片文件
        /// </summary>
        /// <param name="filePath">图片文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintImageFile(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                using (var printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings.PrinterName = printerConfig.PrinterName;
                    
                    // 设置纸张大小
                    if (!string.IsNullOrEmpty(printerConfig.PaperSize))
                    {
                        foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                        {
                            if (paperSize.PaperName.Contains(printerConfig.PaperSize))
                            {
                                printDocument.DefaultPageSettings.PaperSize = paperSize;
                                break;
                            }
                        }
                    }

                    // 加载图片
                    using (var image = Image.FromFile(filePath))
                    {
                        printDocument.PrintPage += (sender, e) =>
                        {
                            var graphics = e.Graphics;
                            var pageBounds = e.MarginBounds;
                            
                            // 计算图片缩放比例以适应页面
                            float scaleX = (float)pageBounds.Width / image.Width;
                            float scaleY = (float)pageBounds.Height / image.Height;
                            float scale = Math.Min(scaleX, scaleY);
                            
                            int scaledWidth = (int)(image.Width * scale);
                            int scaledHeight = (int)(image.Height * scale);
                            
                            // 居中显示
                            int x = pageBounds.X + (pageBounds.Width - scaledWidth) / 2;
                            int y = pageBounds.Y + (pageBounds.Height - scaledHeight) / 2;
                            
                            graphics.DrawImage(image, x, y, scaledWidth, scaledHeight);
                        };

                        printDocument.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"图片打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印文本文件
        /// </summary>
        /// <param name="filePath">文本文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintTextFile(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                using (var printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings.PrinterName = printerConfig.PrinterName;
                    
                    // 设置纸张大小
                    if (!string.IsNullOrEmpty(printerConfig.PaperSize))
                    {
                        foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                        {
                            if (paperSize.PaperName.Contains(printerConfig.PaperSize))
                            {
                                printDocument.DefaultPageSettings.PaperSize = paperSize;
                                break;
                            }
                        }
                    }

                    var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                    
                    printDocument.PrintPage += (sender, e) =>
                    {
                        var graphics = e.Graphics;
                        var font = new Font("宋体", 12);
                        var brush = Brushes.Black;
                        var pageBounds = e.MarginBounds;
                        
                        // 分页打印文本内容
                        var lines = fileContent.Split('\n');
                        float yPosition = pageBounds.Top;
                        float lineHeight = font.GetHeight(graphics);
                        
                        foreach (var line in lines)
                        {
                            if (yPosition + lineHeight > pageBounds.Bottom)
                            {
                                e.HasMorePages = true;
                                return;
                            }
                            
                            graphics.DrawString(line, font, brush, pageBounds.Left, yPosition);
                            yPosition += lineHeight;
                        }
                        
                        e.HasMorePages = false;
                    };

                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"文本文件打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印Word文件
        /// </summary>
        /// <param name="filePath">Word文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintWordFile(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                // 使用Microsoft Word打印
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "winword.exe",
                        Arguments = $"/p \"{filePath}\"",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                await Task.Delay(5000); // 等待打印完成
            }
            catch (Exception ex)
            {
                throw new Exception($"Word文档打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 使用系统默认程序打印
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintWithDefaultProgram(string filePath, PrinterConfiguration printerConfig)
        {
            try
            {
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = "/p",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                await Task.Delay(3000); // 等待打印完成
            }
            catch (Exception ex)
            {
                throw new Exception($"使用默认程序打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印生成的内容（当没有文件时）
        /// </summary>
        /// <param name="examination">检查数据</param>
        /// <param name="printerConfig">打印机配置</param>
        private async Task PrintGeneratedContent(HolExamination examination, PrinterConfiguration printerConfig)
        {
            try
            {
                using (var printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings.PrinterName = printerConfig.PrinterName;
                    
                    // 设置纸张大小
                    if (!string.IsNullOrEmpty(printerConfig.PaperSize))
                    {
                        foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                        {
                            if (paperSize.PaperName.Contains(printerConfig.PaperSize))
                            {
                                printDocument.DefaultPageSettings.PaperSize = paperSize;
                                break;
                            }
                        }
                    }

                    // 设置打印内容
                    printDocument.PrintPage += (sender, e) => PrintDocument_PrintPage(sender, e, examination);

                    // 执行打印
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"生成内容打印失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 打印页面事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="examination">检查数据</param>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e, HolExamination examination)
        {
            try
            {
                var graphics = e.Graphics;
                var font = new Font("宋体", 12);
                var brush = Brushes.Black;
                float yPosition = 50;
                float lineHeight = 25;

                // 打印标题
                graphics.DrawString("检查报告", new Font("宋体", 16, FontStyle.Bold), brush, 50, yPosition);
                yPosition += lineHeight * 2;

                // 打印患者信息
                graphics.DrawString($"患者姓名：{examination.patient?.name ?? "未知"}", font, brush, 50, yPosition);
                yPosition += lineHeight;
                graphics.DrawString($"性别：{examination.patient?.gender ?? "未知"}", font, brush, 50, yPosition);
                yPosition += lineHeight;
                graphics.DrawString($"年龄：{examination.patient?.age?.ToString() ?? "未知"}", font, brush, 50, yPosition);
                yPosition += lineHeight;
                graphics.DrawString($"检查号：{examination.exam_no}", font, brush, 50, yPosition);
                yPosition += lineHeight;
                graphics.DrawString($"检查类型：{examination.exam_type}", font, brush, 50, yPosition);
                yPosition += lineHeight;
                graphics.DrawString($"检查日期：{examination.exam_date:yyyy-MM-dd HH:mm}", font, brush, 50, yPosition);
                yPosition += lineHeight * 2;

                // 打印报告内容
                if (!string.IsNullOrEmpty(examination.report_path) && File.Exists(examination.report_path))
                {
                    graphics.DrawString("报告内容：", new Font("宋体", 14, FontStyle.Bold), brush, 50, yPosition);
                    yPosition += lineHeight;
                    
                    // 这里可以读取并打印报告文件内容
                    // 实际实现中需要根据文件格式（PDF、图片等）进行相应处理
                    graphics.DrawString("（报告文件已生成）", font, brush, 50, yPosition);
                }

                e.HasMorePages = false;
            }
            catch (Exception ex)
            {
                throw new Exception($"打印页面生成失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 保存打印记录
        /// </summary>
        /// <param name="examination">检查数据</param>
        private async Task SavePrintRecord(HolExamination examination)
        {
            try
            {
                var printRecord = new PrintRecordModel
                {
                    exam_id = examination.id,
                    patient_id = examination.patient_id,
                    print_time = DateTime.Now,
                    printed_by = examination.patient_id, // 患者本人打印
                    status = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now
                };

                await _apiCommon.SavePrintRecord(printRecord);
            }
            catch (Exception ex)
            {
                // 打印记录保存失败不影响打印流程，只记录错误
                Debug.WriteLine($"保存打印记录失败：{ex.Message}");
            }
        }

    }
}
