using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using WebIServices.IServices.HospitalIServices;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PrintRecordController(IPrintRecordServices printService, IRedisCacheService redisCacheService)
        : AutherController(redisCacheService)
    {
        /// <summary>
        /// 添加打印记录
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印记录管理", "新增打印记录", ActionType.Add)]
        public async Task<ApiResponse<string>> AddAsync([FromBody] PrintRecordModel record)
        {
            return await printService.AddAsync(record);
        }

        /// <summary>
        /// 修改打印记录
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印记录管理", "修改打印记录", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateAsync([FromBody] PrintRecordModel record)
        {
            return await printService.UpdateAsync(record);
        }

        /// <summary>
        /// 删除打印记录
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印记录管理", "删除打印记录", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync([FromBody] List<long> ids)
        {
            return await printService.DeleteAsync(ids);
        }

        /// <summary>
        /// 打印记录分页查询
        /// </summary>
        [HttpGet]
        [OperationLogFilter("打印记录管理", "分页查询打印记录", ActionType.Search)]
        public async Task<ApiPageResponse<List<PrintRecordModel>>> GetPageAsync(long? examId, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            var res = await printService.GetPageAsync(examId, page, size, count);
            return PageSuccess(res, count);
        }
    }
}
