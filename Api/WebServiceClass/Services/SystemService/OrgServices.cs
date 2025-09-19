using Azure;
using MyNamespace;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.SystemIServices;
using static WebProjectTest.Common.Message;

namespace WebServiceClass.Services.SystemService
{
    public class OrgServices:IBaseService, IOrgServices
    {
        private readonly ISqlHelper _dal;

        public OrgServices(ISqlHelper dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public async Task<ApiResponse<bool>> AddStoreAsync(sys_orgid sys_orgid)
        {
            try
            {
                var count = await _dal.Db.Queryable<sys_orgid>().CountAsync();
                sys_orgid.orgid_code = $"Org-{string.Format("{0:D5}", count + 1)}";
                await _dal.Db.Insertable(sys_orgid)
                    .ExecuteCommandAsync();
                return Success<bool>(true, "商店添加成功");
            }
            catch (Exception)
            {
                return Error<bool>("商店添加失败，请稍后重试");
            }
        }

        public async Task<ApiResponse<bool>> DeleteStoreAsync(List<int> storeIds)
        {
            try
            {
                await _dal.Db.Deleteable<sys_orgid>()
                    .In(storeIds)
                    .ExecuteCommandAsync();
                return Success<bool>(true, "商店删除成功");
            }
            catch (Exception)
            {
                return Error<bool>("商店删除失败，请稍后重试");
            }
        }

        public async Task<ApiPageResponse<List<sys_orgid>>> GetStoreListAsync(string StoreName, string phone, string address, int page, int size, RefAsync<int> count)
        {
            try
            {
                var res = await _dal.Db.Queryable<sys_orgid>()
                    .WhereIF(!string.IsNullOrEmpty(StoreName), a => a.orgid_name.Contains(StoreName))
                    .WhereIF(!string.IsNullOrEmpty(address), a => a.address.Contains(address))
                    .ToPageListAsync(page, size,count);
                return PageSuccess(res,count,"商店列表获取成功");
            }
            catch (Exception)
            {
                return PageError <List<sys_orgid>>( "商店列表获取失败，请稍后重试");
            }
        }

        public async Task<ApiResponse<List<sys_orgid>>> GetStoreListAsync(int orgId)
        {
            try
            {
                var res = await _dal.Db.Queryable<sys_orgid>()
                    .WhereIF(orgId != 1,a=>a.orgid_id == orgId)
                    .ToListAsync();
                return Success(res,  "商店列表获取成功");
            }
            catch (Exception)
            {
                return Error<List<sys_orgid>>("商店列表获取失败，请稍后重试");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStoreAsync(sys_orgid sys_Store)
        {
            try
            {
                await _dal.Db.Updateable(sys_Store)
                    .ExecuteCommandAsync();
                return Success<bool>(true, "信息更新成功");
            }
            catch (Exception)
            {

               return Error<bool>("信息更新失败，请稍后重试");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStoreAsync(int storeId, byte status)
        {
            try
            {
                await _dal.Db.Updateable<sys_orgid>()
                    .SetColumns(a => new sys_orgid { status = status })
                    .Where(a => a.orgid_id == storeId)
                    .ExecuteCommandAsync();
                return Success<bool>(true, "商店状态更新成功");
            }
            catch (Exception)
            {
                return Error<bool>("商店状态更新失败，请稍后重试");
            }
        }
    }
}
