using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WinSelfMachine.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;
using ModelClassLibrary.Model.HolModel;

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
        /// 获取所有激光相机
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPrinter()
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{_Url}/api/Equipment/GetLaserCamera");
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
        public async Task<string> UpdatePrinterStatus(string url, long PrinterId,int status)
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{url}/api/Equipment/UpdatePrintStaus?PrinterId={PrinterId}&status={status}");
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


        public async Task<string> SavePrinterConfig(int type,int action,  HolPrinterConfig holPrinterConfigs)
        {
            var res = "";
            var json = JsonConvert.SerializeObject(holPrinterConfigs ?? new HolPrinterConfig());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var Response = await _httpClient.PostAsync($"{_Url}/api/Equipment/SavePrintConfig?PrinterId={_PrinterId}&Type={type}&Action={action}", content);
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }

        /// <summary>
        /// 根据检查号获取检查数据
        /// </summary>
        /// <param name="examNo">检查号</param>
        /// <returns></returns>
        public async Task<string> GetExaminationByNo(string examNo)
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{_Url}/api/Equipment/GetByExamNo?examNo={examNo}");
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }

        public async Task<string> GetExaminationAllUser()
        {
            var res = "";
            var Response = await _httpClient.GetAsync($"{_Url}/api/Equipment/GetAllUser");
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }

        /// <summary>
        /// 保存打印记录
        /// </summary>
        /// <param name="printRecord">打印记录</param>
        /// <returns></returns>
        public async Task<string> SavePrintRecord(PrintRecordModel printRecord)
        {
            var res = "";
            var json = JsonConvert.SerializeObject(printRecord);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var Response = await _httpClient.PostAsync($"{_Url}/api/Equipment/SavePrintRecord", content);
            if (Response.IsSuccessStatusCode)
            {
                res = await Response.Content.ReadAsStringAsync();
            }
            return res;
        }

        /// <summary>
        /// 生成报告Pdf（返回PDF字节流）。按接口要求生成 time、nonce_str、sign。
        /// </summary>
        /// <param name="examType">检查类型，如：头颅 CT 平扫+胸部 CT 平扫</param>
        /// <param name="date">检查日期，例：2025-10-01</param>
        /// <param name="filename">文件唯一标识</param>
        /// <param name="messageText">读解内容文字</param>
        /// <param name="secretKey">签名密钥 secret_key</param>
        /// <returns>application/pdf 字节流；失败时抛出异常或返回 null</returns>
        public async Task<byte[]> GenerateReportPdf(string examType, string date, string filename, string messageText, string secretKey)
        {
            // 1) 公共参数
            string time = GenerateTimestampSeconds();
            string nonceStr = GenerateNonce(16, 32);
            string sign = ComputeMd5Lower($"nonce_str={nonceStr}&time={time}" + secretKey);

            // 2) 业务参数 content
            var payload = new
            {
                content = new
                {
                    exam_type = examType,
                    date = date,
                    filename = filename,
                    message_text = messageText
                },
                nonce_str = nonceStr,
                sign = sign,
                time = time
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_Url}/ai/responses/report_review", content);
            if (!response.IsSuccessStatusCode)
            {
                // 尝试读取错误信息
                var err = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI报告生成接口请求失败：{response.StatusCode}\n{err}");
            }

            // 判定返回的Content-Type是否为PDF
            var mediaType = response.Content.Headers.ContentType?.MediaType?.ToLower();
            if (mediaType == "application/pdf")
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            // 非PDF则当成JSON错误处理
            var text = await response.Content.ReadAsStringAsync();
            // 兼容失败响应示例
            if (!string.IsNullOrEmpty(text))
            {
                throw new Exception($"AI报告生成返回非PDF：{text}");
            }
            return null;
        }

        /// <summary>
        /// 生成 MD5 小写签名
        /// </summary>
        private static string ComputeMd5Lower(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = md5.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 生成随机字符串（字母数字）长度在[minLen, maxLen]
        /// </summary>
        private static string GenerateNonce(int minLen, int maxLen)
        {
            if (minLen < 1) minLen = 16;
            if (maxLen < minLen) maxLen = minLen;
            var length = new Random().Next(minLen, maxLen + 1);
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rnd = new Random();
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[rnd.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成时间戳（精确到秒，格式：yyyyMMddHHmmss）
        /// </summary>
        private static string GenerateTimestampSeconds()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

    }
}
