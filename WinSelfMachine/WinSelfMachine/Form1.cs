using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine.Controls;

namespace WinSelfMachine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Resize += Form1_Resize;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modeTimeBar1_LeftButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, EventArgs e)
        {
           
        }


        private void MainControl(bool Visible)
        {

        }

        /// <summary>
        /// 窗体大小变化时调整控件布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            // 获取当前窗体大小
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // 计算缩放比例（基于原始设计尺寸 1595x1017）
            float scaleX = (float)formWidth / 1595f;
            float scaleY = (float)formHeight / 1017f;
            
            // 使用平均缩放比例，确保字体大小变化更明显
            float averageScale = (scaleX + scaleY) / 2f;
            
            // 调试输出
            System.Diagnostics.Debug.WriteLine($"窗体大小: {formWidth}x{formHeight}, 缩放比例: X={scaleX:F2}, Y={scaleY:F2}, 平均={averageScale:F2}");

            // 调整3D按钮的位置和大小
            if (_3DButton1 != null)
            {
                _3DButton1.Location = new Point((int)(213 * scaleX), (int)(523 * scaleY));
                _3DButton1.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize = Math.Max(8f, 15f * averageScale);
                _3DButton1.UpdateFontSize(newFontSize); // 使用强制更新方法
                System.Diagnostics.Debug.WriteLine($"按钮1字体大小: {newFontSize:F1}");
            }

            if (_3DButton2 != null)
            {
                _3DButton2.Location = new Point((int)(642 * scaleX), (int)(523 * scaleY));
                _3DButton2.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize2 = Math.Max(8f, 15f * averageScale);
                _3DButton2.UpdateFontSize(newFontSize2); // 使用强制更新方法
            }

            if (_3DButton3 != null)
            {
                _3DButton3.Location = new Point((int)(1071 * scaleX), (int)(523 * scaleY));
                _3DButton3.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize3 = Math.Max(8f, 15f * averageScale);
                _3DButton3.UpdateFontSize(newFontSize3); // 使用强制更新方法
            }

            // 调整标签位置和字体大小
            if (TxtQuery != null)
            {
                TxtQuery.Location = new Point((int)(240 * scaleX), (int)(383 * scaleY));
                float labelFontSize = Math.Max(8f, 15f * averageScale);
                TxtQuery.Font = new Font(TxtQuery.Font.FontFamily, labelFontSize, TxtQuery.Font.Style);
            }

            if (label2 != null)
            {
                label2.Location = new Point(0, (int)(203 * scaleY));
                label2.Size = new Size(formWidth, (int)(84 * scaleY));
                float titleFontSize = Math.Max(12f, 36f * averageScale);
                label2.Font = new Font(label2.Font.FontFamily, titleFontSize, label2.Font.Style);
            }

            if (TxtTitle != null)
            {
                TxtTitle.Location = new Point((int)(132 * scaleX), (int)(52 * scaleY));
                float titleFontSize = Math.Max(10f, 21.75f * averageScale);
                TxtTitle.Font = new Font(TxtTitle.Font.FontFamily, titleFontSize, TxtTitle.Font.Style);
            }

            // 调整按钮位置和字体大小
            if (BtnReportQuery != null)
            {
                BtnReportQuery.Location = new Point((int)(1142 * scaleX), (int)(369 * scaleY));
                BtnReportQuery.Size = new Size((int)(187 * scaleX), (int)(63 * scaleY));
                float buttonFontSize = Math.Max(8f, 12f * averageScale);
                BtnReportQuery.Font = new Font(BtnReportQuery.Font.FontFamily, buttonFontSize, BtnReportQuery.Font.Style);
            }

            // 调整图片框位置
            if (pictureBox1 != null)
            {
                pictureBox1.Location = new Point((int)(22 * scaleX), (int)(44 * scaleY));
                pictureBox1.Size = new Size((int)(66 * scaleX), (int)(66 * scaleY));
            }

            // 调整TxtBnr控件位置、大小和字体
            if (TxtBnr != null)
            {
                TxtBnr.Location = new Point((int)(348 * scaleX), (int)(369 * scaleY));
                TxtBnr.Size = new Size((int)(761 * scaleX), (int)(63 * scaleY));
                float txtBnrFontSize = Math.Max(8f, 15f * averageScale);
                TxtBnr.UpdateFontSize(txtBnrFontSize);
                System.Diagnostics.Debug.WriteLine($"TxtBnr字体大小: {txtBnrFontSize:F1}");
            }
        }

    }
}
