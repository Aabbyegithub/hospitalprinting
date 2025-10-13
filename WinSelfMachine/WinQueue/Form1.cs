using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinQueue
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 获取所有屏幕
            Screen[] screens = Screen.AllScreens;
            // 假设要显示在第二块屏幕（索引为 1，若有多块屏幕可根据实际情况调整）
            if (screens.Length > 1)
            {
                Screen targetScreen = screens[1];
                // 设置窗体的位置为目标屏幕的工作区域（避免被任务栏遮挡等情况）
                this.StartPosition = FormStartPosition.Manual;
                this.Location = targetScreen.WorkingArea.Location;
                // 也可以设置窗体大小为目标屏幕工作区域的大小
                this.Size = targetScreen.WorkingArea.Size;
            }
            // 如果只有一块屏幕，正常显示
        }
    }
}
