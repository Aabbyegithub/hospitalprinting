using Common;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine.Common;
using WinSelfMachine.Controls;

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
         private void Txtbr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\r' || Txtbr.Text.Trim() == "") return;

            Txtbr.Text = "";
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
            this.Text = "医院自助一体机"; // 恢复标题
        }
    }
}
