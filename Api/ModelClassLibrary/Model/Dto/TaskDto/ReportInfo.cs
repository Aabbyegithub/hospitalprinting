using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.TaskDto
{
    /// <summary>
    /// 解析后的报告信息模型
    /// </summary>
    public class ReportInfo
    {
        /// <summary>
        /// PDF文件完整路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 检查号（如：CT20250510001）
        /// </summary>
        public string CheckNumber { get; set; } = string.Empty;

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; } = string.Empty;

        /// <summary>
        /// 患者ID（如：住院号/门诊号）
        /// </summary>
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// 检查类型（如：胸部CT、核磁共振）
        /// </summary>
        public string ExamType { get; set; } = string.Empty;

        /// <summary>
        /// 报告生成时间
        /// </summary>
        public DateTime? ReportTime { get; set; }
    }
}
