namespace BarcodePrintCapture
{
    partial class FormBarcodeTemplate
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
            this.groupBoxTemplate = new System.Windows.Forms.GroupBox();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.textBoxIdLabel = new System.Windows.Forms.TextBox();
            this.labelIdLabel = new System.Windows.Forms.Label();
            this.textBoxNameLabel = new System.Windows.Forms.TextBox();
            this.labelNameLabel = new System.Windows.Forms.Label();
            this.textBoxTimeLabel = new System.Windows.Forms.TextBox();
            this.labelTimeLabel = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTemplate
            // 
            this.groupBoxTemplate.Controls.Add(this.textBoxTitle);
            this.groupBoxTemplate.Controls.Add(this.labelTitle);
            this.groupBoxTemplate.Controls.Add(this.textBoxIdLabel);
            this.groupBoxTemplate.Controls.Add(this.labelIdLabel);
            this.groupBoxTemplate.Controls.Add(this.textBoxNameLabel);
            this.groupBoxTemplate.Controls.Add(this.labelNameLabel);
            this.groupBoxTemplate.Controls.Add(this.textBoxTimeLabel);
            this.groupBoxTemplate.Controls.Add(this.labelTimeLabel);
            this.groupBoxTemplate.Controls.Add(this.labelInfo);
            this.groupBoxTemplate.Location = new System.Drawing.Point(12, 12);
            this.groupBoxTemplate.Name = "groupBoxTemplate";
            this.groupBoxTemplate.Size = new System.Drawing.Size(520, 280);
            this.groupBoxTemplate.TabIndex = 0;
            this.groupBoxTemplate.TabStop = false;
            this.groupBoxTemplate.Text = "条码打印模板配置";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(120, 30);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(370, 21);
            this.textBoxTitle.TabIndex = 8;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(20, 33);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(89, 12);
            this.labelTitle.TabIndex = 7;
            this.labelTitle.Text = "标题文本(&T)：";
            // 
            // textBoxIdLabel
            // 
            this.textBoxIdLabel.Location = new System.Drawing.Point(120, 155);
            this.textBoxIdLabel.Name = "textBoxIdLabel";
            this.textBoxIdLabel.Size = new System.Drawing.Size(370, 21);
            this.textBoxIdLabel.TabIndex = 6;
            // 
            // labelIdLabel
            // 
            this.labelIdLabel.AutoSize = true;
            this.labelIdLabel.Location = new System.Drawing.Point(20, 158);
            this.labelIdLabel.Name = "labelIdLabel";
            this.labelIdLabel.Size = new System.Drawing.Size(95, 12);
            this.labelIdLabel.TabIndex = 5;
            this.labelIdLabel.Text = "编号标签(&I)：";
            // 
            // textBoxNameLabel
            // 
            this.textBoxNameLabel.Location = new System.Drawing.Point(120, 185);
            this.textBoxNameLabel.Name = "textBoxNameLabel";
            this.textBoxNameLabel.Size = new System.Drawing.Size(370, 21);
            this.textBoxNameLabel.TabIndex = 4;
            // 
            // labelNameLabel
            // 
            this.labelNameLabel.AutoSize = true;
            this.labelNameLabel.Location = new System.Drawing.Point(20, 188);
            this.labelNameLabel.Name = "labelNameLabel";
            this.labelNameLabel.Size = new System.Drawing.Size(95, 12);
            this.labelNameLabel.TabIndex = 3;
            this.labelNameLabel.Text = "姓名标签(&N)：";
            // 
            // textBoxTimeLabel
            // 
            this.textBoxTimeLabel.Location = new System.Drawing.Point(120, 215);
            this.textBoxTimeLabel.Name = "textBoxTimeLabel";
            this.textBoxTimeLabel.Size = new System.Drawing.Size(370, 21);
            this.textBoxTimeLabel.TabIndex = 2;
            // 
            // labelTimeLabel
            // 
            this.labelTimeLabel.AutoSize = true;
            this.labelTimeLabel.Location = new System.Drawing.Point(20, 218);
            this.labelTimeLabel.Name = "labelTimeLabel";
            this.labelTimeLabel.Size = new System.Drawing.Size(95, 12);
            this.labelTimeLabel.TabIndex = 1;
            this.labelTimeLabel.Text = "时间标签(&R)：";
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(20, 60);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(470, 80);
            this.labelInfo.TabIndex = 0;
            this.labelInfo.Text = "提示：支持以下占位符\r\n{HOSPITAL_NAME} - 医院名称\r\n{PATIENT_NAME} - 患者姓名\r\n{PATIENT_ID} - 患者编号\r\n{REG_TIME} - 登记时间";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(370, 300);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(457, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormBarcodeTemplate
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(544, 345);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBarcodeTemplate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "打印模板配置";
            this.Load += new System.EventHandler(this.FormBarcodeTemplate_Load);
            this.groupBoxTemplate.ResumeLayout(false);
            this.groupBoxTemplate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTemplate;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label labelIdLabel;
        private System.Windows.Forms.TextBox textBoxIdLabel;
        private System.Windows.Forms.Label labelNameLabel;
        private System.Windows.Forms.TextBox textBoxNameLabel;
        private System.Windows.Forms.Label labelTimeLabel;
        private System.Windows.Forms.TextBox textBoxTimeLabel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}

