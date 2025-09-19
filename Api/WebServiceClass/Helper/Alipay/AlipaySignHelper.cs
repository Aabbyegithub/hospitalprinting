using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebServiceClass.Helper.Alipay
{
    public static class AlipaySignHelper
    {
        /// <summary>
        /// 生成支付宝签名（RSA2）
        /// </summary>
        public static string GenerateSign(SortedDictionary<string, string> parameters, string privateKey)
        {
            var signBuilder = new StringBuilder();
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param.Value) && param.Key != "sign")
                {
                    signBuilder.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}&");
                }
            }
            string signContent = signBuilder.ToString().TrimEnd('&');

            using (var rsa = RSA.Create())
            {
                rsa.ImportFromPem(privateKey.ToCharArray());
                byte[] data = Encoding.UTF8.GetBytes(signContent);
                byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signature);
            }
        }

        /// <summary>
        /// 验证支付宝签名
        /// </summary>
        public static bool VerifySign(SortedDictionary<string, string> parameters, string sign, string publicKey)
        {
            var signBuilder = new StringBuilder();
            foreach (var param in parameters)
            {
                if (!string.IsNullOrEmpty(param.Value) && param.Key != "sign")
                {
                    signBuilder.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}&");
                }
            }
            string signContent = signBuilder.ToString().TrimEnd('&');

            using (var rsa = RSA.Create())
            {
                rsa.ImportFromPem(publicKey.ToCharArray());
                byte[] data = Encoding.UTF8.GetBytes(signContent);
                byte[] signature = Convert.FromBase64String(sign);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        /// <summary>
        /// 构建查询字符串
        /// </summary>
        public static string BuildQueryString(SortedDictionary<string, string> parameters, bool encode = false)
        {
            var sb = new StringBuilder();
            foreach (var kv in parameters)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    string value = encode ? Uri.EscapeDataString(kv.Value) : kv.Value;
                    sb.Append($"{kv.Key}={value}&");
                }
            }
            return sb.ToString().TrimEnd('&');
        }
    }
}
