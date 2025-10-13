using ModelClassLibrary.Model.Dto.TaskDto;
using MyNamespace;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebTaskClass.Common;

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
            // 验证所有路径是否存在
            var validPaths = _monitorPaths.Where(Directory.Exists).ToList();
            var invalidPaths = _monitorPaths.Except(validPaths).ToList();

            // 记录无效路径日志
            if (invalidPaths.Any())
            {
                Console.WriteLine($"警告：以下路径不存在或无法访问：{string.Join("; ", invalidPaths)}");
            }

            if (!validPaths.Any())
            {
                Console.WriteLine("没有有效的监听路径，任务终止。");
                return;
            }

            // 2. 初始化监听服务
            using var reportMonitor = new PdfReportMonitor();
            reportMonitor.AddMonitorPaths();
            reportMonitor.OnReportParsed += HandleParsedReport;

            //启动监听
            try
            {
                // 启动监听（设置监听时长，避免任务无限阻塞）
                reportMonitor.StartMonitoring();

                // 关键：设置监听持续时间（根据Quartz触发器间隔调整）
                // 监听30秒后自动停止，等待下一次定时任务触发
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
            finally
            {
                // 确保监听服务停止
                reportMonitor.StopMonitoring();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 文件解析任务执行结束。");
            }

        }

        /// <summary>
        ///保存解析文件
        /// </summary>
        /// <param name="report"></param>
        static void HandleParsedReport(ReportInfo report)
        {

        }
    }
}
