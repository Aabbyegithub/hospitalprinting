using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 医疗记录实体
    /// </summary>
    public class HolMedicalRecord
    {
        /// <summary>
        /// 检查号（唯一）
        /// </summary>
        public string AccessionNumber { get; set; }

        /// <summary>
        /// 病人编号
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名字
        /// </summary>
        public string EnName { get; set; }

        /// <summary>
        /// 性别（男/女）
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 年龄类型（岁/月/周/天/时）
        /// </summary>
        public string AgeType { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// 检查日期（包含时分秒）
        /// </summary>
        public DateTime StudyDate { get; set; }

        /// <summary>
        /// 检查ID
        /// </summary>
        public string StudyId { get; set; }

        /// <summary>
        /// 设备类型（CT/DX/MR/RF...）
        /// </summary>
        public string Modality { get; set; }

        /// <summary>
        /// 报告状态（已审核/已报告/已打印）
        /// </summary>
        public string ReportStatus { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string InPatientNo { get; set; }

        /// <summary>
        /// 门诊号
        /// </summary>
        public string OutPatientNo { get; set; }

        /// <summary>
        /// 体检号
        /// </summary>
        public string TjNo { get; set; }

        /// <summary>
        /// 身份证号（保存原始影像时必须提供）
        /// </summary>

        public string IdNumber { get; set; }

        /// <summary>
        /// 医保卡号
        /// </summary>
        public string MedicareId { get; set; }

        /// <summary>
        /// 患者类型（门诊/住院/体检）
        /// </summary>
        public string PatientClass { get; set; }

        /// <summary>
        /// 报告医生
        /// </summary>
        public string ReportDoctor { get; set; }

        /// <summary>
        /// 审核医生
        /// </summary>
        public string AuditDoctor { get; set; }

        /// <summary>
        /// 主机名（如RIS系统不支持条码打印，请提供该字段内容）
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime? RegDate { get; set; }

        /// <summary>
        /// 主键（必须有值，且为唯一值）
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// 报告日期（包含时分秒）
        /// </summary>
        public DateTime? ReportDate { get; set; }

        /// <summary>
        /// 审核日期（包含时分秒）
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 检查部位
        /// </summary>
        public string BodyPart { get; set; }

        /// <summary>
        /// PDF报告URL（提供报告单Ftp或Http地址）
        /// </summary>
        public string PdfReportUrl { get; set; }

        /// <summary>
        /// 申请科室
        /// </summary>
        public string ReqDept { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ReqPhysician { get; set; }

        /// <summary>
        /// 病区
        /// </summary>
        public string Ward { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string BedNo { get; set; }

        /// <summary>
        /// 检查方法
        /// </summary>
        public string ExamMethod { get; set; }

        /// <summary>
        /// 检查所见
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 检查结论
        /// </summary>
        public string Impression { get; set; }

        /// <summary>
        /// 建议
        /// </summary>
        public string Recommendation { get; set; }

        /// <summary>
        /// 报告医生电子签名
        /// </summary>
        public byte[] ReportDoctorSign { get; set; }

        /// <summary>
        /// 审核医生电子签名
        /// </summary>
        public byte[] AuditDoctorSign { get; set; }

        /// <summary>
        /// 检查唯一号（DICOM）
        /// </summary>
        public string StudyUid { get; set; }

        /// <summary>
        /// 手机号（保存原始影像时必须提供）
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 云胶片标志（1需要云胶片 0不需要）
        /// </summary>
        public string NeedEfilm { get; set; }

        /// <summary>
        /// 胶片类型（电子胶片/实体胶片）
        /// </summary>
        public string FilmType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public bool Isfee { get; set; }
    }
}
