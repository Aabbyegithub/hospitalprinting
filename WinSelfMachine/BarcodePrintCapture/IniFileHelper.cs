using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BarcodePrintCapture
{
    /// <summary>
    /// INI文件操作辅助类
    /// </summary>
    public class IniFileHelper : IDisposable
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WritePrivateProfileString(
            string section,
            string key,
            string val,
            string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int GetPrivateProfileString(
            string section,
            string key,
            string def,
            StringBuilder retVal,
            int size,
            string filePath);

        /// <summary>
        /// INI文件路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iniFilePath">INI文件路径</param>
        public IniFileHelper(string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
            {
                FileStream fs = File.Create(iniFilePath);
                fs.Close();
            }

            FilePath = iniFilePath;
        }

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        public bool Write(string section, string key, string value)
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key))
                return false;

            return WritePrivateProfileString(section, key, value, FilePath) > 0;
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>读取到的值</returns>
        public string Read(string section, string key, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(section) || string.IsNullOrEmpty(key))
                return defaultValue;

            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(section, key, defaultValue, sb, 255, FilePath);
            return sb.ToString();
        }

        /// <summary>
        /// 读取整数类型配置
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>读取到的整数</returns>
        public int ReadInt(string section, string key, int defaultValue = 0)
        {
            string value = Read(section, key, defaultValue.ToString());
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// 删除指定的节
        /// </summary>
        /// <param name="section">节名</param>
        /// <returns>是否成功</returns>
        public bool DeleteSection(string section)
        {
            return WritePrivateProfileString(section, null, null, FilePath) > 0;
        }

        /// <summary>
        /// 删除指定节中的键
        /// </summary>
        /// <param name="section">节名</param>
        /// <param name="key">键名</param>
        /// <returns>是否成功</returns>
        public bool DeleteKey(string section, string key)
        {
            return WritePrivateProfileString(section, key, null, FilePath) > 0;
        }

        public void Dispose()
        {
            // 暂时无需实现
        }
    }
}

