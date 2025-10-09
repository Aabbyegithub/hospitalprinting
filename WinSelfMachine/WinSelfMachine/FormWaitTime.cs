using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSelfMachine
{
    public partial class FormWaitTime : Form
    {
        public FormWaitTime()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReturn_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 点击单元格触发编辑和删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
               return;
            }
            BtnDelete.Enabled = true;
            BtnEdit.Enabled = true;
            CbmFilmSize.SelectedItem = Table.Rows[e.RowIndex].Cells[0].Value.ToString();
            TxtWaitTime.Text = Table.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        /// <summary>
        /// 添加胶片等待时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}
