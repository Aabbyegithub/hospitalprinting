using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using ModelClassLibrary.Model.Dto.HolDto;

namespace WebIServices.IServices.HospitalIServices
{
    public interface IDoctorServices : IBaseService
    {
        Task<ApiResponse<string>> AddDoctorAsync(HolDoctor doctor, long orgId);

        Task<ApiResponse<string>> UpdateDoctorAsync(HolDoctor doctor);

        Task<ApiResponse<string>> DeleteDoctorAsync(List<long> ids);

        Task<List<HolDoctorDto>> GetDoctorPageAsync(string? name, long? departmentId, int page, int size, RefAsync<int> count, long orgId);

        Task<ApiResponse<HolDoctorDto>> GetDoctorDetailAsync(long id);
    }
}
