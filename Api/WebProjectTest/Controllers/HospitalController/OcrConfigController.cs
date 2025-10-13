using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OcrConfigController(IOcrConfigServices ocrConfigService, ISqlHelper dal, IRedisCacheService redisCacheService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 获取OCR配置
        /// </summary>
        [HttpGet]
        [OperationLogFilter("OCR配置", "获取OCR配置", ActionType.Search)]
        public async Task<ApiResponse<HolOcrConfig>> GetConfigAsync()
        {
            return await ocrConfigService.GetOcrConfigAsync(OrgId);
        }

        /// <summary>
        /// 保存OCR配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("OCR配置", "保存OCR配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> SaveConfigAsync([FromBody] HolOcrConfig ocrConfig)
        {
            return await ocrConfigService.SaveOcrConfigAsync(ocrConfig, OrgId, UserId);
        }

    }
}
