using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace WebServiceClass.Helper.WeChat
{
    /// <summary>
    /// 微信支付签名与XML工具
    /// </summary>
    public static class WeChatSignHelper
    {
        public static string MakeSign(Dictionary<string, string> dict, string key)
        {
            var sorted = dict.Where(kv => !string.IsNullOrEmpty(kv.Value) && kv.Key != "sign")
                             .OrderBy(kv => kv.Key)
                             .Select(kv => $"{kv.Key}={kv.Value}");
            var stringA = string.Join("&", sorted);
            var stringSignTemp = $"{stringA}&key={key}";
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(stringSignTemp));
                return string.Concat(bytes.Select(b => b.ToString("X2")));
            }
        }

        public static string ToXml(Dictionary<string, string> dict)
        {
            var sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (var kv in dict)
            {
                sb.Append($"<{kv.Key}><![CDATA[{kv.Value}]]></{kv.Key}>");
            }
            sb.Append("</xml>");
            return sb.ToString();
        }

        public static Dictionary<string, string> FromXml(string xml)
        {
            var dict = new Dictionary<string, string>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                dict[node.Name] = node.InnerText;
            }
            return dict;
        }
    }
}
