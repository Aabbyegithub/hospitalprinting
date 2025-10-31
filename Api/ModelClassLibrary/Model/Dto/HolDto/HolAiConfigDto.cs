using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    ///<summary>
    ///AI链接配置DTO
    ///</summary>
    public class HolAiConfigDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 所属机构ID
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// 是否启用（0=禁用，1=启用）
        /// </summary>
        public int is_enabled { get; set; } = 0;

        /// <summary>
        /// AI接口URL
        /// </summary>
        [Required(ErrorMessage = "AI接口URL不能为空")]
        public string api_url { get; set; } = string.Empty;

        /// <summary>
        /// API密钥
        /// </summary>
        [Required(ErrorMessage = "API密钥不能为空")]
        public string api_key { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string? remark { get; set; }
    }
}
