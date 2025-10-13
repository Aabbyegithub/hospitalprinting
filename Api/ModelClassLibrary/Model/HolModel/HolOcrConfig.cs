using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 百度云OCR配置表
    /// </summary>
    [SugarTable("hol_ocr_config")]
    public partial class HolOcrConfig
    {
        /// <summary>
        /// 主键ID，自增
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 是否启用：1=启用，0=停用
        /// </summary>
        public int is_enabled { get; set; } = 0;

        /// <summary>
        /// 百度云OCR接口URL
        /// </summary>
        public string? api_url { get; set; }

        /// <summary>
        /// 百度云应用ID
        /// </summary>
        public string? app_id { get; set; }

        /// <summary>
        /// 百度云API Key
        /// </summary>
        public string? api_key { get; set; }

        /// <summary>
        /// 百度云Secret Key
        /// </summary>
        public string? secret_key { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string? access_token { get; set; }

        /// <summary>
        /// 令牌过期时间
        /// </summary>
        public DateTime? token_expires_at { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? remark { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_time { get; set; }
    }
}
