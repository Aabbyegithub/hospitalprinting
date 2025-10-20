using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WinSelfMachine.Common;

namespace Common
{
    public class ApiCommon
    {
        private readonly HttpClient _httpClient;
        private IniFileHelper _iniConfig;
        private string _configFilePath;
        private string _Url;
        private int _PrinterId;
        public ApiCommon() {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniConfig = new IniFileHelper(_configFilePath);
            _Url = _iniConfig.Read("EquipmentUrl", "SerUrl", "").TrimEnd('/');
            _PrinterId =  _iniConfig.ReadInt("Printer", "PrinterId", -1);
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// 获取所有打印机
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPrinter(string url)
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{url}/api/Equipment/GetDepartment");
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }


        /// <summary>
        /// 修改打印机状态
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<string> UpdatePrinterStatus(long PrinterId,int status)
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{_Url}/api/Equipment/UpdatePrintStaus?PrinterId={PrinterId}&status={status}");
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }


        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <returns></returns> 
        public async Task<string> GetPrinterConfig()
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{_Url}/api/Equipment/GetPrintConfig?PrinterId={_PrinterId}");
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }
    }
}
