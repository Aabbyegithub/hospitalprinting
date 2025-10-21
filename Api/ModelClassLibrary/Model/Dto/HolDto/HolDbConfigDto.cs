using System.ComponentModel.DataAnnotations;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 数据库连接配置DTO
    /// </summary>
    public class HolDbConfigDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        [Required(ErrorMessage = "配置名称不能为空")]
        [StringLength(100, ErrorMessage = "配置名称长度不能超过100个字符")]
        public string config_name { get; set; } = string.Empty;

        /// <summary>
        /// 服务器IP/域名
        /// </summary>
        [Required(ErrorMessage = "服务器IP不能为空")]
        [StringLength(100, ErrorMessage = "IP地址长度不能超过100个字符")]
        public string server_ip { get; set; } = string.Empty;

        /// <summary>
        /// 服务器端口
        /// </summary>
        [Range(1, 65535, ErrorMessage = "端口号必须在1-65535之间")]
        public int server_port { get; set; } = 3306;

        /// <summary>
        /// 数据库名称
        /// </summary>
        [Required(ErrorMessage = "数据库名称不能为空")]
        [StringLength(100, ErrorMessage = "数据库名称长度不能超过100个字符")]
        public string database_name { get; set; } = string.Empty;

        /// <summary>
        /// 数据库类型(MySQL/SQLServer/Oracle/PostgreSQL)
        /// </summary>
        [Required(ErrorMessage = "数据库类型不能为空")]
        [StringLength(20)]
        public string database_type { get; set; } = "MySQL";

        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(100, ErrorMessage = "用户名长度不能超过100个字符")]
        public string username { get; set; } = string.Empty;

        /// <summary>
        /// 密码（前端传入明文，后端加密存储）
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(255, ErrorMessage = "密码长度不能超过255个字符")]
        public string password { get; set; } = string.Empty;

        /// <summary>
        /// 所属机构ID
        /// </summary>
        public long org_id { get; set; } = 1;

        /// <summary>
        /// 状态
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
    }
}
