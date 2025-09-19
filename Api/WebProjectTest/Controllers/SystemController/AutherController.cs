using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.AutherModel.AutherDto;
using Newtonsoft.Json;
using SqlSugar;
using System.Security.Claims;
using WebIServices.IBase;

namespace WebProjectTest.Controllers.SystemController
{
    /// <summary>
    /// 获取服务器和人员信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AutherController : ControllerBase
    {
        private readonly IRedisCacheService _redisCacheService;

        public AutherController(IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        protected UserContext UserContext
        {
            get
            {
                // 从请求头中获取 Authorization 字段
                var authorizationHeader = Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    // 提取 token
                    var token = authorizationHeader.Substring("Bearer ".Length).Trim();

                    // 使用 token 作为键从 Redis 中获取保存的用户信息
                    var userBaseJson = _redisCacheService.GetStringAsync(token).Result;

                    if (!string.IsNullOrEmpty(userBaseJson))
                    {
                        // 反序列化为 UserContext 类型
                        return JsonConvert.DeserializeObject<UserContext>(userBaseJson);
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// 用户id
        /// </summary>
        protected int UserId => (int)UserContext?.UserId;
        /// <summary>
        /// 用户名
        /// </summary>
        protected string UserName => UserContext?.UserName;
        /// <summary>
        /// 姓名
        /// </summary>
        protected string Name => UserContext?.Name;
        /// <summary>
        /// 组织
        /// </summary>
        protected int OrgId => (int)UserContext?.OrgId;
        /// <summary>
        /// 组织名称
        /// </summary>
        protected string OrgName => UserContext?.OrgName;
        /// <summary>
        /// 哈希密码
        /// </summary>
        protected string PassWord => UserContext?.PassWord;
        /// <summary>
        /// 盐值
        /// </summary>
        protected string Salt => UserContext?.Salt;
        /// <summary>
        /// 角色
        /// </summary>
        protected int? RoleId => UserContext?.RoleId;

    }
}
