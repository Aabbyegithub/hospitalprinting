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
    /// 文件解析任务
    /// </summary>
    public class FileAnalysisTask : IJob
    {
        private readonly ISqlHelper _dal;
        public FileAnalysisTask(ISqlHelper dal)
        {
            _dal = dal;
        }
        /// <summary>
        /// 配置文件解析任务-定时扫描指定文件夹中的新文件，进行解析处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
           
        }
    }
}
