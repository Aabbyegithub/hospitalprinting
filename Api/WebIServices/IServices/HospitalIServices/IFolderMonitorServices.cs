using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;

namespace WebIServices.IServices.HospitalIServices
{
    /// <summary>
    /// 文件夹监听配置服务接口
    /// </summary>
    public interface IFolderMonitorServices : IBaseService
    {
        /// <summary>
        /// 添加文件夹监听配置
        /// </summary>
        Task<ApiResponse<string>> AddFolderMonitorAsync(HolFolderMonitor folderMonitor, long OrgId, long UserId);

        /// <summary>
        /// 分页查询文件夹监听配置
        /// </summary>
        Task<List<HolFolderMonitor>> GetFolderMonitorPageAsync(string? name, string? ipAddress, int page, int size, RefAsync<int> count, long OrgId);

        /// <summary>
        /// 修改文件夹监听配置
        /// </summary>
        Task<ApiResponse<string>> UpdateFolderMonitorAsync(HolFolderMonitor folderMonitor, long UserId);

        /// <summary>
        /// 删除文件夹监听配置
        /// </summary>
        Task<ApiResponse<string>> DeleteFolderMonitorAsync(List<long> ids, long UserId, long OrgId);

        /// <summary>
        /// 切换状态
        /// </summary>
        Task<ApiResponse<string>> ToggleStatusAsync(long id, long UserId);
    }
}
