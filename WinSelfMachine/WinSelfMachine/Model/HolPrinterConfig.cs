using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSelfMachine.Model
{
    ///<summary>
    ///打印机配置表：每台打印机一条配置
    ///</summary>
    [SugarTable("hol_printer_config")]
    public partial class HolPrinterConfig
    {
        public HolPrinterConfig()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }

        /// <summary>
        /// Desc:对应 hol_printer.id
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int printer_id { get; set; }

        /// <summary>
        /// Desc:0.不屏蔽、1.终端显示屏蔽中间字、2.终端显示屏蔽末尾字
        /// Default:0
        /// Nullable:False
        /// </summary>           
        public int mask_mode { get; set; }

        /// <summary>
        /// Desc:限制天数：0表示不限制
        /// Default:0
        /// Nullable:False
        /// </summary>           
        public int limit_days { get; set; }

        /// <summary>
        /// Desc:允许的检查类型（JSON数组字符串，如 ["CT","MRI"]）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string allowed_exam_types { get; set; }

        /// <summary>
        /// Desc:连接的激光打印机ID（仅胶片打印机使用）
        /// Default:
        /// Nullable:True
        /// </summary>
        public int laser_printer_id { get; set; }   

        /// <summary>
        /// Desc:仅显示未打印：0=否，1=是
        /// Default:0
        /// Nullable:False
        /// </summary>           
        public int only_unprinted { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string remark { get; set; }

        /// <summary>
        /// Desc:胶片尺寸（仅胶片打印机使用）
        /// Default:
        /// Nullable:True
        /// </summary>
        public string film_size { get; set; }  
        /// <summary>
        /// Desc:可用数量
        /// Default:
        /// Nullable:True
        /// </summary>
        public int available_count { get; set; } = 0; 
        /// <summary>
        /// Desc:打印时间（秒）
        /// Default:
        /// Nullable:True
        /// </summary>
        public int print_time_seconds { get; set; } = 0; 

        /// <summary>
        /// Desc:机构ID
        /// Default:
        /// Nullable:False
        /// </summary>           
        public long org_id { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime create_time { get; set; }

        /// <summary>
        /// Desc:更新时间
        /// Default:
        /// Nullable:False
        /// </summary>           
        public DateTime update_time { get; set; }

    }

}
