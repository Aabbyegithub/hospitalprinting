using SqlSugar;
using System;
using System.Linq;
using System.Text;

namespace MyNamespace
{
    ///<summary>
    ///定时任务管理表
    ///</summary>
    public partial class sys_timertask
    {
        public sys_timertask()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>   
     [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id { get; set; }

        /// <summary>
        /// Desc:定时器名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string TimerName { get; set; }

        /// <summary>
        /// Desc:定时器服务类
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string TimerClass { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Desc:运行开始时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? BeginTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Desc:运行结束时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? EndTime { get; set; } = DateTime.Now.AddYears(100);

        /// <summary>
        /// Desc:创建人
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? AddUser { get; set; }

        /// <summary>
        /// Desc:组织
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? OrgId { get; set; }

        /// <summary>
        /// Desc:运行状态：0，未启动，1，启动运行，2，暂停
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? IsStart { get; set; } = 0;
        /// <summary>
        /// Desc:是否删除：0，删除，1，运行
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? isDelete { get; set; } = 1;

        /// <summary>
        /// Desc:设置运行时段
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Corn { get; set; }


        /// <summary>
        /// Desc:运行次数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public long? StartNumber { get; set; } = 0;
        public DateTime? lastRunTime { get; set; }

    }
}
