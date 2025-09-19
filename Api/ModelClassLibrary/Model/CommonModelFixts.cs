using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model
{
    /// <summary>
    ///实体类公共字段（可直接继承）
    /// </summary>
    public class CommonModelFixts
    {
        /// <summary>
        /// Desc:服务器
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int OrgId { get; set; } 

        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int AddUserId { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime AddTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Desc:更新人
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int UpUserId { get; set; }

        /// <summary>
        /// Desc:更新时间
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime UpTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 当前状态
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string IsTrue{ get; set; }


        /// <summary>
        /// 创建人名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AddUser {  get; set; }
        /// <summary>
        /// 更新人名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UpUser { get; set; }

        /// <summary>
        /// 创建时间转化
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string AddTimeString
        {
            get
            {
                return AddTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 更新时间转化
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UpTimeString
        {
            get
            {
                return UpTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
