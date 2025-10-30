using Common;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
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
    public partial class FormAvailableFilm : Form
    {
        private readonly ApiCommon _apiCommon;
        /// <summary>
        /// 胶片管理列表
        /// </summary>
        private List<FilmList> FilmSizeList = new List<FilmList>();
        public FormAvailableFilm()
        {
            InitializeComponent();
            CbmFilmSize.Items.AddRange(CommonList.PrintFilmSizes().ToArray());
            CbmFilmSize.SelectedIndex = 0;
            _apiCommon = new ApiCommon();
            FormAvailableFilm_Load();
        }

        private async Task FormAvailableFilm_Load()
        {
            try
            {
                var response = await _apiCommon.GetPrinterConfig();
                if (!string.IsNullOrEmpty(response))
                {
                    var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolPrinterConfig>>>(response);
                    FilmSizeList = responseData.Response.Where(a => a.available_count != 0)
                        .Select(b => new FilmList
                        {
                            Id = b.id,
                            Size = b.film_size,
                            PageSum = b.available_count
                        }).ToList();
                    if (FilmSizeList != null && FilmSizeList.Count != 0)
                    {
                        Table.Rows.Clear();
                        foreach (var item in FilmSizeList)
                        {
                            Table.Rows.Add(item.Size, item.PageSum);
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

            var confirm = MessageBox.Show($"确定删除尺寸 {size} 的库存吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            var item = FilmSizeList.FirstOrDefault(x => x.Size == size);
            if (item != null)
            {
                FilmSizeList.Remove(item);
            }
            var printerConfig =new HolPrinterConfig
            {
                printer_id = item?.Id ?? 0,
                film_size = size,
                available_count = item?.PageSum ?? 0
            };
            await _apiCommon.SavePrinterConfig(2, 3, printerConfig);

            Table.Rows.RemoveAt(idx);
            BtnDelete.Enabled = false;
            BtnEdit.Enabled = false;
            TxtSum.Clear();
        }

        /// <summary>
        /// 编辑
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

            if (!int.TryParse(TxtSum.Text, out int count) || count < 0)
            {
                MessageBox.Show("请输入有效的可用数量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idx = Table.CurrentRow.Index;
            var size = CbmFilmSize.SelectedItem.ToString();

            var item = FilmSizeList.FirstOrDefault(x => x.Size == size);
            var printerConfig = new HolPrinterConfig
            {
                printer_id = item?.Id ?? 0,
                film_size = size,
                available_count =int.Parse(TxtSum.Text)
            };
            await _apiCommon.SavePrinterConfig(2, 2, printerConfig);

            BtnDelete.Enabled = false;
            BtnEdit.Enabled = false;
            TxtSum.Clear();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (CbmFilmSize.SelectedItem == null)
                {
                    MessageBox.Show("请选择胶片尺寸", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(TxtSum.Text, out int count) || count < 0)
                {
                    MessageBox.Show("请输入有效的可用数量", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var size = CbmFilmSize.SelectedItem.ToString();
                var existing = FilmSizeList.FirstOrDefault(x => x.Size == size);
                if (existing != null)
                {
                    MessageBox.Show("该尺寸已存在，请使用修改功能", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var printerConfig =new HolPrinterConfig
                {
                    printer_id = 0,
                    film_size = size,
                    available_count = int.Parse(TxtSum.Text)
                };
                await _apiCommon.SavePrinterConfig(2, 1, printerConfig);

                BtnDelete.Enabled = false;
                BtnEdit.Enabled = false;
                TxtSum.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            TxtSum.Text = Table.Rows[e.RowIndex].Cells[1].Value.ToString();
        }
    }
}
