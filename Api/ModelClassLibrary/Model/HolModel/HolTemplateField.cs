using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 模板字段配置表
    /// </summary>
    [SugarTable("hol_template_field")]
    public partial class HolTemplateField
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        public long template_id { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string field_name { get; set; }

        /// <summary>
        /// 字段标签
        /// </summary>
        public string field_label { get; set; }

        /// <summary>
        /// 数据来源
        /// </summary>
        public string data_source { get; set; }

        /// <summary>
        /// X坐标位置
        /// </summary>
        public int position_x { get; set; } = 0;

        /// <summary>
        /// Y坐标位置
        /// </summary>
        public int position_y { get; set; } = 0;

        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; } = 100;

        /// <summary>
        /// 高度
        /// </summary>
        public int height { get; set; } = 30;

        /// <summary>
        /// 字体大小
        /// </summary>
        public int font_size { get; set; } = 12;

        /// <summary>
        /// 字体
        /// </summary>
        public string font_family { get; set; } = "Arial";

        /// <summary>
        /// 字体粗细
        /// </summary>
        public string font_weight { get; set; } = "normal";

        /// <summary>
        /// 文本对齐
        /// </summary>
        public string text_align { get; set; } = "left";

        /// <summary>
        /// 字体颜色
        /// </summary>
        public string color { get; set; } = "#000000";

        /// <summary>
        /// 背景颜色
        /// </summary>
        public string? background_color { get; set; }

        /// <summary>
        /// 边框样式
        /// </summary>
        public string? border { get; set; }

        /// <summary>
        /// 格式类型(text/date/number/currency)
        /// </summary>
        public string format_type { get; set; } = "text";

        /// <summary>
        /// 自定义格式
        /// </summary>
        public string? custom_format { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public int is_required { get; set; } = 0;

        /// <summary>
        /// 排序
        /// </summary>
        public int sort_order { get; set; } = 0;

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
