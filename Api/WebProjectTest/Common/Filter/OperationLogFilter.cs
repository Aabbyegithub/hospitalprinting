using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ModelClassLibrary.Model.AutherModel.AutherDto;
using MyNamespace;
using Newtonsoft.Json;
using WebIServices.IBase;
using static ModelClassLibrary.Model.CommonEnmFixts;

namespace WebProjectTest.Common.Filter
{
    /// <summary>
    /// 操作日志过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class OperationLogFilter : Attribute, IAsyncActionFilter
    {
        private readonly string _operationModel;
        private readonly string _Description;
        private readonly ActionType _ActionType;
        public OperationLogFilter(string operationModel, string description, ActionType action = ActionType.Search)
        {
            _operationModel = operationModel;
            _Description = description;
            _ActionType = action;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var dal = serviceProvider.GetRequiredService<ISqlHelper>();
            var redis = serviceProvider.GetRequiredService<IRedisCacheService>();
            var request = context.HttpContext.Request;
            var startTime = DateTime.Now;
            var UserContext = new UserContext();



            var authorizationHeader = request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                // 提取 token
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                // 使用 token 作为键从 Redis 中获取保存的用户信息
                var userBaseJson = await redis.GetStringAsync(token);

                if (!string.IsNullOrEmpty(userBaseJson))
                {
                    // 反序列化为 UserContext 类型
                    UserContext = JsonConvert.DeserializeObject<UserContext>(userBaseJson);
                }
            }
            // 执行后续管道
            var executedContext = await next();

            await dal.Db.Insertable(new lq_operationlog
            {
                ActionType = _ActionType,
                ModuleName = _operationModel,
                OrgId = (int)UserContext?.OrgId,
                AddUserId = (int)UserContext?.UserId,
                UpUserId = (int)UserContext?.UserId,
                UserId = (int)UserContext?.UserId,
                Description = _Description,
                ActionContent = JsonConvert.SerializeObject(context.ActionArguments)
            }).ExecuteCommandAsync();

            // 记录响应信息
            var response = new
            {
                executedContext.HttpContext.Response.StatusCode,
                Result = executedContext.Result is ObjectResult objectResult
                    ? objectResult.Value
                    : null
            };
        }
    }
}
