using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///员工角色关联表
    ///</summary>
    [SugarTable("sys_user_role")]
    public partial class sys_user_role
    {
           public sys_user_role(){


           }
           /// <summary>
           /// Desc:关联ID（主键）
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:员工ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long user_id {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long role_id {get;set;}

        [Navigate(NavigateType.OneToOne, nameof(role_id), nameof(role_id))]//一对一
         public sys_role? role { get; set; } = new();

    }
}
