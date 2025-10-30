using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (_iniConfig == null)
            {
                 var SerUrl = _iniConfig.Read("EquipmentUrl", "SerUrl", "");//请求数据Url
                TxtServicesUrl.Text = SerUrl;
                _iniConfig.ReadInt("EquipmentIsStart", "IsStart", 0);//是否初始化项目
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
            _iniConfig.Write("EquipmentUrl", "SerUrl", TxtServicesUrl.Text);
            _iniConfig.Write("EquipmentIsStart", "IsStart", "1");
            Close();
            
        }
    }
}
