using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.AutherModel.AutherDto
{
    public class UserParam
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>

        public string NewPassword { get; set; }
    }

    public class UserMsg
    {
        /// <summary>
        /// Desc:用户名
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string User { get; set; }

        /// <summary>
        /// Desc:账户名
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Desc:邮件
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
