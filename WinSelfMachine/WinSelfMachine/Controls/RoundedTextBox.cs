using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WinSelfMachine.Controls
{
    public partial class RoundedTextBox : UserControl
    {
        private TextBox innerTextBox;
        private int cornerRadius = 12;
        private int borderThickness = 1;
        private Color borderColor = Color.FromArgb(200, 200, 200);
        private Color borderFocusedColor = Color.FromArgb(41, 123, 244);
        private Color fillColor = Color.White;
        private Padding innerPadding = new Padding(10, 6, 10, 6);
        private string placeholderText = "请输入";
        private Color placeholderColor = Color.FromArgb(120, 120, 120);
        private bool isFocused = false;
        private Font textFont = new Font("微软雅黑", 9);
        private int fontSize = 9;

        public RoundedTextBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Size = new Size(220, 36);

            // 添加Load事件处理程序，确保在运行时创建内部TextBox
            this.Load += RoundedTextBox_Load;
            
            // 设置默认属性
            this.CornerRadius = 12;
            this.BorderThickness = 1;
            this.BorderColor = Color.FromArgb(200, 200, 200);
            this.BorderFocusedColor = Color.FromArgb(41, 123, 244);
            this.FillColor = Color.White;
            this.PlaceholderText = "请输入";
            this.PlaceholderColor = Color.FromArgb(120, 120, 120);
        }

        private void RoundedTextBox_Load(object sender, EventArgs e)
        {
            if (innerTextBox == null)
            {
                innerTextBox = new TextBox();
                innerTextBox.BorderStyle = BorderStyle.None;
                innerTextBox.BackColor = fillColor;
                innerTextBox.Font = textFont;
                innerTextBox.ForeColor = Color.Black;
                innerTextBox.TextChanged += (s, ev) => Invalidate();
                innerTextBox.GotFocus += (s, ev) => { isFocused = true; Invalidate(); };
                innerTextBox.LostFocus += (s, ev) => { isFocused = false; Invalidate(); };
                innerTextBox.KeyDown += (s, ev) => Invalidate();
                innerTextBox.KeyUp += (s, ev) => Invalidate();
                this.Controls.Add(innerTextBox);
                LayoutInnerTextBox();
            }
        }

        [Category("外观")]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); UpdateRegion(); Invalidate(); }
        }

        [Category("外观")]
        public int BorderThickness
        {
            get => borderThickness;
            set { borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color BorderFocusedColor
        {
            get => borderFocusedColor;
            set { borderFocusedColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color FillColor
        {
            get => fillColor;
            set { fillColor = value; if (innerTextBox != null) innerTextBox.BackColor = value; Invalidate(); }
        }

        [Category("布局")]
        public Padding InnerPadding
        {
            get => innerPadding;
            set { innerPadding = value; LayoutInnerTextBox(); Invalidate(); }
        }

        [Category("行为")]
        public override string Text
        {
            get => innerTextBox?.Text ?? string.Empty;
            set { if (innerTextBox != null) innerTextBox.Text = value; Invalidate(); }
        }

        [Category("外观")]
        public string PlaceholderText
        {
            get => placeholderText;
            set { placeholderText = value ?? string.Empty; Invalidate(); }
        }

        [Category("外观")]
        public Color PlaceholderColor
        {
            get => placeholderColor;
            set { placeholderColor = value; Invalidate(); }
        }

        [Category("外观")]
        public int FontSize
        {
            get => fontSize;
            set 
            { 
                fontSize = Math.Max(6, Math.Min(72, value)); // 限制字体大小在6-72之间
                textFont?.Dispose();
                textFont = new Font("微软雅黑", fontSize);
                if (innerTextBox != null) innerTextBox.Font = textFont;
                System.Diagnostics.Debug.WriteLine($"RoundedTextBox FontSize 设置为: {fontSize}");
                Invalidate(); 
            }
        }

        /// <summary>
        /// 强制更新字体大小
        /// </summary>
        public void UpdateFontSize(float newSize)
        {
            int newFontSize = Math.Max(6, Math.Min(72, (int)Math.Round(newSize)));
            fontSize = newFontSize;
            textFont?.Dispose();
            textFont = new Font("微软雅黑", fontSize);
            if (innerTextBox != null) innerTextBox.Font = textFont;
            System.Diagnostics.Debug.WriteLine($"强制更新RoundedTextBox字体大小: {fontSize}");
            Invalidate();
            Refresh();
        }

        [Category("外观")]
        public Font TextFont
        {
            get => textFont;
            set 
            { 
                if (value != null)
                {
                    textFont?.Dispose();
                    textFont = value; 
                    fontSize = (int)value.Size;
                    if (innerTextBox != null) innerTextBox.Font = textFont;
                    Invalidate(); 
                }
            }
        }

        public TextBox InnerTextBox => innerTextBox;


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            LayoutInnerTextBox();
            UpdateRegion();
        }

        private void LayoutInnerTextBox()
        {
            if (innerTextBox == null) return;
            
            int x = innerPadding.Left;
            int y = innerPadding.Top;
            int width = Math.Max(10, this.Width - innerPadding.Horizontal);
            int height = Math.Max(10, this.Height - innerPadding.Vertical);
            
            // 确保TextBox不会超出控件边界
            if (x + width > this.Width) width = this.Width - x;
            if (y + height > this.Height) height = this.Height - y;
            
            innerTextBox.Location = new Point(x, y);
            innerTextBox.Size = new Size(width, height);
        }

        private void UpdateRegion()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                using (var path = CreateRoundedPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), cornerRadius))
                {
                    this.Region = new Region(path);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            // 创建圆角路径
            using (var fillPath = CreateRoundedPath(rect, cornerRadius))
            {
                // 设置裁剪区域
                g.SetClip(fillPath, System.Drawing.Drawing2D.CombineMode.Replace);

                // Fill background
                using (var fillBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(fillBrush, fillPath);
                }

                // Draw border
                if (borderThickness > 0)
                {
                    int inset = borderThickness / 2;
                    Rectangle borderRect = new Rectangle(inset, inset, this.Width - 1 - borderThickness, this.Height - 1 - borderThickness);
                    using (var borderPath = CreateRoundedPath(borderRect, Math.Max(0, cornerRadius - inset)))
                    using (var pen = new Pen(isFocused ? borderFocusedColor : borderColor, borderThickness))
                    {
                        g.DrawPath(pen, borderPath);
                    }
                }

                // Draw placeholder text
                if (!isFocused && string.IsNullOrEmpty(innerTextBox?.Text) && !string.IsNullOrEmpty(placeholderText))
                {
                    using (var sf = new StringFormat { 
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    })
                    using (var brush = new SolidBrush(placeholderColor))
                    {
                        var textRect = new Rectangle(innerPadding.Left, 0, this.Width - innerPadding.Horizontal, this.Height);
                        g.DrawString(placeholderText, textFont, brush, textRect, sf);
                    }
                }

                // Reset clipping region
                g.ResetClip();
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            // 确保圆角半径不超过控件尺寸的一半
            int actualRadius = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2);
            int diameter = actualRadius * 2;
            
            // 如果控件太小，使用矩形
            if (diameter >= bounds.Width || diameter >= bounds.Height)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            path.StartFigure();
            
            // 左上角
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            
            // 右上角
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            
            // 右下角
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            
            // 左下角
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            
            path.CloseFigure();
            return path;
        }
    }
}
