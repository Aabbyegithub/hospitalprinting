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
        private Color placeholderColor = Color.FromArgb(150, 150, 150);
        private bool isFocused = false;

        public RoundedTextBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Size = new Size(220, 36);

            innerTextBox = new TextBox();
            innerTextBox.BorderStyle = BorderStyle.None;
            innerTextBox.Location = new Point(innerPadding.Left, innerPadding.Top);
            innerTextBox.Width = this.Width - innerPadding.Horizontal;
            innerTextBox.Height = this.Height - innerPadding.Vertical;
            // 注意：WinForms TextBox 不支持透明背景，使用填充色
            innerTextBox.BackColor = fillColor;
            innerTextBox.TextChanged += (s, e) => Invalidate();
            innerTextBox.GotFocus += (s, e) => { isFocused = true; Invalidate(); };
            innerTextBox.LostFocus += (s, e) => { isFocused = false; Invalidate(); };
            this.Controls.Add(innerTextBox);
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
            innerTextBox.Location = new Point(innerPadding.Left, innerPadding.Top);
            innerTextBox.Width = Math.Max(10, this.Width - innerPadding.Horizontal);
            innerTextBox.Height = Math.Max(10, this.Height - innerPadding.Vertical);
        }

        private void UpdateRegion()
        {
            using (var path = CreateRoundedPath(new Rectangle(0, 0, this.Width - 1, this.Height - 1), cornerRadius))
            {
                this.Region = new Region(path);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            // Fill
            using (var fillPath = CreateRoundedPath(rect, cornerRadius))
            using (var fillBrush = new SolidBrush(fillColor))
            {
                g.FillPath(fillBrush, fillPath);
            }

            // Border
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

            // Placeholder
            if (!isFocused && string.IsNullOrEmpty(innerTextBox.Text) && !string.IsNullOrEmpty(placeholderText))
            {
                using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
                using (var brush = new SolidBrush(placeholderColor))
                {
                    var textRect = new Rectangle(innerPadding.Left, 0, this.Width - innerPadding.Horizontal, this.Height);
                    g.DrawString(placeholderText, this.Font, brush, textRect, sf);
                }
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            int d = Math.Max(0, radius * 2);
            GraphicsPath path = new GraphicsPath();
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
    }
}
