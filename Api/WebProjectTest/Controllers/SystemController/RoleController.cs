using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using SqlSugar;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;
using WebProjectTest.Common.Filter;
using WebServiceClass.Services.UserService;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.SystemController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoleController(IRedisCacheService redisCacheService, IRoleServices _RoleServices) : AutherController(redisCacheService)
    {

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("系统设置>角色管理", "角色管理查询", ActionType.Search)]
        public async Task<ApiPageResponse<List<sys_role>>> GetRoleListAsync(string? RoleName, int page = 0, int size = 10)
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _RoleServices.GetRoleListAsync(page, size, count, RoleName);
                if (res != null)
                {
                    return PageSuccess(res, count);
                }
                return PageFail<List<sys_role>>("获取数据失败");
            }
            catch (Exception)
            {

                return PageError<List<sys_role>>("服务器错误");
            }
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<sys_role>>> GetAllRoleListAsync()
        {
            try
            {
                var res = await _RoleServices.GetRoleListAsync();
                if (res != null)
                {
                    return Success(res);
                }
                return Fail<List<sys_role>>("获取数据失败");
            }
            catch (Exception)
            {

                return Error<List<sys_role>>("服务器错误");
            }
        }

        /// <summary>
        /// 添加新角色
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns> 
        [HttpPost]
        [OperationLogFilter("系统设置>角色管理", "新增角色", ActionType.Add)]
        public async Task<ApiResponse<bool>> AddRoleAsync([FromBody] sys_role sys_Role)
        {
            try
            {
                await _RoleServices.AddRoleAsync(sys_Role);
                return Success(true);
            }
            catch (Exception)
            {
                return Error<bool>("新增失败");
            }

        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>角色管理", "修改角色", ActionType.Edit)]
        public async Task<ApiResponse<bool>> UpdateRoleAsync([FromBody] sys_role sys_Role)
        {
            try
            {
                await _RoleServices.UpdateRoleAsync(sys_Role);
                return Success(true);
            }
            catch (Exception)
            {
                return Error<bool>("修改失败");
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpDelete("{roleId}")]
        [OperationLogFilter("系统设置>角色管理", "删除角色", ActionType.Delete)]
        public async Task<ApiResponse<bool>> DeleteRoleAsync(int roleId)
        {
            try
            {
                await _RoleServices.DeleteRoleAsync(roleId);
                return Success(true);
            }
            catch (Exception)
            {
                return Error<bool>("删除失败");
            }
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<UserPermission>>> GetUserPermissionsAsync(int role_id)
        {
            try
            {
               var res = await _RoleServices.GetUserPermissionsAsync(role_id);
                return Success(res);
            }
            catch (Exception)
            {
                return Error<List<UserPermission>>("获取失败");
            }
        }

        /// <summary>
        /// 修改角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("系统设置>角色管理", "修改角色权限", ActionType.Edit)]
        public async Task<ApiResponse<bool>> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            try
            {
                var res = await _RoleServices.UpdateRolePermissionsAsync(roleId, permissionIds);
                return Success(true);
            }
            catch (Exception)
            {
                return Error<bool>("修改失败");
            }
        }
    }
}
