using System;
using System.Linq;
using System.Text;
using ModelClassLibrary.Model.Dto.HolDto;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///检验数据表
    ///</summary>
    [SugarTable("hol_examination")]
    public partial class HolExamination
    {
        /// <summary>
        /// 主键ID，自增
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 检查号（唯一标识）
        /// </summary>
        public string exam_no { get; set; }

        /// <summary>
        /// 患者ID（关联 sys_patient 表）
        /// </summary>
        public long patient_id { get; set; }

        /// <summary>
        /// 诊断医生（关联 HolDoctor 表）
        /// </summary>
        public long doctor_id { get; set; }

        /// <summary>
        /// 所属机构ID（关联医院/科室）
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// 检查类型（CT / MRI / 超声 等）
        /// </summary>
        public string exam_type { get; set; }

        /// <summary>
        /// 检查日期
        /// </summary>
        public DateTime exam_date { get; set; }

        /// <summary>
        /// 报告文件路径
        /// </summary>
        public string? report_path { get; set; }

        /// <summary>
        /// 电子胶片路径
        /// </summary>
        public string? image_path { get; set; }

        /// <summary>
        /// 报告文件编号
        /// </summary>
        public string? report_no { get; set; }

        /// <summary>
        /// 电子胶片检查号
        /// </summary>
        public string? image_no{ get; set; }

        /// <summary>
        /// 状态：1=有效，0=过期/删除
        /// </summary>
        public int status { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_time { get; set; }

        /// <summary>
        /// ocr识别：1.信息准确 2.信息有误
        /// </summary>
        public int? ocr_identify_type { get; set; } = 1;

        /// <summary>
        /// 是否已打印：0=未打印，1=已打印
        /// </summary>
        public int is_printed { get; set; } = 0;

        /// <summary>
        /// 是否已上传云端 0未上传1已上传
        /// </summary>
        public int is_upload { get; set; } = 0;

        /// <summary>
        /// 导航属性：对应的患者信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(patient_id))]
        [JsonIgnore]
        public HolPatient? patient { get; set; }

        /// <summary>
        /// 导航属性：对应的医生信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(doctor_id))]
        [JsonIgnore]
        public HolDoctor? doctor { get; set; }



    }
}
