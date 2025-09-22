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
    public partial class RoundedContainer : UserControl
    {
        private int cornerRadius = 12;
        private Color borderColor = Color.FromArgb(200, 200, 200);
        private int borderThickness = 1;
        private Color fillColor = Color.White;
        private bool showShadow = true;
        private int shadowSize = 6;
        private int shadowOffsetX = 0;
        private int shadowOffsetY = 2;
		
		// 标题与分割线
		private string titleText = string.Empty;
		private Color titleColor = Color.FromArgb(40, 40, 40);
		private int titleTopPadding = 10;
		private Font titleFont = null; // 为空则使用控件自身 Font
		private bool dividerEnabled = false;
		private Color dividerColor = Color.FromArgb(120, 100, 140, 220);
		private int dividerThickness = 1;
		private int dividerTopSpacing = 8;
		private int dividerHorizontalPadding = 16;

        public RoundedContainer()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            UpdateRegion();
        }

		[Category("标题")]
		public string TitleText
		{
			get => titleText;
			set { titleText = value ?? string.Empty; Invalidate(); }
		}

		[Category("标题")]
		public Color TitleColor
		{
			get => titleColor;
			set { titleColor = value; Invalidate(); }
		}

		[Category("标题")]
		public int TitleTopPadding
		{
			get => titleTopPadding;
			set { titleTopPadding = Math.Max(0, value); Invalidate(); }
		}

		[Category("标题")]
		public Font TitleFont
		{
			get => titleFont ?? this.Font;
			set { titleFont = value; Invalidate(); }
		}

		[Category("分割线")]
		public bool DividerEnabled
		{
			get => dividerEnabled;
			set { dividerEnabled = value; Invalidate(); }
		}

		[Category("分割线")]
		public Color DividerColor
		{
			get => dividerColor;
			set { dividerColor = value; Invalidate(); }
		}

		[Category("分割线")]
		public int DividerThickness
		{
			get => dividerThickness;
			set { dividerThickness = Math.Max(1, value); Invalidate(); }
		}

		[Category("分割线")]
		public int DividerTopSpacing
		{
			get => dividerTopSpacing;
			set { dividerTopSpacing = Math.Max(0, value); Invalidate(); }
		}

		[Category("分割线")]
		public int DividerHorizontalPadding
		{
			get => dividerHorizontalPadding;
			set { dividerHorizontalPadding = Math.Max(0, value); Invalidate(); }
		}

        [Category("圆角容器")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Math.Max(0, value);
                UpdateRegion();
                Invalidate();
            }
        }

        [Category("圆角容器")]
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        [Category("圆角容器")]
        public int BorderThickness
        {
            get => borderThickness;
            set { borderThickness = Math.Max(0, value); Invalidate(); }
        }

        [Category("圆角容器")]
        public Color FillColor
        {
            get => fillColor;
            set { fillColor = value; Invalidate(); }
        }

        [Category("阴影")]
        public bool ShowShadow
        {
            get => showShadow;
            set { showShadow = value; Invalidate(); }
        }

        [Category("阴影")]
        public int ShadowSize
        {
            get => shadowSize;
            set { shadowSize = Math.Max(0, value); Invalidate(); }
        }

        [Category("阴影")]
        public int ShadowOffsetX
        {
            get => shadowOffsetX;
            set { shadowOffsetX = value; Invalidate(); }
        }

        [Category("阴影")]
        public int ShadowOffsetY
        {
            get => shadowOffsetY;
            set { shadowOffsetY = value; Invalidate(); }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
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

            // Shadow (simple spread without blur)
            if (showShadow && shadowSize > 0)
            {
                using (var shadowPath = CreateRoundedPath(OffsetRect(rect, shadowOffsetX, shadowOffsetY), cornerRadius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                {
                    // Draw multiple expanded paths to simulate a soft edge
                    for (int i = 0; i < shadowSize; i++)
                    {
                        using (var p = CreateRoundedPath(InflateRect(OffsetRect(rect, shadowOffsetX, shadowOffsetY), i, i), cornerRadius + i))
                        {
                            g.FillPath(shadowBrush, p);
                        }
                    }
                }
            }

			// Fill
            using (var fillPath = CreateRoundedPath(rect, cornerRadius))
            using (var fillBrush = new SolidBrush(fillColor))
            {
                g.FillPath(fillBrush, fillPath);
            }

			// Title
			float contentTop = 0f;
			if (!string.IsNullOrEmpty(titleText))
			{
				using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
				using (var brush = new SolidBrush(titleColor))
				{
					var usedFont = TitleFont;
					var textRect = new RectangleF(0, titleTopPadding, this.Width, usedFont.Height + 6);
					g.DrawString(titleText, usedFont, brush, textRect, sf);
					contentTop = textRect.Bottom;
				}
			}

			// Divider under title (center highlight, fade to transparent sides)
			if (dividerEnabled && this.Width > dividerHorizontalPadding * 2)
			{
				int y = (int)Math.Round((contentTop > 0 ? contentTop + dividerTopSpacing : titleTopPadding + (TitleFont).Height + dividerTopSpacing));
				int x1 = dividerHorizontalPadding;
				int x2 = this.Width - dividerHorizontalPadding;
				Rectangle lineRect = new Rectangle(x1, y, Math.Max(1, x2 - x1), Math.Max(2, dividerThickness));
				using (var lgb = new LinearGradientBrush(lineRect, Color.Transparent, Color.Transparent, LinearGradientMode.Horizontal))
				{
					var cb = new ColorBlend();
					// 更柔和的三段渐变：两端透明，中间高亮
					cb.Colors = new Color[] { Color.Transparent, dividerColor, Color.Transparent };
					cb.Positions = new float[] { 0f, 0.5f, 1f };
					lgb.InterpolationColors = cb;
					g.FillRectangle(lgb, lineRect);
				}
			}

            // Border
            if (borderThickness > 0)
            {
                int inset = borderThickness / 2;
                Rectangle borderRect = new Rectangle(inset, inset, this.Width - 1 - borderThickness, this.Height - 1 - borderThickness);
                using (var borderPath = CreateRoundedPath(borderRect, Math.Max(0, cornerRadius - inset)))
                using (var pen = new Pen(borderColor, borderThickness))
                {
                    g.DrawPath(pen, borderPath);
                }
            }
        }

        private static Rectangle OffsetRect(Rectangle r, int dx, int dy)
        {
            return new Rectangle(r.X + dx, r.Y + dy, r.Width, r.Height);
        }

        private static Rectangle InflateRect(Rectangle r, int dw, int dh)
        {
            return new Rectangle(r.X - dw, r.Y - dh, r.Width + dw * 2, r.Height + dh * 2);
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
