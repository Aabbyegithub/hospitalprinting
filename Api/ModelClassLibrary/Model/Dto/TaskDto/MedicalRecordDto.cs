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
        /// 门诊号
        /// </summary>
        public string OutpatientNumber { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string HospitalAdmissionNumber { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string BedNumber { get; set; }
        /// <summary>
        /// 送检医生
        /// </summary>
        public string ReferringDoctor { get; set; }

        /// <summary>
        /// 送检科室
        /// </summary>
        public string DepartmentInspection { get; set; }

        /// <summary>
        /// 检查部位
        /// </summary>
        public string ExamType { get; set; }
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string ClinicalDiagnosis { get; set; }
        /// <summary>
        /// 影像所见
        /// </summary>
        public string ImagingFindings { get; set; }
        /// <summary>
        /// 诊断结论
        /// </summary>
        public string DiagnosisConclusion { get; set; }
        /// <summary>
        /// 报告医生
        /// </summary>
        public string ReportDoctor { get; set; }

        /// <summary>
        /// 审核医生
        /// </summary>
        public string ReviewingDoctor { get; set; }

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
