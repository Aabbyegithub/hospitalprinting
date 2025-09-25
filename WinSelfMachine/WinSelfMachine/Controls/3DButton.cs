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
    public partial class _3DButton : UserControl
    {
        private int cornerRadius = 8;
        private Color topColor = Color.FromArgb(240, 248, 255);
        private Color bottomColor = Color.FromArgb(0, 120, 215);
        private Color shadowColor = Color.FromArgb(50, 0, 0, 0);
        private int shadowOffset = 4;
        private int shadowOffsetX = 3;  // 水平阴影偏移
        private int shadowOffsetY = 3;  // 垂直阴影偏移
        private Color topHighlightColor = Color.FromArgb(100, 255, 255, 255);  // 上方高光颜色（白色半透明）
        private Color bottomShadowColor = Color.FromArgb(80, 0, 0, 0);  // 下方阴影颜色
        private string buttonText = "报告查询";
        private Color textColor = Color.Black;
        private Image buttonIcon;
        private Size iconSize = new Size(24, 24);
        private int iconTextSpacing = 8;
        private bool isPressed = false;
        private int padding = 12;
        private float fontSize = 10f;

        public _3DButton()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Size = new Size(120, 100);
            this.Cursor = Cursors.Hand;
        }

        [Category("外观")]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        public Color TopColor
        {
            get => topColor;
            set { topColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color BottomColor
        {
            get => bottomColor;
            set { bottomColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color ShadowColor
        {
            get => shadowColor;
            set { shadowColor = value; Invalidate(); }
        }

        [Category("外观")]
        public int ShadowOffset
        {
            get => shadowOffset;
            set { shadowOffset = Math.Max(0, value); Invalidate(); }
        }

        [Category("内容")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value ?? string.Empty; Invalidate(); }
        }

        [Category("内容")]
        public Color TextColor
        {
            get => textColor;
            set { textColor = value; Invalidate(); }
        }

        [Category("内容")]
        public Image ButtonIcon
        {
            get => buttonIcon;
            set { buttonIcon = value; Invalidate(); }
        }

        [Category("内容")]
        public Size IconSize
        {
            get => iconSize;
            set { iconSize = value; Invalidate(); }
        }

        [Category("内容")]
        public int IconTextSpacing
        {
            get => iconTextSpacing;
            set { iconTextSpacing = value; Invalidate(); }
        }

        [Category("布局")]
        public int Padding
        {
            get => padding;
            set { padding = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        public float FontSize
        {
            get => fontSize;
            set { 
                fontSize = Math.Max(1f, value); 
                System.Diagnostics.Debug.WriteLine($"3DButton FontSize 设置为: {fontSize}");
                Invalidate(); 
            }
        }

        /// <summary>
        /// 强制更新字体大小
        /// </summary>
        public void UpdateFontSize(float newSize)
        {
            fontSize = Math.Max(1f, newSize);
            System.Diagnostics.Debug.WriteLine($"强制更新字体大小: {fontSize}");
            Invalidate();
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int shadowX = isPressed ? shadowOffset / 2 : shadowOffset;
            int shadowY = isPressed ? shadowOffset / 2 : shadowOffset;

            // 绘制底部阴影
            using (var shadowPath = CreateRoundedPath(new Rectangle(shadowX, shadowY, bounds.Width, bounds.Height), cornerRadius))
            using (var shadowBrush = new SolidBrush(shadowColor))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // 绘制渐变背景
            using (var mainPath = CreateRoundedPath(bounds, cornerRadius))
            using (var gradientBrush = new LinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                g.FillPath(gradientBrush, mainPath);
            }

            // 绘制内容（图标和文字）
            DrawContent(g, bounds);
        }

        private void DrawContent(Graphics g, Rectangle bounds)
        {
            int centerX = bounds.Width / 2;
            int centerY = bounds.Height / 2;
            int totalContentHeight = 0;

            // 计算图标高度
            if (buttonIcon != null)
            {
                totalContentHeight += iconSize.Height;
            }

            // 计算文字高度
            if (!string.IsNullOrEmpty(buttonText))
            {
                using (var font = new Font("微软雅黑", fontSize, FontStyle.Bold))
                {
                    var textSize = g.MeasureString(buttonText, font);
                    totalContentHeight += (int)textSize.Height;
                }
            }

            // 添加间距
            if (buttonIcon != null && !string.IsNullOrEmpty(buttonText))
            {
                totalContentHeight += iconTextSpacing;
            }

            int startY = centerY - totalContentHeight / 2;

            // 绘制图标（在上方）
            if (buttonIcon != null)
            {
                int iconX = centerX - iconSize.Width / 2;
                int iconY = startY;
                g.DrawImage(buttonIcon, iconX, iconY, iconSize.Width, iconSize.Height);
                startY += iconSize.Height + iconTextSpacing;
            }

            // 绘制文字（在下方）
            if (!string.IsNullOrEmpty(buttonText))
            {
                using (var font = new Font("微软雅黑", fontSize, FontStyle.Bold))
                using (var textBrush = new SolidBrush(textColor))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    var textRect = new RectangleF(0, startY, bounds.Width, bounds.Height - startY);
                    g.DrawString(buttonText, font, textBrush, textRect, sf);
                }
            }
        }


        private GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = Math.Max(0, radius * 2);
            
            if (d <= 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }
            
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isPressed = false;
                Invalidate();
                OnClick(EventArgs.Empty);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isPressed = false;
            Invalidate();
        }
    }
}
