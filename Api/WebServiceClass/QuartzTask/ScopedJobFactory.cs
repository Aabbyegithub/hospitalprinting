using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.QuartzTask
{
    public class ScopedJobFactory : IJobFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedJobFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var scope = _serviceScopeFactory.CreateScope();
            var job = scope.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            return job;//new ScopedJob(job, scope);
        }

        public void ReturnJob(IJob job)
        {
            // 如果作业实现了IDisposableJob，它将负责释放作用域
            if (job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    // 可选接口，用于作业管理自己的作用域
    public interface IDisposableJob : IJob, IDisposable
    {
        IServiceScope Scope { get; set; }
    }

    //public class ScopedJob : IJob, IDisposable
    //{
    //    private readonly IJob _job;
    //    private readonly IServiceScope _scope;

    //    public ScopedJob(IJob job, IServiceScope scope)
    //    {
    //        _job = job;
    //        _scope = scope;
    //    }

    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        return _job.Execute(context);
    //    }

    //    public void Dispose()
    //    {
    //        _scope?.Dispose();
    //    }
    //}
}
