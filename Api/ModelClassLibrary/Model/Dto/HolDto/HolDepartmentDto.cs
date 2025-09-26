using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    public class HolDepartmentDto
    {
        public long id { get; set; }

        public string name { get; set; }

        public string? code { get; set; }

        public string? description { get; set; }

        public long? orgid_id { get; set; }

        public string? orgid_name { get; set; }

        public int status { get; set; } = 1;

        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
