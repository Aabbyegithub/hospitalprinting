using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebIServices.IServices.SystemIServices
{
    public interface IRoleServices:IBaseService
    {
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <returns></returns>
        Task<List<sys_role>> GetRoleListAsync(int page, int size, RefAsync<int> count,string RoleName);

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        Task<List<sys_role>> GetRoleListAsync();
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<bool> AddRoleAsync(sys_role sys_Role);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<bool> DeleteRoleAsync(int roleId);
        /// <summary>
        ///修改角色
        ///<summary>
        Task<bool> UpdateRoleAsync(sys_role sys_Role);

        /// <summary>
        /// 修改员工权限
        /// <summary>
        Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds);

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        Task<List<UserPermission>> GetUserPermissionsAsync(int role_id);
    }
}
