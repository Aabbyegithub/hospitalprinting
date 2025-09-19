using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyNamespace;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System.Reflection;
using WebIServices.IBase;
using WebServiceClass.Helper;

namespace WebServiceClass.QuartzTask
{
    public class QuartzHostedService:IHostedService,IDisposable
    {
        private readonly IScheduler _scheduler;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public QuartzHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new StdSchedulerFactory();
            _scheduler = factory.GetScheduler().Result;
            _cancellationTokenSource = new CancellationTokenSource();
            _serviceScopeFactory = serviceScopeFactory;

            // 1. 创建监听器实例（注入依赖容器）
            var taskRunListener = new TaskRunListener(serviceScopeFactory);
            // 2. 注册监听器：监听所有任务（或指定任务组，此处全局监听）
            _scheduler.ListenerManager.AddJobListener(
                taskRunListener,
                GroupMatcher<JobKey>.AnyGroup() // 匹配所有任务组
            );
            // 配置Quartz使用我们的依赖注入容器
            //_scheduler.Context.Put("ServiceScopeFactory", _serviceScopeFactory);
            _scheduler.JobFactory = new ScopedJobFactory(serviceScopeFactory);
        }

        /// <summary>
        /// 开始定时任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync( CancellationToken cancellationToken)
        {
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var _dal = scope.ServiceProvider.GetRequiredService<ISqlHelper>();
                var TimerTask = await _dal.Db.Queryable<sys_timertask>()
                    .Where(a =>DateTime.Now >= a.BeginTime && DateTime.Now < a.EndTime)
                    .Where(a => a.isDelete == 1 && a.IsStart == 1)
                    .ToListAsync();

                if (TimerTask.Count == 0)
                {
                    using (var customLogger = new LoggerHelper(moduleName: "TaskLogger", logFileName: "定时任务日志"))
                    {
                        customLogger.LogInfo("没有需要启动的定时任务");

                    }
                    Console.WriteLine($"没有需要启动的定时任务");
                }
                else
                {
                    var Job = Assembly.Load($"WebTaskClass");
                    var Jobclass = Job.GetTypes()
                    .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
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

                        await _scheduler.ScheduleJob(job, trigger, cancellationToken);
                        Console.WriteLine($"成功注册任务: {itemJob.Name}, Cron: {itemTimer.Corn}");
                    }
                    // 启动调度器
                    await _scheduler.Start(cancellationToken);
                    Console.WriteLine($"定时任务已经启动");
                    using (var customLogger = new LoggerHelper(moduleName: "TaskLogger", logFileName: "定时任务日志"))
                    {
                        customLogger.LogInfo("定时任务已经启动");

                    }
                }
            }
  
        }

        /// <summary>
        /// 停止定时任务
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_scheduler.IsStarted)
            {
                //await _scheduler.Shutdown(true, TimeSpan.FromSeconds(5), cancellationToken);
                await _scheduler.Shutdown(true, cancellationToken);
            }

            _cancellationTokenSource.Cancel();
            Console.WriteLine($"定时任务已经停止");
            //if (_scheduler.IsStarted)
            //{
            //    // 如果你想等待所有作业完成
            //    await _scheduler.Shutdown(true, cancellationToken);

            //    // 或者，如果你想等待一段时间，然后尝试关闭（这需要自定义实现等待逻辑）
            //    // 注意：以下代码是示例逻辑，需要根据实际情况调整
            //    if (await WaitForJobsToFinishAsync(TimeSpan.FromSeconds(5), cancellationToken))
            //    {
            //        await _scheduler.Shutdown(true, cancellationToken);
            //    }
            //    else
            //    {
            //        // 处理超时或取消的情况
            //    }
            //}

            //_cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _scheduler?.Shutdown();
            _cancellationTokenSource.Dispose();
        }
    }


}
