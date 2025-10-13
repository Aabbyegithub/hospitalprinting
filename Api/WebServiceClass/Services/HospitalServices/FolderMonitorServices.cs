using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;

namespace WebServiceClass.Services.HospitalServices
{
    public class FolderMonitorServices(ISqlHelper dal) : IBaseService, IFolderMonitorServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<string>> AddFolderMonitorAsync(HolFolderMonitor folderMonitor, long OrgId, long UserId)
        {
            try
            {
                // 验证必填字段
                if (string.IsNullOrEmpty(folderMonitor.name)) return Error<string>("配置名称不能为空");
                if (string.IsNullOrEmpty(folderMonitor.ip_address)) return Error<string>("IP地址不能为空");
                if (string.IsNullOrEmpty(folderMonitor.folder_paths)) return Error<string>("文件夹路径不能为空");

                folderMonitor.org_id = OrgId;
                folderMonitor.create_time = DateTime.Now;
                folderMonitor.update_time = DateTime.Now;

                var id = await _dal.Db.Insertable(folderMonitor).ExecuteReturnBigIdentityAsync();
                return id > 0 ? Success("添加成功") : Error<string>("保存失败");
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<List<HolFolderMonitor>> GetFolderMonitorPageAsync(string? name, string? ipAddress, int page, int size, RefAsync<int> count, long OrgId)
        {
            return await _dal.Db.Queryable<HolFolderMonitor>()
                .Where(a => a.status == 1)
                .WhereIF(!string.IsNullOrEmpty(name), a => a.name.Contains(name))
                .WhereIF(!string.IsNullOrEmpty(ipAddress), a => a.ip_address.Contains(ipAddress))
                .OrderByDescending(a => a.create_time)
                .ToPageListAsync(page, size, count);
        }

        public async Task<ApiResponse<string>> UpdateFolderMonitorAsync(HolFolderMonitor folderMonitor, long UserId)
        {
            try
            {
                folderMonitor.update_time = DateTime.Now;
                var rows = await _dal.Db.Updateable(folderMonitor).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteFolderMonitorAsync(List<long> ids, long UserId, long OrgId)
        {
            try
            {
                // 逻辑删除
                await _dal.Db.Updateable<HolFolderMonitor>()
                    .SetColumns(a => new HolFolderMonitor { status = 0, update_time = DateTime.Now })
                    .Where(a => ids.Contains(a.id))
                    .ExecuteCommandAsync();

                return Success("删除成功");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> ToggleStatusAsync(long id, long UserId)
        {
            try
            {
                var folderMonitor = await _dal.Db.Queryable<HolFolderMonitor>().FirstAsync(a => a.id == id);
                if (folderMonitor == null) return Error<string>("配置不存在");

                folderMonitor.status = folderMonitor.status == 1 ? 0 : 1;
                folderMonitor.update_time = DateTime.Now;

                var rows = await _dal.Db.Updateable(folderMonitor).ExecuteCommandAsync();
                return rows > 0 ? Success("状态切换成功") : Error<string>("状态切换失败");
            }
            catch (Exception e)
            {
                return Error<string>($"状态切换失败：{e.Message}");
            }
        }
    }
}
