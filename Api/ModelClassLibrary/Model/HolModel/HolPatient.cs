using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("hol_patient")]
    public partial class HolPatient
    {
        public HolPatient()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// Desc:患者姓名
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string name { get; set; } = null!;

        /// <summary>
        /// Desc:性别
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string gender { get; set; } = null!;

        /// <summary>
        /// Desc:年龄
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? age { get; set; }

        /// <summary>
        /// Desc:身份证号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? id_card { get; set; }

        /// <summary>
        /// Desc:联系方式
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? contact { get; set; }

        /// <summary>
        /// Desc:就诊号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? medical_no { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:CURRENT_TIMESTAMP
        /// Nullable:True
        /// </summary>           
        public DateTime? createtime { get; set; }

        /// <summary>
        /// Desc:修改时间
        /// Default:CURRENT_TIMESTAMP
        /// Nullable:True
        /// </summary>           
        public DateTime? updatetime { get; set; }

        /// <summary>
        /// Desc:是否有效（0，1）
        /// Default:1
        /// Nullable:True
        /// </summary>           
        public byte? status { get; set; }

    }
}
