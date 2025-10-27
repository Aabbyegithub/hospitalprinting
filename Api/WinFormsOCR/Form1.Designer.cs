namespace WinFormsOCR
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            打开图片ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            显示DICOM信息ToolStripMenuItem = new ToolStripMenuItem();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            操作ToolStripMenuItem = new ToolStripMenuItem();
            识别选中区域ToolStripMenuItem = new ToolStripMenuItem();
            识别全图ToolStripMenuItem = new ToolStripMenuItem();
            清除选区ToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            适应窗口ToolStripMenuItem = new ToolStripMenuItem();
            原始大小ToolStripMenuItem = new ToolStripMenuItem();
            适应宽度ToolStripMenuItem = new ToolStripMenuItem();
            适应高度ToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            btnCopy = new Button();
            textBox1 = new TextBox();
            label2 = new Label();
            pictureBox2 = new PictureBox();
            label1 = new Label();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 操作ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.Size = new Size(1275, 27);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 打开图片ToolStripMenuItem, toolStripSeparator1, 显示DICOM信息ToolStripMenuItem, 退出ToolStripMenuItem });
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开图片ToolStripMenuItem
            // 
            打开图片ToolStripMenuItem.Name = "打开图片ToolStripMenuItem";
            打开图片ToolStripMenuItem.Size = new Size(167, 22);
            打开图片ToolStripMenuItem.Text = "打开图片";
            打开图片ToolStripMenuItem.Click += 打开图片ToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(164, 6);
            // 
            // 显示DICOM信息ToolStripMenuItem
            // 
            显示DICOM信息ToolStripMenuItem.Name = "显示DICOM信息ToolStripMenuItem";
            显示DICOM信息ToolStripMenuItem.Size = new Size(167, 22);
            显示DICOM信息ToolStripMenuItem.Text = "显示DICOM信息";
            显示DICOM信息ToolStripMenuItem.Click += 显示DICOM信息ToolStripMenuItem_Click;
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(167, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // 操作ToolStripMenuItem
            // 
            操作ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 识别选中区域ToolStripMenuItem, 识别全图ToolStripMenuItem, 清除选区ToolStripMenuItem, toolStripSeparator2, 适应窗口ToolStripMenuItem, 原始大小ToolStripMenuItem, 适应宽度ToolStripMenuItem, 适应高度ToolStripMenuItem });
            操作ToolStripMenuItem.Name = "操作ToolStripMenuItem";
            操作ToolStripMenuItem.Size = new Size(44, 21);
            操作ToolStripMenuItem.Text = "操作";
            // 
            // 识别选中区域ToolStripMenuItem
            // 
            识别选中区域ToolStripMenuItem.Name = "识别选中区域ToolStripMenuItem";
            识别选中区域ToolStripMenuItem.Size = new Size(148, 22);
            识别选中区域ToolStripMenuItem.Text = "识别选中区域";
            识别选中区域ToolStripMenuItem.Click += 识别选中区域ToolStripMenuItem_Click;
            // 
            // 识别全图ToolStripMenuItem
            // 
            识别全图ToolStripMenuItem.Name = "识别全图ToolStripMenuItem";
            识别全图ToolStripMenuItem.Size = new Size(148, 22);
            识别全图ToolStripMenuItem.Text = "识别全图";
            识别全图ToolStripMenuItem.Click += 识别全图ToolStripMenuItem_Click;
            // 
            // 清除选区ToolStripMenuItem
            // 
            清除选区ToolStripMenuItem.Name = "清除选区ToolStripMenuItem";
            清除选区ToolStripMenuItem.Size = new Size(148, 22);
            清除选区ToolStripMenuItem.Text = "清除选区";
            清除选区ToolStripMenuItem.Click += 清除选区ToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(145, 6);
            // 
            // 适应窗口ToolStripMenuItem
            // 
            适应窗口ToolStripMenuItem.Name = "适应窗口ToolStripMenuItem";
            适应窗口ToolStripMenuItem.Size = new Size(148, 22);
            适应窗口ToolStripMenuItem.Text = "适应窗口";
            适应窗口ToolStripMenuItem.Click += 适应窗口ToolStripMenuItem_Click;
            // 
            // 原始大小ToolStripMenuItem
            // 
            原始大小ToolStripMenuItem.Name = "原始大小ToolStripMenuItem";
            原始大小ToolStripMenuItem.Size = new Size(148, 22);
            原始大小ToolStripMenuItem.Text = "原始大小";
            原始大小ToolStripMenuItem.Click += 原始大小ToolStripMenuItem_Click;
            // 
            // 适应宽度ToolStripMenuItem
            // 
            适应宽度ToolStripMenuItem.Name = "适应宽度ToolStripMenuItem";
            适应宽度ToolStripMenuItem.Size = new Size(148, 22);
            适应宽度ToolStripMenuItem.Text = "适应宽度";
            适应宽度ToolStripMenuItem.Click += 适应宽度ToolStripMenuItem_Click;
            // 
            // 适应高度ToolStripMenuItem
            // 
            适应高度ToolStripMenuItem.Name = "适应高度ToolStripMenuItem";
            适应高度ToolStripMenuItem.Size = new Size(148, 22);
            适应高度ToolStripMenuItem.Text = "适应高度";
            适应高度ToolStripMenuItem.Click += 适应高度ToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 27);
            splitContainer1.Margin = new Padding(4, 4, 4, 4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(1275, 801);
            splitContainer1.SplitterDistance = 956;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 4, 4, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(956, 801);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Cursor = Cursors.Cross;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(4, 4, 4, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(952, 795);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnCopy);
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(pictureBox2);
            panel2.Controls.Add(label1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Margin = new Padding(4, 4, 4, 4);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(12, 14, 12, 14);
            panel2.Size = new Size(314, 801);
            panel2.TabIndex = 0;
            // 
            // btnCopy
            // 
            btnCopy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopy.Location = new Point(212, 749);
            btnCopy.Margin = new Padding(4, 4, 4, 4);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(88, 35);
            btnCopy.TabIndex = 2;
            btnCopy.Text = "复制";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Font = new Font("微软雅黑", 10F);
            textBox1.Location = new Point(12, 397);
            textBox1.Margin = new Padding(4, 4, 4, 4);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(290, 342);
            textBox1.TabIndex = 1;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label2.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            label2.Location = new Point(12, 354);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(291, 35);
            label2.TabIndex = 4;
            label2.Text = "识别结果";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Location = new Point(12, 61);
            pictureBox2.Margin = new Padding(4, 4, 4, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(291, 282);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("微软雅黑", 12F, FontStyle.Bold);
            label1.Location = new Point(12, 14);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(290, 42);
            label1.TabIndex = 0;
            label1.Text = "选区放大显示";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 828);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1275, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 17);
            toolStripStatusLabel1.Text = "就绪...";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1275, 850);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 4, 4, 4);
            Name = "Form1";
            Text = "OCR图片识别工具";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开图片ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 显示DICOM信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 操作ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 识别选中区域ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 识别全图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清除选区ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem 适应窗口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始大小ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 适应宽度ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 适应高度ToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}