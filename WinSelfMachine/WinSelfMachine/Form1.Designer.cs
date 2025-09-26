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
            this.TxtQuery = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TxtBnr = new WinSelfMachine.Controls.RoundedTextBox();
            this.BtnReportQuery = new WinSelfMachine.Controls.RoundedButton();
            this._3DButton3 = new WinSelfMachine.Controls._3DButton();
            this._3DButton2 = new WinSelfMachine.Controls._3DButton();
            this._3DButton1 = new WinSelfMachine.Controls._3DButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtQuery
            // 
            this.TxtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TxtQuery.AutoSize = true;
            this.TxtQuery.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtQuery.Location = new System.Drawing.Point(240, 383);
            this.TxtQuery.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.TxtQuery.Name = "TxtQuery";
            this.TxtQuery.Size = new System.Drawing.Size(87, 35);
            this.TxtQuery.TabIndex = 3;
            this.TxtQuery.Text = "查询";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("黑体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(0, 203);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1595, 84);
            this.label2.TabIndex = 4;
            this.label2.Text = "在线报告查询";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtTitle
            // 
            this.TxtTitle.AutoSize = true;
            this.TxtTitle.BackColor = System.Drawing.Color.Transparent;
            this.TxtTitle.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtTitle.Location = new System.Drawing.Point(132, 52);
            this.TxtTitle.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.TxtTitle.Name = "TxtTitle";
            this.TxtTitle.Size = new System.Drawing.Size(483, 51);
            this.TxtTitle.TabIndex = 7;
            this.TxtTitle.Text = "****医院自助一体机";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(22, 44);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(66, 66);
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // TxtBnr
            // 
            this.TxtBnr.BackColor = System.Drawing.Color.Transparent;
            this.TxtBnr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TxtBnr.BorderFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.TxtBnr.BorderThickness = 1;
            this.TxtBnr.CornerRadius = 12;
            this.TxtBnr.FillColor = System.Drawing.Color.White;
            this.TxtBnr.FontSize = 9;
            this.TxtBnr.InnerPadding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.TxtBnr.Location = new System.Drawing.Point(348, 369);
            this.TxtBnr.Name = "TxtBnr";
            this.TxtBnr.PlaceholderColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.TxtBnr.PlaceholderText = "请输入检验号";
            this.TxtBnr.Size = new System.Drawing.Size(761, 63);
            this.TxtBnr.TabIndex = 9;
            this.TxtBnr.TextFont = new System.Drawing.Font("微软雅黑", 9F);
            // 
            // BtnReportQuery
            // 
            this.BtnReportQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnReportQuery.BorderColor = System.Drawing.Color.Transparent;
            this.BtnReportQuery.BorderThickness = 0;
            this.BtnReportQuery.ButtonText = "报告查询";
            this.BtnReportQuery.CornerRadius = 12;
            this.BtnReportQuery.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnReportQuery.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.BtnReportQuery.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.BtnReportQuery.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.BtnReportQuery.Location = new System.Drawing.Point(1142, 369);
            this.BtnReportQuery.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnReportQuery.Name = "BtnReportQuery";
            this.BtnReportQuery.Size = new System.Drawing.Size(187, 63);
            this.BtnReportQuery.TabIndex = 5;
            this.BtnReportQuery.TextColor = System.Drawing.Color.White;
            // 
            // _3DButton3
            // 
            this._3DButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._3DButton3.BackColor = System.Drawing.Color.Transparent;
            this._3DButton3.BottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this._3DButton3.ButtonIcon = ((System.Drawing.Image)(resources.GetObject("_3DButton3.ButtonIcon")));
            this._3DButton3.ButtonText = "设置";
            this._3DButton3.CornerRadius = 40;
            this._3DButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this._3DButton3.FontSize = 10F;
            this._3DButton3.IconSize = new System.Drawing.Size(80, 80);
            this._3DButton3.IconTextSpacing = 8;
            this._3DButton3.Location = new System.Drawing.Point(1071, 523);
            this._3DButton3.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._3DButton3.Name = "_3DButton3";
            this._3DButton3.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton3.ShadowOffset = 4;
            this._3DButton3.Size = new System.Drawing.Size(297, 285);
            this._3DButton3.TabIndex = 2;
            this._3DButton3.TextColor = System.Drawing.Color.Black;
            this._3DButton3.TopColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            // 
            // _3DButton2
            // 
            this._3DButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._3DButton2.BackColor = System.Drawing.Color.Transparent;
            this._3DButton2.BottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this._3DButton2.ButtonIcon = ((System.Drawing.Image)(resources.GetObject("_3DButton2.ButtonIcon")));
            this._3DButton2.ButtonText = "报告打印";
            this._3DButton2.CornerRadius = 40;
            this._3DButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this._3DButton2.FontSize = 10F;
            this._3DButton2.IconSize = new System.Drawing.Size(80, 80);
            this._3DButton2.IconTextSpacing = 8;
            this._3DButton2.Location = new System.Drawing.Point(642, 523);
            this._3DButton2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._3DButton2.Name = "_3DButton2";
            this._3DButton2.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton2.ShadowOffset = 4;
            this._3DButton2.Size = new System.Drawing.Size(297, 285);
            this._3DButton2.TabIndex = 1;
            this._3DButton2.TextColor = System.Drawing.Color.Black;
            this._3DButton2.TopColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            // 
            // _3DButton1
            // 
            this._3DButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._3DButton1.BackColor = System.Drawing.Color.Transparent;
            this._3DButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this._3DButton1.BottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this._3DButton1.ButtonIcon = ((System.Drawing.Image)(resources.GetObject("_3DButton1.ButtonIcon")));
            this._3DButton1.ButtonText = "报告查询";
            this._3DButton1.CornerRadius = 40;
            this._3DButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this._3DButton1.FontSize = 10F;
            this._3DButton1.IconSize = new System.Drawing.Size(80, 80);
            this._3DButton1.IconTextSpacing = 8;
            this._3DButton1.Location = new System.Drawing.Point(213, 523);
            this._3DButton1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this._3DButton1.Name = "_3DButton1";
            this._3DButton1.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton1.ShadowOffset = 4;
            this._3DButton1.Size = new System.Drawing.Size(297, 285);
            this._3DButton1.TabIndex = 0;
            this._3DButton1.TextColor = System.Drawing.Color.Black;
            this._3DButton1.TopColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1595, 1017);
            this.Controls.Add(this.TxtBnr);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TxtTitle);
            this.Controls.Add(this.BtnReportQuery);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtQuery);
            this.Controls.Add(this._3DButton3);
            this.Controls.Add(this._3DButton2);
            this.Controls.Add(this._3DButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls._3DButton _3DButton1;
        private Controls._3DButton _3DButton2;
        private Controls._3DButton _3DButton3;
        private System.Windows.Forms.Label TxtQuery;
        private System.Windows.Forms.Label label2;
        private Controls.RoundedButton BtnReportQuery;
        private System.Windows.Forms.Label TxtTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Controls.RoundedTextBox TxtBnr;
    }
}

