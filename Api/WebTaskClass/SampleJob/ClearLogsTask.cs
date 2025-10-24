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
        private readonly ILoggerHelper _logger;
        private readonly string _baseLogDirectory = "LogInfos";
        private readonly int _keepDays = 7; // 默认保留7天日志
        public ClearLogsTask(ISqlHelper dal, ILoggerHelper logger)
        {
            _dal = dal;
            _logger = logger;
        }
        /// <summary>
        /// 只保留一个月日志
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            await _dal.Db.Deleteable<lq_operationlog>().Where(a => a.AddTime <= DateTime.Now.AddDays(-30)).ExecuteCommandAsync();

            try
            {
                if (!Directory.Exists(_baseLogDirectory))
                {
                    await _logger.LogWarning($"日志根目录不存在：{_baseLogDirectory}，无需清理");
                    return;
                }

                DateTime expireDate = DateTime.Now.AddDays(-_keepDays);
                await _logger.LogInfo($"清理规则：删除 {expireDate:yyyy-MM-dd} 及之前的日志文件","清理日志文件");

                await CleanExpiredLogDirectory(_baseLogDirectory, expireDate);

                await _logger.LogInfo("===== 日志清理任务执行完成 =====","清理日志文件");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"日志清理任务执行失败：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 递归清理过期日志目录和文件
        /// </summary>
        /// <param name="targetDirectory">目标目录</param>
        /// <param name="expireDate">过期日期</param>
        private async Task CleanExpiredLogDirectory(string targetDirectory, DateTime expireDate)
        {
            // 遍历目录下所有子目录（第一层为日期目录：yyyy-MM-dd）
            foreach (var dateDir in Directory.GetDirectories(targetDirectory))
            {
                // 解析日期目录名称（格式：yyyy-MM-dd）
                if (!DateTime.TryParse(Path.GetFileName(dateDir), out DateTime dirDate))
                {
                    await _logger.LogWarning($"跳过非日期格式目录：{dateDir}");
                    continue;
                }

                // 1. 若目录日期过期，删除整个目录（包含所有子目录和文件）
                if (dirDate <= expireDate)
                {
                    try
                    {
                        // 统计目录内文件数量，便于日志记录
                        int fileCount = Directory.GetFiles(dateDir, "*.*", SearchOption.AllDirectories).Length;
                        Directory.Delete(dateDir, recursive: true); // recursive=true：删除所有子目录
                        await _logger.LogInfo($"成功删除过期日志目录：{dateDir}，包含文件数：{fileCount}", "清理日志文件");
                    }
                    catch (Exception ex)
                    {
                        // 单个目录删除失败不影响其他目录，仅记录日志
                        await _logger.LogError($"删除过期目录失败：{dateDir}，原因：{ex.Message}");
                    }
                }
            }
        }
    }
}
