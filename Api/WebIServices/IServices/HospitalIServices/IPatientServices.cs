using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using MyNamespace;

namespace WebIServices.IServices.HospitalIServices
{
    public interface IPatientServices : IBaseService
    {
        Task<ApiResponse<string>> AddPatientAsync(HolPatient patient, long orgId);

        Task<ApiResponse<string>> UpdatePatientAsync(HolPatient patient);

        Task<ApiResponse<string>> DeletePatientAsync(List<long> ids);

        Task<List<HolPatient>> GetPatientPageAsync(string? name, string? medicalNo, int page, int size, RefAsync<int> count, long orgId);

        Task<ApiResponse<HolPatient>> GetPatientDetailAsync(long id);
    }
}
