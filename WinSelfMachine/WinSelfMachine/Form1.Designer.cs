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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TxtTitle = new System.Windows.Forms.Label();
            this.BtnPrintSetting = new WinSelfMachine.Controls.RoundButton();
            this.BtnWaitTime = new WinSelfMachine.Controls.RoundButton();
            this.BtnAvailableFilm = new WinSelfMachine.Controls.RoundButton();
            this.Txtbr = new WinSelfMachine.Controls.RoundTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnClose = new WinSelfMachine.Controls.RoundButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(12, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 38);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // TxtTitle
            // 
            this.TxtTitle.AutoSize = true;
            this.TxtTitle.BackColor = System.Drawing.Color.Transparent;
            this.TxtTitle.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtTitle.Location = new System.Drawing.Point(72, 30);
            this.TxtTitle.Name = "TxtTitle";
            this.TxtTitle.Size = new System.Drawing.Size(276, 29);
            this.TxtTitle.TabIndex = 7;
            this.TxtTitle.Text = "****医院自助一体机";
            // 
            // BtnPrintSetting
            // 
            this.BtnPrintSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnPrintSetting.BackColor = System.Drawing.Color.Transparent;
            this.BtnPrintSetting.BackFillColor = System.Drawing.Color.White;
            this.BtnPrintSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.BtnPrintSetting.BorderThickness = 2;
            this.BtnPrintSetting.CornerDiameter = 16;
            this.BtnPrintSetting.Icon = null;
            this.BtnPrintSetting.IconSize = new System.Drawing.Size(24, 24);
            this.BtnPrintSetting.IconTextSpacing = 5;
            this.BtnPrintSetting.LabelText = "打印设置";
            this.BtnPrintSetting.Location = new System.Drawing.Point(28, 545);
            this.BtnPrintSetting.Name = "BtnPrintSetting";
            this.BtnPrintSetting.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnPrintSetting.ShadowOffset = 3;
            this.BtnPrintSetting.ShowShadow = true;
            this.BtnPrintSetting.Size = new System.Drawing.Size(94, 82);
            this.BtnPrintSetting.TabIndex = 9;
            this.BtnPrintSetting.TextColor = System.Drawing.Color.Black;
            this.BtnPrintSetting.Visible = false;
            this.BtnPrintSetting.Click += new System.EventHandler(this.BtnPrintSetting_Click);
            // 
            // BtnWaitTime
            // 
            this.BtnWaitTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnWaitTime.BackColor = System.Drawing.Color.Transparent;
            this.BtnWaitTime.BackFillColor = System.Drawing.Color.White;
            this.BtnWaitTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.BtnWaitTime.BorderThickness = 2;
            this.BtnWaitTime.CornerDiameter = 16;
            this.BtnWaitTime.Icon = null;
            this.BtnWaitTime.IconSize = new System.Drawing.Size(24, 24);
            this.BtnWaitTime.IconTextSpacing = 5;
            this.BtnWaitTime.LabelText = "等待时间";
            this.BtnWaitTime.Location = new System.Drawing.Point(155, 545);
            this.BtnWaitTime.Name = "BtnWaitTime";
            this.BtnWaitTime.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnWaitTime.ShadowOffset = 3;
            this.BtnWaitTime.ShowShadow = true;
            this.BtnWaitTime.Size = new System.Drawing.Size(94, 82);
            this.BtnWaitTime.TabIndex = 10;
            this.BtnWaitTime.TextColor = System.Drawing.Color.Black;
            this.BtnWaitTime.Visible = false;
            this.BtnWaitTime.Click += new System.EventHandler(this.BtnWaitTime_Click);
            // 
            // BtnAvailableFilm
            // 
            this.BtnAvailableFilm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnAvailableFilm.BackColor = System.Drawing.Color.Transparent;
            this.BtnAvailableFilm.BackFillColor = System.Drawing.Color.White;
            this.BtnAvailableFilm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.BtnAvailableFilm.BorderThickness = 2;
            this.BtnAvailableFilm.CornerDiameter = 16;
            this.BtnAvailableFilm.Icon = null;
            this.BtnAvailableFilm.IconSize = new System.Drawing.Size(24, 24);
            this.BtnAvailableFilm.IconTextSpacing = 5;
            this.BtnAvailableFilm.LabelText = "可用胶片";
            this.BtnAvailableFilm.Location = new System.Drawing.Point(275, 545);
            this.BtnAvailableFilm.Name = "BtnAvailableFilm";
            this.BtnAvailableFilm.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnAvailableFilm.ShadowOffset = 3;
            this.BtnAvailableFilm.ShowShadow = true;
            this.BtnAvailableFilm.Size = new System.Drawing.Size(94, 82);
            this.BtnAvailableFilm.TabIndex = 11;
            this.BtnAvailableFilm.TextColor = System.Drawing.Color.Black;
            this.BtnAvailableFilm.Visible = false;
            this.BtnAvailableFilm.Click += new System.EventHandler(this.BtnAvailableFilm_Click);
            // 
            // Txtbr
            // 
            this.Txtbr.BackColor = System.Drawing.Color.Transparent;
            this.Txtbr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Txtbr.BorderFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.Txtbr.BorderWidth = 1;
            this.Txtbr.CornerRadius = 8;
            this.Txtbr.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Txtbr.FontSize = 15;
            this.Txtbr.Location = new System.Drawing.Point(277, 393);
            this.Txtbr.MaxLength = 32767;
            this.Txtbr.Name = "Txtbr";
            this.Txtbr.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.Txtbr.PasswordChar = '\0';
            this.Txtbr.ReadOnly = false;
            this.Txtbr.Size = new System.Drawing.Size(200, 30);
            this.Txtbr.TabIndex = 12;
            this.Txtbr.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.Txtbr.UseSystemPasswordChar = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(168, 398);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "检验单号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(149, 453);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(365, 35);
            this.label2.TabIndex = 14;
            this.label2.Text = "请在左下角刷条码取片";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.BackColor = System.Drawing.Color.Transparent;
            this.BtnClose.BackFillColor = System.Drawing.Color.White;
            this.BtnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.BtnClose.BorderThickness = 2;
            this.BtnClose.CornerDiameter = 16;
            this.BtnClose.Icon = null;
            this.BtnClose.IconSize = new System.Drawing.Size(24, 24);
            this.BtnClose.IconTextSpacing = 5;
            this.BtnClose.LabelText = "关闭";
            this.BtnClose.Location = new System.Drawing.Point(523, 545);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnClose.ShadowOffset = 3;
            this.BtnClose.ShowShadow = true;
            this.BtnClose.Size = new System.Drawing.Size(94, 82);
            this.BtnClose.TabIndex = 15;
            this.BtnClose.TextColor = System.Drawing.Color.Black;
            this.BtnClose.Visible = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(689, 674);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Txtbr);
            this.Controls.Add(this.BtnAvailableFilm);
            this.Controls.Add(this.BtnWaitTime);
            this.Controls.Add(this.BtnPrintSetting);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TxtTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label TxtTitle;
        private Controls.RoundButton BtnPrintSetting;
        private Controls.RoundButton BtnWaitTime;
        private Controls.RoundButton BtnAvailableFilm;
        private Controls.RoundTextBox Txtbr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Controls.RoundButton BtnClose;
    }
}

