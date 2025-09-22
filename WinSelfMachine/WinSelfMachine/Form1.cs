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
            PanQueay.Visible = true;
        }


        private void MainControl(bool Visible)
        {

        }

    }
}
