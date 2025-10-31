using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Common
{
    /// <summary>
    /// License 数据结构
    /// </summary>
    public class LicenseInfo
    {
        /// <summary>
        /// 机器码（MD5哈希后的唯一标识）
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 授权开始时间（精确到秒）
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 授权结束时间（精确到秒）
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 授权类型（如：试用版、正式版等）
        /// </summary>
        public string LicenseType { get; set; }

        /// <summary>
        /// 额外信息（可选）
        /// </summary>
        public string ExtraInfo { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime GenerateTime { get; set; }
    }

    /// <summary>
    /// License 验证结果
    /// </summary>
    public class LicenseValidateResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// License 信息
        /// </summary>
        public LicenseInfo LicenseInfo { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        public int RemainingDays { get; set; }

        /// <summary>
        /// 剩余小时
        /// </summary>
        public int RemainingHours { get; set; }

        /// <summary>
        /// 剩余分钟
        /// </summary>
        public int RemainingMinutes { get; set; }

        /// <summary>
        /// 剩余秒数
        /// </summary>
        public int RemainingSeconds { get; set; }
    }

    /// <summary>
    /// License 加密解密帮助类
    /// 使用机器码和时间进行管控，时间精确到秒
    /// </summary>
    public static class LicenseHelper
    {
        // AES 加密密钥和初始化向量（建议在生产环境中使用配置文件或安全存储）
        private static readonly string DefaultKey = "HospitalPrintSystemLicenseKey123456"; // 32字符
        private static readonly string DefaultIV = "1234567890123456"; // 16字符

        /// <summary>
        /// 获取机器唯一标识码
        /// 通过 CPU ID、主板序列号、硬盘序列号等生成唯一标识
        /// </summary>
        /// <returns>机器码（MD5哈希后的字符串）</returns>
        public static string GetMachineCode()
        {
            try
            {
                string machineCode = string.Empty;

                // 获取 CPU ID
                string cpuId = GetCPUSerialNumber();
                
                // 获取主板序列号
                string motherboardId = GetMotherboardSerialNumber();
                
                // 获取硬盘序列号
                string diskId = GetDiskSerialNumber();

                // 获取 MAC 地址
                string macAddress = GetMACAddress();

                // 组合所有硬件信息
                string hardwareInfo = $"{cpuId}|{motherboardId}|{diskId}|{macAddress}";

                // 生成 MD5 哈希作为机器码
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(hardwareInfo));
                    machineCode = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
                }

                return machineCode;
            }
            catch (Exception ex)
            {
                // 如果获取失败，返回基于机器名的简单标识
                string fallback = Environment.MachineName + Environment.UserName;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(fallback));
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
                }
            }
        }

        /// <summary>
        /// 获取 CPU 序列号
        /// </summary>
        private static string GetCPUSerialNumber()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["ProcessorId"]?.ToString() ?? "";
                    }
                }
            }
            catch
            {
                // 忽略错误，返回空字符串
            }
            return "";
        }

        /// <summary>
        /// 获取主板序列号
        /// </summary>
        private static string GetMotherboardSerialNumber()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj["SerialNumber"]?.ToString() ?? "";
                    }
                }
            }
            catch
            {
                // 忽略错误，返回空字符串
            }
            return "";
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        private static string GetDiskSerialNumber()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE MediaType='Fixed hard disk media'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string serial = obj["SerialNumber"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(serial))
                        {
                            return serial.Trim();
                        }
                    }
                }
            }
            catch
            {
                // 忽略错误，返回空字符串
            }
            return "";
        }

        /// <summary>
        /// 获取 MAC 地址
        /// </summary>
        private static string GetMACAddress()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapter WHERE MACAddress IS NOT NULL"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string mac = obj["MACAddress"]?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(mac))
                        {
                            return mac.Replace(":", "").Replace("-", "");
                        }
                    }
                }
            }
            catch
            {
                // 忽略错误，返回空字符串
            }
            return "";
        }

        /// <summary>
        /// 生成 License（加密）
        /// </summary>
        /// <param name="licenseInfo">License 信息</param>
        /// <param name="key">加密密钥（32字符），如果为空则使用默认密钥</param>
        /// <param name="iv">初始化向量（16字符），如果为空则使用默认 IV</param>
        /// <returns>加密后的 License 字符串（Base64）</returns>
        public static string GenerateLicense(LicenseInfo licenseInfo, string key = null, string iv = null)
        {
            try
            {
                // 设置生成时间
                licenseInfo.GenerateTime = DateTime.Now;

                // 将 License 信息序列化为 JSON
                string json = JsonConvert.SerializeObject(licenseInfo);

                // AES 加密
                string encrypted = EncryptAES(json, key ?? DefaultKey, iv ?? DefaultIV);

                return encrypted;
            }
            catch (Exception ex)
            {
                throw new Exception($"生成 License 失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证并解密 License
        /// </summary>
        /// <param name="encryptedLicense">加密后的 License 字符串</param>
        /// <param name="key">解密密钥（32字符），如果为空则使用默认密钥</param>
        /// <param name="iv">初始化向量（16字符），如果为空则使用默认 IV</param>
        /// <returns>验证结果</returns>
        public static LicenseValidateResult ValidateLicense(string encryptedLicense, string key = null, string iv = null)
        {
            LicenseValidateResult result = new LicenseValidateResult
            {
                IsValid = false,
                LicenseInfo = null
            };

            try
            {
                // 解密 License
                string decryptedJson = DecryptAES(encryptedLicense, key ?? DefaultKey, iv ?? DefaultIV);

                // 反序列化
                LicenseInfo licenseInfo = JsonConvert.DeserializeObject<LicenseInfo>(decryptedJson);

                if (licenseInfo == null)
                {
                    result.ErrorMessage = "License 数据格式错误";
                    return result;
                }

                // 验证机器码
                string currentMachineCode = GetMachineCode();
                if (licenseInfo.MachineCode != currentMachineCode)
                {
                    result.ErrorMessage = $"机器码不匹配。当前机器码: {currentMachineCode}, License 机器码: {licenseInfo.MachineCode}";
                    return result;
                }

                // 验证时间（精确到秒）
                DateTime now = DateTime.Now;
                if (now < licenseInfo.StartTime)
                {
                    TimeSpan diff = licenseInfo.StartTime - now;
                    result.ErrorMessage = $"License 尚未生效，将在 {diff.Days} 天 {diff.Hours} 小时 {diff.Minutes} 分钟 {diff.Seconds} 秒后生效";
                    return result;
                }

                if (now > licenseInfo.EndTime)
                {
                    TimeSpan diff = now - licenseInfo.EndTime;
                    result.ErrorMessage = $"License 已过期，已过期 {diff.Days} 天 {diff.Hours} 小时 {diff.Minutes} 分钟 {diff.Seconds} 秒";
                    return result;
                }

                // 计算剩余时间
                TimeSpan remaining = licenseInfo.EndTime - now;
                result.RemainingDays = remaining.Days;
                result.RemainingHours = remaining.Hours;
                result.RemainingMinutes = remaining.Minutes;
                result.RemainingSeconds = remaining.Seconds;

                // 验证通过
                result.IsValid = true;
                result.LicenseInfo = licenseInfo;
                result.ErrorMessage = "License 验证通过";

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"License 验证失败: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        private static string EncryptAES(string plainText, string key, string iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            // 确保密钥长度为 32 字节（256 位）
            if (keyBytes.Length != 32)
            {
                byte[] tempKey = new byte[32];
                Array.Copy(keyBytes, 0, tempKey, 0, Math.Min(keyBytes.Length, 32));
                if (keyBytes.Length < 32)
                {
                    // 如果密钥长度不足，用零填充
                    for (int i = keyBytes.Length; i < 32; i++)
                    {
                        tempKey[i] = 0;
                    }
                }
                keyBytes = tempKey;
            }

            // 确保 IV 长度为 16 字节（128 位）
            if (ivBytes.Length != 16)
            {
                byte[] tempIV = new byte[16];
                Array.Copy(ivBytes, 0, tempIV, 0, Math.Min(ivBytes.Length, 16));
                if (ivBytes.Length < 16)
                {
                    // 如果 IV 长度不足，用零填充
                    for (int i = ivBytes.Length; i < 16; i++)
                    {
                        tempIV[i] = 0;
                    }
                }
                ivBytes = tempIV;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        private static string DecryptAES(string cipherText, string key, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            // 确保密钥长度为 32 字节（256 位）
            if (keyBytes.Length != 32)
            {
                byte[] tempKey = new byte[32];
                Array.Copy(keyBytes, 0, tempKey, 0, Math.Min(keyBytes.Length, 32));
                if (keyBytes.Length < 32)
                {
                    // 如果密钥长度不足，用零填充
                    for (int i = keyBytes.Length; i < 32; i++)
                    {
                        tempKey[i] = 0;
                    }
                }
                keyBytes = tempKey;
            }

            // 确保 IV 长度为 16 字节（128 位）
            if (ivBytes.Length != 16)
            {
                byte[] tempIV = new byte[16];
                Array.Copy(ivBytes, 0, tempIV, 0, Math.Min(ivBytes.Length, 16));
                if (ivBytes.Length < 16)
                {
                    // 如果 IV 长度不足，用零填充
                    for (int i = ivBytes.Length; i < 16; i++)
                    {
                        tempIV[i] = 0;
                    }
                }
                ivBytes = tempIV;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        /// <summary>
        /// 保存 License 到文件
        /// </summary>
        /// <param name="license">加密后的 License 字符串</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveLicenseToFile(string license, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, license, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存 License 文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 从文件读取 License
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>加密后的 License 字符串</returns>
        public static string ReadLicenseFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"License 文件不存在: {filePath}");
                }

                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"读取 License 文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 创建试用版 License（方便测试）
        /// </summary>
        /// <param name="trialDays">试用天数</param>
        /// <param name="licenseType">授权类型</param>
        /// <returns>加密后的 License 字符串</returns>
        public static string CreateTrialLicense(int trialDays, string licenseType = "试用版")
        {
            LicenseInfo licenseInfo = new LicenseInfo
            {
                MachineCode = GetMachineCode(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(trialDays),
                LicenseType = licenseType,
                ExtraInfo = $"试用版授权，有效期 {trialDays} 天"
            };

            return GenerateLicense(licenseInfo);
        }

        /// <summary>
        /// 创建正式版 License
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="licenseType">授权类型</param>
        /// <param name="extraInfo">额外信息</param>
        /// <returns>加密后的 License 字符串</returns>
        public static string CreateFullLicense(DateTime startTime, DateTime endTime, string licenseType = "正式版", string extraInfo = "")
        {
            LicenseInfo licenseInfo = new LicenseInfo
            {
                MachineCode = GetMachineCode(),
                StartTime = startTime,
                EndTime = endTime,
                LicenseType = licenseType,
                ExtraInfo = extraInfo
            };

            return GenerateLicense(licenseInfo);
        }
    }
}
