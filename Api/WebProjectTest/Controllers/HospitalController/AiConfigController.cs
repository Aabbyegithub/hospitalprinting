using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.Dto.HolDto;
using static WebProjectTest.Common.Message;
using WebIServices.IServices.HospitalIServices;
using WebIServices.IBase;
using WebProjectTest.Controllers.SystemController;
using WebProjectTest.Common.Filter;
using static ModelClassLibrary.Model.CommonEnmFixts;

namespace WebProjectTest.Controllers.HospitalController
{
    /// <summary>
    /// AI配置控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AiConfigController(IAiConfigServices aiConfigServices, ISqlHelper dal, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        private readonly IAiConfigServices _aiConfigServices = aiConfigServices;


        /// <summary>
        /// 获取AI配置
        /// </summary>
        /// <param name="orgId">机构ID</param>
        /// <returns></returns>
        [HttpGet]
        [OperationLogFilter("AI配置", "获取AI配置", ActionType.Search)]
        public async Task<ApiResponse<HolAiConfigDto>> GetConfig([FromQuery] long orgId = 1)
        {
            return await _aiConfigServices.GetConfigAsync(orgId);
        }

        /// <summary>
        /// 保存AI配置
        /// </summary>
        /// <param name="dto">配置DTO</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("AI配置", "保存AI配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> SaveConfig([FromBody] HolAiConfigDto dto)
        {
            return await _aiConfigServices.SaveConfigAsync(dto);
        }

        /// <summary>
        /// 测试AI连接
        /// </summary>
        /// <param name="dto">配置DTO</param>
        /// <returns></returns>
        [HttpPost]
        [OperationLogFilter("AI配置", "测试AI连接", ActionType.Search)]
        public async Task<ApiResponse<string>> TestConnection([FromBody] HolAiConfigDto dto)
        {
            return await _aiConfigServices.TestConnectionAsync(dto);
        }
    }
}
