using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using ModelClassLibrary.Model.Dto.HolDto;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DepartmentController(IRedisCacheService redisCacheService, IDepartmentServices _departmentService) : AutherController(redisCacheService)
    {
        /// <summary>添加科室</summary>
        [HttpPost]
        [OperationLogFilter("科室管理", "新增科室", ActionType.Add)]
        public async Task<ApiResponse<string>> AddDepartmentAsync([FromBody] HolDepartment dept)
        {
            return await _departmentService.AddDepartmentAsync(dept, OrgId);
        }

        /// <summary>修改科室</summary>
        [HttpPost]
        [OperationLogFilter("科室管理", "修改科室", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateDepartmentAsync([FromBody] HolDepartment dept)
        {
            return await _departmentService.UpdateDepartmentAsync(dept);
        }

        /// <summary>删除科室（逻辑删除）</summary>
        [HttpPost]
        [OperationLogFilter("科室管理", "删除科室", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteDepartmentAsync([FromBody] List<long> ids)
        {
            return await _departmentService.DeleteDepartmentAsync(ids);
        }

        /// <summary>分页查询科室</summary>
        [HttpGet]
        [OperationLogFilter("科室管理", "查询科室", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolDepartmentDto>>> GetDepartmentPageAsync(string? name, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _departmentService.GetDepartmentPageAsync(name, page, size, count, OrgId);
                return PageSuccess(res, count);
            }
            catch (Exception)
            {
                return PageError<List<HolDepartmentDto>>("服务器错误");
            }
        }

        /// <summary>获取科室详情</summary>
        [HttpGet]
        [OperationLogFilter("科室管理", "查看科室详情", ActionType.Search)]
        public async Task<ApiResponse<HolDepartmentDto>> GetDepartmentDetailAsync(long id)
        {
            return await _departmentService.GetDepartmentDetailAsync(id);
        }
    }

}
