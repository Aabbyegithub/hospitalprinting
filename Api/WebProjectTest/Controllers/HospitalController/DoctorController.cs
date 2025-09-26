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
    public class DoctorController(IRedisCacheService redisCacheService, IDoctorServices _doctorService) : AutherController(redisCacheService)
    {
        /// <summary>添加医生</summary>
        [HttpPost]
        [OperationLogFilter("医生管理", "新增医生", ActionType.Add)]
        public async Task<ApiResponse<string>> AddDoctorAsync([FromBody] HolDoctor doctor)
        {
            return await _doctorService.AddDoctorAsync(doctor, OrgId);
        }

        /// <summary>修改医生</summary>
        [HttpPost]
        [OperationLogFilter("医生管理", "修改医生", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdateDoctorAsync([FromBody] HolDoctor doctor)
        {
            return await _doctorService.UpdateDoctorAsync(doctor);
        }

        /// <summary>删除医生（逻辑删除）</summary>
        [HttpPost]
        [OperationLogFilter("医生管理", "删除医生", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeleteDoctorAsync([FromBody] List<long> ids)
        {
            return await _doctorService.DeleteDoctorAsync(ids);
        }

        /// <summary>分页查询医生（带科室名）</summary>
        [HttpGet]
        [OperationLogFilter("医生管理", "查询医生", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolDoctorDto>>> GetDoctorPageAsync(
            string? name, long? departmentId, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _doctorService.GetDoctorPageAsync(name, departmentId, page, size, count, OrgId);
                return PageSuccess(res, count);
            }
            catch (Exception)
            {
                return PageError<List<HolDoctorDto>>("服务器错误");
            }
        }

        /// <summary>获取医生详情（带科室名）</summary>
        [HttpGet]
        [OperationLogFilter("医生管理", "查看医生详情", ActionType.Search)]
        public async Task<ApiResponse<HolDoctorDto>> GetDoctorDetailAsync(long id)
        {
            return await _doctorService.GetDoctorDetailAsync(id);
        }
    }

}
