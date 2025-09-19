using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.Helper
{
    /// <summary>
    /// 转换时间
    /// </summary>
    public class DateTimeHelper
    {
        // 解析日期时间字符串为 DateTime 对象
        public DateTime ParseDateTime(string dateTimeString)
        {
            return DateTime.Parse(dateTimeString);
        }

        // 格式化 DateTime 对象为指定格式的字符串
        public string FormatDateTime(DateTime dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        // 获取当前日期时间，并支持不同的时区
        public DateTime GetCurrentDateTime(string timeZoneId = "")
        {
            if (!string.IsNullOrEmpty(timeZoneId))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
            }
            else
            {
                return DateTime.Now;
            }
        }

        // 将 DateTime 对象转换为 UNIX 时间戳
        public double ConvertToUnixTimeStamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        // 计算两个日期时间之间的时间差，支持跨年和时区的情况
        public TimeSpan CalculateTimeDifference(DateTime startDateTime, DateTime endDateTime)
        {
            return endDateTime - startDateTime;
        }

        // 判断某一年是否是闰年
        public bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        // 获取某一月的天数
        public int GetDaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        // 在日期时间上进行加减操作
        public DateTime Add(DateTime dateTime, TimeSpan timeSpan)
        {
            return dateTime.Add(timeSpan);
        }

        // 本地化显示日期时间，包括显示为相对时间
        // 实现略复杂，可以使用第三方库或自行编写逻辑
        // 可以根据需求返回不同格式的字符串，例如“刚刚”、“几分钟前”等
        public string LocalizeDateTime(DateTime dateTime)
        {
            TimeSpan timeAgo = DateTime.Now - dateTime;
            if (timeAgo.TotalSeconds < 60)
            {
                return "刚刚";
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return $"{(int)timeAgo.TotalMinutes}分钟前";
            }
            else if (timeAgo.TotalHours < 24)
            {
                return $"{(int)timeAgo.TotalHours}小时前";
            }
            else
            {
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        // 根据给定的日期时间范围，生成日期时间序列
        public DateTime[] GenerateDateTimeSeries(DateTime startDateTime, DateTime endDateTime)
        {
            int totalDays = (int)(endDateTime - startDateTime).TotalDays;
            DateTime[] dateTimeSeries = new DateTime[totalDays + 1];
            for (int i = 0; i <= totalDays; i++)
            {
                dateTimeSeries[i] = startDateTime.AddDays(i);
            }
            return dateTimeSeries;
        }
    }
}
