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
    public partial class RoundTextBox : UserControl
    {
        #region 私有字段
        private int cornerRadius = 8;
        private Color borderColor = Color.FromArgb(200, 200, 200);
        private Color borderFocusedColor = Color.FromArgb(41, 123, 244);
        private int borderWidth = 1;
        private bool isFocused = false;
        private bool isHovered = false;
        #endregion

        #region 构造函数
        public RoundTextBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.ResizeRedraw | 
                         ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            
            // 设置内部TextBox
            if (tbContent != null)
            {
                tbContent.BorderStyle = BorderStyle.None;
                tbContent.BackColor = Color.White;
                tbContent.TextAlign = HorizontalAlignment.Left;
                tbContent.GotFocus += (s, e) => { isFocused = true; Invalidate(); };
                tbContent.LostFocus += (s, e) => { isFocused = false; Invalidate(); };
                tbContent.MouseEnter += (s, e) => { isHovered = true; Invalidate(); };
                tbContent.MouseLeave += (s, e) => { isHovered = false; Invalidate(); };
                
                // 初始布局
                LayoutTextBox();
            }
        }
        #endregion

        #region 属性
        [Category("外观")]
        [Description("圆角半径")]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        [Description("边框颜色")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("外观")]
        [Description("获得焦点时的边框颜色")]
        public Color BorderFocusedColor
        {
            get => borderFocusedColor;
            set { borderFocusedColor = value; Invalidate(); }
        }

        [Category("外观")]
        [Description("边框宽度")]
        public int BorderWidth
        {
            get => borderWidth;
            set { borderWidth = Math.Max(0, value); Invalidate(); }
        }

        [Category("行为")]
        [Description("文本内容")]
        public override string Text
        {
            get => tbContent?.Text ?? string.Empty;
            set { if (tbContent != null) tbContent.Text = value; }
        }

        [Category("行为")]
        [Description("文本对齐方式")]
        public HorizontalAlignment TextAlign
        {
            get => tbContent?.TextAlign ?? HorizontalAlignment.Left;
            set { if (tbContent != null) tbContent.TextAlign = value; }
        }

        [Category("外观")]
        [Description("字体")]
        public override Font Font
        {
            get => base.Font;
            set 
            { 
                base.Font = value;
                if (tbContent != null) tbContent.Font = value;
                LayoutTextBox();
            }
        }

        [Category("外观")]
        [Description("字体大小")]
        public int FontSize
        {
            get => (int)this.Font.Size;
            set 
            { 
                if (value > 0)
                {
                    this.Font = new Font(this.Font.FontFamily, value, this.Font.Style);
                }
            }
        }

        [Category("行为")]
        [Description("是否只读")]
        public bool ReadOnly
        {
            get => tbContent?.ReadOnly ?? false;
            set { if (tbContent != null) tbContent.ReadOnly = value; }
        }

        [Category("行为")]
        [Description("密码字符")]
        public char PasswordChar
        {
            get => tbContent?.PasswordChar ?? '\0';
            set { if (tbContent != null) tbContent.PasswordChar = value; }
        }

        [Category("行为")]
        [Description("是否使用系统密码字符")]
        public bool UseSystemPasswordChar
        {
            get => tbContent?.UseSystemPasswordChar ?? false;
            set { if (tbContent != null) tbContent.UseSystemPasswordChar = value; }
        }

        [Category("行为")]
        [Description("最大长度")]
        public int MaxLength
        {
            get => tbContent?.MaxLength ?? 0;
            set { if (tbContent != null) tbContent.MaxLength = value; }
        }
        #endregion

        #region 私有方法
        private void LayoutTextBox()
        {
            if (tbContent == null) return;
            
            // 计算合适的字体高度
            int fontHeight = (int)Math.Ceiling(this.Font.GetHeight());
            int textBoxHeight = Math.Max(fontHeight + 4, this.Height - this.Padding.Vertical);
            
            // 计算垂直居中位置
            int textBoxY = (this.Height - textBoxHeight) / 2;
            
            // 确保Y坐标不会超出边界
            textBoxY = Math.Max(this.Padding.Top, textBoxY);
            textBoxY = Math.Min(textBoxY, this.Height - textBoxHeight - this.Padding.Bottom);
            
            tbContent.Location = new Point(this.Padding.Left, textBoxY);
            tbContent.Size = new Size(this.Width - this.Padding.Horizontal, textBoxHeight);
        }
        #endregion

        #region 重写方法
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            LayoutTextBox();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            // 设置内部TextBox背景色
            if (tbContent != null)
            {
                tbContent.BackColor = this.Enabled ? Color.White : ColorTranslator.FromHtml("#E5E5E5");
            }
            
            // 绘制背景
            Color backgroundColor = this.Enabled ? Color.White : ColorTranslator.FromHtml("#E5E5E5");
            FillRadius(this.ClientRectangle, graphics, backgroundColor, true, true, true, true, cornerRadius);

            // 绘制边框
            Color currentBorderColor = isFocused ? borderFocusedColor : borderColor;
            DrawRadius(this.ClientRectangle, graphics, currentBorderColor, true, true, true, true, borderWidth, cornerRadius);
        }
        #endregion

        private void DrawRadius(Rectangle rectangle, Graphics graphics, Color color, bool leftTop = true, bool rightTop = true, bool rightBottom = true, bool leftBottom = true, int penWidth = 1, int radius = 5)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath graphicsPath = new GraphicsPath();
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width - 1;
            int height = rectangle.Height - 2;

            int diameter = radius * 2;

            if (leftTop)
            {
                //左上圆角
                graphicsPath.AddArc(x, y, diameter, diameter, 180, 90);
                if (rightTop)
                {
                    //上边
                    graphicsPath.AddLine(x + radius, y, x + width - radius, y);
                }
                else
                {
                    //上边
                    graphicsPath.AddLine(x + radius, y, x + width, y);
                }
            }
            else
            {
                if (rightTop)
                {
                    //上边
                    graphicsPath.AddLine(x, y, x + width - radius, y);
                }
                else
                {
                    //上边
                    graphicsPath.AddLine(x, y, x, y);
                }
            }


            if (rightTop)
            {
                //右上圆角
                graphicsPath.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
                if (rightBottom)
                {
                    //右边
                    graphicsPath.AddLine(x + width, y + radius, x + width, y + height - radius);
                }
                else
                {
                    //右边
                    graphicsPath.AddLine(x + width, y + radius, x + width, y + height);
                }
            }
            else
            {
                if (rightBottom)
                {
                    //右边
                    graphicsPath.AddLine(x + width, y, x + width, y + height - radius);
                }
                else
                {
                    //右边
                    graphicsPath.AddLine(x + width, y, x + width, y + height);
                }
            }

            if (rightBottom)
            {
                //右下圆角
                graphicsPath.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
                if (leftBottom)
                {
                    //下边
                    graphicsPath.AddLine(x + width - radius, y + height, x + radius, y + height);
                }
                else
                {
                    //下边
                    graphicsPath.AddLine(x + width - radius, y + height, x, y + height);
                }
            }
            else
            {
                if (leftBottom)
                {
                    //下边
                    graphicsPath.AddLine(x + width, y + height, x + radius, y + height);
                }
                else
                {
                    //下边
                    graphicsPath.AddLine(x + width, y + height, x, y + height);
                }

            }

            if (leftBottom)
            {
                //左下圆角
                graphicsPath.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
                if (leftTop)
                {
                    //左边
                    graphicsPath.AddLine(x, y + height - radius, x, y + radius);
                }
                else
                {
                    //左边
                    graphicsPath.AddLine(x, y + height - radius, x, y);
                }
            }
            else
            {
                if (leftTop)
                {
                    //左边
                    graphicsPath.AddLine(x, y + height, x, y + radius);
                }
                else
                {
                    //左边
                    graphicsPath.AddLine(x, y + height, x, y);
                }
            }
            graphics.SmoothingMode = SmoothingMode.AntiAlias;  //使绘图质量最高，即消除锯齿
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.DrawPath(new Pen(color, penWidth), graphicsPath);
            graphics.SmoothingMode = SmoothingMode.Default;
            graphics.InterpolationMode = InterpolationMode.Default;
            graphics.CompositingQuality = CompositingQuality.Default;
        }

        private void FillRadius(Rectangle rectangle, Graphics graphics, Color color, bool leftTop = true, bool rightTop = true, bool rightBottom = true, bool leftBottom = true, int penWidth = 1, int radius = 5)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            GraphicsPath graphicsPath = new GraphicsPath();
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width - 1;
            int height = rectangle.Height - 2;

            int diameter = radius * 2;

            if (leftTop)
            {
                //左上圆角
                graphicsPath.AddArc(x, y, diameter, diameter, 180, 90);
                if (rightTop)
                {
                    //上边
                    graphicsPath.AddLine(x + radius, y, x + width - radius, y);
                }
                else
                {
                    //上边
                    graphicsPath.AddLine(x + radius, y, x + width, y);
                }
            }
            else
            {
                if (rightTop)
                {
                    //上边
                    graphicsPath.AddLine(x, y, x + width - radius, y);
                }
                else
                {
                    //上边
                    graphicsPath.AddLine(x, y, x, y);
                }
            }


            if (rightTop)
            {
                //右上圆角
                graphicsPath.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
                if (rightBottom)
                {
                    //右边
                    graphicsPath.AddLine(x + width, y + radius, x + width, y + height - radius);
                }
                else
                {
                    //右边
                    graphicsPath.AddLine(x + width, y + radius, x + width, y + height);
                }
            }
            else
            {
                if (rightBottom)
                {
                    //右边
                    graphicsPath.AddLine(x + width, y, x + width, y + height - radius);
                }
                else
                {
                    //右边
                    graphicsPath.AddLine(x + width, y, x + width, y + height);
                }
            }

            if (rightBottom)
            {
                //右下圆角
                graphicsPath.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
                if (leftBottom)
                {
                    //下边
                    graphicsPath.AddLine(x + width - radius, y + height, x + radius, y + height);
                }
                else
                {
                    //下边
                    graphicsPath.AddLine(x + width - radius, y + height, x, y + height);
                }
            }
            else
            {
                if (leftBottom)
                {
                    //下边
                    graphicsPath.AddLine(x + width, y + height, x + radius, y + height);
                }
                else
                {
                    //下边
                    graphicsPath.AddLine(x + width, y + height, x, y + height);
                }

            }

            if (leftBottom)
            {
                //左下圆角
                graphicsPath.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
                if (leftTop)
                {
                    //左边
                    graphicsPath.AddLine(x, y + height - radius, x, y + radius);
                }
                else
                {
                    //左边
                    graphicsPath.AddLine(x, y + height - radius, x, y);
                }
            }
            else
            {
                if (leftTop)
                {
                    //左边
                    graphicsPath.AddLine(x, y + height, x, y + radius);
                }
                else
                {
                    //左边
                    graphicsPath.AddLine(x, y + height, x, y);
                }
            }
            graphics.FillPath(new SolidBrush(color), graphicsPath);
        }



    }
}
