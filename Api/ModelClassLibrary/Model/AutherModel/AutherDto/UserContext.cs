using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.AutherModel.AutherDto
{
    public class UserContext
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public long OrgId { get; set; }
        public string OrgName { get; set; }
        public string PassWord { get; set; }
        public string Salt { get; set; }
        public int? RoleId { get; set; }
    }
}
