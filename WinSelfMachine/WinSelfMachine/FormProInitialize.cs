using Common;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using WinSelfMachine.Common;
using static Common.Response;

namespace WinSelfMachine
{
    public partial class FormProInitialize : Form
    {
        private IniFileHelper _iniConfig;
        private string _configFilePath;
        private readonly ApiCommon _apiCommon;
        private int _IsDisplay = 1;
        private int _IsShow = 1;
        public FormProInitialize()
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniConfig = new IniFileHelper(_configFilePath);
            InitializeComponent();
            _apiCommon = new ApiCommon();
            // 将单选按钮设为手动互斥，避免同容器下相互影响
            RedYes.AutoCheck = false;
            RedNo.AutoCheck = false;
            RedYes2.AutoCheck = false;
            RedNo2.AutoCheck = false;

            // 绑定点击事件以实现分组互斥
            RedYes.Click += RedYes_Click;
            RedNo.Click += RedNo_Click;
            RedYes2.Click += RedYes2_Click;
            RedNo2.Click += RedNo2_Click;
            LoadConfig();
        }


        private void LoadConfig()
        {
            if (_iniConfig != null)
            {
                 var SerUrl = _iniConfig.Read("EquipmentUrl", "SerUrl", "");//请求数据Url
                TxtServicesUrl.Text = SerUrl;
                _iniConfig.ReadInt("EquipmentIsStart", "IsStart", 0);//是否初始化项目
                var PrinterId =  _iniConfig.ReadInt("Printer", "PrinterId", -1);//打印机配置
                _iniConfig.Read("Printer", "PrinterName", "");//打印机名称
               _IsDisplay = _iniConfig.ReadInt("IsDisplay", "Display",1);
               _IsShow = _iniConfig.ReadInt("IsShow", "IsShow",1);
                if (_IsDisplay ==1)
                {
                    RedYes.Checked = true;
                }
                else
                {
                    RedNo.Checked = true;
                }
                if (_IsShow == 1)
                {
                    RedYes2.Checked = true;
                }
                else
                {
                    RedNo2.Checked = true;
                }
                CbmDep.SelectedValue = PrinterId;
            }
        }
        /// <summary>
        /// 属性值变化时，实时刷新设备配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TxtServicesUrl_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var Url = TxtServicesUrl.Text.TrimEnd('/');
                var Response = await _apiCommon.GetPrinter(Url);
                if (!string.IsNullOrEmpty(Response))
                {
                    var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolPrinter>>>(Response);
                    CbmDep.DataSource = responseData.Response;
                    CbmDep.DisplayMember = "name";
                    CbmDep.ValueMember = "id";
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtServicesUrl.Text))
            {
                MessageBox.Show("请填写数据请求连接Url","警告",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(CbmDep.Text))
            {
                MessageBox.Show("请选择打印机配置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 写入配置
                _iniConfig.Write("EquipmentUrl", "SerUrl", TxtServicesUrl.Text.Trim());
                _iniConfig.Write("EquipmentIsStart", "IsStart", "1");
                _iniConfig.Write("Printer", "PrinterId", CbmDep.SelectedValue?.ToString() ?? "-1");
                _iniConfig.Write("Printer", "PrinterName", CbmDep.Text);
                _iniConfig.Write("IsDisplay", "Display", _IsDisplay.ToString());
                _iniConfig.Write("IsShow", "IsShow", _IsShow.ToString());

                // 同步设备状态
                var url = TxtServicesUrl.Text.TrimEnd('/');
                int printerId;
                int.TryParse(CbmDep.SelectedValue?.ToString(), out printerId);
                if (!string.IsNullOrEmpty(url) && printerId > -1)
                {
                    await _apiCommon.UpdatePrinterStatus(url, printerId, 1);
                }

                // 提示并重启以加载最新配置
                MessageBox.Show("设置已保存，程序将重启以加载最新配置。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RedYes_CheckedChanged(object sender, EventArgs e)
        {
            if (RedYes.Checked)
            {
                RedNo.Checked = false;
                _IsDisplay = 1;

            }
        }

        private void RedNo_CheckedChanged(object sender, EventArgs e)
        {
            if (RedNo.Checked)
            {
                RedYes.Checked = false;_IsDisplay = 0;
            }
        }

        private void RedYes2_CheckedChanged(object sender, EventArgs e)
        {
            if (RedYes2.Checked)
            {
                RedNo2.Checked = false;
                _IsShow = 1;
            }
        }

        private void RedNo2_CheckedChanged(object sender, EventArgs e)
        {
            if (RedNo2.Checked)
            {
                RedYes2.Checked = false;
                _IsShow = 0;
            }
        }

        // 分组一：显示设置
        private void RedYes_Click(object sender, EventArgs e)
        {
            RedYes.Checked = true;
            RedNo.Checked = false;
            _IsDisplay = 1;
        }

        private void RedNo_Click(object sender, EventArgs e)
        {
            RedNo.Checked = true;
            RedYes.Checked = false;
            _IsDisplay = 0;
        }

        // 分组二：是否显示某项
        private void RedYes2_Click(object sender, EventArgs e)
        {
            RedYes2.Checked = true;
            RedNo2.Checked = false;
            _IsShow = 1;
        }

        private void RedNo2_Click(object sender, EventArgs e)
        {
            RedNo2.Checked = true;
            RedYes2.Checked = false;
            _IsShow = 0;
        }
    }
}
