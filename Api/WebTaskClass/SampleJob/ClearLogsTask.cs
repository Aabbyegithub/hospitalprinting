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
    public class ClearLogsTask:IJob
    {
        private readonly ISqlHelper _dal;
        public ClearLogsTask(ISqlHelper dal)
        {
            _dal = dal;
        }
        /// <summary>
        /// 只保留一个月日志
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            await _dal.Db.Deleteable<lq_operationlog>().Where(a => a.AddTime <= DateTime.Now.AddDays(-30)).ExecuteCommandAsync();
        }
    }
}
