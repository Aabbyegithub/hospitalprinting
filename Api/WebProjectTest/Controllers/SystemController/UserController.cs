using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.AutherModel.AutherDto;
using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using Newtonsoft.Json;
using SqlSugar;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;
using WebIServices.ITask;
using WebProjectTest.Common.Filter;
using WebServiceClass.Base;
using WebServiceClass.Helper;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebProjectTest.Controllers.SystemController
{
    /// <summary>
    /// 验证登陆
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController(IRedisCacheService redisCacheService, ISqlHelper dal, TokenService token,IUserServices _UserService) : AutherController(redisCacheService)
    {
        private readonly TokenService _token = token;

        /// <summary>
        /// 验证登陆
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <param name="orgid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<UserResult>> LoginAsync(string UserName, string PassWord)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(PassWord))
            {
                return Fail<UserResult>("账号或密码不能为空！");
            }
            try
            {
                var res = await _UserService.UserLoginAsync(UserName, PassWord);
                if (res != null)
                {
                    return Success(res);
                }
                return Fail<UserResult>("账号或密码错误！");
            }
            catch (Exception)
            {
                return Error<UserResult>("服务器错误");
            }
        }

        /// <summary>
        /// 退出系统，删除redis相关数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("账号登出", $"人员退出系统", ActionType.Exit)]
        public async Task<ApiResponse<string>> logoutAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                // 提取 token
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var user = await dal.Db.Queryable<sys_user>().FirstAsync(a=>a.user_id == UserId);
                user.last_login_time = DateTime.Now;
                await dal.Db.Updateable(user).ExecuteCommandAsync();
                try
                {
                    await redisCacheService.RemoveAsync(token);
                   
                }
                catch (Exception)
                {
                    await dal.Db.Insertable(new lq_operationlog
                    {
                        ActionType = ActionType.Exit,
                        ModuleName = "账号登出异常",
                        OrgId = OrgId,
                        AddUserId = UserId,
                        UpUserId = UserId,
                        UserId = UserId,
                        Description = "人员退出系统",
                        ActionContent = $"账号：{UserName}员工姓名:{Name}---->账号登出异常请检查redis连接，缓存未及时清理"
                    }).ExecuteCommandAsync();
                }

            }
             return Success("退出成功");

        }
        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("系统设置>员工管理", "用户分页查询", ActionType.Search)]
        public async Task<ApiPageResponse<List<sys_user>>> GetUserAsync(string? name,string? username,string? phone, int page = 0, int size = 10)
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _UserService.GetUserPageAsync(name,username,phone, page, size, count, RoleId, OrgId);
                if (res != null)
                {
                    return PageSuccess(res, count);
                }
                return PageFail<List<sys_user>>("获取数据失败");
            }
            catch (Exception)
            {

                return PageError<List<sys_user>>("服务器错误");
            }
        }

        /// <summary>
        /// 获取员工权限
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiPageResponse<List<UserPermission>>> GetUserMenuAsync()
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _UserService.GetUserPermissionsAsync((int)RoleId);
                if (res != null)
                {
                    return PageSuccess(res, count);
                }
                return PageFail<List<UserPermission>>("获取数据失败");
            }
            catch (Exception e)
            {

                return PageError<List<UserPermission>>($"服务器错误{e.Message}");
            }
        }

        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [OperationLogFilter("系统设置>员工管理", "添加新员工", ActionType.Add)]
        public async Task<ApiResponse<string>> AddUserAsync([FromBody] sys_user User)
        {
            return await _UserService.AddUserAsync(User,OrgId);
        }

        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>员工管理", "修改员工信息", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpUserAsync([FromBody] sys_user User)
        {
            return await _UserService.UpUserAsync(User);
        }

        /// <summary>
        /// 注销账号
        /// </summary>
        [HttpPost]
        [OperationLogFilter("系统设置>员工管理", "删除员工账号", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteAsync(List<int> Ids)
        {
            return await _UserService.DeleteUserAsync(Ids);
        }

        /// <summary>
        /// 获取个人信息
        /// </summary>
        [HttpGet]
        [OperationLogFilter("系统设置>员工管理", "查看个人信息", ActionType.Search)]
        public async Task<ApiResponse<sys_user>> GetUserDetialAsync()
        {
            return await _UserService.GetUserDetialAsync(UserId);
        }
    }
}
