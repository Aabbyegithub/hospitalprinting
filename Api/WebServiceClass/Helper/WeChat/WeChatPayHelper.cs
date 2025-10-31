using Microsoft.Extensions.Options;
using ModelClassLibrary.Model.Dto.PayDto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WebServiceClass.Helper.WeChat
{
    public class WeChatPayHelper
    {
        private readonly WeChatPayConfig _config;

        // 通过依赖注入获取配置
        public WeChatPayHelper(IOptions<WeChatPayConfig> configOptions)
        {
            _config = configOptions.Value;
        }

        #region 扫码支付（CodePay）
        public bool CodePay(string orderNo, decimal amount, string authCode)
        {
            var url = "https://api.mch.weixin.qq.com/pay/micropay";

            var param = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", "商品描述" },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "auth_code", authCode }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, _config.Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml).Result;
            var result = WeChatSignHelper.FromXml(resultXml);

            return result.ContainsKey("result_code") && result["result_code"] == "SUCCESS";
        }
        #endregion

        #region Native支付（扫码支付-生成二维码链接）
        public string NativePay(string orderNo, decimal amount, string productDesc)
        {
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var param = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", productDesc },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "notify_url", _config.NotifyUrl },
                { "trade_type", "NATIVE" }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, _config.Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml).Result;
            var result = WeChatSignHelper.FromXml(resultXml);

            return result.ContainsKey("code_url") ? result["code_url"] : string.Empty;
        }
        #endregion

        #region JSAPI支付（公众号、小程序）
        public Dictionary<string, string> JsApiPay(string orderNo, decimal amount, string openId, string productDesc)
        {
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var param = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", productDesc },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "notify_url", _config.NotifyUrl },
                { "trade_type", "JSAPI" },
                { "openid", openId }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, _config.Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml).Result;
            var result = WeChatSignHelper.FromXml(resultXml);

            if (!result.ContainsKey("prepay_id"))
                return null;

            // 二次签名返回前端
            var jsapiParam = new Dictionary<string, string>
            {
                { "appId", _config.AppId },
                { "timeStamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                { "nonceStr", Guid.NewGuid().ToString("N") },
                { "package", "prepay_id=" + result["prepay_id"] },
                { "signType", "MD5" }
            };
            jsapiParam["paySign"] = WeChatSignHelper.MakeSign(jsapiParam, _config.Key);

            return jsapiParam;
        }
        #endregion

        #region 新增：退款功能
        /// <summary>
        /// 微信支付退款
        /// </summary>
        /// <param name="orderNo">商户订单号（原支付订单号）</param>
        /// <param name="refundNo">退款单号（需唯一）</param>
        /// <param name="totalAmount">订单总金额（元）</param>
        /// <param name="refundAmount">退款金额（元）</param>
        /// <param name="refundDesc">退款原因（可选）</param>
        /// <returns>退款结果（包含微信退款单号等信息）</returns>
        public Dictionary<string, string> Refund(
            string orderNo, 
            string refundNo, 
            decimal totalAmount, 
            decimal refundAmount, 
            string refundDesc = "")
        {
            var url = "https://api.mch.weixin.qq.com/secapi/pay/refund";

            // 1. 准备请求参数
            var param = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "out_trade_no", orderNo }, // 原支付订单号
                { "out_refund_no", refundNo }, // 退款单号（需唯一）
                { "total_fee", ((int)(totalAmount * 100)).ToString() }, // 订单总金额（分）
                { "refund_fee", ((int)(refundAmount * 100)).ToString() }, // 退款金额（分）
                { "refund_desc", refundDesc } // 退款原因（可选）
            };

            // 2. 生成签名
            param["sign"] = WeChatSignHelper.MakeSign(param, _config.Key);

            // 3. 转换为XML格式
            var xml = WeChatSignHelper.ToXml(param);

            // 4. 发送HTTPS请求（需携带证书）
            var resultXml = HttpPostWithCert(url, xml).Result;

            // 5. 解析返回结果
            var result = WeChatSignHelper.FromXml(resultXml);

            // 6. 验证签名（确保返回结果来自微信）
            if (!VerifySign(result))
            {
                result["error"] = "签名验证失败";
                return result;
            }

            return result;
        }

        /// <summary>
        /// 验证微信返回结果的签名
        /// </summary>
        public bool VerifySign(Dictionary<string, string> result)
        {
            if (!result.ContainsKey("sign"))
                return false;

            var sign = result["sign"];
            result.Remove("sign"); // 移除原有签名
            var newSign = WeChatSignHelper.MakeSign(result, _config.Key); // 重新生成签名
            return sign == newSign;
        }

        /// <summary>
        /// 带证书的HTTPS POST请求（退款接口必须）
        /// </summary>
        private async Task<string> HttpPostWithCert(string url, string data)
        {
            // 创建带证书的HttpClientHandler
            using (var handler = new HttpClientHandler())
            {
                // 加载微信支付证书（.p12文件）
                // 注意：_config.CertPath 是配置文件中指定的相对路径，需确保程序能正确解析（可结合 IWebHostEnvironment.ContentRootPath 获取绝对路径）
                string absoluteCertPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _config.CertPath);
                handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    absoluteCertPath, 
                    _config.MchId, // 证书密码默认为商户号
                    System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet
                ));

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // 设置超时时间
                    var content = new StringContent(data, Encoding.UTF8, "application/xml");
                    var response = await client.PostAsync(url, content);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        #endregion

        #region 通用HTTP请求（不带证书）
        private async Task<string> HttpPost(string url, string data)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> CallCustomerUnifiedRechargeApi(string url, string attach, string description, string outTradeNo, int total, string code, string storeNum, long userId)
        {
            var payload = new
            {
                temp = '1',
                attach,
                description,
                out_trade_no = outTradeNo,
                total,
                code,
                storeNum,
                userId
            };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
        #endregion
    }
}