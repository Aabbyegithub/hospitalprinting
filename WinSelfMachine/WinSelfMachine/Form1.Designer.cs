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
            this.modeTimeBar1 = new WinSelfMachine.Controls.ModeTimeBar();
            this.SuspendLayout();
            // 
            // modeTimeBar1
            // 
            this.modeTimeBar1.BackColor = System.Drawing.Color.RosyBrown;
            this.modeTimeBar1.HospitalText = "苏州XX医院";
            this.modeTimeBar1.Icon = null;
            this.modeTimeBar1.IsOn = true;
            this.modeTimeBar1.Location = new System.Drawing.Point(138, 178);
            this.modeTimeBar1.ModeText = "关爱模式";
            this.modeTimeBar1.Name = "modeTimeBar1";
            this.modeTimeBar1.Size = new System.Drawing.Size(356, 46);
            this.modeTimeBar1.Spacing = 10;
            this.modeTimeBar1.TabIndex = 0;
            this.modeTimeBar1.TimeText = "17:27";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(752, 1061);
            this.Controls.Add(this.modeTimeBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ModeTimeBar modeTimeBar1;
    }
}

