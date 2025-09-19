using ModelClassLibrary.Model.AutherModel.AutherDto;
using ModelClassLibrary.Model.Dto.SystemDto;
using MyNamespace;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;
using WebIServices.ITask;
using WebServiceClass.Base;
using WebServiceClass.Helper;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;

namespace WebServiceClass.Services.UserService
{
    public class UserServices: IBaseService, IUserServices
    {
        private readonly ISqlHelper _dal;
        private readonly IRedisCacheService _redis;
        private readonly TokenService _token;
        public UserServices(ISqlHelper dal, TokenService token, IRedisCacheService redis)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
            _token = token;
            _redis = redis;
        }

        public async Task<UserResult> UserLoginAsync(string UserName, string PassWord)
        {
            var User = await _dal.Db.Queryable<sys_user>()
                .Includes(a=>a.user_role)
                .Includes(a=>a.store)
                .FirstAsync(a => a.username == UserName && a.IsDelete == 1);
            if (User == null)
            {

                return null;
            }
            else
            {

                var Istrue = PasswordHelper.VerifyPassword(PassWord, User.password, User.Salt);
                if (!Istrue)
                {
                    return null;
                }
                else
                {

                    await _dal.Db.Insertable(new lq_operationlog
                    {
                        ActionType = ActionType.Landing,
                        ModuleName = "系统登陆",
                        OrgId =(int)User.orgid_id,
                        AddUserId = (int)User.user_id,
                        UpUserId = (int)User.user_id,
                        UserId = (int)User.user_id,
                        Description = "人员登陆",
                        ActionContent = $"账号：{UserName},员工姓名：{User.name}"
                    }).ExecuteCommandAsync();
                    var context = new UserContext
                    {
                        UserId = User.user_id,
                        UserName = User.username,
                        Name = User.name,
                        OrgId = (int)User.orgid_id,
                        OrgName =User.store.orgid_name,
                        PassWord = User.password,
                        Salt = User.Salt,
                        RoleId = (int)User.user_role.role_id
                    };
                    string token = _token.GenerateToken(User.name);
                    await _redis.SetStringAsync(token, JsonConvert.SerializeObject(context));
                    return new UserResult() { UserId = (int)User.user_id, OrgId = (long)User.orgid_id, UserName = User.name,AvatarUrl=User.AvatarUrl, Token = token };
                }

            }
        }

        public async Task<List<sys_user>> GetUserPageAsync(string? name, string? username, string? phone, int page, int size, RefAsync<int> count, int? RoleId, int OrgId)
        {
            var Data = await _dal.Db.Queryable<sys_user>()
                .Includes(a=>a.user_role).Includes(a=>a.user_role.role)
                .WhereIF(!string.IsNullOrEmpty(name), a => a.name.Contains(name))
                .WhereIF(!string.IsNullOrEmpty(username), a => a.username.Contains(username))
                .WhereIF(!string.IsNullOrEmpty(phone), a => a.phone.Contains(phone))
                .ToPageListAsync(page, size, count);
            return Data;
        }

        public async Task<ApiResponse<string>> AddUserAsync(sys_user User,long Orgid)
        {
            try
            {
                var (hashpassword, salt) = PasswordHelper.HashPassword(User.password);
                var istrue = await _dal.Db.Queryable<sys_user>().FirstAsync(a => a.username == User.username && a.IsDelete == 1);
                if (istrue != null)
                {
                    return Fail<string>("账号已存在！");
                }
                User.password = hashpassword; User.Salt = salt; User.IsDelete = 1;User.orgid_id = Orgid;
                var id = await _dal.Db.Insertable(User).ExecuteReturnBigIdentityAsync();
                if (id == 0)
                    return Error<string>("账号密码保存失败！");
                else
                {
                    await _dal.Db.Insertable(new sys_user_role
                    {
                        user_id = id,
                        role_id = User.RoleId,
                    }).ExecuteCommandAsync();
                    return Success<string>(id.ToString(), "账号创建成功！");
                }

            }
            catch (Exception)
            {

                 return Error<string>("账号密码保存失败！");
            }

        }

        public async Task<ApiResponse<string>> DeleteUserAsync(List<int> Ids)
        {
            try
            {
               await _dal.Db.Deleteable<sys_user>().In(Ids).ExecuteCommandAsync();
                await _dal.Db.Deleteable<sys_user_role>().In(a => a.user_id,Ids ).ExecuteCommandAsync();
                return Success("删除成功");
            }
            catch (Exception)
            {
                return Error<string>("删除失败!");
            } 
        }

        public async Task<ApiResponse<string>> UpUserAsync(sys_user User)
        {
            try
            {
                var (hashpassword, salt) = PasswordHelper.HashPassword(User.password);
                var istrue = await _dal.Db.Queryable<sys_user>().FirstAsync(a => a.username == User.username);
                if (istrue == null)
                {
                    return Fail<string>("账号不存在！");
                }
                User.password = hashpassword; User.Salt = salt; User.IsDelete = 1;
                var id = await _dal.Db.Updateable(User).ExecuteCommandAsync();
                if (id == 0)
                    return Error<string>("账号密码保存失败！");
                else
                {
                    if( _dal.Db.Queryable<sys_user_role>().First(a=>a.user_id == User.user_id)!=null)
                    await _dal.Db.Updateable<sys_user_role>().SetColumns(a=>new sys_user_role { role_id = User.RoleId }).Where(a=>a.user_id == User.user_id).ExecuteCommandAsync();
                    else
                        await _dal.Db.Insertable(new sys_user_role
                        {
                            user_id = User.user_id,
                            role_id = User.RoleId,
                        }).ExecuteCommandAsync();
                    return Success<string>(id.ToString(), "账号更新成功！");
                }

            }
            catch (Exception)
            {

                 return Error<string>("账号密码保存失败！");
            }

        }

        public async Task<List<UserPermission>> GetUserPermissionsAsync(int RoleId)
        {
            var menu =await _dal.Db.Queryable<sys_role_permission>().Includes(a=>a.permission)
                .Where(a=> a.role_id == RoleId)
                .Select(a=>new sys_permission()
                {
                    permission_id = a.permission.permission_id,
                    permission_key = a.permission.permission_key,
                    permission_name = a.permission.permission_name,
                    permission_icon = a.permission.permission_icon,
                    permission_router = a.permission.permission_router,
                    parent_id = a.permission.parent_id
                }).OrderBy(a=>a.permission_id).ToListAsync();
            return menu.Where(a=>a.parent_id == 0).Select(a=>new UserPermission
            {
                groupKey = a.permission_key,
                groupTitle = a.permission_name,
                icon = a.permission_icon,
                children = menu.Where(b => b.parent_id == a.permission_id).Select(b => new UserPermissionItem
                {
                    key = b.permission_key,
                    name = b.permission_router,
                    title = b.permission_name,
                    icon = b.permission_icon
                }).ToList()
            }).ToList();


        }
    }
}
