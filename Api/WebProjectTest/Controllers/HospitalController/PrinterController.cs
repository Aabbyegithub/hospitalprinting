using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using ModelClassLibrary.Model.HolModel;
using WebIServices.IServices.HospitalIServices;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PrinterController(IPrinterServices printerService, ISqlHelper dal, IRedisCacheService redisCacheService)
    : AutherController(redisCacheService)
    {
        /// <summary>新增打印机</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "新增打印机", ActionType.Add)]
        public async Task<ApiResponse<string>> AddAsync([FromBody] HolPrinter printer)
            => await printerService.AddPrinterAsync(printer, OrgId, UserId);

        /// <summary>分页查询打印机</summary>
        [HttpGet]
        [OperationLogFilter("打印管理", "分页查询打印机", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolPrinter>>> GetPageAsync(string? name, int? type, int? status, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            var res = await printerService.GetPrinterPageAsync(name, type, status, page, size, count, OrgId);
            return PageSuccess(res, count);
        }

        /// <summary>修改打印机</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "修改打印机", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateAsync([FromBody] HolPrinter printer)
            => await printerService.UpdatePrinterAsync(printer, UserId);

        /// <summary>删除打印机</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "删除打印机", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync([FromBody] List<long> ids)
            => await printerService.DeletePrinterAsync(ids, UserId, OrgId);

        /// <summary>切换启用状态</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "切换打印机状态", ActionType.Edit)]
        public async Task<ApiResponse<string>> ToggleStatusAsync([FromQuery] long id, [FromQuery] int status)
            => await printerService.TogglePrinterStatusAsync(id, status, UserId);

        /// <summary>测试打印</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "测试打印", ActionType.Print)]
        public async Task<ApiResponse<string>> TestPrintAsync([FromQuery] long id)
            => await printerService.TestPrintAsync(id, UserId);
    }
}
