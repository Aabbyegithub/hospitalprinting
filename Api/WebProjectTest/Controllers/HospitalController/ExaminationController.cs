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

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ExaminationController(IExaminationServices examService, ISqlHelper dal, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 新增检查记录
        /// </summary>
        [HttpPost]
        [OperationLogFilter("检查管理", "新增检查数据", ActionType.Add)]
        public async Task<ApiResponse<string>> AddAsync([FromBody] HolExamination exam)
        {
            return await examService.AddExaminationAsync(exam, OrgId, UserId);
        }

        /// <summary>
        /// 分页查询检查数据
        /// </summary>
        [HttpGet]
        [OperationLogFilter("检查管理", "分页查询检查数据", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolExamination>>> GetPageAsync(string? examNo, string? patientName, DateTime? examDate, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            var res = await examService.GetExaminationPageAsync(examNo, patientName, examDate, page, size, count, OrgId);
            return PageSuccess(res, count);
        }

        /// <summary>
        /// 修改检查记录
        /// </summary>
        [HttpPost]
        [OperationLogFilter("检查管理", "修改检查数据", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateAsync([FromBody] HolExamination exam)
        {
            return await examService.UpdateExaminationAsync(exam, UserId);
        }

        /// <summary>
        /// 删除检查记录（逻辑删除+日志追溯）
        /// </summary>
        [HttpPost]
        [OperationLogFilter("检查管理", "删除检查数据", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync([FromBody] List<long> ids)
        {
            return await examService.DeleteExaminationAsync(ids, UserId, OrgId);
        }

        /// <summary>
        /// 打印检查报告（仅支持单次打印）
        /// </summary>
        [HttpPost]
        [OperationLogFilter("检查管理", "打印报告", ActionType.Print)]
        public async Task<ApiResponse<string>> PrintAsync([FromQuery] long examId)
        {
            return await examService.PrintExaminationAsync(examId);
        }

        /// <summary>
        /// 解锁打印状态（管理员操作）
        /// </summary>
        [HttpPost]
        [OperationLogFilter("检查管理", "解锁打印", ActionType.Edit)]
        public async Task<ApiResponse<string>> UnlockPrintAsync([FromQuery] long examId)
        {
            return await examService.UnlockPrintAsync(examId);
        }
    }
}
