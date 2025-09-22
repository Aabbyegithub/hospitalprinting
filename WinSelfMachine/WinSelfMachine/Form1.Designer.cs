namespace WinSelfMachine
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.PanQueay = new System.Windows.Forms.Panel();
            this.roundedContainer1 = new WinSelfMachine.Controls.RoundedContainer();
            this.roundButton7 = new WinSelfMachine.Controls.RoundButton();
            this.roundButton2 = new WinSelfMachine.Controls.RoundButton();
            this.roundButton1 = new WinSelfMachine.Controls.RoundButton();
            this.carousel1 = new WinSelfMachine.Controls.Carousel();
            this.modeTimeBar1 = new WinSelfMachine.Controls.ModeTimeBar();
            this.arcContainer1 = new WinSelfMachine.Controls.ArcContainer();
            this.PanQueay.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanQueay
            // 
            this.PanQueay.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PanQueay.BackgroundImage")));
            this.PanQueay.Controls.Add(this.roundedContainer1);
            this.PanQueay.Location = new System.Drawing.Point(-3, 28);
            this.PanQueay.Name = "PanQueay";
            this.PanQueay.Size = new System.Drawing.Size(756, 1037);
            this.PanQueay.TabIndex = 10;
            // 
            // roundedContainer1
            // 
            this.roundedContainer1.BackColor = System.Drawing.Color.Transparent;
            this.roundedContainer1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundedContainer1.BorderThickness = 1;
            this.roundedContainer1.CornerRadius = 12;
            this.roundedContainer1.DividerColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))), ((int)(((byte)(220)))));
            this.roundedContainer1.DividerEnabled = true;
            this.roundedContainer1.DividerHorizontalPadding = 16;
            this.roundedContainer1.DividerThickness = 1;
            this.roundedContainer1.DividerTopSpacing = 8;
            this.roundedContainer1.FillColor = System.Drawing.Color.White;
            this.roundedContainer1.Location = new System.Drawing.Point(33, 18);
            this.roundedContainer1.Name = "roundedContainer1";
            this.roundedContainer1.ShadowOffsetX = 0;
            this.roundedContainer1.ShadowOffsetY = 2;
            this.roundedContainer1.ShadowSize = 6;
            this.roundedContainer1.ShowShadow = true;
            this.roundedContainer1.Size = new System.Drawing.Size(686, 243);
            this.roundedContainer1.TabIndex = 0;
            this.roundedContainer1.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer1.TitleFont = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer1.TitleText = "身份检索";
            this.roundedContainer1.TitleTopPadding = 10;
            // 
            // roundButton7
            // 
            this.roundButton7.BackColor = System.Drawing.Color.Transparent;
            this.roundButton7.BackFillColor = System.Drawing.Color.Aqua;
            this.roundButton7.CornerDiameter = 16;
            this.roundButton7.Icon = ((System.Drawing.Image)(resources.GetObject("roundButton7.Icon")));
            this.roundButton7.IconSize = new System.Drawing.Size(80, 80);
            this.roundButton7.IconTextSpacing = 5;
            this.roundButton7.LabelText = "设置";
            this.roundButton7.Location = new System.Drawing.Point(102, 673);
            this.roundButton7.Name = "roundButton7";
            this.roundButton7.Size = new System.Drawing.Size(232, 228);
            this.roundButton7.TabIndex = 9;
            this.roundButton7.TextColor = System.Drawing.Color.Black;
            // 
            // roundButton2
            // 
            this.roundButton2.BackColor = System.Drawing.Color.Transparent;
            this.roundButton2.BackFillColor = System.Drawing.Color.Orchid;
            this.roundButton2.CornerDiameter = 16;
            this.roundButton2.Icon = ((System.Drawing.Image)(resources.GetObject("roundButton2.Icon")));
            this.roundButton2.IconSize = new System.Drawing.Size(80, 80);
            this.roundButton2.IconTextSpacing = 5;
            this.roundButton2.LabelText = "报告打印";
            this.roundButton2.Location = new System.Drawing.Point(369, 449);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Size = new System.Drawing.Size(251, 452);
            this.roundButton2.TabIndex = 4;
            this.roundButton2.TextColor = System.Drawing.Color.Black;
            // 
            // roundButton1
            // 
            this.roundButton1.BackColor = System.Drawing.Color.Transparent;
            this.roundButton1.BackFillColor = System.Drawing.Color.LightGreen;
            this.roundButton1.CornerDiameter = 16;
            this.roundButton1.Icon = ((System.Drawing.Image)(resources.GetObject("roundButton1.Icon")));
            this.roundButton1.IconSize = new System.Drawing.Size(80, 80);
            this.roundButton1.IconTextSpacing = 5;
            this.roundButton1.LabelText = "报告查询";
            this.roundButton1.Location = new System.Drawing.Point(102, 449);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Size = new System.Drawing.Size(232, 198);
            this.roundButton1.TabIndex = 3;
            this.roundButton1.TextColor = System.Drawing.Color.Black;
            // 
            // carousel1
            // 
            this.carousel1.AutoPlayInterval = 3000;
            this.carousel1.BackColor = System.Drawing.Color.DarkCyan;
            this.carousel1.CornerRadius = 15;
            this.carousel1.CurrentIndex = 4;
            this.carousel1.EnableAutoPlay = true;
            this.carousel1.ItemHeight = 340;
            this.carousel1.ItemSpacing = 20;
            this.carousel1.ItemWidth = 630;
            this.carousel1.Location = new System.Drawing.Point(56, 28);
            this.carousel1.Name = "carousel1";
            this.carousel1.ShowIndicators = true;
            this.carousel1.Size = new System.Drawing.Size(631, 345);
            this.carousel1.TabIndex = 2;
            // 
            // modeTimeBar1
            // 
            this.modeTimeBar1.BackColor = System.Drawing.Color.DarkCyan;
            this.modeTimeBar1.CornerRadius = 12;
            this.modeTimeBar1.HospitalText = "苏州XX医院";
            this.modeTimeBar1.Icon = ((System.Drawing.Image)(resources.GetObject("modeTimeBar1.Icon")));
            this.modeTimeBar1.IsOn = true;
            this.modeTimeBar1.LeftButtonIcon = ((System.Drawing.Image)(resources.GetObject("modeTimeBar1.LeftButtonIcon")));
            this.modeTimeBar1.LeftButtonSize = 20;
            this.modeTimeBar1.LeftButtonVisible = true;
            this.modeTimeBar1.Location = new System.Drawing.Point(-3, -1);
            this.modeTimeBar1.ModeText = "关爱模式";
            this.modeTimeBar1.Name = "modeTimeBar1";
            this.modeTimeBar1.Size = new System.Drawing.Size(756, 32);
            this.modeTimeBar1.Spacing = 10;
            this.modeTimeBar1.SwitchBorderColor = System.Drawing.Color.White;
            this.modeTimeBar1.SwitchOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.modeTimeBar1.SwitchOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.modeTimeBar1.TabIndex = 1;
            this.modeTimeBar1.TimeText = "下午 17:00";
            this.modeTimeBar1.LeftButtonClicked += new System.EventHandler(this.modeTimeBar1_LeftButtonClicked);
            // 
            // arcContainer1
            // 
            this.arcContainer1.ArcHeight = 30;
            this.arcContainer1.BackColor = System.Drawing.Color.Transparent;
            this.arcContainer1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.arcContainer1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.arcContainer1.BorderWidth = 1;
            this.arcContainer1.EnableGradient = true;
            this.arcContainer1.GradientAngle = 90;
            this.arcContainer1.GradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.arcContainer1.GradientStartColor = System.Drawing.Color.DarkCyan;
            this.arcContainer1.Location = new System.Drawing.Point(-3, -1);
            this.arcContainer1.Name = "arcContainer1";
            this.arcContainer1.ShowBorder = true;
            this.arcContainer1.Size = new System.Drawing.Size(756, 325);
            this.arcContainer1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(752, 1061);
            this.Controls.Add(this.PanQueay);
            this.Controls.Add(this.roundButton7);
            this.Controls.Add(this.roundButton2);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.carousel1);
            this.Controls.Add(this.modeTimeBar1);
            this.Controls.Add(this.arcContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.PanQueay.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ArcContainer arcContainer1;
        private Controls.ModeTimeBar modeTimeBar1;
        private Controls.Carousel carousel1;
        private Controls.RoundButton roundButton1;
        private Controls.RoundButton roundButton2;
        private Controls.RoundButton roundButton7;
        private System.Windows.Forms.Panel PanQueay;
        private Controls.RoundedContainer roundedContainer1;
    }
}

