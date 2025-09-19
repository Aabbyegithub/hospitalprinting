using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNamespace;
using SqlSugar;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;
using WebProjectTest.Common.Filter;
using WebServiceClass.Services.SystemService;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.SystemController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OrgController(IRedisCacheService redisCacheService, IOrgServices _StoreServices) : AutherController(redisCacheService)
    {

        /// <summary>
        /// 获取门店列表
        /// </summary>
        /// <param name="StoreName"></param>
        /// <param name="phone"></param>
        /// <param name="address"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("系统设置>门店设置", "门店设置查询", ActionType.Search)]
        public async Task<ApiPageResponse<List<sys_orgid>>> GetStoreListAsync(string? StoreName, string? phone, string? address, int page, int size)
        {
            RefAsync<int> count = 0;
            return await _StoreServices.GetStoreListAsync(StoreName, phone, address,page, size, count);
        }

        /// <summary>
        /// 获取所有门店列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<List<sys_orgid>>> GetAllStoreListAsync()
        {
            return await _StoreServices.GetStoreListAsync(OrgId);
        }

        /// <summary>
        /// 新增门店
        /// </summary>
        /// <param name="sys_Store"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>门店设置", "新增门店", ActionType.Add)]
        public async Task<ApiResponse<bool>> AddStoreAsync([FromBody] sys_orgid sys_Store)
        {
            if (sys_Store == null)
            {
                return Fail<bool>("商店信息不能为空");
            }
            return await _StoreServices.AddStoreAsync(sys_Store);
        }

        /// <summary>
        /// 删除门店'
        /// </summary>
        /// <param name="storeIds"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>门店设置", "删除门店", ActionType.Delete)]
        public async Task<ApiResponse<bool>> DeleteStoreAsync([FromBody] List<int> storeIds)
        {
            if (storeIds == null || !storeIds.Any())
            {
                return Fail<bool>("请选择要删除的商店");
            }
            return await _StoreServices.DeleteStoreAsync(storeIds);
        }

        /// <summary>
        /// 修改门店信息
        /// </summary>
        /// <param name="sys_Store"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>门店设置", "修改门店信息", ActionType.Edit)]
        public async Task<ApiResponse<bool>> UpdateStoreAsync([FromBody] sys_orgid sys_Store)
        {
            if (sys_Store == null)
            {
                return Fail<bool>("商店信息不能为空");
            }
            return await _StoreServices.UpdateStoreAsync(sys_Store);
        }

        /// <summary>
        /// 修改门店状态
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("系统设置>门店设置", "修改门店状态", ActionType.Edit)]
        public async Task<ApiResponse<bool>> UpdateStoreStatusAsync(int storeId, byte status)
        {
            if (storeId <= 0)
            {
                return Fail<bool>("请选择要修改的商店");
            }
            return await _StoreServices.UpdateStoreAsync(storeId, status);
        }
    }
}
