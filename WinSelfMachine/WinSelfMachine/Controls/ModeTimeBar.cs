using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSelfMachine.Controls
{
    public partial class ModeTimeBar : UserControl
    {
        private Timer timer;
        private bool _isOn = true;
        // 开关相关绘制参数
        private int switchHeight = 16;
        private int switchWidth = 30;
        private int switchInnerSize = 12;
        private int switchOffset = 2;
        private int cornerRadius = 12; // 圆角半径（更圆润）
        
        // 布局相关参数
        private int padding = 10; // 内边距
        private int spacing = 10; // 元素间距
        
        // 开关颜色相关参数
        private Color _switchOnColor = Color.FromArgb(41, 123, 244); // 选中时的背景色
        private Color _switchOffColor = Color.FromArgb(200, 200, 200); // 未选中时的背景色
        private Color _switchBorderColor = Color.White; // 边框颜色

        public ModeTimeBar()
        {
            InitializeComponent();
            this.Size = new Size(400, 30);
            this.DoubleBuffered = true;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeText = DateTime.Now.ToString("HH:mm");
        }

        private Image _icon;
        private string _hospitalText = "苏州XX医院";
        private string _modeText = "关爱模式";
        private string _timeText = DateTime.Now.ToString("HH:mm");

        [Category("自定义")]
        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                Invalidate();
            }
        }

        [Category("自定义")]
        public Image Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                Invalidate();
            }
        }

        [Category("自定义")]
        public string HospitalText
        {
            get => _hospitalText;
            set
            {
                _hospitalText = value;
                Invalidate();
            }
        }

        [Category("自定义")]
        public string ModeText
        {
            get => _modeText;
            set
            {
                _modeText = value;
                Invalidate();
            }
        }

        [Category("自定义")]
        public string TimeText
        {
            get => _timeText;
            set
            {
                _timeText = value;
                Invalidate();
            }
        }

        [Category("布局")]
        public int Padding
        {
            get => padding;
            set
            {
                padding = value;
                Invalidate();
            }
        }

        [Category("布局")]
        public int Spacing
        {
            get => spacing;
            set
            {
                spacing = value;
                Invalidate();
            }
        }

        [Category("开关样式")]
        public Color SwitchOnColor
        {
            get => _switchOnColor;
            set
            {
                _switchOnColor = value;
                Invalidate();
            }
        }

        [Category("开关样式")]
        public Color SwitchOffColor
        {
            get => _switchOffColor;
            set
            {
                _switchOffColor = value;
                Invalidate();
            }
        }

        [Category("开关样式")]
        public Color SwitchBorderColor
        {
            get => _switchBorderColor;
            set
            {
                _switchBorderColor = value;
                Invalidate();
            }
        }

        [Category("开关样式")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        // 创建圆角矩形路径（更圆润的圆角）
        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            // 确保圆角半径不超过矩形尺寸的一半，避免过度圆角
            int actualRadius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            
            // 如果矩形太小，直接创建普通矩形
            if (actualRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            path.StartFigure();
            
            // 左上角
            path.AddArc(rect.X, rect.Y, actualRadius * 2, actualRadius * 2, 180, 90);
            // 右上角
            path.AddArc(rect.Right - actualRadius * 2, rect.Y, actualRadius * 2, actualRadius * 2, 270, 90);
            // 右下角
            path.AddArc(rect.Right - actualRadius * 2, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 0, 90);
            // 左下角
            path.AddArc(rect.X, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // 计算各元素的位置（动态布局）
            int currentX = padding;
            int centerY = this.Height / 2;
            int iconSize = 16;
            
            // 绘制图标
            if (_icon != null)
            {
                int iconY = centerY - iconSize / 2;
                g.DrawImage(_icon, currentX, iconY, iconSize, iconSize);
                currentX += iconSize + spacing;
            }
            
            // 绘制医院名称
            using (var textBrush = new SolidBrush(Color.White))
            {
                var font = new Font("微软雅黑", 10, FontStyle.Regular);
                var textSize = g.MeasureString(_hospitalText, font);
                int textY = centerY - (int)textSize.Height / 2;
                g.DrawString(_hospitalText, font, textBrush, currentX, textY);
                currentX += (int)textSize.Width + spacing;
            }
            
            // 计算右侧元素的位置（从右往左布局）
            var timeFont = new Font("微软雅黑", 10, FontStyle.Regular);
            var timeSize = g.MeasureString(_timeText, timeFont);
            var modeFont = new Font("微软雅黑", 10, FontStyle.Regular);
            var modeSize = g.MeasureString(_modeText, modeFont);
            
            // 时间文字位置（最右边）
            int timeX = this.Width - padding - (int)timeSize.Width;
            int timeY = centerY - (int)timeSize.Height / 2;
            
            // 模式文字位置（时间左边）
            int modeX = timeX - spacing - (int)modeSize.Width;
            int modeY = centerY - (int)modeSize.Height / 2;
            
            // 开关位置（模式文字左边）
            int switchX = modeX - spacing - switchWidth;
            Rectangle switchRect = new Rectangle(switchX, centerY - switchHeight / 2, switchWidth, switchHeight);
            
            // 创建圆角矩形路径
            using (GraphicsPath switchPath = CreateRoundedRectanglePath(switchRect, cornerRadius))
            {
                // 根据开关状态选择背景颜色
                Color backgroundColor = _isOn ? _switchOnColor : _switchOffColor;
                
                // 绘制开关外框（圆角矩形）
                using (var switchPen = new Pen(_switchBorderColor, 1))
                using (var switchBrush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(switchBrush, switchPath);
                    g.DrawPath(switchPen, switchPath);
                }
            }
            
            // 绘制开关内圆（白色，根据状态决定位置）
            int innerX = _isOn ? switchRect.X + switchOffset : switchRect.X + switchWidth - switchInnerSize - switchOffset;
            Rectangle switchInnerRect = new Rectangle(innerX, switchRect.Y + switchOffset, switchInnerSize, switchInnerSize);
            using (var innerBrush = new SolidBrush(Color.White))
                g.FillEllipse(innerBrush, switchInnerRect);
            
            // 绘制模式文字
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(_modeText, modeFont, textBrush, modeX, modeY);
            }
            
            // 绘制时间文字
            using (var textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(_timeText, timeFont, textBrush, timeX, timeY);
            }
        }

        // 处理鼠标点击，切换开关状态
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // 重新计算开关位置（与绘制时保持一致）
            using (var g = this.CreateGraphics())
            {
                var timeFont = new Font("微软雅黑", 10, FontStyle.Regular);
                var timeSize = g.MeasureString(_timeText, timeFont);
                var modeFont = new Font("微软雅黑", 10, FontStyle.Regular);
                var modeSize = g.MeasureString(_modeText, modeFont);
                
                int timeX = this.Width - padding - (int)timeSize.Width;
                int modeX = timeX - spacing - (int)modeSize.Width;
                int switchX = modeX - spacing - switchWidth;
                int centerY = this.Height / 2;
                
                Rectangle switchRect = new Rectangle(switchX, centerY - switchHeight / 2, switchWidth, switchHeight);
                
                if (switchRect.Contains(e.Location))
                {
                    IsOn = !IsOn;
                }
            }
        }
    }
}
    