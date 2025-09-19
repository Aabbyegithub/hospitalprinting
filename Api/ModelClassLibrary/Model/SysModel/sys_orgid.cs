using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///组织信息表
    ///</summary>
    [SugarTable("sys_orgid")]
    public partial class sys_orgid
    {
        public sys_orgid()
        {


        }
        /// <summary>
        /// Desc:组织ID（主键）
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long? orgid_id {get;set;}

           /// <summary>
           /// Desc:名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string orgid_name {get;set;} = null!;

           /// <summary>
           /// Desc:地址
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string address {get;set;} = null!;


           /// <summary>
           /// Desc:状态（1-启用；0-停用）
           /// Default:1
           /// Nullable:False
           /// </summary>           
           public byte status {get;set;}

        /// <summary>
        /// Desc:创建时间
        /// Default:CURRENT_TIMESTAMP
        /// Nullable:False
        /// </summary>           
        public DateTime created_at { get; set; } = DateTime.Now;

           /// <summary>
           /// Desc:更新时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>           
           public DateTime updated_at {get;set;} = DateTime.Now;

        /// <summary>
        /// 组织编码
        /// </summary>

        public string? orgid_code { get;set;}

    }
}
