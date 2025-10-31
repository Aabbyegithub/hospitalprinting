using MyNamespace;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.HolModel
{
    /// <summary>
    /// 打印记录表
    /// </summary>
    [SugarTable("hol_print_record")]
    public class PrintRecordModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
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

        /// <summary>
        /// 导航属性：对应的诊断信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(exam_id))]
        [JsonIgnore]
        public HolExamination? holExamination { get; set; }

        /// <summary>
        /// 导航属性：对应的诊断信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(patient_id))]
        [JsonIgnore]
        public HolPatient? holPatient { get; set; }
    }
}
