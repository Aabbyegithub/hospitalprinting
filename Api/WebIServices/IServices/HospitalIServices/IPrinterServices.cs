using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using ModelClassLibrary.Model.HolModel;
using ModelClassLibrary.Model.Dto.HolDto;

namespace WebIServices.IServices.HospitalIServices
{
    public interface IPrinterServices : IBaseService
    {
        Task<ApiResponse<string>> AddPrinterAsync(HolPrinter printer, long OrgId, long UserId);
        Task<ApiResponse<string>> UpdatePrinterAsync(HolPrinter printer, long UserId);
        Task<ApiResponse<string>> DeletePrinterAsync(List<long> ids, long UserId, long OrgId);
        Task<List<HolPrinter>> GetPrinterPageAsync(string? name, int? type, int? status, int page, int size, RefAsync<int> count, long OrgId);
        Task<ApiResponse<string>> TogglePrinterStatusAsync(long id, int status, long UserId);// 切换打印机启用状态
        Task<ApiResponse<string>> TestPrintAsync(long id, long UserId); // 预留测试打印动作

        Task<HolPrinterConfigDto?> GetConfigAsync(long printerId, long OrgId);//获取打印机配置
        Task<ApiResponse<string>> SaveConfigAsync(HolPrinterConfigDto dto, long OrgId, long UserId);//保存打印机配置

        Task<List<HolPrinterConfigDto>?> GetFilmSizeConfigsAsync(long printerId, long OrgId);//获取胶片尺寸配置

        Task<ApiResponse<string>> SaveFilmSizeConfigAsync(HolPrinterConfigDto dto, long OrgId, long UserId);//保存胶片尺寸配置

        Task<ApiResponse<string>> DeleteAllFilmSizeConfigsAsync(long printerId, long UserId, long OrgId);
    }
}
