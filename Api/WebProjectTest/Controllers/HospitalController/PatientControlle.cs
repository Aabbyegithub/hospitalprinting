using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using static ModelClassLibrary.Model.CommonEnmFixts;
using static WebProjectTest.Common.Message;
using System.Security.Cryptography;
using WebIServices.IBase;
using WebProjectTest.Common.Filter;
using WebProjectTest.Controllers.SystemController;
using MyNamespace;
using WebIServices.IServices.HospitalIServices;
using WebServiceClass.Base;

namespace WebProjectTest.Controllers.HospitalController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PatientController(IRedisCacheService redisCacheService, IPatientServices _patientService) : AutherController(redisCacheService)
    {
        /// <summary>
        /// 添加患者信息
        /// </summary>
        [HttpPost]
        [OperationLogFilter("患者管理", "新增患者", ActionType.Add)]
        public async Task<ApiResponse<string>> AddPatientAsync([FromBody] HolPatient patient)
        {
            return await _patientService.AddPatientAsync(patient, OrgId);
        }

        /// <summary>
        /// 修改患者信息
        /// </summary>
        [HttpPost]
        [OperationLogFilter("患者管理", "修改患者", ActionType.Edit)]
        public async Task<ApiResponse<string>> UpdatePatientAsync([FromBody] HolPatient patient)
        {
            return await _patientService.UpdatePatientAsync(patient);
        }

        /// <summary>
        /// 删除患者信息（逻辑删除）
        /// </summary>
        [HttpPost]
        [OperationLogFilter("患者管理", "删除患者", ActionType.Delete)]
        public async Task<ApiResponse<string>> DeletePatientAsync([FromBody] List<long> ids)
        {
            return await _patientService.DeletePatientAsync(ids);
        }

        /// <summary>
        /// 分页查询患者信息
        /// </summary>
        [HttpGet]
        [OperationLogFilter("患者管理", "查询患者", ActionType.Search)]
        public async Task<ApiPageResponse<List<HolPatient>>> GetPatientPageAsync(string? name, string? medicalNo, int page = 1, int size = 10)
        {
            RefAsync<int> count = 0;
            try
            {
                var res = await _patientService.GetPatientPageAsync(name, medicalNo, page, size, count, OrgId);
                return PageSuccess(res, count);
            }
            catch (Exception)
            {
                return PageError<List<HolPatient>>("服务器错误");
            }
        }

        /// <summary>
        /// 获取患者详情
        /// </summary>
        [HttpGet]
        [OperationLogFilter("患者管理", "查看患者详情", ActionType.Search)]
        public async Task<ApiResponse<HolPatient>> GetPatientDetailAsync(long id)
        {
            return await _patientService.GetPatientDetailAsync(id);
        }
    }
}
