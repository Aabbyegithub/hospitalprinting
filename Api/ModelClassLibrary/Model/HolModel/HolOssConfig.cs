using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// OSS配置表
    /// </summary>
    [SugarTable("hol_oss_config")]
    public class HolOssConfig
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// 是否启用：0=禁用，1=启用
        /// </summary>
        public int is_enabled { get; set; } = 0;

        /// <summary>
        /// OSS访问域名
        /// </summary>
        public string? endpoint { get; set; }

        /// <summary>
        /// AccessKey ID
        /// </summary>
        public string? access_key_id { get; set; }

        /// <summary>
        /// AccessKey Secret
        /// </summary>
        public string? access_key_secret { get; set; }

        /// <summary>
        /// 存储桶名称
        /// </summary>
        public string? bucket_name { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public string? region { get; set; }

        /// <summary>
        /// 文件夹前缀
        /// </summary>
        public string? folder_prefix { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_time { get; set; } = DateTime.Now;
    }
}
