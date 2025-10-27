using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Newtonsoft.Json;
using MyNamespace;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 打印记录表
    /// </summary>
    [SugarTable("hol_print_record")]
    public partial class HolPrintRecord
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 检查ID
        /// </summary>
        public long exam_id { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public long template_id { get; set; }

        /// <summary>
        /// 打印类型(report胶片/patient患者)
        /// </summary>
        public string print_type { get; set; }

        /// <summary>
        /// 打印数据(JSON格式)
        /// </summary>
        [SugarColumn(ColumnDataType = "json")]
        public string? print_data { get; set; }

        /// <summary>
        /// 打印结果(JSON格式)
        /// </summary>
        [SugarColumn(ColumnDataType = "json")]
        public string? print_result { get; set; }

        /// <summary>
        /// 打印状态:1=成功,0=失败
        /// </summary>
        public int print_status { get; set; } = 1;

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime print_time { get; set; }

        /// <summary>
        /// 打印人
        /// </summary>
        public long? print_by { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public long org_id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 导航属性：检查记录
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(exam_id))]
        [JsonIgnore]
        public HolExamination? examination { get; set; }

        /// <summary>
        /// 导航属性：打印模板
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(template_id))]
        [JsonIgnore]
        public HolPrintTemplate? print_template { get; set; }
    }
}
