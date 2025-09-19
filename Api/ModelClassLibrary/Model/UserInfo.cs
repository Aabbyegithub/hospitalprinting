using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model
{
    /// <summary>
    /// 用户信息表
    /// </summary>
    [SugarTable("Sys_UserInfo")]
    [Tenant("0")]
    public class UserInfo : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 姓名/昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Email { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string PhoneNum { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "date")]
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// 岗位/职位
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Post { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? DeptID { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "nvarchar(max)")]
        public string RoleIds { get; set; }
        /// <summary>
        /// 密码错误次数
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? ErrorCount { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? LockTime { get; set; }
    }

}
