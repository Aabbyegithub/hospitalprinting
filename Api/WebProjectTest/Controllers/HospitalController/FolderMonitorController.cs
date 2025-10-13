using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FolderMonitorController(IFolderMonitorServices folderMonitorService, ISqlHelper dal, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 新增文件夹监听配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("文件夹监听", "新增文件夹监听配置", ActionType.Add)]
        public async Task<ApiResponse<string>> AddAsync([FromBody] HolFolderMonitor folderMonitor)
        {
            return await folderMonitorService.AddFolderMonitorAsync(folderMonitor, OrgId, UserId);
        }

        /// <summary>
        /// 分页查询文件夹监听配置
        /// </summary>
        [HttpGet]
        [OperationLogFilter("文件夹监听", "分页查询文件夹监听配置", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolFolderMonitor>>> GetPageAsync(string? name, string? ipAddress, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            var res = await folderMonitorService.GetFolderMonitorPageAsync(name, ipAddress, page, size, count, OrgId);
            return PageSuccess(res, count);
        }

        /// <summary>
        /// 修改文件夹监听配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("文件夹监听", "修改文件夹监听配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateAsync([FromBody] HolFolderMonitor folderMonitor)
        {
            return await folderMonitorService.UpdateFolderMonitorAsync(folderMonitor, UserId);
        }

        /// <summary>
        /// 删除文件夹监听配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("文件夹监听", "删除文件夹监听配置", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync([FromBody] List<long> ids)
        {
            return await folderMonitorService.DeleteFolderMonitorAsync(ids, UserId, OrgId);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        [HttpPost]
        [OperationLogFilter("文件夹监听", "切换状态", ActionType.Edit)]
        public async Task<ApiResponse<string>> ToggleStatusAsync([FromQuery] long id)
        {
            return await folderMonitorService.ToggleStatusAsync(id, UserId);
        }
    }
}
