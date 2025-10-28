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
            this.label1 = new System.Windows.Forms.Label();
            this.TxtServicesUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CbmDep = new System.Windows.Forms.ComboBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.RedYes = new System.Windows.Forms.RadioButton();
            this.RedNo = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(21, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器Url:";
            // 
            // TxtServicesUrl
            // 
            this.TxtServicesUrl.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtServicesUrl.Location = new System.Drawing.Point(130, 17);
            this.TxtServicesUrl.Name = "TxtServicesUrl";
            this.TxtServicesUrl.Size = new System.Drawing.Size(325, 30);
            this.TxtServicesUrl.TabIndex = 1;
            this.TxtServicesUrl.TextChanged += new System.EventHandler(this.TxtServicesUrl_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(31, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "配置设备:";
            // 
            // CbmDep
            // 
            this.CbmDep.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CbmDep.FormattingEnabled = true;
            this.CbmDep.Location = new System.Drawing.Point(130, 70);
            this.CbmDep.Name = "CbmDep";
            this.CbmDep.Size = new System.Drawing.Size(325, 28);
            this.CbmDep.TabIndex = 3;
            // 
            // BtnSave
            // 
            this.BtnSave.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnSave.Location = new System.Drawing.Point(184, 172);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(84, 39);
            this.BtnSave.TabIndex = 4;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F);
            this.label3.Location = new System.Drawing.Point(21, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "是否打开可选择界面:";
            // 
            // RedYes
            // 
            this.RedYes.AutoSize = true;
            this.RedYes.Font = new System.Drawing.Font("宋体", 15F);
            this.RedYes.Location = new System.Drawing.Point(229, 123);
            this.RedYes.Name = "RedYes";
            this.RedYes.Size = new System.Drawing.Size(47, 24);
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
            this.RedNo.Location = new System.Drawing.Point(298, 123);
            this.RedNo.Name = "RedNo";
            this.RedNo.Size = new System.Drawing.Size(47, 24);
            this.RedNo.TabIndex = 7;
            this.RedNo.TabStop = true;
            this.RedNo.Text = "否";
            this.RedNo.UseVisualStyleBackColor = true;
            this.RedNo.CheckedChanged += new System.EventHandler(this.RedNo_CheckedChanged);
            // 
            // FormProInitialize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 230);
            this.Controls.Add(this.RedNo);
            this.Controls.Add(this.RedYes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.CbmDep);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TxtServicesUrl);
            this.Controls.Add(this.label1);
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
    }
}