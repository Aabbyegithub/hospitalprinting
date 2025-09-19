using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.AutherModel.AutherDto
{
    /// <summary>
    /// 登陆验证返回
    /// </summary>
    public class UserResult
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 组织
        /// </summary>
        public long OrgId { get; set; }

        /// <summary>
        /// 权限验证码
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 是都第一次登陆
        /// </summary>
        public bool IsFirst { get; set; }
    }
}
