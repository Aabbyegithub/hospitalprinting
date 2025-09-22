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
    public partial class ArcContainer : UserControl
    {
        private int arcHeight = 30; // 圆弧高度
        private Color backgroundColor = Color.FromArgb(240, 248, 255); // 背景颜色
        private Color borderColor = Color.FromArgb(200, 200, 200); // 边框颜色
        private int borderWidth = 1; // 边框宽度
        private bool showBorder = true; // 是否显示边框
        private bool enableGradient = true; // 是否启用渐变效果
        private Color gradientStartColor = Color.FromArgb(240, 248, 255); // 渐变起始颜色
        private Color gradientEndColor = Color.FromArgb(255, 255, 255); // 渐变结束颜色
        private int gradientAngle = 90; // 渐变角度

        public ArcContainer()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(800, 300);
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 创建圆弧容器路径
            using (GraphicsPath path = CreateArcContainerPath())
            {
                // 绘制背景
                if (enableGradient)
                {
                    // 渐变背景
                    using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                        this.ClientRectangle, gradientStartColor, gradientEndColor, gradientAngle))
                    {
                        g.FillPath(gradientBrush, path);
                    }
                }
                else
                {
                    // 纯色背景
                    using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
                    {
                        g.FillPath(bgBrush, path);
                    }
                }

                // 绘制边框
                if (showBorder)
                {
                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawPath(borderPen, path);
                    }
                }
            }
        }

        private GraphicsPath CreateArcContainerPath()
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = this.ClientRectangle;

            // 确保圆弧高度不超过控件高度的一半
            int actualArcHeight = Math.Min(arcHeight, rect.Height / 2);

            // 开始绘制路径
            path.StartFigure();

            // 顶部直线
            path.AddLine(rect.Left, rect.Top + actualArcHeight, rect.Right, rect.Top + actualArcHeight);

            // 右上角圆弧
            path.AddArc(rect.Right - actualArcHeight * 2, rect.Top, actualArcHeight * 2, actualArcHeight * 2, 270, 90);

            // 右侧直线
            path.AddLine(rect.Right, rect.Top + actualArcHeight, rect.Right, rect.Bottom - actualArcHeight);

            // 右下角圆弧
            path.AddArc(rect.Right - actualArcHeight * 2, rect.Bottom - actualArcHeight * 2, actualArcHeight * 2, actualArcHeight * 2, 0, 90);

            // 底部圆弧（主要特征）
            int bottomArcWidth = rect.Width - actualArcHeight * 2;
            if (bottomArcWidth > 0)
            {
                // 底部中心圆弧
                Rectangle bottomArcRect = new Rectangle(
                    rect.Left + actualArcHeight, 
                    rect.Bottom - actualArcHeight * 2, 
                    bottomArcWidth, 
                    actualArcHeight * 2);
                path.AddArc(bottomArcRect, 0, 180);
            }

            // 左下角圆弧
            path.AddArc(rect.Left, rect.Bottom - actualArcHeight * 2, actualArcHeight * 2, actualArcHeight * 2, 90, 90);

            // 左侧直线
            path.AddLine(rect.Left, rect.Bottom - actualArcHeight, rect.Left, rect.Top + actualArcHeight);

            // 左上角圆弧
            path.AddArc(rect.Left, rect.Top, actualArcHeight * 2, actualArcHeight * 2, 180, 90);

            path.CloseFigure();
            return path;
        }

        // 属性
        [Category("圆弧样式")]
        public int ArcHeight
        {
            get => arcHeight;
            set
            {
                arcHeight = Math.Max(10, Math.Min(value, this.Height / 2));
                Invalidate();
            }
        }

        [Category("圆弧样式")]
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                Invalidate();
            }
        }

        [Category("圆弧样式")]
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        [Category("圆弧样式")]
        public int BorderWidth
        {
            get => borderWidth;
            set
            {
                borderWidth = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("圆弧样式")]
        public bool ShowBorder
        {
            get => showBorder;
            set
            {
                showBorder = value;
                Invalidate();
            }
        }

        [Category("渐变效果")]
        public bool EnableGradient
        {
            get => enableGradient;
            set
            {
                enableGradient = value;
                Invalidate();
            }
        }

        [Category("渐变效果")]
        public Color GradientStartColor
        {
            get => gradientStartColor;
            set
            {
                gradientStartColor = value;
                Invalidate();
            }
        }

        [Category("渐变效果")]
        public Color GradientEndColor
        {
            get => gradientEndColor;
            set
            {
                gradientEndColor = value;
                Invalidate();
            }
        }

        [Category("渐变效果")]
        public int GradientAngle
        {
            get => gradientAngle;
            set
            {
                gradientAngle = value;
                Invalidate();
            }
        }

        // 重写 Region 属性以支持不规则形状
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (this.IsHandleCreated)
            {
                using (GraphicsPath path = CreateArcContainerPath())
                {
                    this.Region = new Region(path);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }
    }
}
