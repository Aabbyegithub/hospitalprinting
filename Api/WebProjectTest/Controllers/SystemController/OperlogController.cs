using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNamespace;
using SqlSugar;
using SqlSugar.Extensions;
using WebIServices.IBase;
using WebProjectTest.Common.Filter;
using WebServiceClass.Helper;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.SystemController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OperlogController(IRedisCacheService redisCacheService, ISqlHelper dal) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="User"></param>
        /// <param name="actionType"></param>
        /// <param name="ActionModel"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Page"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("系统设置>日志管理", "系统日志分页查询", ActionType.Search)]
        public async Task<ApiPageResponse<List<lq_operationlog>>> GetOperationlogAsync(string? User, ActionType? actionType, string? ActionModel, string? StartTime, string? EndTime, int Page = 1, int Size = 10)
        {
            RefAsync<int> Count = 0;
            var Date = await dal.Db.Queryable<lq_operationlog>().WhereIF(OrgId !=1,a => a.OrgId == OrgId)
                .LeftJoin<sys_user>((a, b) => a.UserId == b.user_id)
                .WhereIF(!string.IsNullOrWhiteSpace(User), (a, b) => b.name.Contains(User))
                .WhereIF(!string.IsNullOrWhiteSpace(ActionModel), (a, b) => a.ModuleName.Contains(ActionModel))
                .WhereIF(actionType.HasValue, (a, b) => a.ActionType == actionType)
                .WhereIF(!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime),
                (a, b) => a.ActionTime >= StartTime.ObjToDate() && a.ActionTime < EndTime.ObjToDate().AddDays(1)).OrderByDescending(a => a.ActionTime)
                .Select((a, b) => new lq_operationlog { AddUser = b.name }, true).ToPageListAsync(Page, Size, Count);
            Date.ForEach(item => { item.ActionTypeName = item.ActionType.GetDescription(); });
            return PageSuccess(Date, Count);
        }
        /// <summary>
        /// 操作日志保存
        /// </summary>
        /// <param name="operationlog"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SaveOperationlogAsync([FromBody] lq_operationlog operationlog)
        {
            await dal.Db.Insertable(new lq_operationlog
            {
                ActionType = operationlog.ActionType,
                ModuleName = operationlog.ModuleName,
                Description = operationlog.Description,
                UserId = UserId,
                ActionTime = DateTime.Now,
                AddUserId = UserId,
                AddTime = DateTime.Now,
                UpTime = DateTime.Now,
                UpUserId = UserId,
                OrgId = OrgId
            }).ExecuteCommandAsync();
        }
    }
}
