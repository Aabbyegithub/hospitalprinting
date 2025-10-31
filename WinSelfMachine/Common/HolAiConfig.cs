using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    ///<summary>
    ///AI链接配置表：每个机构一条配置
    ///</summary>
    public partial class HolAiConfig
    {
        public HolAiConfig()
        {
        }

        /// <summary>
        /// Desc:主键ID
        /// Default:
        /// Nullable:False
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// Desc:所属机构ID
        /// Default:
        /// Nullable:False
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// Desc:是否启用（0=禁用，1=启用）
        /// Default:0
        /// Nullable:False
        /// </summary>
        public int is_enabled { get; set; } = 0;

        /// <summary>
        /// Desc:AI接口URL
        /// Default:
        /// Nullable:False
        /// </summary>
        public string api_url { get; set; } = string.Empty;

        /// <summary>
        /// Desc:API密钥（加密存储）
        /// Default:
        /// Nullable:False
        /// </summary>
        public string api_key { get; set; } = string.Empty;

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:False
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// Desc:更新时间
        /// Default:
        /// Nullable:False
        /// </summary>
        public DateTime update_time { get; set; }

        /// <summary>
        /// Desc:创建用户ID
        /// Default:
        /// Nullable:True
        /// </summary>
        public long? create_user_id { get; set; }

        /// <summary>
        /// Desc:更新用户ID
        /// Default:
        /// Nullable:True
        /// </summary>
        public long? update_user_id { get; set; }
    }
}
