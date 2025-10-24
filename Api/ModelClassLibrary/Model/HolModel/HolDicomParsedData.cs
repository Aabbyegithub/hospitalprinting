using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///DICOM解析数据表
    ///</summary>
    [SugarTable("hol_dicom_parsed_data")]
    public partial class HolDicomParsedData
    {
           public HolDicomParsedData(){


           }
           /// <summary>
           /// Desc:主键ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:DICOM文件路径
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string file_path {get;set;} = null!;

           /// <summary>
           /// Desc:文件名
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string file_name {get;set;} = null!;

           /// <summary>
           /// Desc:解析时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime parse_time {get;set;}

           /// <summary>
           /// Desc:患者ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_id {get;set;}

           /// <summary>
           /// Desc:患者姓名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_name {get;set;}

           /// <summary>
           /// Desc:出生日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_birth_date {get;set;}

           /// <summary>
           /// Desc:性别
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_sex {get;set;}

           /// <summary>
           /// Desc:年龄
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? patient_age {get;set;}

           /// <summary>
           /// Desc:检查实例UID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_instance_uid {get;set;}

           /// <summary>
           /// Desc:检查日期
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_date {get;set;}

           /// <summary>
           /// Desc:检查时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_time {get;set;}

           /// <summary>
           /// Desc:检查描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_description {get;set;}

           /// <summary>
           /// Desc:检查ID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_id {get;set;}

           /// <summary>
           /// Desc:检查号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? accession_number {get;set;}

           /// <summary>
           /// Desc:SOP实例UID
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? sop_instance_uid {get;set;}

           /// <summary>
           /// Desc:实例号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? instance_number {get;set;}

           /// <summary>
           /// Desc:图像类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? image_type {get;set;}

           /// <summary>
           /// Desc:科室名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? institutional_department_name {get;set;}

           /// <summary>
           /// Desc:检查备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_comments {get;set;}

           /// <summary>
           /// Desc:检查状态
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_status_id {get;set;}

           /// <summary>
           /// Desc:检查优先级
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? study_priority_id {get;set;}

           /// <summary>
           /// Desc:申请医生
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? referring_physician_name {get;set;}

           /// <summary>
           /// Desc:执行医生
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? performing_physician_name {get;set;}

           /// <summary>
           /// Desc:操作员
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? operator_name {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime create_time {get;set;}

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime update_time {get;set;}

           /// <summary>
           /// Desc:是否删除
           /// Default:true
           /// Nullable:False
           /// </summary>           
           public bool is_deleted {get;set;}

        /// <summary>
        /// Desc:是否校验通过
        /// Default:true
        /// Nullable:False
        /// </summary>           
        public bool is_verify { get; set; } = true;

        /// <summary>
        /// 设备类型
        /// </summary>
        public string  modality { get; set; }

    }
}
