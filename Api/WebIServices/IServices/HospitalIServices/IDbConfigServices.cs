using ModelClassLibrary.Model.HolModel;
using WebIServices.IBase;
using static WebProjectTest.Common.Message;

namespace WebServiceClass.Services.HospitalServices
{
    /// <summary>
    /// 数据库配置服务接口
    /// </summary>
    public interface IDbConfigServices : IBaseService
    {
        /// <summary>
        /// 获取数据库配置列表
        /// </summary>
        Task<ApiResponse<List<HolDbConfigDto>>> GetConfigListAsync(string configName, long orgId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取数据库配置详情
        /// </summary>
        Task<ApiResponse<HolDbConfigDto>> GetConfigByIdAsync(long id);

        /// <summary>
        /// 新增数据库配置
        /// </summary>
        Task<ApiResponse<string>> AddConfigAsync(HolDbConfigDto config);

        /// <summary>
        /// 更新数据库配置
        /// </summary>
        Task<ApiResponse<string>> UpdateConfigAsync(HolDbConfigDto config);

        /// <summary>
        /// 删除数据库配置
        /// </summary>
        Task<ApiResponse<string>> DeleteConfigAsync(List<long> ids);

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        Task<ApiResponse<string>> TestConnectionAsync(HolDbConfigDto config);
    }
}
