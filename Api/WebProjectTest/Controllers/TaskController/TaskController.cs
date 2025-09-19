using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNamespace;
using SqlSugar;
using WebIServices.ITask;
using WebProjectTest.Common.Filter;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.TaskController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        /// <summary>
        /// 开启定时服务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Open)]
        public async Task<ApiResponse<string>> StartTaskAsync()
        {
            return await _taskService.StartAsync(new CancellationToken());

        }

        /// <summary>
        /// 关闭定时服务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Close)]
        public async Task<ApiResponse<string>> StopTaskAsync()
        {
            return await _taskService.StopAsync(new CancellationToken());

        }

        /// <summary>
        /// 任务管理器新添加一条调度任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Add)]
        public async Task<ApiResponse<string>> AddJobAsync(string jobId, string jobName, string cronExpression)
        {
            return await _taskService.AddJobAsync(jobId, jobName, cronExpression, default);

        }

        /// <summary>
        /// 任务管理器移除一个定时任务
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Remove)]
        public async Task<ApiResponse<string>> RemoveJobAsync(string jobId)
        {
            return await _taskService.RemoveJobAsync(jobId, default);

        }

        /// <summary>
        /// 定时任务暂停
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Pause)]
        public async Task<ApiResponse<string>> PauseJobAsync(string jobId)
        {
            return await _taskService.PauseJobAsync(jobId, default);

        }

        /// <summary>
        /// 定时任务恢复运行
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>定时器管理", "启动定时服务", ActionType.Open)]
        public async Task<ApiResponse<string>> ResumeJobAsync(string jobId)
        {
            return await _taskService.ResumeJobAsync(jobId, default);

        }
        

         /// <summary>
        /// 获取定时任务列表（筛选+分页）
        /// </summary>
        [HttpGet]
        [OperationLogFilter("系统管理>定时任务", "定时任务列表查询", ActionType.Search)]
        public async Task<ApiPageResponse<List<sys_timertask>>> GetTimerTaskList(
            string? jobName,
            int pageIndex = 1,
            int pageSize = 10)
        {
            RefAsync<int> totalCount = 0;
            return await _taskService.GetTimerTaskListAsync(jobName, pageIndex, pageSize, totalCount);
        }

        /// <summary>
        /// 新增定时任务
        /// </summary>
        [HttpPost]
        [OperationLogFilter("系统管理>定时任务", "新增定时任务", ActionType.Add)]
        public async Task<ApiResponse<bool>> AddTimerTask([FromBody] sys_timertask task)
        {
            return await _taskService.AddTimerTaskAsync(task);
        }

        /// <summary>
        /// 编辑定时任务
        /// </summary>
        [HttpPost]
        [OperationLogFilter("系统管理>定时任务", "编辑定时任务", ActionType.Edit)]
        public async Task<ApiResponse<bool>> UpdateTimerTask([FromBody] sys_timertask task)
        {
            return await _taskService.UpdateTimerTaskAsync(task);
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        [HttpPost]
        [OperationLogFilter("系统管理>定时任务", "删除定时任务", ActionType.Delete)]
        public async Task<ApiResponse<bool>> DeleteTimerTask(long taskId)
        {
            return await _taskService.DeleteTimerTaskAsync(taskId);
        }

        /// <summary>
        /// 获取定时任务详情
        /// </summary>
        [HttpGet]
        [OperationLogFilter("系统管理>定时任务", "定时任务详情", ActionType.Search)]
        public async Task<ApiResponse<sys_timertask>> GetTimerTaskDetail(long taskId)
        {
            return await _taskService.GetTimerTaskDetailAsync(taskId);
        }
    }
}
