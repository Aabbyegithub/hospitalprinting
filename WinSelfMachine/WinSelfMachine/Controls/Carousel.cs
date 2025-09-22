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
    public partial class Carousel : UserControl
    {
        private Timer autoPlayTimer;
        private int currentIndex = 0;
        private List<CarouselItem> items = new List<CarouselItem>();
        private bool isMouseOver = false;
        private int hoveredItemIndex = -1;

        // 轮播图相关参数
        private int itemSpacing = 20; // 项目间距
        private int itemWidth = 300; // 项目宽度
        private int itemHeight = 200; // 项目高度
        private int cornerRadius = 15; // 圆角半径
        private int autoPlayInterval = 3000; // 自动播放间隔（毫秒）
        private bool enableAutoPlay = true; // 是否启用自动播放
        private bool showIndicators = true; // 是否显示指示器
        private int indicatorSize = 8; // 指示器大小
        private int indicatorSpacing = 10; // 指示器间距

        // 颜色相关
        private Color backgroundColor = Color.FromArgb(240, 248, 255);
        private Color itemBackgroundColor = Color.White;
        private Color itemBorderColor = Color.FromArgb(200, 200, 200);
        private Color indicatorColor = Color.FromArgb(180, 180, 180);
        private Color activeIndicatorColor = Color.FromArgb(41, 123, 244);
        private Color textColor = Color.FromArgb(51, 51, 51);
        private Color titleColor = Color.FromArgb(41, 123, 244);

        public Carousel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(800, 250);
            // 设置控件样式以支持透明
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles.Opaque, true);


            // 初始化自动播放定时器
            autoPlayTimer = new Timer();
            autoPlayTimer.Interval = autoPlayInterval;
            autoPlayTimer.Tick += AutoPlayTimer_Tick;

            // 添加示例数据
            InitializeSampleData();

            // 启动自动播放
            if (enableAutoPlay)
                autoPlayTimer.Start();
        }

        private void InitializeSampleData()
        {
            // 添加测试数据
            AddItem("挂号缴费不排队", "提前预约 自助缴费", "扫描二维码即可使用", null);
            AddItem("智能导诊服务", "AI智能问诊 精准分科", "让就医更便捷高效", null);
            AddItem("在线报告查询", "检查结果实时推送", "随时随地查看报告", null);
            AddItem("预约挂号", "在线预约 到院取号", "避免排队等待", null);
            AddItem("缴费服务", "多种支付方式", "安全便捷的缴费体验", null);
        }

        private void AutoPlayTimer_Tick(object sender, EventArgs e)
        {
            if (!isMouseOver && items.Count > 1)
            {
                NextItem();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isMouseOver = true;
            if (enableAutoPlay)
                autoPlayTimer.Stop();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isMouseOver = false;
            hoveredItemIndex = -1;
            if (enableAutoPlay)
                autoPlayTimer.Start();
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // 检测鼠标悬停的项目
            int newHoveredIndex = GetItemAtPosition(e.Location);
            if (newHoveredIndex != hoveredItemIndex)
            {
                hoveredItemIndex = newHoveredIndex;
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int clickedIndex = GetItemAtPosition(e.Location);
            if (clickedIndex >= 0 && clickedIndex < items.Count)
            {
                // 切换到点击的项目
                currentIndex = clickedIndex;
                Invalidate();

                // 触发点击事件
                OnItemClick(new CarouselItemClickEventArgs(items[clickedIndex], clickedIndex));
            }
        }

        private int GetItemAtPosition(Point position)
        {
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            for (int i = 0; i < items.Count; i++)
            {
                int itemX = centerX - itemWidth / 2 + (i - currentIndex) * (itemWidth + itemSpacing);
                int itemY = centerY - itemHeight / 2;
                Rectangle itemRect = new Rectangle(itemX, itemY, itemWidth, itemHeight);

                if (itemRect.Contains(position))
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetTotalWidth()
        {
            return items.Count * itemWidth + (items.Count - 1) * itemSpacing;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 绘制轮播图外层容器（与卡片样式一致）
            DrawCarouselContainer(g);

            if (items.Count == 0) return;

            // 计算中心位置
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;

            // 绘制轮播项目（一次只显示一张完整图片）
            for (int i = 0; i < items.Count; i++)
            {
                int itemX = centerX - itemWidth / 2 + (i - currentIndex) * (itemWidth + itemSpacing);
                int itemY = centerY - itemHeight / 2;

                // 只绘制可见区域的项目
                Rectangle itemRect = new Rectangle(itemX, itemY, itemWidth, itemHeight);
                Rectangle visibleRect = Rectangle.Intersect(itemRect, this.ClientRectangle);

                if (!visibleRect.IsEmpty)
                {
                    DrawCarouselItem(g, items[i], itemX, itemY, i == currentIndex, i == hoveredItemIndex);
                }
            }

            // 绘制指示器
            if (showIndicators && items.Count > 1)
            {
                DrawIndicators(g);
            }
        }

        private void DrawCarouselContainer(Graphics g)
        {
            // 隐藏外部容器，不绘制任何内容
            // 让卡片自己绘制背景，实现完全贴合效果
        }

        private void DrawCarouselItem(Graphics g, CarouselItem item, int x, int y, bool isActive, bool isHovered)
        {
            Rectangle itemRect = new Rectangle(x, y, itemWidth, itemHeight);

            // 绘制所有卡片的背景和边框
            using (GraphicsPath path = CreateRoundedRectanglePath(itemRect, cornerRadius))
            {
                // 绘制阴影效果
                if (isActive || isHovered)
                {
                    Rectangle shadowRect = new Rectangle(x + 2, y + 2, itemWidth, itemHeight);
                    using (GraphicsPath shadowPath = CreateRoundedRectanglePath(shadowRect, cornerRadius))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // 绘制项目背景
                Color bgColor = isHovered ? Color.FromArgb(250, 250, 250) : itemBackgroundColor;
                using (var bgBrush = new SolidBrush(bgColor))
                {
                    g.FillPath(bgBrush, path);
                }

                // 绘制边框
                using (var borderPen = new Pen(isActive ? activeIndicatorColor : itemBorderColor, isActive ? 2 : 1))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // 绘制内容
            DrawItemContent(g, item, itemRect, isActive);
        }

        private void DrawItemContent(Graphics g, CarouselItem item, Rectangle rect, bool isActive)
        {
            int padding = 15;
            Rectangle contentRect = new Rectangle(rect.X + padding, rect.Y + padding,
                                                rect.Width - padding * 2, rect.Height - padding * 2);

            // 如果有图片，优先显示图片
            if (item.Image != null)
            {
                // 计算图片显示区域（占据大部分空间）
                int imagePadding = 10;
                Rectangle imageRect = new Rectangle(
                    contentRect.X + imagePadding,
                    contentRect.Y + imagePadding,
                    contentRect.Width - imagePadding * 2,
                    contentRect.Height - 60 - imagePadding * 2
                );

                // 绘制图片（保持宽高比）
                DrawImageWithAspectRatio(g, item.Image, imageRect);

                // 在底部绘制标题（小字体）
                using (var titleFont = new Font("微软雅黑", 12, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    int titleY = imageRect.Bottom + 10;
                    var titleSize = g.MeasureString(item.Title, titleFont);
                    int titleX = contentRect.X + (contentRect.Width - (int)titleSize.Width) / 2;
                    g.DrawString(item.Title, titleFont, titleBrush, titleX, titleY);
                }
            }
            else
            {
                // 没有图片时，显示文字内容
                using (var titleFont = new Font("微软雅黑", 14, FontStyle.Bold))
                using (var titleBrush = new SolidBrush(titleColor))
                {
                    var titleSize = g.MeasureString(item.Title, titleFont);
                    int titleY = contentRect.Y + 20;
                    int titleX = contentRect.X + (contentRect.Width - (int)titleSize.Width) / 2;
                    g.DrawString(item.Title, titleFont, titleBrush, titleX, titleY);

                    // 绘制副标题
                    using (var subtitleFont = new Font("微软雅黑", 10, FontStyle.Regular))
                    using (var subtitleBrush = new SolidBrush(textColor))
                    {
                        int subtitleY = titleY + (int)titleSize.Height + 15;
                        var subtitleSize = g.MeasureString(item.Subtitle, subtitleFont);
                        int subtitleX = contentRect.X + (contentRect.Width - (int)subtitleSize.Width) / 2;
                        g.DrawString(item.Subtitle, subtitleFont, subtitleBrush, subtitleX, subtitleY);
                    }
                }
            }
        }

        private void DrawImageWithAspectRatio(Graphics g, Image image, Rectangle destRect)
        {
            if (image == null) return;

            // 计算保持宽高比的缩放
            float imageAspect = (float)image.Width / image.Height;
            float destAspect = (float)destRect.Width / destRect.Height;

            Rectangle drawRect;
            if (imageAspect > destAspect)
            {
                // 图片更宽，以宽度为准
                int drawHeight = (int)(destRect.Width / imageAspect);
                drawRect = new Rectangle(destRect.X, destRect.Y + (destRect.Height - drawHeight) / 2,
                                       destRect.Width, drawHeight);
            }
            else
            {
                // 图片更高，以高度为准
                int drawWidth = (int)(destRect.Height * imageAspect);
                drawRect = new Rectangle(destRect.X + (destRect.Width - drawWidth) / 2, destRect.Y,
                                       drawWidth, destRect.Height);
            }

            g.DrawImage(image, drawRect);
        }

        private void DrawIndicators(Graphics g)
        {
            int totalWidth = items.Count * indicatorSize + (items.Count - 1) * indicatorSpacing;
            int startX = (this.Width - totalWidth) / 2;
            int indicatorY = this.Height - 30;

            for (int i = 0; i < items.Count; i++)
            {
                int indicatorX = startX + i * (indicatorSize + indicatorSpacing);
                Color color = (i == currentIndex) ? activeIndicatorColor : indicatorColor;

                using (var brush = new SolidBrush(color))
                {
                    g.FillEllipse(brush, indicatorX, indicatorY, indicatorSize, indicatorSize);
                }
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int actualRadius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

            if (actualRadius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, actualRadius * 2, actualRadius * 2, 180, 90);
            path.AddArc(rect.Right - actualRadius * 2, rect.Y, actualRadius * 2, actualRadius * 2, 270, 90);
            path.AddArc(rect.Right - actualRadius * 2, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - actualRadius * 2, actualRadius * 2, actualRadius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        public void NextItem()
        {
            if (items.Count > 1)
            {
                currentIndex = (currentIndex + 1) % items.Count;
                Invalidate();
            }
        }

        public void PreviousItem()
        {
            if (items.Count > 1)
            {
                currentIndex = (currentIndex - 1 + items.Count) % items.Count;
                Invalidate();
            }
        }

        public void AddItem(string title, string subtitle, string description, Image image)
        {
            items.Add(new CarouselItem { Title = title, Subtitle = subtitle, Description = description, Image = image });
            Invalidate();
        }

        public void AddImageItem(Image image, string title = "")
        {
            items.Add(new CarouselItem { Title = title, Subtitle = "", Description = "", Image = image });
            Invalidate();
        }

        public void AddImageFromFile(string imagePath, string title = "")
        {
            try
            {
                Image image = Image.FromFile(imagePath);
                AddImageItem(image, title);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"无法加载图片文件: {imagePath}", ex);
            }
        }

        public void AddImagesFromFiles(string[] imagePaths, string[] titles = null)
        {
            for (int i = 0; i < imagePaths.Length; i++)
            {
                string title = (titles != null && i < titles.Length) ? titles[i] : "";
                AddImageFromFile(imagePaths[i], title);
            }
        }

        public bool ShowImageSelectionDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|所有文件|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "选择轮播图片";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        AddImagesFromFiles(openFileDialog.FileNames);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载图片时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return false;
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                items.RemoveAt(index);
                if (currentIndex >= items.Count)
                    currentIndex = Math.Max(0, items.Count - 1);
                Invalidate();
            }
        }

        public void ClearItems()
        {
            items.Clear();
            currentIndex = 0;
            Invalidate();
        }

        // 事件定义
        public event EventHandler<CarouselItemClickEventArgs> ItemClick;

        protected virtual void OnItemClick(CarouselItemClickEventArgs e)
        {
            ItemClick?.Invoke(this, e);
        }

        // 属性
        [Category("轮播图")]
        public bool EnableAutoPlay
        {
            get => enableAutoPlay;
            set
            {
                enableAutoPlay = value;
                if (enableAutoPlay)
                    autoPlayTimer.Start();
                else
                    autoPlayTimer.Stop();
            }
        }

        [Category("轮播图")]
        public int AutoPlayInterval
        {
            get => autoPlayInterval;
            set
            {
                autoPlayInterval = Math.Max(1000, value);
                autoPlayTimer.Interval = autoPlayInterval;
            }
        }

        [Category("轮播图")]
        public bool ShowIndicators
        {
            get => showIndicators;
            set
            {
                showIndicators = value;
                Invalidate();
            }
        }

        [Category("轮播图")]
        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (value >= 0 && value < items.Count)
                {
                    currentIndex = value;
                    Invalidate();
                }
            }
        }

        [Category("样式")]
        public int ItemWidth
        {
            get => itemWidth;
            set
            {
                itemWidth = Math.Max(100, value);
                Invalidate();
            }
        }

        [Category("样式")]
        public int ItemHeight
        {
            get => itemHeight;
            set
            {
                itemHeight = Math.Max(150, value);
                Invalidate();
            }
        }

        [Category("样式")]
        public int ItemSpacing
        {
            get => itemSpacing;
            set
            {
                itemSpacing = Math.Max(10, value);
                Invalidate();
            }
        }

        [Category("样式")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }
    }

    // 轮播图项目类
    public class CarouselItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public Image Image { get; set; }
        public object Tag { get; set; }
    }

    // 轮播图项目点击事件参数
    public class CarouselItemClickEventArgs : EventArgs
    {
        public CarouselItem Item { get; }
        public int Index { get; }

        public CarouselItemClickEventArgs(CarouselItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}
