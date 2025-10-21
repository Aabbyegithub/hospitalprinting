using Common;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine.Common;
using static Common.Response;
using static WinSelfMachine.Model.HolModel;

namespace WinSelfMachine
{
    public partial class FormWaitTime : Form
    {
        private readonly ApiCommon _apiCommon;
        private List<WaitTimeList> _WaitTimeList;

        public FormWaitTime()
        {
            InitializeComponent();
            CbmFilmSize.Items.AddRange(CommonList.PrintFilmSizes().ToArray());
            CbmFilmSize.SelectedIndex = 0;
            _apiCommon = new ApiCommon();
            FormWaitTime_Load();
        }

        private async Task FormWaitTime_Load()
        {
            try
            {
                _WaitTimeList.Clear();
                var response =await _apiCommon.GetPrinterConfig();
                if (!string.IsNullOrEmpty(response))
                {
                    var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolPrinterConfig>>>(response);
                    _WaitTimeList = responseData.Response.Where(a=>a.print_time_seconds !=0)
                        .Select(b=>new WaitTimeList 
                        { 
                            Id = b.id,
                            Size = b.film_size,
                            WaitTime = b.print_time_seconds
                        }).ToList();
                    if (_WaitTimeList != null && _WaitTimeList.Count != 0)
                    {
                        Table.Rows.Clear();
                        foreach (var item in _WaitTimeList)
                        {
                            Table.Rows.Add(item.Size, item.WaitTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
        private async Task BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (CbmFilmSize.SelectedItem == null)
                {
                    MessageBox.Show("请选择胶片尺寸", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(TxtWaitTime.Text, out int waitSeconds) || waitSeconds < 0)
                {
                    MessageBox.Show("请输入有效的等待时间(秒)", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var size = CbmFilmSize.SelectedItem.ToString();

                if (_WaitTimeList == null)
                {
                    _WaitTimeList = new List<WaitTimeList>();
                }

                var existing = _WaitTimeList.FirstOrDefault(x => x.Size == size);
                if (existing != null)
                {
                    MessageBox.Show("该尺寸已存在，请使用修改功能", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var printerConfig = new HolPrinterConfig
                {
                    printer_id = 0,
                    film_size = size,
                    print_time_seconds = waitSeconds
                };

                await _apiCommon.SavePrinterConfig(1,1,printerConfig);

                await FormWaitTime_Load();

                BtnDelete.Enabled = false;
                BtnEdit.Enabled = false;
                TxtWaitTime.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (Table.CurrentRow == null || Table.CurrentRow.Index < 0)
            {
                return;
            }

            var idx = Table.CurrentRow.Index;
            var size = Table.Rows[idx].Cells[0].Value?.ToString();

            if (string.IsNullOrEmpty(size))
            {
                return;
            }

            var confirm = MessageBox.Show($"确定删除尺寸 {size} 的等待时间吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            var item = _WaitTimeList.FirstOrDefault(x => x.Size == size);

            var printerConfig = new HolPrinterConfig
            {
                printer_id = item?.Id ?? 0,
                film_size = size,
                print_time_seconds = item?.WaitTime ?? 0
            };
            await _apiCommon.SavePrinterConfig(1, 3, printerConfig);

            await FormWaitTime_Load();

            BtnDelete.Enabled = false;
            BtnEdit.Enabled = false;
            TxtWaitTime.Clear();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (Table.CurrentRow == null || Table.CurrentRow.Index < 0)
            {
                return;
            }

            if (CbmFilmSize.SelectedItem == null)
            {
                MessageBox.Show("请选择胶片尺寸", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(TxtWaitTime.Text, out int waitSeconds) || waitSeconds < 0)
            {
                MessageBox.Show("请输入有效的等待时间(秒)", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idx = Table.CurrentRow.Index;
            var size = CbmFilmSize.SelectedItem.ToString();
            var item = _WaitTimeList.FirstOrDefault(x => x.Size == size);
            var printerConfig = new HolPrinterConfig
            {
                printer_id = item?.Id ?? 0,
                film_size = size,
                print_time_seconds = TxtWaitTime.Text.ObjToInt()
            };
            await _apiCommon.SavePrinterConfig(1, 2, printerConfig);

            BtnDelete.Enabled = false;
            BtnEdit.Enabled = false;
            TxtWaitTime.Clear();
        }
    }
}
