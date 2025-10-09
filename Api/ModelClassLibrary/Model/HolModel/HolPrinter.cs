using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    [SugarTable("hol_printer")]
    public class HolPrinter
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        public string name { get; set; }
        public int type { get; set; }                // 1=自助,2=胶片,3=报告
        public string? model { get; set; }
        public string? ip_address { get; set; }
        public int? port { get; set; }
        public string? resolution { get; set; }
        public string? paper_size { get; set; }
        public string? location { get; set; }
        public int status { get; set; } = 1;         // 1=启用,0=停用
        public long org_id { get; set; }
        public string? remark { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }
}
