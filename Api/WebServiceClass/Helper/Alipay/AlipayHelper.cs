using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WebServiceClass.Helper.Alipay
{
    public static class AlipayPayHelper
    {
        private static readonly string AppId = "你的支付宝AppId";
        private static readonly string PrivateKey = "你的商户私钥（RSA2）";
        private static readonly string PublicKey = "支付宝公钥";
        private static readonly string GatewayUrl = "https://openapi.alipay.com/gateway.do";

        /// <summary>
        /// 支付宝手机网站支付（WAP支付）
        /// </summary>
        public static string WapPay(string orderNo, decimal amount, string subject, string returnUrl, string notifyUrl)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "app_id", AppId },
                { "method", "alipay.trade.wap.pay" },
                { "format", "JSON" },
                { "charset", "UTF-8" },
                { "sign_type", "RSA2" },
                { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "version", "1.0" },
                { "notify_url", notifyUrl },
                { "return_url", returnUrl },
                { "biz_content", JsonConvert.SerializeObject(new
                    {
                        out_trade_no = orderNo,
                        total_amount = amount.ToString("F2"),
                        subject = subject,
                        product_code = "QUICK_WAP_WAY"
                    })
                }
            };

            string sign = AlipaySignHelper.GenerateSign(parameters, PrivateKey);
            parameters.Add("sign", sign);
            return $"{GatewayUrl}?{AlipaySignHelper.BuildQueryString(parameters)}";
        }

        /// <summary>
        /// 支付宝APP支付
        /// </summary>
        public static string AppPay(string orderNo, decimal amount, string subject, string notifyUrl)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "app_id", AppId },
                { "method", "alipay.trade.app.pay" },
                { "format", "JSON" },
                { "charset", "UTF-8" },
                { "sign_type", "RSA2" },
                { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "version", "1.0" },
                { "notify_url", notifyUrl },
                { "biz_content", JsonConvert.SerializeObject(new
                    {
                        out_trade_no = orderNo,
                        total_amount = amount.ToString("F2"),
                        subject = subject,
                        product_code = "QUICK_MSECURITY_PAY"
                    })
                }
            };

            string sign = AlipaySignHelper.GenerateSign(parameters, PrivateKey);
            parameters.Add("sign", sign);

            // APP支付直接返回拼接的订单字符串
            return AlipaySignHelper.BuildQueryString(parameters, true);
        }

        /// <summary>
        /// 支付宝条码支付（当面付）
        /// </summary>
        public static string BarcodePay(string orderNo, decimal amount, string subject, string authCode)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "app_id", AppId },
                { "method", "alipay.trade.pay" },
                { "format", "JSON" },
                { "charset", "utf-8" },
                { "sign_type", "RSA2" },
                { "timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "version", "1.0" },
                { "biz_content", JsonConvert.SerializeObject(new
                    {
                        out_trade_no = orderNo,
                        scene = "bar_code",
                        auth_code = authCode,
                        subject = subject,
                        total_amount = amount.ToString("F2")
                    })
                }
            };

            string sign = AlipaySignHelper.GenerateSign(parameters, PrivateKey);
            parameters.Add("sign", sign);

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = client.PostAsync(GatewayUrl, content).Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
