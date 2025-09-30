namespace WinSelfMachine.Controls
{
    partial class RoundTextBox
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox tbContent;

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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbContent = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbContent
            // 
            this.tbContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbContent.Location = new System.Drawing.Point(8, 6);
            this.tbContent.Margin = new System.Windows.Forms.Padding(0);
            this.tbContent.Name = "tbContent";
            this.tbContent.Size = new System.Drawing.Size(184, 18);
            this.tbContent.TabIndex = 0;
            this.tbContent.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // RoundTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbContent);
            this.Name = "RoundTextBox";
            this.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.Size = new System.Drawing.Size(200, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}