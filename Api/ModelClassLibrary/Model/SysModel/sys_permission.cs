using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace MyNamespace
{
    ///<summary>
    ///权限表
    ///</summary>
    [SugarTable("sys_permission")]
    public partial class sys_permission
    {
           public sys_permission(){


           }
           /// <summary>
           /// Desc:权限ID（主键）
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long permission_id {get;set;}

           /// <summary>
           /// Desc:权限名称（如"订单管理"）
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string permission_name {get;set;} = null!;

           /// <summary>
           /// Desc:权限标识（如"order:manage"）
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string permission_key {get;set;} = null!;


        /// <summary>
        /// Desc:权限路由
        /// Default:
        /// Nullable:False
        /// </summary>  
        public string permission_router { get;set;}

        /// <summary>
        /// Desc:图标
        /// Default:
        /// Nullable:False
        /// </summary>  
        public string permission_icon { get;set;} 

           /// <summary>
           /// Desc:父权限ID（用于层级）
           /// Default:0
           /// Nullable:False
           /// </summary>           
       public long parent_id {get;set;}

          [SugarColumn(IsIgnore = true)]
          public bool isSelect{get;set;}


    }
}
