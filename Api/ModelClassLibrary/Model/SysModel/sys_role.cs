using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///角色表
    ///</summary>
    [SugarTable("sys_role")]
    public partial class sys_role
    {
           public sys_role(){


           }
           /// <summary>
           /// Desc:角色ID（主键）
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long role_id {get;set;}

           /// <summary>
           /// Desc:角色名称（店长/收银员/厨师）
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string? role_name {get;set;} = null!;

           /// <summary>
           /// Desc:角色描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? description {get;set;}

    }
}
