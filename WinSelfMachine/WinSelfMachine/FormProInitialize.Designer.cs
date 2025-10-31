namespace WinSelfMachine
{
    partial class FormProInitialize
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProInitialize));
            this.label1 = new System.Windows.Forms.Label();
            this.TxtServicesUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CbmDep = new System.Windows.Forms.ComboBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.RedYes = new System.Windows.Forms.RadioButton();
            this.RedNo = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.RedNo2 = new System.Windows.Forms.RadioButton();
            this.RedYes2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(38, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器Url:";
            // 
            // TxtServicesUrl
            // 
            this.TxtServicesUrl.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtServicesUrl.Location = new System.Drawing.Point(238, 30);
            this.TxtServicesUrl.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.TxtServicesUrl.Name = "TxtServicesUrl";
            this.TxtServicesUrl.Size = new System.Drawing.Size(592, 47);
            this.TxtServicesUrl.TabIndex = 1;
            this.TxtServicesUrl.TextChanged += new System.EventHandler(this.TxtServicesUrl_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(57, 130);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "配置设备:";
            // 
            // CbmDep
            // 
            this.CbmDep.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CbmDep.FormattingEnabled = true;
            this.CbmDep.Location = new System.Drawing.Point(238, 122);
            this.CbmDep.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.CbmDep.Name = "CbmDep";
            this.CbmDep.Size = new System.Drawing.Size(592, 43);
            this.CbmDep.TabIndex = 3;
            // 
            // BtnSave
            // 
            this.BtnSave.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnSave.Location = new System.Drawing.Point(341, 380);
            this.BtnSave.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(154, 68);
            this.BtnSave.TabIndex = 4;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F);
            this.label3.Location = new System.Drawing.Point(38, 217);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(383, 35);
            this.label3.TabIndex = 5;
            this.label3.Text = "是否启用文件列表界面:";
            // 
            // RedYes
            // 
            this.RedYes.AutoSize = true;
            this.RedYes.Font = new System.Drawing.Font("宋体", 15F);
            this.RedYes.Location = new System.Drawing.Point(420, 215);
            this.RedYes.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.RedYes.Name = "RedYes";
            this.RedYes.Size = new System.Drawing.Size(75, 39);
            this.RedYes.TabIndex = 6;
            this.RedYes.TabStop = true;
            this.RedYes.Text = "是";
            this.RedYes.UseVisualStyleBackColor = true;
            this.RedYes.CheckedChanged += new System.EventHandler(this.RedYes_CheckedChanged);
            // 
            // RedNo
            // 
            this.RedNo.AutoSize = true;
            this.RedNo.Font = new System.Drawing.Font("宋体", 15F);
            this.RedNo.Location = new System.Drawing.Point(546, 215);
            this.RedNo.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.RedNo.Name = "RedNo";
            this.RedNo.Size = new System.Drawing.Size(75, 39);
            this.RedNo.TabIndex = 7;
            this.RedNo.TabStop = true;
            this.RedNo.Text = "否";
            this.RedNo.UseVisualStyleBackColor = true;
            this.RedNo.CheckedChanged += new System.EventHandler(this.RedNo_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 15F);
            this.label4.Location = new System.Drawing.Point(38, 317);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(313, 35);
            this.label4.TabIndex = 8;
            this.label4.Text = "是否启用二级界面:";
            // 
            // RedNo2
            // 
            this.RedNo2.AutoSize = true;
            this.RedNo2.Font = new System.Drawing.Font("宋体", 15F);
            this.RedNo2.Location = new System.Drawing.Point(497, 315);
            this.RedNo2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.RedNo2.Name = "RedNo2";
            this.RedNo2.Size = new System.Drawing.Size(75, 39);
            this.RedNo2.TabIndex = 10;
            this.RedNo2.TabStop = true;
            this.RedNo2.Text = "否";
            this.RedNo2.UseVisualStyleBackColor = true;
            this.RedNo2.CheckedChanged += new System.EventHandler(this.RedNo2_CheckedChanged);
            // 
            // RedYes2
            // 
            this.RedYes2.AutoSize = true;
            this.RedYes2.Font = new System.Drawing.Font("宋体", 15F);
            this.RedYes2.Location = new System.Drawing.Point(371, 315);
            this.RedYes2.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.RedYes2.Name = "RedYes2";
            this.RedYes2.Size = new System.Drawing.Size(75, 39);
            this.RedYes2.TabIndex = 9;
            this.RedYes2.TabStop = true;
            this.RedYes2.Text = "是";
            this.RedYes2.UseVisualStyleBackColor = true;
            this.RedYes2.CheckedChanged += new System.EventHandler(this.RedYes2_CheckedChanged);
            // 
            // FormProInitialize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 472);
            this.Controls.Add(this.RedNo2);
            this.Controls.Add(this.RedYes2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.RedNo);
            this.Controls.Add(this.RedYes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.CbmDep);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtServicesUrl);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "FormProInitialize";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目初始化设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtServicesUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CbmDep;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton RedYes;
        private System.Windows.Forms.RadioButton RedNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton RedNo2;
        private System.Windows.Forms.RadioButton RedYes2;
    }
}