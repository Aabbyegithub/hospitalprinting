using Microsoft.AspNetCore.Mvc;
using WebServiceClass.Services.HospitalServices;
using WebProjectTest.Common;
using ModelClassLibrary.Model.HolModel;
using static WebProjectTest.Common.Message;
using WebProjectTest.Common.Filter;
using static ModelClassLibrary.Model.CommonEnmFixts;

namespace WebProjectTest.Controllers
{
    /// <summary>
    /// 数据库配置控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DbConnController : ControllerBase
    {
        private readonly IDbConfigServices _dbConfigServices;

        public DbConnController(IDbConfigServices dbConfigServices)
        {
            _dbConfigServices = dbConfigServices;
        }

        /// <summary>
        /// 获取数据库配置列表
        /// </summary>
        [HttpGet]
        [OperationLogFilter("数据库配置", "获取数据库配置列表", ActionType.Search)]
        public async Task<ApiResponse<List<HolDbConfigDto>>> GetConfigList([FromQuery] string configName = "",[FromQuery] long orgId = 1,[FromQuery] int pageIndex = 1,[FromQuery] int pageSize = 10)
        {
            return await _dbConfigServices.GetConfigListAsync(configName, orgId, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取数据库配置详情
        /// </summary>
        [HttpGet]
        [OperationLogFilter("数据库配置", "获取数据库配置详情", ActionType.Search)]
        public async Task<ApiResponse<HolDbConfigDto>> GetConfigById([FromQuery] long id)
        {
            return await _dbConfigServices.GetConfigByIdAsync(id);
        }

        /// <summary>
        /// 新增数据库配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("数据库配置", "新增数据库配置", ActionType.Add)]
        public async Task<ApiResponse<string>> AddConfig([FromBody] HolDbConfigDto config)
        {
            return await _dbConfigServices.AddConfigAsync(config);
        }

        /// <summary>
        /// 更新数据库配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("数据库配置", "更新数据库配置", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateConfig([FromBody] HolDbConfigDto config)
        {
            return await _dbConfigServices.UpdateConfigAsync(config);
        }

        /// <summary>
        /// 删除数据库配置
        /// </summary>
        [HttpPost]
        [OperationLogFilter("数据库配置", "删除数据库配置", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteConfig([FromBody] List<long> ids)
        {
            return await _dbConfigServices.DeleteConfigAsync(ids);
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        [HttpPost]
        [OperationLogFilter("数据库配置", "测试数据库连接", ActionType.Search)]
        public async Task<ApiResponse<string>> TestConnection([FromBody] HolDbConfigDto config)
        {
            return await _dbConfigServices.TestConnectionAsync(config);
        }
    }
}
