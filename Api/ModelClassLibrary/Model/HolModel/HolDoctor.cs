using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    ///<summary>
    ///医生信息表
    ///</summary>
    [SugarTable("hol_doctor")]
    public class HolDoctor
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>医生姓名</summary>
        public string name { get; set; }

        /// <summary>性别：0=未知,1=男,2=女</summary>
        public int gender { get; set; }

        /// <summary>联系电话</summary>
        public string? phone { get; set; }

        /// <summary>所属科室ID</summary>
        public long? department_id { get; set; }

        /// <summary>职称</summary>
        public string? title { get; set; }

        /// <summary>医生简介</summary>
        public string? introduction { get; set; }

        /// <summary>所属机构ID</summary>
        public long? orgid_id { get; set; }

        /// <summary>状态：1=有效,0=删除</summary>
        public int status { get; set; } = 1;

        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }

        /// <summary>
        /// 导航属性：对应的科室信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(department_id))]
        public HolDepartment holdepartment { get; set; }
    }

}
