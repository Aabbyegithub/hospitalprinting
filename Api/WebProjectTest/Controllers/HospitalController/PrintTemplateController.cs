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
using MyNamespace;
using WebIServices.IServices.HospitalIServices;
using ModelClassLibrary.Model.HolModel;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PrintTemplateController(IPrintTemplateServices templateService, ISqlHelper dal, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 新增打印模板
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印模板管理", "新增打印模板", ActionType.Add)]
        public async Task<ApiResponse<string>> AddAsync([FromBody] HolPrintTemplate template)
        {
            return await templateService.AddPrintTemplateAsync(template, OrgId, UserId);
        }

        /// <summary>
        /// 分页查询打印模板
        /// </summary>
        [HttpGet]
        [OperationLogFilter("打印模板管理", "分页查询打印模板", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolPrintTemplate>>> GetPageAsync(string? name, string? templateType, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            var res = await templateService.GetPrintTemplatePageAsync(name, templateType, page, size, count, OrgId);
            return PageSuccess(res, count);
        }

        /// <summary>
        /// 修改打印模板
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印模板管理", "修改打印模板", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateAsync([FromBody] HolPrintTemplate template)
        {
            return await templateService.UpdatePrintTemplateAsync(template, UserId);
        }

        /// <summary>
        /// 删除打印模板
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印模板管理", "删除打印模板", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync([FromBody] List<long> ids)
        {
            return await templateService.DeletePrintTemplateAsync(ids, UserId, OrgId);
        }

        /// <summary>
        /// 获取模板详情
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<HolPrintTemplate>> GetDetailAsync([FromQuery] long templateId)
        {
            return await templateService.GetTemplateDetailAsync(templateId);
        }

        /// <summary>
        /// 设置默认模板
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印模板管理", "设置默认模板", ActionType.Edit)]
        public async Task<ApiResponse<string>> SetDefaultAsync([FromQuery] long templateId)
        {
            return await templateService.SetDefaultTemplateAsync(templateId, OrgId, UserId);
        }

        /// <summary>
        /// 预览模板
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<string>> PreviewAsync([FromQuery] long templateId, [FromQuery] long examId)
        {
            return await templateService.PreviewTemplateAsync(templateId, examId);
        }

        /// <summary>
        /// 打印模板
        /// </summary>
        [HttpPost]
        [OperationLogFilter("打印模板管理", "打印模板", ActionType.Print)]
        public async Task<ApiResponse<string>> PrintAsync([FromQuery] long templateId, [FromQuery] long examId)
        {
            return await templateService.PrintTemplateAsync(templateId, examId, UserId, OrgId);
        }

        /// <summary>
        /// 获取可用的数据字段
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<List<object>>> GetAvailableFieldsAsync([FromQuery] string templateType)
        {
            return await templateService.GetAvailableFieldsAsync(templateType);
        }
    }
}
