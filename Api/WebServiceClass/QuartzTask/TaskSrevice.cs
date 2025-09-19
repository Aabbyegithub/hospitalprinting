using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ModelClassLibrary.Model;
using Quartz;
using Quartz.Impl;
using SqlSugar;
using System.Reflection;
using WebIServices.IBase;
using WebIServices.ITask;
using WebServiceClass.Helper;
using static WebProjectTest.Common.Message;
using MyNamespace;
using SqlSugar.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl.Matchers;

namespace WebServiceClass.QuartzTask
{
    public class TaskSrevice:IBaseService,ITaskService
    {
        private readonly IScheduler _scheduler;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Dictionary<string, IJobDetail> _jobs = new Dictionary<string, IJobDetail>();
        private readonly Dictionary<string, ITrigger> _triggers = new Dictionary<string, ITrigger>();
        private readonly ISqlHelper _dal;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TaskSrevice(ISqlHelper dal,IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler().Result;
            _cancellationTokenSource = new CancellationTokenSource();
            _dal = dal;
            // 1. 创建监听器实例（注入依赖容器）
            var taskRunListener = new TaskRunListener(serviceScopeFactory);
            // 2. 注册监听器：监听所有任务（或指定任务组，此处全局监听）
            _scheduler.ListenerManager.AddJobListener(
                taskRunListener,
                GroupMatcher<JobKey>.AnyGroup() // 匹配所有任务组
            );
            _scheduler.JobFactory = new ScopedJobFactory(serviceScopeFactory);
        }
        /// <summary>
        /// 开始定时任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> StartAsync(CancellationToken cancellationToken)
        {
            var TimerTask = await _dal.Db.Queryable<sys_timertask>()
            .Where(a => DateTime.Now >= a.BeginTime && DateTime.Now < a.EndTime)
            .Where(a => a.isDelete == 1 && (a.IsStart == 0|| a.IsStart == 2))
            .ToListAsync();

            if (TimerTask.Count == 0)
            {
                Console.WriteLine($"没有需要启动的定时任务");
                using (var customLogger = new LoggerHelper(moduleName: "TaskLogger"))
                {
                    customLogger.LogInfo("没有需要启动的定时任务");
                    
                }
                return Fail<string>("没有需要启动的定时任务");
            }
            else
            {
                var Job = Assembly.Load($"WebTaskClass");
                var Jobclass = Job.GetTypes();
                foreach (var itemTimer in TimerTask)
                {
                    var itemJob = Jobclass.FirstOrDefault(a => a.Name.Contains(itemTimer.TimerClass));
                    if (itemJob == null)
                    {
                        Console.WriteLine($"定时服务：{itemTimer.TimerClass}找不到工作类");
                        continue;
                    }

                    // 配置Job和Trigger
                    var job = JobBuilder.Create(itemJob)
                                        .WithIdentity(itemTimer.Id.ToString())
                                        .Build();
                    var trigger = TriggerBuilder.Create()
                                                //.WithSimpleSchedule(x => x
                                                //    .WithIntervalInSeconds(5) // 每5秒执行一次
                                                //    .RepeatForever())
                                                .WithIdentity($"{itemJob.Name}-trigger")
                                                .WithCronSchedule(itemTimer.Corn)
                                                .Build();
                    _jobs[itemTimer.Id.ToString()] = job;
                    _triggers[itemTimer.Id.ToString()] = trigger;
                    await _scheduler.ScheduleJob(job, trigger, cancellationToken);
                    Console.WriteLine($"成功注册任务: {itemJob.Name}, Cron: {itemTimer.Corn}");
                }
                // 启动调度器
                await _scheduler.Start(cancellationToken);
                await _dal.Db.Updateable<sys_timertask>().SetColumns(a => new sys_timertask { IsStart = 1 }) .Where(a => DateTime.Now >= a.BeginTime && DateTime.Now < a.EndTime)
                        .Where(a => a.isDelete == 1 && (a.IsStart == 0 || a.IsStart == 2))
                        .ExecuteCommandAsync();
                Console.WriteLine($"定时任务已经启动");
                using (var customLogger = new LoggerHelper(moduleName: "TaskLogger",logFileName:"定时任务日志"))
                {
                    customLogger.LogInfo("定时任务已经启动");

                }
                return Success("定时任务开始执行");
            }
            
        }

        /// <summary>
        /// 停止定时任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> StopAsync(CancellationToken cancellationToken)
        {
            if (_scheduler.IsStarted)
            {
                //await _scheduler.Shutdown(true, TimeSpan.FromSeconds(5), cancellationToken);
                await _scheduler.Shutdown(true, cancellationToken);
            }

            _cancellationTokenSource.Cancel();

            var TimerTask = await _dal.Db.Updateable<sys_timertask>().SetColumns(a=>new sys_timertask { IsStart = 0})
                .Where(a => a.isDelete == 1 &&( a.IsStart == 1|| a.IsStart ==2))
                .ExecuteCommandAsync();
            Console.WriteLine($"定时任务已经停止");
            using (var customLogger = new LoggerHelper(moduleName: "TaskLogger",logFileName:"定时任务日志"))
            {
                customLogger.LogInfo("定时任务全部关闭");

            }
            return Success("定时任务全部关闭");
        }

        // 添加新的方法来动态创建和管理任务
        /// <summary>
        /// 添加一个新的定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobType"></param>
        /// <param name="cronExpression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> AddJobAsync(string jobId,string jobName, string cronExpression, CancellationToken cancellationToken)
        {
            var Job = Assembly.Load($"WebTaskClass");
            var Jobclass = Job.GetTypes().FirstOrDefault(type => type.Name == jobName);
            // 创建JobDetail
            if (Jobclass == null) { Console.WriteLine($"未找到该定时任务"); return Fail<string>("未找到该定时任务"); }
            var job = JobBuilder.Create(Job.GetTypes().FirstOrDefault(type =>type.Name == jobName))
                                .WithIdentity(jobId)
                                .Build();
            _jobs[jobId] = job;

            // 创建Trigger
            var trigger = TriggerBuilder.Create()
                                        .WithIdentity($"{jobId}-trigger")
                                        .WithCronSchedule(cronExpression)
                                        .Build();
            _triggers[jobId] = trigger;

            // 将Job和Trigger加入调度器
            await _scheduler.ScheduleJob(job, trigger, cancellationToken);
            
            // 启动调度器
            await _scheduler.Start(cancellationToken);
            await _dal.Db.Updateable<sys_timertask>().SetColumns(a => a.IsStart == 1).Where(a => a.Id == jobId.ObjToInt()).ExecuteCommandAsync();
            Console.WriteLine($"添加一个新的定时任务，重新启动定时器");
            using (var customLogger = new LoggerHelper(moduleName: "TaskLogger"))
            {
                customLogger.LogInfo("添加一个新的定时任务，重新启动定时器");

            }
            return Success("已添加！！！重新启动定时器");
        }

        /// <summary>
        /// 移除定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> RemoveJobAsync(string jobId, CancellationToken cancellationToken)
        {
            var jobKey = new JobKey(jobId);
            var triggerKey = new TriggerKey($"{jobId}-trigger");
            if (await _scheduler.CheckExists(triggerKey) || await _scheduler.CheckExists(jobKey, cancellationToken))
            {
                await _scheduler.PauseTrigger(triggerKey, cancellationToken);
                await _scheduler.UnscheduleJob(triggerKey, cancellationToken);
                await _scheduler.DeleteJob(jobKey, cancellationToken);

                _jobs.Remove(jobId);
                _triggers.Remove(jobId);
                 Console.WriteLine($"移除一个定时任务");
                using (var customLogger = new LoggerHelper(moduleName: "TaskLogger"))
                {
                    customLogger.LogInfo("移除一个定时任务");

                }
                 await _dal.Db.Updateable<sys_timertask>().SetColumns(a => a.IsStart == 0).Where(a => a.Id == jobId.ObjToInt()).ExecuteCommandAsync();
                return Success("该定时任务已经移除");
            }else
            {
                 return Fail<string>( $"该定时任务未运行");
            }
        }

        /// <summary>
        /// 暂停定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> PauseJobAsync(string jobId, CancellationToken cancellationToken)
        {
             var jobKey = new JobKey(jobId);
            var triggerKey = new TriggerKey($"{jobId}-trigger");
            if (await _scheduler.CheckExists(triggerKey) || await _scheduler.CheckExists(jobKey, cancellationToken))
            {
                await _scheduler.PauseTrigger(triggerKey, cancellationToken);
                await _scheduler.UnscheduleJob(triggerKey, cancellationToken);
                 await _scheduler.PauseJob(jobKey, cancellationToken);
                Console.WriteLine($"Job with ID '{jobId}' paused.");
                Console.WriteLine($"该{jobId}定时任务已经暂停");
                 await _dal.Db.Updateable<sys_timertask>().SetColumns(a => a.IsStart == 2).Where(a => a.Id == jobId.ObjToInt()).ExecuteCommandAsync();
                return Success("该定时任务已经暂停");
            }
            else
            {
                Console.WriteLine($"Job with ID '{jobId}' not found.");
                return Fail<string>($"没找到该任务"); 
            }
        }

        /// <summary>
        /// 恢复定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> ResumeJobAsync(string jobId, CancellationToken cancellationToken)
        {
            var jobKey = new JobKey(jobId);
            var triggerKey = new TriggerKey($"{jobId}-trigger");
            if (await _scheduler.CheckExists(triggerKey) || await _scheduler.CheckExists(jobKey, cancellationToken))
            {
                await _scheduler.ResumeTrigger(triggerKey, cancellationToken);
                await _scheduler.ResumeJob(jobKey, cancellationToken);
                var TimerTask = await _dal.Db.Updateable<sys_timertask>().SetColumns(a => new sys_timertask { IsStart = 1 })
                    .Where(a => a.Id == jobId.ObjToInt())
                    .ExecuteCommandAsync();
                Console.WriteLine($"Job with ID '{jobId}' resumed.");
                Console.WriteLine($"该{jobId}定时任务已经恢复");
               return Success("该定时任务已经恢复");
            }
            else
            {
                Console.WriteLine($"Job with ID '{jobId}' not found.");
                 return Fail<string>($"该找到该任务");
            }
        }

        public async Task<ApiPageResponse<List<sys_timertask>>> GetTimerTaskListAsync(string? jobName, int pageIndex, int pageSize, RefAsync<int> totalCount)
        {
            var query = _dal.Db.Queryable<sys_timertask>();
            if (!string.IsNullOrEmpty(jobName))
                query = query.Where(x => x.TimerName.Contains(jobName));
            var list = await query.OrderBy(x => x.Id, OrderByType.Desc)
                                  .ToPageListAsync(pageIndex, pageSize, totalCount);
            return PageSuccess(list, totalCount);
        }

        public async Task<ApiResponse<bool>> AddTimerTaskAsync(sys_timertask task)
        {
            var result = await _dal.Db.Insertable(task).ExecuteCommandAsync() > 0;
            return Success(result, result ? "新增成功" : "新增失败");
        }

        public async Task<ApiResponse<bool>> UpdateTimerTaskAsync(sys_timertask task)
        {
            var result = await _dal.Db.Updateable(task).ExecuteCommandAsync() > 0;
            return Success(result, result ? "修改成功" : "修改失败");
        }

        public async Task<ApiResponse<bool>> DeleteTimerTaskAsync(long taskId)
        {
            var task = await _dal.Db.Queryable<sys_timertask>().FirstAsync(a=>a.Id == taskId);
            if (task == null) return Error<bool>("没有该定时任务");
            if(task.IsStart == 1)return Error<bool>("定时任务正在运行，请勿删除");
            var result = await _dal.Db.Deleteable<sys_timertask>().Where(x => x.Id == taskId).ExecuteCommandAsync() > 0;
            return Success(result, result ? "删除成功" : "删除失败");
        }

        public async Task<ApiResponse<sys_timertask>> GetTimerTaskDetailAsync(long taskId)
        {
            var entity = await _dal.Db.Queryable<sys_timertask>().FirstAsync(x => x.Id == taskId);
            return Success(entity, entity != null ? "查询成功" : "未找到记录");
        }
    }
}
