using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using ModelClassLibrary.Model.HolModel;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///检查数据表
    ///</summary>
    [SugarTable("hol_examination")]
    public partial class HolExamination
    {
           public HolExamination(){


           }
           /// <summary>
           /// Desc:主键ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:检查号（唯一标识）
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string exam_no {get;set;} = null!;

           /// <summary>
           /// Desc:患者ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long patient_id {get;set;}
           public string patientexid {get;set;}

           /// <summary>
           /// Desc:所属机构ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long org_id {get;set;}

           /// <summary>
           /// Desc:检查类型（CT/MRI/超声等）
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? exam_type {get;set;}

           /// <summary>
           /// Desc:检查日期
           /// Default:
           /// Nullable:False
           /// </summary>           
           public DateTime exam_date {get;set;}

           /// <summary>
           /// Desc:报告文件路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? report_path {get;set;}

           /// <summary>
           /// Desc:电子胶片路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? image_path {get;set;}

        /// <summary>
        /// Desc:状态：1=有效，0=过期/删除
        /// Default:1
        /// Nullable:True
        /// </summary>           
        public byte? status { get; set; } = 1;

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           public DateTime? create_time {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>           
           public DateTime? update_time {get;set;}

        /// <summary>
        /// Desc:是否已打印：0=未打印，1=已打印
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public byte? is_printed { get; set; } = 0;

           /// <summary>
           /// Desc:报告编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? report_no {get;set;}

           /// <summary>
           /// Desc:胶片检查号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? image_no {get;set;}

           /// <summary>
           /// Desc:诊断医生
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long doctor_id {get;set;}

        /// <summary>
        /// Desc:ocr识别：1.信息准确 2.信息有误
        /// Default:1
        /// Nullable:True
        /// </summary>           
        public int? ocr_identify_type { get; set; } = 2;

        /// <summary>
        /// Desc:是否已上传云端 0未上传1已上传
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public int? is_upload { get; set; } = 0;

           /// <summary>
           /// Desc:检查号(唯一标识)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? accession_number {get;set;}

           /// <summary>
           /// Desc:设备类型(CT/DX/MR/RF等)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? modality {get;set;}

           /// <summary>
           /// Desc:报告状态(已审核/已报告/已打印)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? report_status {get;set;}

           /// <summary>
           /// Desc:卡号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? card_no {get;set;}

           /// <summary>
           /// Desc:住院号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? in_patient_no {get;set;}

           /// <summary>
           /// Desc:门诊号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? out_patient_no {get;set;}

           /// <summary>
           /// Desc:体检号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? tj_no {get;set;}

           /// <summary>
           /// Desc:身份证号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? id_number {get;set;}

           /// <summary>
           /// Desc:医保卡号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? medicare_id {get;set;}

           /// <summary>
           /// Desc:患者类型(门诊/住院/体检)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_class {get;set;}

           /// <summary>
           /// Desc:报告医生
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? report_doctor {get;set;}

           /// <summary>
           /// Desc:审核医生
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? audit_doctor {get;set;}

           /// <summary>
           /// Desc:登记时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? reg_date {get;set;}

           /// <summary>
           /// Desc:报告日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? report_date {get;set;}

           /// <summary>
           /// Desc:审核日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? audit_date {get;set;}

           /// <summary>
           /// Desc:检查部位
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? body_part {get;set;}

           /// <summary>
           /// Desc:PDF报告URL
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? pdf_report_url {get;set;}

           /// <summary>
           /// Desc:申请科室
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? req_dept {get;set;}

           /// <summary>
           /// Desc:申请人
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? req_physician {get;set;}

           /// <summary>
           /// Desc:病区
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? ward {get;set;}

           /// <summary>
           /// Desc:床号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? bed_no {get;set;}

           /// <summary>
           /// Desc:检查方法
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? exam_method {get;set;}

           /// <summary>
           /// Desc:检查所见
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? description {get;set;}

           /// <summary>
           /// Desc:检查结论
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? impression {get;set;}

           /// <summary>
           /// Desc:建议
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? recommendation {get;set;}

           /// <summary>
           /// Desc:报告医生电子签名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public byte[] report_doctor_sign {get;set;}

           /// <summary>
           /// Desc:审核医生电子签名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public byte[] audit_doctor_sign {get;set;}

           /// <summary>
           /// Desc:检查唯一号(DICOM)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? studyuid {get;set;}

           /// <summary>
           /// Desc:手机号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? phone {get;set;}

           /// <summary>
           /// Desc:云胶片标志(1需要0不需要)
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public byte? need_efilm {get;set;}

           /// <summary>
           /// Desc:胶片类型(电子胶片/实体胶片)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? filmtype {get;set;}

        /// <summary>
        /// Desc:胶片能否打印:1=可以打印,0=不能打印
        /// Default:1
        /// Nullable:True
        /// </summary>           
        public byte? isfees { get; set; } = 1;
        /// <summary>
        /// 导航属性：对应的患者信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(patient_id))]
        //[JsonIgnore]
        public MyNamespace.HolPatient patient { get; set; }

        /// <summary>
        /// 导航属性：对应的医生信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(doctor_id))]
        //[JsonIgnore]
        public ModelClassLibrary.Model.HolModel.HolDoctor doctor { get; set; }

    }
}
