using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    public class HolDoctorDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public int gender { get; set; }
        public string? phone { get; set; }
        public long? department_id { get; set; }
        public string? department_name { get; set; }  // 来自 hol_department
        public string? title { get; set; }
        public string? introduction { get; set; }
        public long? orgid_id { get; set; }

        public string? orgid_name { get; set; }
        public int status { get; set; }
        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
