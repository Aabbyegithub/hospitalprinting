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
    public interface IOrgServices : IBaseService
    {
        /// <summary>
        /// 获取商店列表
        /// </summary>
        /// <returns></returns>
        Task<ApiPageResponse<List<sys_orgid>>> GetStoreListAsync(string StoreName,string phone,string address, int page, int size, RefAsync<int> count);
        Task<ApiResponse<List<sys_orgid>>> GetStoreListAsync(int orgId);
        /// <summary>
        /// 添加商店
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> AddStoreAsync(sys_orgid sys_Store);
        /// <summary>
        /// 删除商店
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> DeleteStoreAsync(List<int> storeIds);

        ///<summary>
        ///修改商店
        ///<summary>
        Task<ApiResponse<bool>> UpdateStoreAsync(sys_orgid sys_Store);

        /// <summary>
        /// 修改商店状态
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> UpdateStoreAsync(int storeId, byte status);
    }
}
