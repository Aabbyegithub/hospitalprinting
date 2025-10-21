using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static WebProjectTest.Common.Message;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using WebIServices.IServices.HospitalIServices;
using ModelClassLibrary.Model.HolModel;
using static ModelClassLibrary.Model.CommonEnmFixts;
using WebIServices.IBase;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OssConfigController(IOssConfigServices ossConfigService, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 获取OSS配置
        /// </summary>
        [HttpGet]
        [OperationLogFilter("阿里云OSS配置", "获取OSS配置", ActionType.Search)]
        public async Task<ApiResponse<HolOssConfigDto>> GetConfigAsync()
        {
            return await ossConfigService.GetConfigAsync(OrgId);
        }

        /// <summary>
        /// 保存OSS配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("阿里云OSS配置", "保存OSS配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> SaveConfigAsync([FromBody] HolOssConfigDto config)
        {
            return await ossConfigService.SaveConfigAsync(config, OrgId, UserId);
        }

        /// <summary>
        /// 测试OSS连接
        /// </summary>
        [HttpPost]
        public async Task<ApiResponse<string>> TestConnectionAsync([FromBody] HolOssConfigDto config)
        {
            return await ossConfigService.TestConnectionAsync(config);
        }

        /// <summary>
        /// 上传文件到OSS
        /// </summary>
        [HttpPost]
        [OperationLogFilter("阿里云OSS配置", "上传文件到OSS", ActionType.Upload)]
        public async Task<ApiResponse<string>> UploadFileAsync([FromQuery] long examId, [FromQuery] string filePath, [FromQuery] string fileName)
        {
            return await ossConfigService.UploadFileAsync(examId, filePath, fileName, OrgId);
        }
    }
}
