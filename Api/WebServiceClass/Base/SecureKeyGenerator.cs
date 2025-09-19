using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.Base
{
    public class SecureKeyGenerator
    {
        public static string GenerateSecureKey(int keyLength = 32)
        {
            // 定义可能的字符集合
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}|;:'\",.<>/?";

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[keyLength];
                rng.GetBytes(data); // 生成随机字节数组

                StringBuilder result = new StringBuilder(keyLength);
                foreach (byte b in data)
                {
                    // 将字节转换为索引并映射到字符集
                    result.Append(chars[b % (chars.Length)]);
                }

                return result.ToString();
            }
        }
    }
}
