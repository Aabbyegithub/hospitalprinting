using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 条形码配置表
    /// </summary>
    [SugarTable("hol_barcode_config")]
    public partial class HolBarcodeConfig
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
        /// 条形码名称
        /// </summary>
        public string barcode_name { get; set; }

        /// <summary>
        /// 条形码类型(CODE128/QR/CODE39等)
        /// </summary>
        public string barcode_type { get; set; }

        /// <summary>
        /// 数据来源字段
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
        public int height { get; set; } = 50;

        /// <summary>
        /// 字体大小
        /// </summary>
        public int font_size { get; set; } = 12;

        /// <summary>
        /// 是否显示文本
        /// </summary>
        public int show_text { get; set; } = 1;

        /// <summary>
        /// 文本位置(top/bottom/left/right)
        /// </summary>
        public string text_position { get; set; } = "bottom";

        /// <summary>
        /// 自定义格式
        /// </summary>
        public string? custom_format { get; set; }

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
