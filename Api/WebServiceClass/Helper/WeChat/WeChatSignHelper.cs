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
        // 生成签名（MD5 方式，微信支付默认）
        public static string MakeSign(Dictionary<string, string> param, string key)
        {
            // 1. 按 key 升序排序
            var sortedParam = new SortedDictionary<string, string>(param);
            // 2. 拼接参数为 key1=value1&key2=value2...
            var sb = new StringBuilder();
            foreach (var pair in sortedParam)
            {
                if (string.IsNullOrEmpty(pair.Value) || pair.Key.Equals("sign", StringComparison.OrdinalIgnoreCase))
                    continue;
                sb.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }
            // 3. 拼接 API 密钥
            sb.Append("key=").Append(key);
            // 4. MD5 加密并转大写
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                var hash = md5.ComputeHash(bytes);
                var sign = new StringBuilder();
                foreach (var b in hash)
                    sign.Append(b.ToString("X2"));
                return sign.ToString();
            }
        }

        // 字典转 XML（微信支付请求格式）
        public static string ToXml(Dictionary<string, string> param)
        {
            var xml = new StringBuilder("<xml>");
            foreach (var pair in param)
            {
                xml.Append($"<{pair.Key}><![CDATA[{pair.Value}]]></{pair.Key}>");
            }
            xml.Append("</xml>");
            return xml.ToString();
        }

        // XML 转字典（微信支付响应解析）
        public static Dictionary<string, string> FromXml(string xml)
        {
            var dict = new Dictionary<string, string>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var root = doc.DocumentElement;
            if (root == null) return dict;
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                    dict[node.Name] = node.InnerText;
            }
            return dict;
        }
    }
}
