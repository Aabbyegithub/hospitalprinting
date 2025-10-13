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
using ModelClassLibrary.Model.Dto.HolDto;

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

        /// <summary>获取打印机配置</summary>
        [HttpGet]
        [OperationLogFilter("打印管理", "获取打印机配置", ActionType.Search)]
        public async Task<ApiResponse<HolPrinterConfigDto?>> GetConfig([FromQuery] long printerId)
        {
            var cfg = await printerService.GetConfigAsync(printerId, OrgId);
            return Success(cfg);
        }

        /// <summary>保存打印机配置</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "保存打印机配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> SaveConfig([FromBody] HolPrinterConfigDto dto)
        {
            return await printerService.SaveConfigAsync(dto, OrgId, UserId);
        }

        /// <summary>获取胶片尺寸配置</summary>
        [HttpGet]
        [OperationLogFilter("打印管理", "获取胶片尺寸配置", ActionType.Search)]
        public async Task<ApiResponse<List<HolPrinterConfigDto>>> GetFilmSizeConfigs([FromQuery] long printerId)
        {
            var configs = await printerService.GetFilmSizeConfigsAsync(printerId, OrgId);
            return Success(configs);
        }

        /// <summary>保存胶片尺寸配置</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "保存胶片尺寸配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> SaveFilmSizeConfig([FromBody] HolPrinterConfigDto dto)
        {
            return await printerService.SaveFilmSizeConfigAsync(dto, OrgId, UserId);
        }

        /// <summary>删除打印机的所有胶片尺寸配置</summary>
        [HttpPost]
        [OperationLogFilter("打印管理", "删除胶片尺寸配置", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAllFilmSizeConfigs([FromQuery] long printerId)
        {
            return await printerService.DeleteAllFilmSizeConfigsAsync(printerId, UserId, OrgId);
        }
    }
}
