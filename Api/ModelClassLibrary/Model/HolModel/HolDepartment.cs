using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    ///<summary>
    ///科室信息表
    ///</summary>
    [SugarTable("hol_department")]
    public class HolDepartment
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>科室名称</summary>
        public string name { get; set; }

        /// <summary>科室编码（唯一）</summary>
        public string? code { get; set; }

        /// <summary>科室简介</summary>
        public string? description { get; set; }

        /// <summary>所属机构ID</summary>
        public long? orgid_id { get; set; }

        /// <summary>状态：1=有效,0=删除</summary>
        public int status { get; set; } = 1;

        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }
    }

}
