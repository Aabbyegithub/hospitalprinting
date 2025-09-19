using Quartz;
using System.Threading.Tasks;
using WebServiceClass.Helper;
using WebIServices.IBase;
using Microsoft.Extensions.DependencyInjection;
using MyNamespace;
using SqlSugar.Extensions;

namespace WebServiceClass.QuartzTask
{
    // 任务运行监听器：捕获任务运行事件并记录
    public class TaskRunListener : IJobListener
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public string Name => "TaskRunListener"; // 监听器名称

        // 构造函数注入依赖容器（复用QuartzHostedService的ServiceScopeFactory）
        public TaskRunListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        // 任务开始前触发：记录“开始时间”和“初始状态”
        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dal = scope.ServiceProvider.GetRequiredService<ISqlHelper>();
                var taskName = context.JobDetail.Key.Name;
                // 1. 查询当前任务ID（关联sys_timertask表）
                var taskId = await _dal.Db.Queryable<sys_timertask>()
                    .Where(t => t.TimerClass.Contains(taskName))
                    .Select(t => t.Id)
                    .FirstAsync();
                //await _dal.Db.Updateable<sys_timertask>().SetColumns(a => new sys_timertask { StartNumber = a.StartNumber + 1 }).Where(a => a.Id == taskId).ExecuteCommandAsync();
            }
        }

        // 任务完成后触发：更新“结束时间”“运行状态”和“累计次数”
        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dal = scope.ServiceProvider.GetRequiredService<ISqlHelper>();
                var taskId = context.JobDetail.Key.Name;
                var endTime = DateTime.Now;
                var runStatus = jobException == null ? 1 : 0; // 1=成功，0=失败
                var errorMsg = jobException?.ToString() ?? "";

                await _dal.Db.Updateable<sys_timertask>()
                    .SetColumns(a => new sys_timertask { StartNumber = a.StartNumber + 1 ,lastRunTime = DateTime.Now})
                    .Where(a => a.Id == taskId.ObjToInt()).ExecuteCommandAsync();

            }
        }

        // 任务被否决时触发（如触发器过期，可空实现）
        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}