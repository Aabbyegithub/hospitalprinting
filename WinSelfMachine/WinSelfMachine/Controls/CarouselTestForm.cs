using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinSelfMachine.Controls
{
    /// <summary>
    /// 轮播图测试窗体
    /// </summary>
    public partial class CarouselTestForm : Form
    {
        private Carousel carousel;
        private Button addImageButton;
        private Button clearButton;

        public CarouselTestForm()
        {
            InitializeComponent();
            SetupControls();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // 设置窗体属性
            this.Text = "轮播图测试";
            this.Size = new Size(800, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            
            this.ResumeLayout(false);
        }

        private void SetupControls()
        {
            try
            {
                // 创建轮播图
                carousel = new Carousel();
                carousel.Dock = DockStyle.Fill;
                carousel.Margin = new Padding(20);
                
                // 设置轮播图属性
                carousel.ItemWidth = 300;
                carousel.ItemHeight = 200;
                carousel.ItemSpacing = 20;
                carousel.CornerRadius = 15;
                carousel.EnableAutoPlay = true;
                carousel.AutoPlayInterval = 3000;
                carousel.ShowIndicators = true;
                
                // 启用渐变背景
                carousel.EnableGradientBackground = true;
                carousel.GradientStartColor = Color.FromArgb(41, 123, 244);
                carousel.GradientEndColor = Color.White;
                carousel.GradientAngle = 90;
                
                // 创建控制按钮
                addImageButton = new Button();
                addImageButton.Text = "添加图片";
                addImageButton.Size = new Size(100, 30);
                addImageButton.Location = new Point(20, 20);
                addImageButton.Click += AddImageButton_Click;
                
                clearButton = new Button();
                clearButton.Text = "清空图片";
                clearButton.Size = new Size(100, 30);
                clearButton.Location = new Point(130, 20);
                clearButton.Click += ClearButton_Click;
                
                // 添加控件到窗体
                this.Controls.Add(carousel);
                this.Controls.Add(addImageButton);
                this.Controls.Add(clearButton);
                
                // 订阅轮播图事件
                carousel.ItemClick += Carousel_ItemClick;
                
                // 添加一些测试数据
                AddTestData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建控件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddTestData()
        {
            try
            {
                // 添加测试项目
                carousel.AddItem("挂号缴费不排队", "提前预约 自助缴费", "扫描二维码即可使用", null);
                carousel.AddItem("智能导诊服务", "AI智能问诊 精准分科", "让就医更便捷高效", null);
                carousel.AddItem("在线报告查询", "检查结果实时推送", "随时随地查看报告", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加测试数据时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddImageButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (carousel.ShowImageSelectionDialog())
                {
                    MessageBox.Show("图片添加成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加图片时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                carousel.ClearItems();
                MessageBox.Show("图片已清空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清空图片时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Carousel_ItemClick(object sender, CarouselItemClickEventArgs e)
        {
            try
            {
                MessageBox.Show($"您点击了: {e.Item.Title ?? "未命名图片"}", "轮播图点击", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理点击事件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"窗体加载时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}






