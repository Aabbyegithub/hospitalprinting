using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;

namespace WebServiceClass.Services.SystemService
{
    public class RoleServices:IBaseService,IRoleServices
    {
        private readonly ISqlHelper _dal;

        public RoleServices(ISqlHelper dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public async Task<bool> AddRoleAsync(sys_role sys_Role)
        {
            return await _dal.Db.Insertable(sys_Role).ExecuteCommandIdentityIntoEntityAsync();
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            return await _dal.Db.Deleteable<sys_role>().In(roleId).ExecuteCommandHasChangeAsync();
        }

        public async Task<List<sys_role>> GetRoleListAsync(int page, int size, RefAsync<int> count,string RoleName)
        {
            return await _dal.Db.Queryable<sys_role>()
                .WhereIF(!string.IsNullOrEmpty(RoleName),a=>a.role_name.Contains(RoleName))
                .ToPageListAsync(page,size,count);
        }

        public async Task<List<sys_role>> GetRoleListAsync()
        {
            return await _dal.Db.Queryable<sys_role>()
                .ToListAsync();
        }

        public async Task<List<UserPermission>> GetUserPermissionsAsync(int role_id)
        {
            var menu = await _dal.Db.Queryable<sys_role_permission>()
                .Where(a=>a.role_id == role_id)
                .RightJoin<sys_permission>((a,b)=>a.permission_id == b.permission_id).Select((a,b)=>new sys_permission() {permission_id = b.permission_id, isSelect = string.IsNullOrEmpty(a.id.ToString()) ? false:true},true)
                 .ToListAsync();
            return menu.Where(a => a.parent_id == 0).Select(a => new UserPermission
            {
                permissionId = a.permission_id,
                groupKey = a.permission_key,
                groupTitle = a.permission_name,
                icon = a.permission_icon,
                parent_id = a.parent_id.ToString(),
                isselect =a.isSelect, 
                children = menu.Where(b => b.parent_id == a.permission_id).Select(b => new UserPermissionItem
                {
                    permissionId = b.permission_id,
                    key = b.permission_key,
                    name = b.permission_router,
                    title = b.permission_name,
                    icon = b.permission_icon,
                    parent_id = b.parent_id.ToString(),
                     isselect =b.isSelect, 
                }).ToList()
            }).ToList();
        }

        public async Task<bool> UpdateRoleAsync(sys_role sys_Role)
        {
            return await _dal.Db.Updateable(sys_Role).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            try
            {
                await _dal.Db.Ado.BeginTranAsync();
                await _dal.Db.Deleteable<sys_role_permission>(a => a.role_id == roleId).ExecuteCommandAsync();
                var list = permissionIds.Select(a => new sys_role_permission
                {
                    role_id = roleId,
                    permission_id = a
                }).ToList();
                await _dal.Db.Insertable(list).ExecuteCommandAsync();
                await _dal.Db.Ado.CommitTranAsync();
                return true;
            }
            catch (Exception)
            {
                await _dal.Db.Ado.RollbackTranAsync();
                return false;
            }
        }
    }
}
