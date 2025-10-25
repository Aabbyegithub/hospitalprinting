using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{

    public class PrintRecordModel
    {
        public long id { get; set; }

        /// <summary>
        /// 检查数据ID
        /// </summary>
        public long exam_id { get; set; }

        /// <summary>
        /// 患者ID
        /// </summary>
        public long patient_id { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime print_time { get; set; } = DateTime.Now;

        /// <summary>
        /// 打印人（患者本人）
        /// </summary>
        public long printed_by { get; set; }

        /// <summary>
        /// 状态：1=有效，0=过期/删除
        /// </summary>
        public int status { get; set; } = 1;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_time { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime update_time { get; set; }
    }
}
