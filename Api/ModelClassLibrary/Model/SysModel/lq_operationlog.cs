using System;
using System.Linq;
using System.Text;
using ModelClassLibrary.Model;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;

namespace MyNamespace
{
    ///<summary>
    ///系统操作日志
    ///</summary>
    [SugarTable("sys_operationlog")]
    public partial class lq_operationlog:CommonModelFixts
    {
           public lq_operationlog(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long Id {get;set;}

           /// <summary>
           /// Desc:操作人
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int UserId {get;set;}

           /// <summary>
           /// Desc:操作类型
           /// Default:
           /// Nullable:False
           /// </summary>           
           public ActionType ActionType {get;set;}
          [SugarColumn(IsIgnore =true)]
           public string ActionTypeName {get;set;}

           /// <summary>
           /// Desc:模块名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ModuleName {get;set;}

           /// <summary>
           /// Desc:操作描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string? Description {get;set;}

        /// <summary>
        /// Desc:操作内容
        /// Default:
        /// Nullable:True
        /// </summary>  
        public string? ActionContent {get;set;}

        /// <summary>
        /// Desc:操作时间
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime ActionTime { get; set; } = DateTime.Now;

        [Navigate(NavigateType.OneToOne, nameof(UserId), nameof(sys_user.user_id))]//一对一 
        public sys_user? staff { get; set; }

    }
}
