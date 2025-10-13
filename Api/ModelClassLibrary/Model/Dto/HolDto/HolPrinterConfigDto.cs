using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    public class HolPrinterConfigDto
    {
        public int id { get; set; }
        public int printer_id { get; set; }
        public int mask_mode { get; set; }
        public int limit_days { get; set; }
        public List<string> allowed_exam_types { get; set; } = new();
        public int only_unprinted { get; set; }
        public int? laser_printer_id { get; set; }   // 连接的激光打印机ID
        public string? film_size { get; set; }        // 新增：胶片尺寸
        public int available_count { get; set; }     // 新增：可用数量
        public int print_time_seconds { get; set; }  // 新增：打印时间（秒）
        public string? remark { get; set; }
    }
}
