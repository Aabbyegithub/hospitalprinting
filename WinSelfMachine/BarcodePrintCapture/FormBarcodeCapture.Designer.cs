namespace BarcodePrintCapture
{
    partial class FormBarcodeCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBarcodeCapture));
            this.groupBoxCapture = new System.Windows.Forms.GroupBox();
            this.labelProcessName = new System.Windows.Forms.Label();
            this.textBoxProcessName = new System.Windows.Forms.TextBox();
            this.labelWindowTitle = new System.Windows.Forms.Label();
            this.textBoxWindowTitle = new System.Windows.Forms.TextBox();
            this.labelClassName = new System.Windows.Forms.Label();
            this.textBoxClassName = new System.Windows.Forms.TextBox();
            this.labelIdContent = new System.Windows.Forms.Label();
            this.textBoxIdContent = new System.Windows.Forms.TextBox();
            this.labelNameContent = new System.Windows.Forms.Label();
            this.textBoxNameContent = new System.Windows.Forms.TextBox();
            this.btnGetInfo = new System.Windows.Forms.Button();
            this.labelId = new System.Windows.Forms.Label();
            this.comboBoxId = new System.Windows.Forms.ComboBox();
            this.labelName = new System.Windows.Forms.Label();
            this.comboBoxName = new System.Windows.Forms.ComboBox();
            this.groupBoxConfig = new System.Windows.Forms.GroupBox();
            this.labelConfigProcessName = new System.Windows.Forms.Label();
            this.textBoxConfigProcessName = new System.Windows.Forms.TextBox();
            this.labelConfigWindowTitle = new System.Windows.Forms.Label();
            this.textBoxConfigWindowTitle = new System.Windows.Forms.TextBox();
            this.labelConfigClassName = new System.Windows.Forms.Label();
            this.textBoxConfigClassName = new System.Windows.Forms.TextBox();
            this.labelIdCoords = new System.Windows.Forms.Label();
            this.textBoxIdCoords = new System.Windows.Forms.TextBox();
            this.labelNameCoords = new System.Windows.Forms.Label();
            this.textBoxNameCoords = new System.Windows.Forms.TextBox();
            this.checkBoxAutoPrint = new System.Windows.Forms.CheckBox();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.labelInterval = new System.Windows.Forms.Label();
            this.labelSeconds = new System.Windows.Forms.Label();
            this.checkBoxPrintRegTime = new System.Windows.Forms.CheckBox();
            this.checkBoxHoverWindow = new System.Windows.Forms.CheckBox();
            this.labelHospitalName = new System.Windows.Forms.Label();
            this.textBoxHospitalName = new System.Windows.Forms.TextBox();
            this.labelPrinterName = new System.Windows.Forms.Label();
            this.comboBoxPrinter = new System.Windows.Forms.ComboBox();
            this.btnPrintOptions = new System.Windows.Forms.Button();
            this.btnTemplateConfig = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.groupBoxManualInput = new System.Windows.Forms.GroupBox();
            this.labelManualId = new System.Windows.Forms.Label();
            this.textBoxManualId = new System.Windows.Forms.TextBox();
            this.labelManualName = new System.Windows.Forms.Label();
            this.textBoxManualName = new System.Windows.Forms.TextBox();
            this.btnManualPrint = new System.Windows.Forms.Button();
            this.groupBoxCapture.SuspendLayout();
            this.groupBoxConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxCapture
            // 
            this.groupBoxCapture.Controls.Add(this.textBoxResults);
            this.groupBoxCapture.Controls.Add(this.comboBoxName);
            this.groupBoxCapture.Controls.Add(this.labelName);
            this.groupBoxCapture.Controls.Add(this.comboBoxId);
            this.groupBoxCapture.Controls.Add(this.labelId);
            this.groupBoxCapture.Controls.Add(this.btnGetInfo);
            this.groupBoxCapture.Controls.Add(this.textBoxNameContent);
            this.groupBoxCapture.Controls.Add(this.labelNameContent);
            this.groupBoxCapture.Controls.Add(this.textBoxIdContent);
            this.groupBoxCapture.Controls.Add(this.labelIdContent);
            this.groupBoxCapture.Controls.Add(this.textBoxClassName);
            this.groupBoxCapture.Controls.Add(this.labelClassName);
            this.groupBoxCapture.Controls.Add(this.textBoxWindowTitle);
            this.groupBoxCapture.Controls.Add(this.labelWindowTitle);
            this.groupBoxCapture.Controls.Add(this.textBoxProcessName);
            this.groupBoxCapture.Controls.Add(this.labelProcessName);
            this.groupBoxCapture.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCapture.Name = "groupBoxCapture";
            this.groupBoxCapture.Size = new System.Drawing.Size(600, 360);
            this.groupBoxCapture.TabIndex = 0;
            this.groupBoxCapture.TabStop = false;
            this.groupBoxCapture.Text = "获取信息";
            // 
            // labelProcessName
            // 
            this.labelProcessName.AutoSize = true;
            this.labelProcessName.Location = new System.Drawing.Point(20, 30);
            this.labelProcessName.Name = "labelProcessName";
            this.labelProcessName.Size = new System.Drawing.Size(53, 12);
            this.labelProcessName.TabIndex = 0;
            this.labelProcessName.Text = "进程名：";
            // 
            // textBoxProcessName
            // 
            this.textBoxProcessName.Location = new System.Drawing.Point(80, 27);
            this.textBoxProcessName.Name = "textBoxProcessName";
            this.textBoxProcessName.Size = new System.Drawing.Size(150, 21);
            this.textBoxProcessName.TabIndex = 1;
            // 
            // labelWindowTitle
            // 
            this.labelWindowTitle.AutoSize = true;
            this.labelWindowTitle.Location = new System.Drawing.Point(250, 30);
            this.labelWindowTitle.Name = "labelWindowTitle";
            this.labelWindowTitle.Size = new System.Drawing.Size(65, 12);
            this.labelWindowTitle.TabIndex = 2;
            this.labelWindowTitle.Text = "窗口标题：";
            // 
            // textBoxWindowTitle
            // 
            this.textBoxWindowTitle.Location = new System.Drawing.Point(320, 27);
            this.textBoxWindowTitle.Name = "textBoxWindowTitle";
            this.textBoxWindowTitle.Size = new System.Drawing.Size(150, 21);
            this.textBoxWindowTitle.TabIndex = 3;
            // 
            // labelClassName
            // 
            this.labelClassName.AutoSize = true;
            this.labelClassName.Location = new System.Drawing.Point(480, 30);
            this.labelClassName.Name = "labelClassName";
            this.labelClassName.Size = new System.Drawing.Size(65, 12);
            this.labelClassName.TabIndex = 4;
            this.labelClassName.Text = "子窗口类名：";
            // 
            // textBoxClassName
            // 
            this.textBoxClassName.Location = new System.Drawing.Point(550, 27);
            this.textBoxClassName.Name = "textBoxClassName";
            this.textBoxClassName.Size = new System.Drawing.Size(35, 21);
            this.textBoxClassName.TabIndex = 5;
            // 
            // labelIdContent
            // 
            this.labelIdContent.AutoSize = true;
            this.labelIdContent.Location = new System.Drawing.Point(20, 65);
            this.labelIdContent.Name = "labelIdContent";
            this.labelIdContent.Size = new System.Drawing.Size(53, 12);
            this.labelIdContent.TabIndex = 6;
            this.labelIdContent.Text = "Id内容：";
            // 
            // textBoxIdContent
            // 
            this.textBoxIdContent.Location = new System.Drawing.Point(80, 62);
            this.textBoxIdContent.Name = "textBoxIdContent";
            this.textBoxIdContent.Size = new System.Drawing.Size(150, 21);
            this.textBoxIdContent.TabIndex = 7;
            // 
            // labelNameContent
            // 
            this.labelNameContent.AutoSize = true;
            this.labelNameContent.Location = new System.Drawing.Point(250, 65);
            this.labelNameContent.Name = "labelNameContent";
            this.labelNameContent.Size = new System.Drawing.Size(41, 12);
            this.labelNameContent.TabIndex = 8;
            this.labelNameContent.Text = "姓名：";
            // 
            // textBoxNameContent
            // 
            this.textBoxNameContent.Location = new System.Drawing.Point(320, 62);
            this.textBoxNameContent.Name = "textBoxNameContent";
            this.textBoxNameContent.Size = new System.Drawing.Size(150, 21);
            this.textBoxNameContent.TabIndex = 9;
            // 
            // btnGetInfo
            // 
            this.btnGetInfo.Location = new System.Drawing.Point(485, 60);
            this.btnGetInfo.Name = "btnGetInfo";
            this.btnGetInfo.Size = new System.Drawing.Size(100, 25);
            this.btnGetInfo.TabIndex = 10;
            this.btnGetInfo.Text = "获取信息";
            this.btnGetInfo.UseVisualStyleBackColor = true;
            this.btnGetInfo.Click += new System.EventHandler(this.btnGetInfo_Click);
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.Location = new System.Drawing.Point(20, 100);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(23, 12);
            this.labelId.TabIndex = 11;
            this.labelId.Text = "ID:";
            // 
            // comboBoxId
            // 
            this.comboBoxId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxId.FormattingEnabled = true;
            this.comboBoxId.Location = new System.Drawing.Point(80, 97);
            this.comboBoxId.Name = "comboBoxId";
            this.comboBoxId.Size = new System.Drawing.Size(250, 20);
            this.comboBoxId.TabIndex = 12;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(340, 100);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 12);
            this.labelName.TabIndex = 13;
            this.labelName.Text = "姓名：";
            // 
            // comboBoxName
            // 
            this.comboBoxName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxName.FormattingEnabled = true;
            this.comboBoxName.Location = new System.Drawing.Point(380, 97);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(200, 20);
            this.comboBoxName.TabIndex = 14;
            // 
            // textBoxResults
            // 
            this.textBoxResults.Location = new System.Drawing.Point(20, 130);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ReadOnly = true;
            this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxResults.Size = new System.Drawing.Size(560, 220);
            this.textBoxResults.TabIndex = 15;
            // 
            // groupBoxConfig
            // 
            this.groupBoxConfig.Controls.Add(this.btnCancel);
            this.groupBoxConfig.Controls.Add(this.btnSave);
            this.groupBoxConfig.Controls.Add(this.btnTemplateConfig);
            this.groupBoxConfig.Controls.Add(this.btnPrintOptions);
            this.groupBoxConfig.Controls.Add(this.comboBoxPrinter);
            this.groupBoxConfig.Controls.Add(this.labelPrinterName);
            this.groupBoxConfig.Controls.Add(this.textBoxHospitalName);
            this.groupBoxConfig.Controls.Add(this.labelHospitalName);
            this.groupBoxConfig.Controls.Add(this.checkBoxHoverWindow);
            this.groupBoxConfig.Controls.Add(this.checkBoxPrintRegTime);
            this.groupBoxConfig.Controls.Add(this.labelSeconds);
            this.groupBoxConfig.Controls.Add(this.labelInterval);
            this.groupBoxConfig.Controls.Add(this.numericUpDownInterval);
            this.groupBoxConfig.Controls.Add(this.checkBoxAutoPrint);
            this.groupBoxConfig.Controls.Add(this.textBoxNameCoords);
            this.groupBoxConfig.Controls.Add(this.labelNameCoords);
            this.groupBoxConfig.Controls.Add(this.textBoxIdCoords);
            this.groupBoxConfig.Controls.Add(this.labelIdCoords);
            this.groupBoxConfig.Controls.Add(this.textBoxConfigClassName);
            this.groupBoxConfig.Controls.Add(this.labelConfigClassName);
            this.groupBoxConfig.Controls.Add(this.textBoxConfigWindowTitle);
            this.groupBoxConfig.Controls.Add(this.labelConfigWindowTitle);
            this.groupBoxConfig.Controls.Add(this.textBoxConfigProcessName);
            this.groupBoxConfig.Controls.Add(this.labelConfigProcessName);
            this.groupBoxConfig.Location = new System.Drawing.Point(12, 390);
            this.groupBoxConfig.Name = "groupBoxConfig";
            this.groupBoxConfig.Size = new System.Drawing.Size(600, 290);
            this.groupBoxConfig.TabIndex = 1;
            this.groupBoxConfig.TabStop = false;
            this.groupBoxConfig.Text = "配置";
            // 
            // labelConfigProcessName
            // 
            this.labelConfigProcessName.AutoSize = true;
            this.labelConfigProcessName.Location = new System.Drawing.Point(20, 25);
            this.labelConfigProcessName.Name = "labelConfigProcessName";
            this.labelConfigProcessName.Size = new System.Drawing.Size(53, 12);
            this.labelConfigProcessName.TabIndex = 0;
            this.labelConfigProcessName.Text = "进程名：";
            // 
            // textBoxConfigProcessName
            // 
            this.textBoxConfigProcessName.Location = new System.Drawing.Point(80, 22);
            this.textBoxConfigProcessName.Name = "textBoxConfigProcessName";
            this.textBoxConfigProcessName.Size = new System.Drawing.Size(150, 21);
            this.textBoxConfigProcessName.TabIndex = 1;
            // 
            // labelConfigWindowTitle
            // 
            this.labelConfigWindowTitle.AutoSize = true;
            this.labelConfigWindowTitle.Location = new System.Drawing.Point(250, 25);
            this.labelConfigWindowTitle.Name = "labelConfigWindowTitle";
            this.labelConfigWindowTitle.Size = new System.Drawing.Size(65, 12);
            this.labelConfigWindowTitle.TabIndex = 2;
            this.labelConfigWindowTitle.Text = "窗口标题：";
            // 
            // textBoxConfigWindowTitle
            // 
            this.textBoxConfigWindowTitle.Location = new System.Drawing.Point(320, 22);
            this.textBoxConfigWindowTitle.Name = "textBoxConfigWindowTitle";
            this.textBoxConfigWindowTitle.Size = new System.Drawing.Size(150, 21);
            this.textBoxConfigWindowTitle.TabIndex = 3;
            // 
            // labelConfigClassName
            // 
            this.labelConfigClassName.AutoSize = true;
            this.labelConfigClassName.Location = new System.Drawing.Point(480, 25);
            this.labelConfigClassName.Name = "labelConfigClassName";
            this.labelConfigClassName.Size = new System.Drawing.Size(65, 12);
            this.labelConfigClassName.TabIndex = 4;
            this.labelConfigClassName.Text = "子窗口类名：";
            // 
            // textBoxConfigClassName
            // 
            this.textBoxConfigClassName.Location = new System.Drawing.Point(550, 22);
            this.textBoxConfigClassName.Name = "textBoxConfigClassName";
            this.textBoxConfigClassName.Size = new System.Drawing.Size(35, 21);
            this.textBoxConfigClassName.TabIndex = 5;
            // 
            // labelIdCoords
            // 
            this.labelIdCoords.AutoSize = true;
            this.labelIdCoords.Location = new System.Drawing.Point(20, 60);
            this.labelIdCoords.Name = "labelIdCoords";
            this.labelIdCoords.Size = new System.Drawing.Size(83, 12);
            this.labelIdCoords.TabIndex = 6;
            this.labelIdCoords.Text = "Id窗口坐标：";
            // 
            // textBoxIdCoords
            // 
            this.textBoxIdCoords.Location = new System.Drawing.Point(110, 57);
            this.textBoxIdCoords.Name = "textBoxIdCoords";
            this.textBoxIdCoords.Size = new System.Drawing.Size(100, 21);
            this.textBoxIdCoords.TabIndex = 7;
            // 
            // labelNameCoords
            // 
            this.labelNameCoords.AutoSize = true;
            this.labelNameCoords.Location = new System.Drawing.Point(220, 60);
            this.labelNameCoords.Name = "labelNameCoords";
            this.labelNameCoords.Size = new System.Drawing.Size(89, 12);
            this.labelNameCoords.TabIndex = 8;
            this.labelNameCoords.Text = "姓名窗口坐标：";
            // 
            // textBoxNameCoords
            // 
            this.textBoxNameCoords.Location = new System.Drawing.Point(320, 57);
            this.textBoxNameCoords.Name = "textBoxNameCoords";
            this.textBoxNameCoords.Size = new System.Drawing.Size(100, 21);
            this.textBoxNameCoords.TabIndex = 9;
            // 
            // checkBoxAutoPrint
            // 
            this.checkBoxAutoPrint.AutoSize = true;
            this.checkBoxAutoPrint.Location = new System.Drawing.Point(20, 90);
            this.checkBoxAutoPrint.Name = "checkBoxAutoPrint";
            this.checkBoxAutoPrint.Size = new System.Drawing.Size(96, 16);
            this.checkBoxAutoPrint.TabIndex = 10;
            this.checkBoxAutoPrint.Text = "自动定时打印";
            this.checkBoxAutoPrint.UseVisualStyleBackColor = true;
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(130, 88);
            this.numericUpDownInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(60, 21);
            this.numericUpDownInterval.TabIndex = 11;
            this.numericUpDownInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // labelInterval
            // 
            this.labelInterval.AutoSize = true;
            this.labelInterval.Location = new System.Drawing.Point(196, 92);
            this.labelInterval.Name = "labelInterval";
            this.labelInterval.Size = new System.Drawing.Size(11, 12);
            this.labelInterval.TabIndex = 12;
            this.labelInterval.Text = "秒";
            // 
            // labelSeconds
            // 
            this.labelSeconds.AutoSize = true;
            this.labelSeconds.Location = new System.Drawing.Point(196, 92);
            this.labelSeconds.Name = "labelSeconds";
            this.labelSeconds.Size = new System.Drawing.Size(11, 12);
            this.labelSeconds.TabIndex = 12;
            this.labelSeconds.Text = "秒";
            // 
            // checkBoxPrintRegTime
            // 
            this.checkBoxPrintRegTime.AutoSize = true;
            this.checkBoxPrintRegTime.Checked = true;
            this.checkBoxPrintRegTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrintRegTime.Location = new System.Drawing.Point(20, 115);
            this.checkBoxPrintRegTime.Name = "checkBoxPrintRegTime";
            this.checkBoxPrintRegTime.Size = new System.Drawing.Size(96, 16);
            this.checkBoxPrintRegTime.TabIndex = 13;
            this.checkBoxPrintRegTime.Text = "打印登记时间";
            this.checkBoxPrintRegTime.UseVisualStyleBackColor = true;
            // 
            // checkBoxHoverWindow
            // 
            this.checkBoxHoverWindow.AutoSize = true;
            this.checkBoxHoverWindow.Checked = true;
            this.checkBoxHoverWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHoverWindow.Location = new System.Drawing.Point(20, 140);
            this.checkBoxHoverWindow.Name = "checkBoxHoverWindow";
            this.checkBoxHoverWindow.Size = new System.Drawing.Size(96, 16);
            this.checkBoxHoverWindow.TabIndex = 14;
            this.checkBoxHoverWindow.Text = "悬停窗点击打印";
            this.checkBoxHoverWindow.UseVisualStyleBackColor = true;
            // 
            // labelHospitalName
            // 
            this.labelHospitalName.AutoSize = true;
            this.labelHospitalName.Location = new System.Drawing.Point(150, 115);
            this.labelHospitalName.Name = "labelHospitalName";
            this.labelHospitalName.Size = new System.Drawing.Size(65, 12);
            this.labelHospitalName.TabIndex = 15;
            this.labelHospitalName.Text = "医院名称：";
            // 
            // textBoxHospitalName
            // 
            this.textBoxHospitalName.Location = new System.Drawing.Point(220, 112);
            this.textBoxHospitalName.Name = "textBoxHospitalName";
            this.textBoxHospitalName.Size = new System.Drawing.Size(150, 21);
            this.textBoxHospitalName.TabIndex = 16;
            this.textBoxHospitalName.Text = "中国大医院";
            // 
            // labelPrinterName
            // 
            this.labelPrinterName.AutoSize = true;
            this.labelPrinterName.Location = new System.Drawing.Point(150, 140);
            this.labelPrinterName.Name = "labelPrinterName";
            this.labelPrinterName.Size = new System.Drawing.Size(77, 12);
            this.labelPrinterName.TabIndex = 17;
            this.labelPrinterName.Text = "条码打印机：";
            // 
            // comboBoxPrinter
            // 
            this.comboBoxPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrinter.FormattingEnabled = true;
            this.comboBoxPrinter.Location = new System.Drawing.Point(235, 137);
            this.comboBoxPrinter.Name = "comboBoxPrinter";
            this.comboBoxPrinter.Size = new System.Drawing.Size(200, 20);
            this.comboBoxPrinter.TabIndex = 18;
            // 
            // btnPrintOptions
            // 
            this.btnPrintOptions.Location = new System.Drawing.Point(450, 135);
            this.btnPrintOptions.Name = "btnPrintOptions";
            this.btnPrintOptions.Size = new System.Drawing.Size(80, 25);
            this.btnPrintOptions.TabIndex = 19;
            this.btnPrintOptions.Text = "测试打印";
            this.btnPrintOptions.UseVisualStyleBackColor = true;
            this.btnPrintOptions.Click += new System.EventHandler(this.btnPrintOptions_Click);
            // 
            // btnTemplateConfig
            // 
            this.btnTemplateConfig.Location = new System.Drawing.Point(20, 165);
            this.btnTemplateConfig.Name = "btnTemplateConfig";
            this.btnTemplateConfig.Size = new System.Drawing.Size(130, 30);
            this.btnTemplateConfig.TabIndex = 22;
            this.btnTemplateConfig.Text = "打印模板配置";
            this.btnTemplateConfig.UseVisualStyleBackColor = true;
            this.btnTemplateConfig.Click += new System.EventHandler(this.btnTemplateConfig_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(430, 220);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 20;
            this.btnSave.Text = "保存配置";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(520, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBoxManualInput
            // 
            this.groupBoxManualInput.Controls.Add(this.btnManualPrint);
            this.groupBoxManualInput.Controls.Add(this.textBoxManualName);
            this.groupBoxManualInput.Controls.Add(this.labelManualName);
            this.groupBoxManualInput.Controls.Add(this.textBoxManualId);
            this.groupBoxManualInput.Controls.Add(this.labelManualId);
            this.groupBoxManualInput.Location = new System.Drawing.Point(12, 698);
            this.groupBoxManualInput.Name = "groupBoxManualInput";
            this.groupBoxManualInput.Size = new System.Drawing.Size(600, 85);
            this.groupBoxManualInput.TabIndex = 2;
            this.groupBoxManualInput.TabStop = false;
            this.groupBoxManualInput.Text = "手动输入打印（从浏览器复制粘贴）";
            // 
            // labelManualId
            // 
            this.labelManualId.AutoSize = true;
            this.labelManualId.Location = new System.Drawing.Point(20, 30);
            this.labelManualId.Name = "labelManualId";
            this.labelManualId.Size = new System.Drawing.Size(53, 12);
            this.labelManualId.TabIndex = 0;
            this.labelManualId.Text = "员工ID：";
            // 
            // textBoxManualId
            // 
            this.textBoxManualId.Location = new System.Drawing.Point(80, 27);
            this.textBoxManualId.Name = "textBoxManualId";
            this.textBoxManualId.Size = new System.Drawing.Size(150, 21);
            this.textBoxManualId.TabIndex = 1;
            // 
            // labelManualName
            // 
            this.labelManualName.AutoSize = true;
            this.labelManualName.Location = new System.Drawing.Point(250, 30);
            this.labelManualName.Name = "labelManualName";
            this.labelManualName.Size = new System.Drawing.Size(41, 12);
            this.labelManualName.TabIndex = 2;
            this.labelManualName.Text = "姓名：";
            // 
            // textBoxManualName
            // 
            this.textBoxManualName.Location = new System.Drawing.Point(300, 27);
            this.textBoxManualName.Name = "textBoxManualName";
            this.textBoxManualName.Size = new System.Drawing.Size(150, 21);
            this.textBoxManualName.TabIndex = 3;
            // 
            // btnManualPrint
            // 
            this.btnManualPrint.Location = new System.Drawing.Point(470, 25);
            this.btnManualPrint.Name = "btnManualPrint";
            this.btnManualPrint.Size = new System.Drawing.Size(110, 25);
            this.btnManualPrint.TabIndex = 4;
            this.btnManualPrint.Text = "立即打印 ►";
            this.btnManualPrint.UseVisualStyleBackColor = true;
            this.btnManualPrint.Click += new System.EventHandler(this.btnManualPrint_Click);
            // 
            // FormBarcodeCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 795);
            this.Controls.Add(this.groupBoxManualInput);
            this.Controls.Add(this.groupBoxConfig);
            this.Controls.Add(this.groupBoxCapture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBarcodeCapture";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "条码打印捕获版(v1.1.1)";
            this.Load += new System.EventHandler(this.FormBarcodeCapture_Load);
            this.groupBoxCapture.ResumeLayout(false);
            this.groupBoxCapture.PerformLayout();
            this.groupBoxConfig.ResumeLayout(false);
            this.groupBoxConfig.PerformLayout();
            this.groupBoxManualInput.ResumeLayout(false);
            this.groupBoxManualInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCapture;
        private System.Windows.Forms.Label labelProcessName;
        private System.Windows.Forms.TextBox textBoxProcessName;
        private System.Windows.Forms.Label labelWindowTitle;
        private System.Windows.Forms.TextBox textBoxWindowTitle;
        private System.Windows.Forms.Label labelClassName;
        private System.Windows.Forms.TextBox textBoxClassName;
        private System.Windows.Forms.Label labelIdContent;
        private System.Windows.Forms.TextBox textBoxIdContent;
        private System.Windows.Forms.Label labelNameContent;
        private System.Windows.Forms.TextBox textBoxNameContent;
        private System.Windows.Forms.Button btnGetInfo;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.ComboBox comboBoxId;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.ComboBox comboBoxName;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.GroupBox groupBoxConfig;
        private System.Windows.Forms.Label labelConfigProcessName;
        private System.Windows.Forms.TextBox textBoxConfigProcessName;
        private System.Windows.Forms.Label labelConfigWindowTitle;
        private System.Windows.Forms.TextBox textBoxConfigWindowTitle;
        private System.Windows.Forms.Label labelConfigClassName;
        private System.Windows.Forms.TextBox textBoxConfigClassName;
        private System.Windows.Forms.Label labelIdCoords;
        private System.Windows.Forms.TextBox textBoxIdCoords;
        private System.Windows.Forms.Label labelNameCoords;
        private System.Windows.Forms.TextBox textBoxNameCoords;
        private System.Windows.Forms.CheckBox checkBoxAutoPrint;
        private System.Windows.Forms.NumericUpDown numericUpDownInterval;
        private System.Windows.Forms.Label labelInterval;
        private System.Windows.Forms.Label labelSeconds;
        private System.Windows.Forms.CheckBox checkBoxPrintRegTime;
        private System.Windows.Forms.CheckBox checkBoxHoverWindow;
        private System.Windows.Forms.Label labelHospitalName;
        private System.Windows.Forms.TextBox textBoxHospitalName;
        private System.Windows.Forms.Label labelPrinterName;
        private System.Windows.Forms.ComboBox comboBoxPrinter;
        private System.Windows.Forms.Button btnPrintOptions;
        private System.Windows.Forms.Button btnTemplateConfig;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBoxManualInput;
        private System.Windows.Forms.Label labelManualId;
        private System.Windows.Forms.TextBox textBoxManualId;
        private System.Windows.Forms.Label labelManualName;
        private System.Windows.Forms.TextBox textBoxManualName;
        private System.Windows.Forms.Button btnManualPrint;
    }
}

