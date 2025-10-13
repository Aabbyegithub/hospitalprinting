using MyNamespace;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebTaskClass.SampleJob
{
    /// <summary>
    /// OCR识别任务
    /// </summary>
    public class OCRIdentifyTask : IJob
    {
        private readonly ISqlHelper _dal;
        public OCRIdentifyTask(ISqlHelper dal)
        {
            _dal = dal;
        }
        /// <summary>
        /// OCR识别任务-识别影像/报告中的内容与数据库解析的内容进行对比，
        /// 更新识别状态（不一致触发弹窗预警）提示管理员进行处理人工核对
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
           
        }
    }
}
