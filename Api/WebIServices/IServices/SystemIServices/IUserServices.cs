using ModelClassLibrary.Model.AutherModel.AutherDto;
using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using static WebProjectTest.Common.Message;

namespace WebIServices.IServices.SystemIServices
{
    public interface IUserServices : IBaseService
    {

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWord"></param>
        /// <param name="orgid"></param>
        /// <returns></returns>
        Task<UserResult> UserLoginAsync(string UserName, string PassWord);

        /// <summary>
        /// 查询用户分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="count"></param>
        /// <param name="RoleId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<List<sys_user>> GetUserPageAsync(string? name, string? username, string? phone, int page, int size, RefAsync<int> count, int? RoleId, int OrgId);

        /// <summary>
        /// 创建一个用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ApiResponse<string>> AddUserAsync(sys_user user,long Orgid);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        Task<ApiResponse<string>> DeleteUserAsync(List<int> Ids);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ApiResponse<string>> UpUserAsync(sys_user user);

        /// <summary>
        /// 获取员工权限
        /// <summary>
        Task<List<UserPermission>> GetUserPermissionsAsync(int userId);
    }
}
