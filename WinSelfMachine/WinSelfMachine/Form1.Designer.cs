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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TxtTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnSetting = new WinSelfMachine.Controls.RoundButton();
            this.BtnClose = new WinSelfMachine.Controls.RoundButton();
            this.Txtbr = new WinSelfMachine.Controls.RoundTextBox();
            this.BtnAvailableFilm = new WinSelfMachine.Controls.RoundButton();
            this.BtnWaitTime = new WinSelfMachine.Controls.RoundButton();
            this.BtnPrintSetting = new WinSelfMachine.Controls.RoundButton();
            this.roundedContainer1 = new WinSelfMachine.Controls.RoundedContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.姓名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.检查日期 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.类型 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.胶片数量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.报告数量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.是否打印 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.胶片路径 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.报告路径 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.是否已打印 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patient_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TxtSelect = new System.Windows.Forms.Label();
            this.BtnConfirmPaint = new System.Windows.Forms.Button();
            this.BtnCancelPrint = new System.Windows.Forms.Button();
            this.roundedContainer2 = new WinSelfMachine.Controls.RoundedContainer();
            this.BtnAiAnalysis = new WinSelfMachine.Controls.RoundedButton();
            this.BtnDirect = new WinSelfMachine.Controls.RoundedButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            // BtnSetting
            // 
            this.BtnSetting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSetting.BackColor = System.Drawing.Color.Transparent;
            this.BtnSetting.BackFillColor = System.Drawing.Color.White;
            this.BtnSetting.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.BtnSetting.BorderThickness = 2;
            this.BtnSetting.CornerDiameter = 16;
            this.BtnSetting.Icon = null;
            this.BtnSetting.IconSize = new System.Drawing.Size(24, 24);
            this.BtnSetting.IconTextSpacing = 5;
            this.BtnSetting.LabelText = "启动设置";
            this.BtnSetting.Location = new System.Drawing.Point(394, 613);
            this.BtnSetting.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.BtnSetting.Name = "BtnSetting";
            this.BtnSetting.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnSetting.ShadowOffset = 3;
            this.BtnSetting.ShowShadow = true;
            this.BtnSetting.Size = new System.Drawing.Size(94, 82);
            this.BtnSetting.TabIndex = 16;
            this.BtnSetting.TextColor = System.Drawing.Color.Black;
            this.BtnSetting.Visible = false;
            this.BtnSetting.Click += new System.EventHandler(this.BtnSetting_Click);
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
            this.BtnClose.Location = new System.Drawing.Point(523, 613);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
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
            // Txtbr
            // 
            this.Txtbr.BackColor = System.Drawing.Color.Transparent;
            this.Txtbr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.Txtbr.BorderFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.Txtbr.BorderWidth = 1;
            this.Txtbr.CornerRadius = 8;
            this.Txtbr.Font = new System.Drawing.Font("宋体", 15F);
            this.Txtbr.FontSize = 15;
            this.Txtbr.Location = new System.Drawing.Point(277, 390);
            this.Txtbr.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.Txtbr.MaxLength = 32767;
            this.Txtbr.Name = "Txtbr";
            this.Txtbr.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.Txtbr.PasswordChar = '\0';
            this.Txtbr.ReadOnly = false;
            this.Txtbr.Size = new System.Drawing.Size(200, 33);
            this.Txtbr.TabIndex = 12;
            this.Txtbr.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.Txtbr.UseSystemPasswordChar = false;
            this.Txtbr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txtbr_KeyPress);
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
            this.BtnAvailableFilm.Location = new System.Drawing.Point(275, 613);
            this.BtnAvailableFilm.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
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
            this.BtnWaitTime.Location = new System.Drawing.Point(155, 613);
            this.BtnWaitTime.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
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
            this.BtnPrintSetting.Location = new System.Drawing.Point(28, 613);
            this.BtnPrintSetting.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
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
            // roundedContainer1
            // 
            this.roundedContainer1.BackColor = System.Drawing.Color.Transparent;
            this.roundedContainer1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundedContainer1.BorderThickness = 1;
            this.roundedContainer1.BottomLeft = true;
            this.roundedContainer1.BottomRight = true;
            this.roundedContainer1.CornerRadius = 12;
            this.roundedContainer1.DividerColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))), ((int)(((byte)(220)))));
            this.roundedContainer1.DividerEnabled = true;
            this.roundedContainer1.DividerHorizontalPadding = 16;
            this.roundedContainer1.DividerThickness = 1;
            this.roundedContainer1.DividerTopSpacing = 8;
            this.roundedContainer1.FillColor = System.Drawing.Color.White;
            this.roundedContainer1.Font = new System.Drawing.Font("宋体", 9F);
            this.roundedContainer1.FontSize = 9;
            this.roundedContainer1.Location = new System.Drawing.Point(43, 85);
            this.roundedContainer1.Name = "roundedContainer1";
            this.roundedContainer1.ShadowOffsetX = 0;
            this.roundedContainer1.ShadowOffsetY = 2;
            this.roundedContainer1.ShadowSize = 6;
            this.roundedContainer1.ShowShadow = true;
            this.roundedContainer1.Size = new System.Drawing.Size(599, 473);
            this.roundedContainer1.TabIndex = 17;
            this.roundedContainer1.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer1.TitleFont = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer1.TitleText = "请选择要打印的胶片和报告";
            this.roundedContainer1.TitleTopPadding = 10;
            this.roundedContainer1.TopLeft = true;
            this.roundedContainer1.TopRight = true;
            this.roundedContainer1.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.Peru;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.姓名,
            this.检查日期,
            this.类型,
            this.胶片数量,
            this.报告数量,
            this.是否打印,
            this.胶片路径,
            this.报告路径,
            this.是否已打印,
            this.id,
            this.patient_id});
            this.dataGridView1.Location = new System.Drawing.Point(53, 143);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 72;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(577, 345);
            this.dataGridView1.TabIndex = 18;
            this.dataGridView1.Visible = false;
            // 
            // 姓名
            // 
            this.姓名.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.姓名.FillWeight = 30.81218F;
            this.姓名.HeaderText = "姓名";
            this.姓名.MinimumWidth = 9;
            this.姓名.Name = "姓名";
            this.姓名.ReadOnly = true;
            // 
            // 检查日期
            // 
            this.检查日期.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.检查日期.FillWeight = 60.81218F;
            this.检查日期.HeaderText = "检查日期";
            this.检查日期.MinimumWidth = 9;
            this.检查日期.Name = "检查日期";
            this.检查日期.ReadOnly = true;
            // 
            // 类型
            // 
            this.类型.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.类型.FillWeight = 30.81218F;
            this.类型.HeaderText = "类型";
            this.类型.MinimumWidth = 9;
            this.类型.Name = "类型";
            this.类型.ReadOnly = true;
            // 
            // 胶片数量
            // 
            this.胶片数量.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.胶片数量.FillWeight = 40.81218F;
            this.胶片数量.HeaderText = "胶片数量";
            this.胶片数量.MinimumWidth = 9;
            this.胶片数量.Name = "胶片数量";
            this.胶片数量.ReadOnly = true;
            // 
            // 报告数量
            // 
            this.报告数量.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.报告数量.FillWeight = 40.81218F;
            this.报告数量.HeaderText = "报告数量";
            this.报告数量.MinimumWidth = 9;
            this.报告数量.Name = "报告数量";
            this.报告数量.ReadOnly = true;
            // 
            // 是否打印
            // 
            this.是否打印.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.是否打印.FillWeight = 40.81218F;
            this.是否打印.HeaderText = "是否打印";
            this.是否打印.MinimumWidth = 9;
            this.是否打印.Name = "是否打印";
            this.是否打印.ReadOnly = true;
            // 
            // 胶片路径
            // 
            this.胶片路径.HeaderText = "胶片路径";
            this.胶片路径.MinimumWidth = 9;
            this.胶片路径.Name = "胶片路径";
            this.胶片路径.ReadOnly = true;
            this.胶片路径.Visible = false;
            this.胶片路径.Width = 175;
            // 
            // 报告路径
            // 
            this.报告路径.HeaderText = "报告路径";
            this.报告路径.MinimumWidth = 9;
            this.报告路径.Name = "报告路径";
            this.报告路径.ReadOnly = true;
            this.报告路径.Visible = false;
            this.报告路径.Width = 175;
            // 
            // 是否已打印
            // 
            this.是否已打印.HeaderText = "是否已打印";
            this.是否已打印.MinimumWidth = 9;
            this.是否已打印.Name = "是否已打印";
            this.是否已打印.ReadOnly = true;
            this.是否已打印.Visible = false;
            this.是否已打印.Width = 175;
            // 
            // id
            // 
            this.id.HeaderText = "id";
            this.id.MinimumWidth = 9;
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            this.id.Width = 175;
            // 
            // patient_id
            // 
            this.patient_id.HeaderText = "patient_id";
            this.patient_id.MinimumWidth = 9;
            this.patient_id.Name = "patient_id";
            this.patient_id.ReadOnly = true;
            this.patient_id.Visible = false;
            this.patient_id.Width = 175;
            // 
            // TxtSelect
            // 
            this.TxtSelect.AutoSize = true;
            this.TxtSelect.BackColor = System.Drawing.Color.White;
            this.TxtSelect.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtSelect.ForeColor = System.Drawing.Color.SaddleBrown;
            this.TxtSelect.Location = new System.Drawing.Point(63, 513);
            this.TxtSelect.Name = "TxtSelect";
            this.TxtSelect.Size = new System.Drawing.Size(0, 20);
            this.TxtSelect.TabIndex = 19;
            this.TxtSelect.Visible = false;
            // 
            // BtnConfirmPaint
            // 
            this.BtnConfirmPaint.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnConfirmPaint.Location = new System.Drawing.Point(406, 506);
            this.BtnConfirmPaint.Name = "BtnConfirmPaint";
            this.BtnConfirmPaint.Size = new System.Drawing.Size(93, 35);
            this.BtnConfirmPaint.TabIndex = 20;
            this.BtnConfirmPaint.Text = "确认";
            this.BtnConfirmPaint.UseVisualStyleBackColor = true;
            this.BtnConfirmPaint.Visible = false;
            this.BtnConfirmPaint.Click += new System.EventHandler(this.BtnConfirmPaint_Click);
            // 
            // BtnCancelPrint
            // 
            this.BtnCancelPrint.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnCancelPrint.Location = new System.Drawing.Point(523, 506);
            this.BtnCancelPrint.Name = "BtnCancelPrint";
            this.BtnCancelPrint.Size = new System.Drawing.Size(98, 35);
            this.BtnCancelPrint.TabIndex = 21;
            this.BtnCancelPrint.Text = "放弃打印";
            this.BtnCancelPrint.UseVisualStyleBackColor = true;
            this.BtnCancelPrint.Visible = false;
            this.BtnCancelPrint.Click += new System.EventHandler(this.BtnCancelPrint_Click);
            // 
            // roundedContainer2
            // 
            this.roundedContainer2.BackColor = System.Drawing.Color.Transparent;
            this.roundedContainer2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundedContainer2.BorderThickness = 1;
            this.roundedContainer2.BottomLeft = true;
            this.roundedContainer2.BottomRight = true;
            this.roundedContainer2.CornerRadius = 12;
            this.roundedContainer2.DividerColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))), ((int)(((byte)(220)))));
            this.roundedContainer2.DividerEnabled = false;
            this.roundedContainer2.DividerHorizontalPadding = 16;
            this.roundedContainer2.DividerThickness = 1;
            this.roundedContainer2.DividerTopSpacing = 8;
            this.roundedContainer2.FillColor = System.Drawing.Color.White;
            this.roundedContainer2.Font = new System.Drawing.Font("宋体", 9F);
            this.roundedContainer2.FontSize = 9;
            this.roundedContainer2.Location = new System.Drawing.Point(171, 241);
            this.roundedContainer2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.roundedContainer2.Name = "roundedContainer2";
            this.roundedContainer2.ShadowOffsetX = 0;
            this.roundedContainer2.ShadowOffsetY = 2;
            this.roundedContainer2.ShadowSize = 6;
            this.roundedContainer2.ShowShadow = true;
            this.roundedContainer2.Size = new System.Drawing.Size(338, 155);
            this.roundedContainer2.TabIndex = 22;
            this.roundedContainer2.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer2.TitleFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer2.TitleText = "";
            this.roundedContainer2.TitleTopPadding = 10;
            this.roundedContainer2.TopLeft = true;
            this.roundedContainer2.TopRight = true;
            this.roundedContainer2.Visible = false;
            // 
            // BtnAiAnalysis
            // 
            this.BtnAiAnalysis.BorderColor = System.Drawing.Color.Transparent;
            this.BtnAiAnalysis.BorderThickness = 0;
            this.BtnAiAnalysis.ButtonText = "报告解读(含营养结构建议)";
            this.BtnAiAnalysis.CornerRadius = 12;
            this.BtnAiAnalysis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnAiAnalysis.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.BtnAiAnalysis.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.BtnAiAnalysis.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.BtnAiAnalysis.Font = new System.Drawing.Font("宋体", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnAiAnalysis.Location = new System.Drawing.Point(215, 264);
            this.BtnAiAnalysis.Name = "BtnAiAnalysis";
            this.BtnAiAnalysis.Size = new System.Drawing.Size(252, 45);
            this.BtnAiAnalysis.TabIndex = 23;
            this.BtnAiAnalysis.TextColor = System.Drawing.Color.White;
            this.BtnAiAnalysis.Visible = false;
            this.BtnAiAnalysis.Click += new System.EventHandler(this.BtnAiAnalysis_Click);
            // 
            // BtnDirect
            // 
            this.BtnDirect.BorderColor = System.Drawing.Color.Transparent;
            this.BtnDirect.BorderThickness = 0;
            this.BtnDirect.ButtonText = "普通打印";
            this.BtnDirect.CornerRadius = 12;
            this.BtnDirect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnDirect.FillColor = System.Drawing.Color.PeachPuff;
            this.BtnDirect.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.BtnDirect.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.BtnDirect.Font = new System.Drawing.Font("宋体", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnDirect.Location = new System.Drawing.Point(265, 326);
            this.BtnDirect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BtnDirect.Name = "BtnDirect";
            this.BtnDirect.Size = new System.Drawing.Size(127, 45);
            this.BtnDirect.TabIndex = 24;
            this.BtnDirect.TextColor = System.Drawing.Color.Black;
            this.BtnDirect.Visible = false;
            this.BtnDirect.Click += new System.EventHandler(this.BtnDirect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(689, 725);
            this.Controls.Add(this.BtnDirect);
            this.Controls.Add(this.BtnAiAnalysis);
            this.Controls.Add(this.roundedContainer2);
            this.Controls.Add(this.BtnCancelPrint);
            this.Controls.Add(this.BtnConfirmPaint);
            this.Controls.Add(this.TxtSelect);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.roundedContainer1);
            this.Controls.Add(this.BtnSetting);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Txtbr);
            this.Controls.Add(this.BtnAvailableFilm);
            this.Controls.Add(this.BtnWaitTime);
            this.Controls.Add(this.BtnPrintSetting);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.TxtTitle);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
        private Controls.RoundButton BtnSetting;
        private Controls.RoundedContainer roundedContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label TxtSelect;
        private System.Windows.Forms.Button BtnConfirmPaint;
        private System.Windows.Forms.Button BtnCancelPrint;
        private System.Windows.Forms.DataGridViewTextBoxColumn 姓名;
        private System.Windows.Forms.DataGridViewTextBoxColumn 检查日期;
        private System.Windows.Forms.DataGridViewTextBoxColumn 类型;
        private System.Windows.Forms.DataGridViewTextBoxColumn 胶片数量;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报告数量;
        private System.Windows.Forms.DataGridViewCheckBoxColumn 是否打印;
        private System.Windows.Forms.DataGridViewTextBoxColumn 胶片路径;
        private System.Windows.Forms.DataGridViewTextBoxColumn 报告路径;
        private System.Windows.Forms.DataGridViewTextBoxColumn 是否已打印;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn patient_id;
        private Controls.RoundedContainer roundedContainer2;
        private Controls.RoundedButton BtnAiAnalysis;
        private Controls.RoundedButton BtnDirect;
    }
}

