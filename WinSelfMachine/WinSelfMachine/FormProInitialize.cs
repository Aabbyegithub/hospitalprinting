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
        public FormProInitialize()
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniConfig = new IniFileHelper(_configFilePath);
            InitializeComponent();
            _apiCommon = new ApiCommon();
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
                if (_IsDisplay ==1)
                {
                    RedYes.Checked = true;
                }
                else
                {
                    RedNo.Checked = true;
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

            _iniConfig.Write("EquipmentUrl", "SerUrl", TxtServicesUrl.Text);
            _iniConfig.Write("EquipmentIsStart", "IsStart", "1");
            _iniConfig.Write("Printer", "PrinterId", CbmDep.SelectedValue.ToString());
            _iniConfig.Write("Printer", "PrinterName",CbmDep.SelectedText);
            _iniConfig.Write("IsDisplay", "Display",_IsDisplay.ToString());
            var Url = TxtServicesUrl.Text.TrimEnd('/');
            await _apiCommon.UpdatePrinterStatus(Url, CbmDep.SelectedIndex,1);
            Close();
            
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
    }
}
