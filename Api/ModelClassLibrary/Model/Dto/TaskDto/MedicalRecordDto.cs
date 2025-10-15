using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.TaskDto
{
    /// <summary>
    /// 医疗记录数据传输对象
    /// </summary>
    public class MedicalRecordDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 胶片检查号
        /// </summary>
        public string FilmCheckNumber { get; set; }

        /// <summary>
        /// 报告编号
        /// </summary>
        public string ReportNumber { get; set; }

        /// <summary>
        /// 检查类型
        /// </summary>
        public string ExamType { get; set; }

        /// <summary>
        /// 检查日期
        /// </summary>
        public DateTime? ExamDate { get; set; }

        /// <summary>
        /// 原始文件路径
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// OCR识别的完整文本
        /// </summary>
        public string FullOcrText { get; set; }

        /// <summary>
        /// 验证状态
        /// </summary>
        public string ValidationStatus { get; set; }

        /// <summary>
        /// 识别时间
        /// </summary>
        public DateTime? RecognizeTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
