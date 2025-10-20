using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinSelfMachine.Common
{
    public class IniFileHelper
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int WritePrivateProfileString(
            string section,
            string key,
            string val,
            string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString
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
            // 检查文件是否存在
            if (!File.Exists(iniFilePath))
            {
                // 创建文件
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
    }
}
