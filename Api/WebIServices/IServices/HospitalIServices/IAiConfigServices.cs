using ModelClassLibrary.Model.Dto.HolDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using static WebProjectTest.Common.Message;

namespace WebIServices.IServices.HospitalIServices
{
    /// <summary>
    /// AI配置服务接口
    /// </summary>
    public interface IAiConfigServices:IBaseService
    {
        /// <summary>
        /// 获取AI配置
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <returns></returns>
        Task<ApiResponse<HolAiConfigDto>> GetConfigAsync(long orgId);

        /// <summary>
        /// 保存AI配置
        /// </summary>
        /// <param name="dto">配置DTO</param>
        /// <returns></returns>
        Task<ApiResponse<string>> SaveConfigAsync(HolAiConfigDto dto);

        /// <summary>
        /// 测试AI连接
        /// </summary>
        /// <param name="dto">配置DTO</param>
        /// <returns></returns>
        Task<ApiResponse<string>> TestConnectionAsync(HolAiConfigDto dto);
    }
}
