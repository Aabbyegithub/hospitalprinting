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
    public interface IDepartmentServices : IBaseService
    {
        Task<ApiResponse<string>> AddDepartmentAsync(HolDepartment dept, long orgId);

        Task<ApiResponse<string>> UpdateDepartmentAsync(HolDepartment dept);

        Task<ApiResponse<string>> DeleteDepartmentAsync(List<long> ids);

        Task<List<HolDepartmentDto>> GetDepartmentPageAsync(string? name, int page, int size, RefAsync<int> count, long orgId);

        Task<ApiResponse<HolDepartmentDto>> GetDepartmentDetailAsync(long id);
    }

}
