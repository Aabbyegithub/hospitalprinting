using ModelClassLibrary.Model.HolModel;
using WebIServices.IBase;
using static WebProjectTest.Common.Message;

namespace WebIServices.IServices.HospitalIServices
{
    /// <summary>
    /// 阿里云OSS配置服务接口
    /// </summary>
    public interface IOssConfigServices : IBaseService
    {
        /// <summary>
        /// 获取OSS配置
        /// </summary>
        Task<ApiResponse<HolOssConfigDto>> GetConfigAsync(long orgId);

        /// <summary>
        /// 保存OSS配置
        /// </summary>
        Task<ApiResponse<string>> SaveConfigAsync(HolOssConfigDto config, long orgId, long userId);

        /// <summary>
        /// 测试OSS连接
        /// </summary>
        Task<ApiResponse<string>> TestConnectionAsync(HolOssConfigDto config);

        /// <summary>
        /// 上传文件到OSS
        /// </summary>
        Task<ApiResponse<string>> UploadFileAsync(long examId, string filePath, string fileName, long orgId);
    }
}
