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
            this.Layout = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.roundedButton1 = new WinSelfMachine.Controls.RoundedButton();
            this.BtnReturn = new WinSelfMachine.Controls.RoundedButton();
            this.roundedContainer2 = new WinSelfMachine.Controls.RoundedContainer();
            this.roundTextBox1 = new WinSelfMachine.Controls.RoundTextBox();
            this.BtnSelect = new WinSelfMachine.Controls.RoundedButton();
            this.roundedContainer3 = new WinSelfMachine.Controls.RoundedContainer();
            this.roundedContainer1 = new WinSelfMachine.Controls.RoundedContainer();
            this.TxtBnr = new WinSelfMachine.Controls.RoundTextBox();
            this.BtnReportQuery = new WinSelfMachine.Controls.RoundedButton();
            this._3DButton3 = new WinSelfMachine.Controls._3DButton();
            this._3DButton2 = new WinSelfMachine.Controls._3DButton();
            this._3DButton1 = new WinSelfMachine.Controls._3DButton();
            this.Table = new WinSelfMachine.Controls.TableLayoutControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Layout)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtQuery
            // 
            this.TxtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TxtQuery.AutoSize = true;
            this.TxtQuery.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtQuery.Location = new System.Drawing.Point(131, 219);
            this.TxtQuery.Name = "TxtQuery";
            this.TxtQuery.Size = new System.Drawing.Size(51, 20);
            this.TxtQuery.TabIndex = 3;
            this.TxtQuery.Text = "查询";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("黑体", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(0, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(870, 48);
            this.label2.TabIndex = 4;
            this.label2.Text = "在线报告查询";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // Layout
            // 
            this.Layout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Layout.BackColor = System.Drawing.Color.Transparent;
            this.Layout.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Layout.BackgroundImage")));
            this.Layout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Layout.Location = new System.Drawing.Point(819, 12);
            this.Layout.Name = "Layout";
            this.Layout.Size = new System.Drawing.Size(29, 28);
            this.Layout.TabIndex = 10;
            this.Layout.TabStop = false;
            this.Layout.Click += new System.EventHandler(this.Layout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(251, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 26;
            this.label1.Text = "患者信息：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(459, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 20);
            this.label3.TabIndex = 27;
            this.label3.Text = "检验单号：";
            // 
            // roundedButton1
            // 
            this.roundedButton1.BorderColor = System.Drawing.Color.Transparent;
            this.roundedButton1.BorderThickness = 0;
            this.roundedButton1.ButtonText = "打印";
            this.roundedButton1.CornerRadius = 12;
            this.roundedButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.roundedButton1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.roundedButton1.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.roundedButton1.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.roundedButton1.Location = new System.Drawing.Point(711, 156);
            this.roundedButton1.Name = "roundedButton1";
            this.roundedButton1.Size = new System.Drawing.Size(100, 36);
            this.roundedButton1.TabIndex = 28;
            this.roundedButton1.TextColor = System.Drawing.Color.White;
            // 
            // BtnReturn
            // 
            this.BtnReturn.BorderColor = System.Drawing.Color.Transparent;
            this.BtnReturn.BorderThickness = 0;
            this.BtnReturn.ButtonText = "返回";
            this.BtnReturn.CornerRadius = 12;
            this.BtnReturn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnReturn.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.BtnReturn.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.BtnReturn.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.BtnReturn.Location = new System.Drawing.Point(711, 107);
            this.BtnReturn.Name = "BtnReturn";
            this.BtnReturn.Size = new System.Drawing.Size(100, 36);
            this.BtnReturn.TabIndex = 25;
            this.BtnReturn.TextColor = System.Drawing.Color.White;
            this.BtnReturn.Click += new System.EventHandler(this.BtnReturn_Load);
            // 
            // roundedContainer2
            // 
            this.roundedContainer2.BackColor = System.Drawing.Color.Transparent;
            this.roundedContainer2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundedContainer2.BorderThickness = 1;
            this.roundedContainer2.BottomLeft = false;
            this.roundedContainer2.BottomRight = true;
            this.roundedContainer2.CornerRadius = 12;
            this.roundedContainer2.DividerColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))), ((int)(((byte)(220)))));
            this.roundedContainer2.DividerEnabled = true;
            this.roundedContainer2.DividerHorizontalPadding = 16;
            this.roundedContainer2.DividerThickness = 1;
            this.roundedContainer2.DividerTopSpacing = 8;
            this.roundedContainer2.FillColor = System.Drawing.Color.White;
            this.roundedContainer2.Font = new System.Drawing.Font("宋体", 9F);
            this.roundedContainer2.FontSize = 9;
            this.roundedContainer2.Location = new System.Drawing.Point(243, 97);
            this.roundedContainer2.Name = "roundedContainer2";
            this.roundedContainer2.ShadowOffsetX = 0;
            this.roundedContainer2.ShadowOffsetY = 2;
            this.roundedContainer2.ShadowSize = 6;
            this.roundedContainer2.ShowShadow = true;
            this.roundedContainer2.Size = new System.Drawing.Size(586, 423);
            this.roundedContainer2.TabIndex = 24;
            this.roundedContainer2.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer2.TitleFont = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer2.TitleText = "查询结果";
            this.roundedContainer2.TitleTopPadding = 10;
            this.roundedContainer2.TopLeft = false;
            this.roundedContainer2.TopRight = true;
            // 
            // roundTextBox1
            // 
            this.roundTextBox1.BackColor = System.Drawing.Color.Transparent;
            this.roundTextBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundTextBox1.BorderFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.roundTextBox1.BorderWidth = 1;
            this.roundTextBox1.CornerRadius = 8;
            this.roundTextBox1.Font = new System.Drawing.Font("宋体", 9F);
            this.roundTextBox1.FontSize = 9;
            this.roundTextBox1.Location = new System.Drawing.Point(77, 340);
            this.roundTextBox1.MaxLength = 32767;
            this.roundTextBox1.Name = "roundTextBox1";
            this.roundTextBox1.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.roundTextBox1.PasswordChar = '\0';
            this.roundTextBox1.ReadOnly = false;
            this.roundTextBox1.Size = new System.Drawing.Size(126, 30);
            this.roundTextBox1.TabIndex = 23;
            this.roundTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.roundTextBox1.UseSystemPasswordChar = false;
            // 
            // BtnSelect
            // 
            this.BtnSelect.BorderColor = System.Drawing.Color.Transparent;
            this.BtnSelect.BorderThickness = 0;
            this.BtnSelect.ButtonText = "查询";
            this.BtnSelect.CornerRadius = 12;
            this.BtnSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelect.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.BtnSelect.FillDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(113)))), ((int)(((byte)(234)))));
            this.BtnSelect.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(133)))), ((int)(((byte)(254)))));
            this.BtnSelect.Location = new System.Drawing.Point(93, 398);
            this.BtnSelect.Name = "BtnSelect";
            this.BtnSelect.Size = new System.Drawing.Size(100, 36);
            this.BtnSelect.TabIndex = 22;
            this.BtnSelect.TextColor = System.Drawing.Color.White;
            // 
            // roundedContainer3
            // 
            this.roundedContainer3.BackColor = System.Drawing.Color.Transparent;
            this.roundedContainer3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.roundedContainer3.BorderThickness = 1;
            this.roundedContainer3.BottomLeft = true;
            this.roundedContainer3.BottomRight = false;
            this.roundedContainer3.CornerRadius = 12;
            this.roundedContainer3.DividerColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(100)))), ((int)(((byte)(140)))), ((int)(((byte)(220)))));
            this.roundedContainer3.DividerEnabled = true;
            this.roundedContainer3.DividerHorizontalPadding = 16;
            this.roundedContainer3.DividerThickness = 1;
            this.roundedContainer3.DividerTopSpacing = 8;
            this.roundedContainer3.FillColor = System.Drawing.Color.White;
            this.roundedContainer3.Font = new System.Drawing.Font("宋体", 9F);
            this.roundedContainer3.FontSize = 9;
            this.roundedContainer3.Location = new System.Drawing.Point(42, 97);
            this.roundedContainer3.Name = "roundedContainer3";
            this.roundedContainer3.ShadowOffsetX = 0;
            this.roundedContainer3.ShadowOffsetY = 2;
            this.roundedContainer3.ShadowSize = 6;
            this.roundedContainer3.ShowShadow = true;
            this.roundedContainer3.Size = new System.Drawing.Size(203, 423);
            this.roundedContainer3.TabIndex = 21;
            this.roundedContainer3.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer3.TitleFont = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer3.TitleText = "身份检索";
            this.roundedContainer3.TitleTopPadding = 10;
            this.roundedContainer3.TopLeft = true;
            this.roundedContainer3.TopRight = false;
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
            this.roundedContainer1.DividerEnabled = false;
            this.roundedContainer1.DividerHorizontalPadding = 16;
            this.roundedContainer1.DividerThickness = 1;
            this.roundedContainer1.DividerTopSpacing = 8;
            this.roundedContainer1.FillColor = System.Drawing.Color.White;
            this.roundedContainer1.Font = new System.Drawing.Font("宋体", 9F);
            this.roundedContainer1.FontSize = 9;
            this.roundedContainer1.Location = new System.Drawing.Point(42, 97);
            this.roundedContainer1.Name = "roundedContainer1";
            this.roundedContainer1.ShadowOffsetX = 0;
            this.roundedContainer1.ShadowOffsetY = 2;
            this.roundedContainer1.ShadowSize = 6;
            this.roundedContainer1.ShowShadow = true;
            this.roundedContainer1.Size = new System.Drawing.Size(787, 422);
            this.roundedContainer1.TabIndex = 13;
            this.roundedContainer1.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.roundedContainer1.TitleFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundedContainer1.TitleText = "";
            this.roundedContainer1.TitleTopPadding = 10;
            this.roundedContainer1.TopLeft = true;
            this.roundedContainer1.TopRight = true;
            this.roundedContainer1.Visible = false;
            // 
            // TxtBnr
            // 
            this.TxtBnr.BackColor = System.Drawing.Color.Transparent;
            this.TxtBnr.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TxtBnr.BorderFocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(123)))), ((int)(((byte)(244)))));
            this.TxtBnr.BorderWidth = 1;
            this.TxtBnr.CornerRadius = 8;
            this.TxtBnr.Font = new System.Drawing.Font("宋体", 9F);
            this.TxtBnr.FontSize = 9;
            this.TxtBnr.Location = new System.Drawing.Point(188, 204);
            this.TxtBnr.MaxLength = 32767;
            this.TxtBnr.Name = "TxtBnr";
            this.TxtBnr.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.TxtBnr.PasswordChar = '\0';
            this.TxtBnr.ReadOnly = false;
            this.TxtBnr.Size = new System.Drawing.Size(412, 43);
            this.TxtBnr.TabIndex = 9;
            this.TxtBnr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxtBnr.UseSystemPasswordChar = false;
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
            this.BtnReportQuery.Location = new System.Drawing.Point(623, 211);
            this.BtnReportQuery.Name = "BtnReportQuery";
            this.BtnReportQuery.Size = new System.Drawing.Size(102, 36);
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
            this._3DButton3.Location = new System.Drawing.Point(584, 299);
            this._3DButton3.Name = "_3DButton3";
            this._3DButton3.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton3.ShadowOffset = 4;
            this._3DButton3.Size = new System.Drawing.Size(162, 163);
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
            this._3DButton2.Location = new System.Drawing.Point(350, 299);
            this._3DButton2.Name = "_3DButton2";
            this._3DButton2.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton2.ShadowOffset = 4;
            this._3DButton2.Size = new System.Drawing.Size(162, 163);
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
            this._3DButton1.Location = new System.Drawing.Point(116, 299);
            this._3DButton1.Name = "_3DButton1";
            this._3DButton1.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._3DButton1.ShadowOffset = 4;
            this._3DButton1.Size = new System.Drawing.Size(162, 163);
            this._3DButton1.TabIndex = 0;
            this._3DButton1.TextColor = System.Drawing.Color.Black;
            this._3DButton1.TopColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this._3DButton1.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // Table
            // 
            this.Table.AutoFill = false;
            this.Table.BackColor = System.Drawing.Color.White;
            this.Table.CellFont = new System.Drawing.Font("微软雅黑", 11F);
            this.Table.HeaderBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.Table.HeaderFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.Table.HeaderForeColor = System.Drawing.Color.Black;
            this.Table.HeaderHeight = 45;
            this.Table.Location = new System.Drawing.Point(264, 204);
            this.Table.Name = "Table";
            this.Table.RowBackColor = System.Drawing.Color.White;
            this.Table.RowForeColor = System.Drawing.Color.Black;
            this.Table.RowHeight = 40;
            this.Table.SelectedRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(255)))));
            this.Table.ShowRemainingSpace = true;
            this.Table.Size = new System.Drawing.Size(547, 300);
            this.Table.TabIndex = 29;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(870, 581);
            this.Controls.Add(this.Table);
            this.Controls.Add(this.roundedButton1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnReturn);
            this.Controls.Add(this.roundedContainer2);
            this.Controls.Add(this.roundTextBox1);
            this.Controls.Add(this.BtnSelect);
            this.Controls.Add(this.roundedContainer3);
            this.Controls.Add(this.roundedContainer1);
            this.Controls.Add(this.Layout);
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
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Layout)).EndInit();
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
        private Controls.RoundTextBox TxtBnr;
        private System.Windows.Forms.PictureBox Layout;
        private Controls.RoundedContainer roundedContainer1;
        private Controls.RoundedContainer roundedContainer3;
        private Controls.RoundedButton BtnSelect;
        private Controls.RoundTextBox roundTextBox1;
        private Controls.RoundedContainer roundedContainer2;
        private Controls.RoundedButton BtnReturn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private Controls.RoundedButton roundedButton1;
        private Controls.TableLayoutControl Table;
    }
}

