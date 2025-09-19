using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.SystemDto
{
    /// <summary>
    /// 用户权限DTO
    /// </summary>
    public class UserPermission
    {
        public long permissionId { get; set; }
        public string groupKey { get; set; }
        public string groupTitle { get; set; }
        public string icon { get; set; }
        public string parent_id { get; set; }
        public bool isselect { get; set; }

        public List<UserPermissionItem> children { get; set; }
    }

    public class UserPermissionItem
    {
        public long permissionId { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string parent_id { get; set; }
        public bool isselect { get; set; }
    }
}
