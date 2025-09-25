using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HospitalSelfService.Controls
{
    public class HospitalButton : Control
    {
        // 按钮状态枚举
        private enum ButtonState
        {
            Normal,
            Hover,
            Pressed,
            Disabled
        }

        // 私有字段
        private ButtonState _currentState = ButtonState.Normal;
        private Image _buttonImage;
        private int _imageSize = 24;
        private int _cornerRadius = 8;
        private bool _showGlowEffect = true;
        private Color _baseColor = Color.FromArgb(65, 115, 255); // 默认蓝色
        private int _shadowDepth = 3; // 阴影深度

        // 公共属性
        [Category("外观")]
        [Description("按钮上显示的图片")]
        public Image ButtonImage
        {
            get => _buttonImage;
            set
            {
                _buttonImage = value;
                Invalidate();
            }
        }

        [Category("外观")]
        [Description("图片尺寸")]
        public int ImageSize
        {
            get => _imageSize;
            set
            {
                if (value > 0) // 确保尺寸为正数
                {
                    _imageSize = value;
                    Invalidate();
                }
            }
        }

        [Category("外观")]
        [Description("圆角半径")]
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value); // 确保半径不为负
                Invalidate();
            }
        }

        [Category("外观")]
        [Description("是否显示发光效果")]
        public bool ShowGlowEffect
        {
            get => _showGlowEffect;
            set
            {
                _showGlowEffect = value;
                Invalidate();
            }
        }

        [Category("外观")]
        [Description("按钮基准颜色")]
        public Color BaseColor
        {
            get => _baseColor;
            set
            {
                _baseColor = value;
                Invalidate();
            }
        }

        [Category("外观")]
        [Description("阴影深度")]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set
            {
                _shadowDepth = Math.Max(0, Math.Min(10, value)); // 限制在0-10之间
                Invalidate();
            }
        }

        // 构造函数
        public HospitalButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            Size = new Size(100, 100); // 增大默认尺寸以适应图片在上的布局
            Font = new Font("微软雅黑", 10f);
            ForeColor = Color.White;
        }

        // 创建圆角矩形路径
        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            // 确保矩形有效
            if (rect.Width <= 0 || rect.Height <= 0)
                return new GraphicsPath();
                
            // 确保半径有效
            radius = Math.Max(0, radius);
            radius = Math.Min(radius, rect.Width / 2);
            radius = Math.Min(radius, rect.Height / 2);

            GraphicsPath path = new GraphicsPath();
            
            // 只有当半径大于0时才绘制圆角，否则绘制普通矩形
            if (radius > 0)
            {
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            }
            else
            {
                path.AddRectangle(rect);
            }
            
            path.CloseAllFigures();
            return path;
        }

        // 重绘控件
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // 防止Parent为null时出错
            if (Parent != null)
                g.Clear(Parent.BackColor);
            else
                g.Clear(Color.Transparent);

            // 确保控件尺寸有效
            if (Width <= 0 || Height <= 0)
                return;

            // 计算按钮区域（考虑阴影）
            int effectiveShadowDepth = _currentState == ButtonState.Disabled ? 0 : _shadowDepth;
            var buttonRect = new Rectangle(0, 0, Width, Height - effectiveShadowDepth);
            
            // 确保按钮区域有效
            if (buttonRect.Width <= 0 || buttonRect.Height <= 0)
                return;

            // 根据状态调整颜色
            Color backColor;
            Color borderColor;
            Color glowColor = Color.FromArgb(100, _baseColor);

            // 计算状态颜色（基于基准色调整）
            int brightnessOffset;
            switch (_currentState)
            {
                case ButtonState.Hover:
                    brightnessOffset = 20;
                    break;
                case ButtonState.Pressed:
                    brightnessOffset = -20;
                    break;
                case ButtonState.Disabled:
                    brightnessOffset = -60;
                    break;
                default:
                    brightnessOffset = 0;
                    break;
            }

            backColor = AdjustColorBrightness(_baseColor, brightnessOffset);
            borderColor = AdjustColorBrightness(_baseColor, brightnessOffset - 15);

            if (_currentState == ButtonState.Disabled)
            {
                backColor = Color.FromArgb(180, 190, 210);
                borderColor = Color.FromArgb(150, 160, 180);
            }

            // 绘制底部阴影
            if (effectiveShadowDepth > 0)
            {
                for (int i = 1; i <= effectiveShadowDepth; i++)
                {
                    var shadowRect = new Rectangle(
                        i, 
                        buttonRect.Bottom + (i - 1), 
                        Width - 2 * i, 
                        effectiveShadowDepth - i + 1);
                    
                    // 确保阴影矩形有效
                    if (shadowRect.Width > 0 && shadowRect.Height > 0)
                    {
                        using (GraphicsPath shadowPath = CreateRoundedRectangle(shadowRect, Math.Max(0, _cornerRadius - 2)))
                        {
                            int alpha = (int)(50 - (i * 15)); // 阴影透明度渐变
                            alpha = Math.Max(0, alpha); // 确保透明度不为负
                            
                            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                            {
                                g.FillPath(shadowBrush, shadowPath);
                            }
                        }
                    }
                }
            }

            // 绘制发光效果
            if (_showGlowEffect && _currentState != ButtonState.Disabled)
            {
                var glowRect = new Rectangle(1, 1, buttonRect.Width - 2, buttonRect.Height - 2);
                if (glowRect.Width > 0 && glowRect.Height > 0)
                {
                    using (Pen glowPen = new Pen(glowColor, 3))
                    {
                        glowPen.LineJoin = LineJoin.Round; // 使发光效果边角更平滑
                        using (GraphicsPath glowPath = CreateRoundedRectangle(glowRect, _cornerRadius + 2))
                        {
                            g.DrawPath(glowPen, glowPath);
                        }
                    }
                }
            }

            // 绘制按钮背景
            using (GraphicsPath buttonPath = CreateRoundedRectangle(buttonRect, _cornerRadius))
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    g.FillPath(brush, buttonPath);
                }

                using (Pen pen = new Pen(borderColor, 1))
                {
                    pen.LineJoin = LineJoin.Round; // 使边框边角更平滑
                    g.DrawPath(pen, buttonPath);
                }
            }

            // 计算文本和图片位置（图片在文字上方）
            Size textSize = TextRenderer.MeasureText(Text, Font);
            int contentWidth = Math.Max(textSize.Width, _imageSize);
            int contentX = (Width - contentWidth) / 2;
            int totalContentHeight = (_buttonImage != null ? _imageSize + 8 : 0) + textSize.Height;
            int contentY = (buttonRect.Height - totalContentHeight) / 2;

            // 确保内容区域在按钮范围内
            contentY = Math.Max(0, contentY);

            // 绘制图片
            if (_buttonImage != null && _currentState != ButtonState.Disabled)
            {
                int imageY = contentY;
                // 绘制图片前检查图片是否有效
                if (_buttonImage != null)
                {
                    g.DrawImage(_buttonImage, 
                               contentX, 
                               imageY, 
                               _imageSize, 
                               _imageSize);
                }
                contentY += _imageSize + 8; // 图片和文字间距
            }

            // 绘制文本
            Color textColor = _currentState == ButtonState.Disabled ? Color.FromArgb(120, 120, 120) : ForeColor;
            TextRenderer.DrawText(g, 
                                 Text, 
                                 Font, 
                                 new Point(contentX, contentY), 
                                 textColor,
                                 TextFormatFlags.HorizontalCenter);
        }

        // 调整颜色亮度的辅助方法
        private Color AdjustColorBrightness(Color color, int offset)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, Math.Min(255, color.R + offset)),
                Math.Max(0, Math.Min(255, color.G + offset)),
                Math.Max(0, Math.Min(255, color.B + offset))
            );
        }

        // 处理尺寸变化，确保控件不会过小
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // 确保最小尺寸，避免绘图错误
            if (Width < 40) Width = 40;
            if (Height < 40) Height = 40;
        }

        // 状态变更事件
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (Enabled)
                _currentState = ButtonState.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (Enabled)
                _currentState = ButtonState.Normal;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Enabled && e.Button == MouseButtons.Left)
                _currentState = ButtonState.Pressed;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (Enabled)
                _currentState = ClientRectangle.Contains(e.Location) ? ButtonState.Hover : ButtonState.Normal;
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            _currentState = Enabled ? ButtonState.Normal : ButtonState.Disabled;
            Invalidate();
        }

        // 处理点击事件
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            // 可以在这里添加自定义点击逻辑
        }
    }
}
    