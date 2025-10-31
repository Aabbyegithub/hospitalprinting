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
    public partial class RoundedButton : UserControl
    {
        private int cornerRadius = 12;
        private Color fillColor = Color.FromArgb(41, 123, 244);
        private Color fillHoverColor = Color.FromArgb(51, 133, 254);
        private Color fillDownColor = Color.FromArgb(31, 113, 234);
        private Color borderColor = Color.Transparent;
        private int borderThickness = 0;
        private Color textColor = Color.White;
        private string buttonText = "按钮";
        private bool isHovered = false;
        private bool isPressed = false;

        public RoundedButton()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.Cursor = Cursors.Hand;
            this.Size = new Size(100, 36);
        }

        [Category("外观")]
        public int CornerRadius
        {
            get => cornerRadius;
            set { cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        public Color FillColor
        {
            get => fillColor;
            set { fillColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color FillHoverColor
        {
            get => fillHoverColor;
            set { fillHoverColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color FillDownColor
        {
            get => fillDownColor;
            set { fillDownColor = value; Invalidate(); }
        }

        [Category("外观")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("外观")]
        public int BorderThickness
        {
            get => borderThickness;
            set { borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("外观")]
        public Color TextColor
        {
            get => textColor;
            set { textColor = value; Invalidate(); }
        }

        [Category("外观")]
        public string ButtonText
        {
            get => buttonText;
            set { buttonText = value ?? string.Empty; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            Color currentFill = isPressed ? fillDownColor : (isHovered ? fillHoverColor : fillColor);

            using (var path = CreateRoundedPath(rect, cornerRadius))
            using (var brush = new SolidBrush(currentFill))
            {
                g.FillPath(brush, path);
                if (borderThickness > 0)
                {
                    int inset = borderThickness / 2;
                    Rectangle borderRect = new Rectangle(inset, inset, this.Width - 1 - borderThickness, this.Height - 1 - borderThickness);
                    using (var bPath = CreateRoundedPath(borderRect, Math.Max(0, cornerRadius - inset)))
                    using (var pen = new Pen(borderColor, borderThickness))
                    {
                        g.DrawPath(pen, bPath);
                    }
                }
            }

            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var textBrush = new SolidBrush(textColor))
            {
                g.DrawString(buttonText, this.Font, textBrush, rect, sf);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            Invalidate();
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
            if (isPressed && e.Button == MouseButtons.Left)
            {
                isPressed = false;
                Invalidate();
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
