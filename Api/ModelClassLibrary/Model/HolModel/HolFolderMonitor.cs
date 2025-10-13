using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 文件夹监听配置表
    /// </summary>
    [SugarTable("hol_folder_monitor")]
    public partial class HolFolderMonitor
    {
        /// <summary>
        /// 主键ID，自增
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string ip_address { get; set; }

        /// <summary>
        /// 文件夹路径，多个路径用##分隔
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string folder_paths { get; set; }

        /// <summary>
        /// 状态：1=启用，0=停用
        /// </summary>
        public int status { get; set; } = 1;

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
