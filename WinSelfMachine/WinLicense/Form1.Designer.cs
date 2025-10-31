namespace WinLicense
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
            this.LblMachineCode = new System.Windows.Forms.Label();
            this.TxtMachineCode = new System.Windows.Forms.TextBox();
            this.LblStartTime = new System.Windows.Forms.Label();
            this.DtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.LblEndTime = new System.Windows.Forms.Label();
            this.DtpEndTime = new System.Windows.Forms.DateTimePicker();
            this.LblLicenseType = new System.Windows.Forms.Label();
            this.CmbLicenseType = new System.Windows.Forms.ComboBox();
            this.LblExtraInfo = new System.Windows.Forms.Label();
            this.TxtExtraInfo = new System.Windows.Forms.TextBox();
            this.BtnGenerate = new System.Windows.Forms.Button();
            this.BtnValidate = new System.Windows.Forms.Button();
            this.BtnTrial7Days = new System.Windows.Forms.Button();
            this.BtnTrial30Days = new System.Windows.Forms.Button();
            this.BtnFullYear = new System.Windows.Forms.Button();
            this.LblPreview = new System.Windows.Forms.Label();
            this.TxtPreview = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LblMachineCode
            // 
            this.LblMachineCode.AutoSize = true;
            this.LblMachineCode.Location = new System.Drawing.Point(30, 30);
            this.LblMachineCode.Name = "LblMachineCode";
            this.LblMachineCode.Size = new System.Drawing.Size(94, 21);
            this.LblMachineCode.TabIndex = 0;
            this.LblMachineCode.Text = "机器码：";
            // 
            // TxtMachineCode
            // 
            this.TxtMachineCode.Font = new System.Drawing.Font("Consolas", 9F);
            this.TxtMachineCode.Location = new System.Drawing.Point(125, 27);
            this.TxtMachineCode.Name = "TxtMachineCode";
            this.TxtMachineCode.Size = new System.Drawing.Size(650, 32);
            this.TxtMachineCode.TabIndex = 1;
            // 
            // LblStartTime
            // 
            this.LblStartTime.AutoSize = true;
            this.LblStartTime.Location = new System.Drawing.Point(30, 80);
            this.LblStartTime.Name = "LblStartTime";
            this.LblStartTime.Size = new System.Drawing.Size(115, 21);
            this.LblStartTime.TabIndex = 2;
            this.LblStartTime.Text = "开始时间：";
            // 
            // DtpStartTime
            // 
            this.DtpStartTime.Location = new System.Drawing.Point(146, 77);
            this.DtpStartTime.Name = "DtpStartTime";
            this.DtpStartTime.Size = new System.Drawing.Size(300, 31);
            this.DtpStartTime.TabIndex = 3;
            // 
            // LblEndTime
            // 
            this.LblEndTime.AutoSize = true;
            this.LblEndTime.Location = new System.Drawing.Point(470, 80);
            this.LblEndTime.Name = "LblEndTime";
            this.LblEndTime.Size = new System.Drawing.Size(115, 21);
            this.LblEndTime.TabIndex = 4;
            this.LblEndTime.Text = "结束时间：";
            // 
            // DtpEndTime
            // 
            this.DtpEndTime.Location = new System.Drawing.Point(586, 77);
            this.DtpEndTime.Name = "DtpEndTime";
            this.DtpEndTime.Size = new System.Drawing.Size(300, 31);
            this.DtpEndTime.TabIndex = 5;
            // 
            // LblLicenseType
            // 
            this.LblLicenseType.AutoSize = true;
            this.LblLicenseType.Location = new System.Drawing.Point(30, 130);
            this.LblLicenseType.Name = "LblLicenseType";
            this.LblLicenseType.Size = new System.Drawing.Size(115, 21);
            this.LblLicenseType.TabIndex = 6;
            this.LblLicenseType.Text = "授权类型：";
            // 
            // CmbLicenseType
            // 
            this.CmbLicenseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbLicenseType.FormattingEnabled = true;
            this.CmbLicenseType.Items.AddRange(new object[] {
            "试用版",
            "正式版",
            "企业版",
            "永久版"});
            this.CmbLicenseType.Location = new System.Drawing.Point(146, 127);
            this.CmbLicenseType.Name = "CmbLicenseType";
            this.CmbLicenseType.Size = new System.Drawing.Size(200, 29);
            this.CmbLicenseType.TabIndex = 7;
            // 
            // LblExtraInfo
            // 
            this.LblExtraInfo.AutoSize = true;
            this.LblExtraInfo.Location = new System.Drawing.Point(30, 180);
            this.LblExtraInfo.Name = "LblExtraInfo";
            this.LblExtraInfo.Size = new System.Drawing.Size(115, 21);
            this.LblExtraInfo.TabIndex = 8;
            this.LblExtraInfo.Text = "额外信息：";
            // 
            // TxtExtraInfo
            // 
            this.TxtExtraInfo.Location = new System.Drawing.Point(146, 177);
            this.TxtExtraInfo.Name = "TxtExtraInfo";
            this.TxtExtraInfo.Size = new System.Drawing.Size(740, 31);
            this.TxtExtraInfo.TabIndex = 9;
            // 
            // BtnGenerate
            // 
            this.BtnGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.BtnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnGenerate.ForeColor = System.Drawing.Color.White;
            this.BtnGenerate.Location = new System.Drawing.Point(146, 230);
            this.BtnGenerate.Name = "BtnGenerate";
            this.BtnGenerate.Size = new System.Drawing.Size(150, 45);
            this.BtnGenerate.TabIndex = 10;
            this.BtnGenerate.Text = "生成License";
            this.BtnGenerate.UseVisualStyleBackColor = false;
            this.BtnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // BtnValidate
            // 
            this.BtnValidate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(125)))), ((int)(((byte)(50)))));
            this.BtnValidate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnValidate.ForeColor = System.Drawing.Color.White;
            this.BtnValidate.Location = new System.Drawing.Point(320, 230);
            this.BtnValidate.Name = "BtnValidate";
            this.BtnValidate.Size = new System.Drawing.Size(150, 45);
            this.BtnValidate.TabIndex = 11;
            this.BtnValidate.Text = "验证License";
            this.BtnValidate.UseVisualStyleBackColor = false;
            this.BtnValidate.Click += new System.EventHandler(this.BtnValidate_Click);
            // 
            // BtnTrial7Days
            // 
            this.BtnTrial7Days.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.BtnTrial7Days.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnTrial7Days.ForeColor = System.Drawing.Color.White;
            this.BtnTrial7Days.Location = new System.Drawing.Point(494, 230);
            this.BtnTrial7Days.Name = "BtnTrial7Days";
            this.BtnTrial7Days.Size = new System.Drawing.Size(100, 45);
            this.BtnTrial7Days.TabIndex = 12;
            this.BtnTrial7Days.Text = "试用7天";
            this.BtnTrial7Days.UseVisualStyleBackColor = false;
            this.BtnTrial7Days.Click += new System.EventHandler(this.BtnTrial7Days_Click);
            // 
            // BtnTrial30Days
            // 
            this.BtnTrial30Days.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(0)))));
            this.BtnTrial30Days.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnTrial30Days.ForeColor = System.Drawing.Color.White;
            this.BtnTrial30Days.Location = new System.Drawing.Point(610, 230);
            this.BtnTrial30Days.Name = "BtnTrial30Days";
            this.BtnTrial30Days.Size = new System.Drawing.Size(100, 45);
            this.BtnTrial30Days.TabIndex = 13;
            this.BtnTrial30Days.Text = "试用30天";
            this.BtnTrial30Days.UseVisualStyleBackColor = false;
            this.BtnTrial30Days.Click += new System.EventHandler(this.BtnTrial30Days_Click);
            // 
            // BtnFullYear
            // 
            this.BtnFullYear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(39)))), ((int)(((byte)(176)))));
            this.BtnFullYear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnFullYear.ForeColor = System.Drawing.Color.White;
            this.BtnFullYear.Location = new System.Drawing.Point(726, 230);
            this.BtnFullYear.Name = "BtnFullYear";
            this.BtnFullYear.Size = new System.Drawing.Size(100, 45);
            this.BtnFullYear.TabIndex = 14;
            this.BtnFullYear.Text = "正式1年";
            this.BtnFullYear.UseVisualStyleBackColor = false;
            this.BtnFullYear.Click += new System.EventHandler(this.BtnFullYear_Click);
            // 
            // LblPreview
            // 
            this.LblPreview.AutoSize = true;
            this.LblPreview.Location = new System.Drawing.Point(30, 300);
            this.LblPreview.Name = "LblPreview";
            this.LblPreview.Size = new System.Drawing.Size(115, 21);
            this.LblPreview.TabIndex = 15;
            this.LblPreview.Text = "预览信息：";
            // 
            // TxtPreview
            // 
            this.TxtPreview.Font = new System.Drawing.Font("Consolas", 9F);
            this.TxtPreview.Location = new System.Drawing.Point(30, 330);
            this.TxtPreview.Multiline = true;
            this.TxtPreview.Name = "TxtPreview";
            this.TxtPreview.ReadOnly = true;
            this.TxtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TxtPreview.Size = new System.Drawing.Size(856, 200);
            this.TxtPreview.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 560);
            this.Controls.Add(this.TxtPreview);
            this.Controls.Add(this.LblPreview);
            this.Controls.Add(this.BtnFullYear);
            this.Controls.Add(this.BtnTrial30Days);
            this.Controls.Add(this.BtnTrial7Days);
            this.Controls.Add(this.BtnValidate);
            this.Controls.Add(this.BtnGenerate);
            this.Controls.Add(this.TxtExtraInfo);
            this.Controls.Add(this.LblExtraInfo);
            this.Controls.Add(this.CmbLicenseType);
            this.Controls.Add(this.LblLicenseType);
            this.Controls.Add(this.DtpEndTime);
            this.Controls.Add(this.LblEndTime);
            this.Controls.Add(this.DtpStartTime);
            this.Controls.Add(this.LblStartTime);
            this.Controls.Add(this.TxtMachineCode);
            this.Controls.Add(this.LblMachineCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "License生成工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblMachineCode;
        private System.Windows.Forms.TextBox TxtMachineCode;
        private System.Windows.Forms.Label LblStartTime;
        private System.Windows.Forms.DateTimePicker DtpStartTime;
        private System.Windows.Forms.Label LblEndTime;
        private System.Windows.Forms.DateTimePicker DtpEndTime;
        private System.Windows.Forms.Label LblLicenseType;
        private System.Windows.Forms.ComboBox CmbLicenseType;
        private System.Windows.Forms.Label LblExtraInfo;
        private System.Windows.Forms.TextBox TxtExtraInfo;
        private System.Windows.Forms.Button BtnGenerate;
        private System.Windows.Forms.Button BtnValidate;
        private System.Windows.Forms.Button BtnTrial7Days;
        private System.Windows.Forms.Button BtnTrial30Days;
        private System.Windows.Forms.Button BtnFullYear;
        private System.Windows.Forms.Label LblPreview;
        private System.Windows.Forms.TextBox TxtPreview;
    }
}

