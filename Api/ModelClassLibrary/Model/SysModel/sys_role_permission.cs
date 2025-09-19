using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///角色权限关联表
    ///</summary>
    [SugarTable("sys_role_permission")]
    public partial class sys_role_permission
    {
           public sys_role_permission(){


           }
           /// <summary>
           /// Desc:关联ID（主键）
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long id {get;set;}

           /// <summary>
           /// Desc:角色ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long role_id {get;set;}

           /// <summary>
           /// Desc:权限ID
           /// Default:
           /// Nullable:False
           /// </summary>           
           public long permission_id {get;set;}

        [Navigate(NavigateType.OneToOne,nameof(permission_id), nameof(sys_permission.permission_id))]//一对一 
        public sys_permission? permission { get; set; }


    }
}
