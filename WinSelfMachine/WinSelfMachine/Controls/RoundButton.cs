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
    public partial class RoundButton : UserControl
    {
        GraphicsPath graphicsPath = new GraphicsPath();
        int cornerDiameter = 16;
        // 图标相关属性
        private Image icon;
        public Image Icon
        {
            get { return icon; }
            set 
            { 
                icon = value; 
                Invalidate(); 
            }
        }
        private Size iconSize = new Size(24, 24);
        public Size IconSize
        {
            get { return iconSize; }
            set 
            { 
                iconSize = value; 
                Invalidate(); 
            }
        }
        private int iconTextSpacing = 5;
        public int IconTextSpacing
        {
            get { return iconTextSpacing; }
            set 
            { 
                iconTextSpacing = value; 
                Invalidate(); 
            }
        }
        public RoundButton()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
            this.Paint += RoundButton_Paint;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            penBrush = new SolidBrush(textColor);
        }
        public int CornerDiameter
        {
            get { return cornerDiameter; }
            set { cornerDiameter = value; Invalidate(); }
        }
        string labelText = "label1";
        public String LabelText
        {
            get
            {
                return labelText;
            }
            set
            {
                labelText = value;
                var size = GetSizeF(value);
                if (size.Width > this.Width)
                {
                    this.Width = size.Width;
                }
                if (size.Height > this.Height)
                {
                    this.Height = size.Height;
                }
                Invalidate();
            }
        }
        Color backFillColor = Color.AntiqueWhite;
        public Color BackFillColor
        {
            get
            {
                return backFillColor;
            }
            set
            {
                backFillColor = value;
                Invalidate();
            }
        }
        private Size GetSizeF(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this.MinimumSize;
            }
            using (var g = this.CreateGraphics())
            {
                var textSize = g.MeasureString(value, this.Font);
                // 有图标时，增加高度以容纳图标和间距
                float totalHeight = textSize.Height;
                if (Icon != null)
                {
                    totalHeight += IconSize.Height + iconTextSpacing;
                }
                return new Size((int)Math.Max(textSize.Width, Icon != null ? IconSize.Width : 0) + 10, (int)totalHeight + 10);
            }
        }
        /// <summary>
        /// 右上角弧线
        /// </summary>
        /// <returns></returns>
        private Rectangle GetCornerRect1()
        {
            return new Rectangle(Width - cornerDiameter - 1, 1, cornerDiameter, cornerDiameter);
        }
        /// <summary>
        /// 右边线段
        /// </summary>
        /// <returns></returns>
        private Tuple<Point, Point> GetLine1()
        {
            return new Tuple<Point, Point>(new Point(Width - 1, cornerDiameter / 2),
                new Point(Width - 1, Height - cornerDiameter / 2));
        }
        /// <summary>
        /// 右下角弧线
        /// </summary>
        /// <returns></returns>
        private Rectangle GetCornerRect2()
        {
            return new Rectangle(Width - cornerDiameter - 1, Height - cornerDiameter - 1, cornerDiameter, cornerDiameter);
        }
        /// <summary>
        /// 下边线段
        /// </summary>
        /// <returns></returns>
        private Tuple<Point, Point> GetLine2()
        {
            return new Tuple<Point, Point>(new Point(cornerDiameter / 2 + 1, Height - 1),
                new Point(Width - cornerDiameter / 2 - 1, Height - 1));
        }
        /// <summary>
        /// 左下角弧线
        /// </summary>
        /// <returns></returns>
        private Rectangle GetCornerRect3()
        {
            return new Rectangle(1, Height - cornerDiameter - 1, cornerDiameter, cornerDiameter);
        }
        /// <summary>
        /// 左边线段
        /// </summary>
        /// <returns></returns>
        private Tuple<Point, Point> GetLine3()
        {
            return new Tuple<Point, Point>(new Point(1, Height - cornerDiameter / 2 - 1),
                new Point(1, cornerDiameter / 2 + 1));
        }
        /// <summary>
        /// 左上角弧线
        /// </summary>
        /// <returns></returns>
        private Rectangle GetCornerRect4()
        {
            return new Rectangle(1, 1, cornerDiameter, cornerDiameter);
        }
        /// <summary>
        /// 上边线段
        /// </summary>
        /// <returns></returns>
        private Tuple<Point, Point> GetLine4()
        {
            return new Tuple<Point, Point>(
                new Point(cornerDiameter / 2 + 1, 1), new Point(Width - cornerDiameter / 2 - 1, 1));
        }
        StringFormat stringFormat = new StringFormat();
        Color textColor = Color.Black;
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
                penBrush.Color = textColor;
                Invalidate();
            }
        }
        SolidBrush penBrush;
        private void RoundButton_Paint(object sender, PaintEventArgs e)
        {
            using (var g = e.Graphics)
            {
                g.SmoothingMode = SmoothingMode.HighQuality;  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;

                graphicsPath.Reset();

                graphicsPath.AddArc(GetCornerRect1(), 270, 90);
                var points = GetLine1();
                graphicsPath.AddLine(points.Item1, points.Item2);

                graphicsPath.AddArc(GetCornerRect2(), 0, 90);
                points = GetLine2();
                graphicsPath.AddLine(points.Item1, points.Item2);

                graphicsPath.AddArc(GetCornerRect3(), 90, 90);
                points = GetLine3();
                graphicsPath.AddLine(points.Item1, points.Item2);

                graphicsPath.AddArc(GetCornerRect4(), 180, 90);
                points = GetLine4();
                graphicsPath.AddLine(points.Item1, points.Item2);
                g.FillPath(new SolidBrush(backFillColor), graphicsPath);
                // 绘制图标（在上）
                if (Icon != null)
                {
                    int iconX = (this.Width - IconSize.Width) / 2;
                    int iconY = (int)(this.Height - IconSize.Height - g.MeasureString(labelText, this.Font).Height - iconTextSpacing) / 2;
                    g.DrawImage(Icon, iconX, iconY, IconSize.Width, IconSize.Height);
                }
                // 绘制文本（在下）
                RectangleF textRect;
                if (Icon != null)
                {
                    float textY = (this.Height - g.MeasureString(labelText, this.Font).Height) / 2;
                    if (textY < IconSize.Height + iconTextSpacing)
                    {
                        textY = IconSize.Height + iconTextSpacing;
                    }
                    textRect = new RectangleF(0, textY, this.Width, this.Height - textY);
                }
                else
                {
                    textRect = new RectangleF(0, 0, Width, Height);
                }
                g.DrawString(labelText, this.Font, penBrush, textRect, stringFormat);
            }
        }
    }
}