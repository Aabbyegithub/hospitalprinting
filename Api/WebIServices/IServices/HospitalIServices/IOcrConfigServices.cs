using ModelClassLibrary.Model.HolModel;
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
    /// 百度云OCR配置服务接口
    /// </summary>
    public interface IOcrConfigServices : IBaseService
    {
        /// <summary>
        /// 获取OCR配置
        /// </summary>
        Task<ApiResponse<HolOcrConfig>> GetOcrConfigAsync(long OrgId);

        /// <summary>
        /// 保存OCR配置
        /// </summary>
        Task<ApiResponse<string>> SaveOcrConfigAsync(HolOcrConfig ocrConfig, long OrgId, long UserId);

        /// <summary>
        /// 测试OCR连接
        /// </summary>
        Task<ApiResponse<string>> TestOcrConnectionAsync(HolOcrConfig ocrConfig);
    }
}
