using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Newtonsoft.Json;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 打印模板表
    /// </summary>
    [SugarTable("hol_print_template")]
    public partial class HolPrintTemplate
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 模板描述
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// 模板类型(report胶片/patient患者)
        /// </summary>
        public string template_type { get; set; }

        /// <summary>
        /// 条形码数据来源字段
        /// </summary>
        public string barcode_data_source { get; set; }

        /// <summary>
        /// 条形码类型(CODE128/QR/CODE39)
        /// </summary>
        public string barcode_type { get; set; } = "CODE128";

        /// <summary>
        /// 显示字段配置(JSON格式)
        /// </summary>
        [SugarColumn(ColumnDataType = "json")]
        public string display_fields { get; set; }

        /// <summary>
        /// 是否默认模板
        /// </summary>
        public int is_default { get; set; } = 0;

        /// <summary>
        /// 状态:1=启用,0=禁用
        /// </summary>
        public int status { get; set; } = 1;

        /// <summary>
        /// 所属机构ID
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

        /// <summary>
        /// 创建人
        /// </summary>
        public long? create_by { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public long? update_by { get; set; }
    }
}
