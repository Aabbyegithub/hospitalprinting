using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace WebServiceClass.Helper.WeChat
{
    /// <summary>
    /// 微信支付核心逻辑
    /// </summary>
    public static class WeChatPayHelper
    {
        private static readonly string AppId = "你的AppId";
        private static readonly string MchId = "你的商户号";
        private static readonly string Key = "你的API密钥";
        private static readonly string NotifyUrl = "https://你的回调地址";

        #region 扫码支付（CodePay）
        public static bool CodePay(string orderNo, decimal amount, string authCode)
        {
            var url = "https://api.mch.weixin.qq.com/pay/micropay";

            var param = new Dictionary<string, string>
            {
                { "appid", AppId },
                { "mch_id", MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", "商品描述" },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "auth_code", authCode }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml);
            var result = WeChatSignHelper.FromXml(resultXml);

            return result.ContainsKey("result_code") && result["result_code"] == "SUCCESS";
        }
        #endregion

        #region Native支付（扫码支付-生成二维码链接）
        public static string NativePay(string orderNo, decimal amount, string productDesc)
        {
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var param = new Dictionary<string, string>
            {
                { "appid", AppId },
                { "mch_id", MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", productDesc },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "notify_url", NotifyUrl },
                { "trade_type", "NATIVE" }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml);
            var result = WeChatSignHelper.FromXml(resultXml);

            return result.ContainsKey("code_url") ? result["code_url"] : string.Empty;
        }
        #endregion

        #region JSAPI支付（公众号、小程序）
        public static Dictionary<string, string> JsApiPay(string orderNo, decimal amount, string openId, string productDesc)
        {
            var url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            var param = new Dictionary<string, string>
            {
                { "appid", AppId },
                { "mch_id", MchId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "body", productDesc },
                { "out_trade_no", orderNo },
                { "total_fee", ((int)(amount * 100)).ToString() },
                { "spbill_create_ip", "127.0.0.1" },
                { "notify_url", NotifyUrl },
                { "trade_type", "JSAPI" },
                { "openid", openId }
            };
            param["sign"] = WeChatSignHelper.MakeSign(param, Key);

            var xml = WeChatSignHelper.ToXml(param);
            var resultXml = HttpPost(url, xml);
            var result = WeChatSignHelper.FromXml(resultXml);

            if (!result.ContainsKey("prepay_id"))
                return null;

            // 二次签名返回前端
            var jsapiParam = new Dictionary<string, string>
            {
                { "appId", AppId },
                { "timeStamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
                { "nonceStr", Guid.NewGuid().ToString("N") },
                { "package", "prepay_id=" + result["prepay_id"] },
                { "signType", "MD5" }
            };
            jsapiParam["paySign"] = WeChatSignHelper.MakeSign(jsapiParam, Key);

            return jsapiParam;
        }
        #endregion

        #region 通用HTTP请求
        private static string HttpPost(string url, string data)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(data, Encoding.UTF8, "application/xml");
                var response = client.PostAsync(url, content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }


        public static async Task<string> CallCustomerUnifiedRechargeApi(string url, string attach, string description, string outTradeNo, int total, string code, string storeNum, long userId)
        {
            var payload = new
            {
                temp='1',
                attach,
                description,
                out_trade_no = outTradeNo,
                total,
                code,
                storeNum,
                userId
            };
            var json = JsonConvert.SerializeObject(payload);
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
