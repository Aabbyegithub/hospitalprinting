using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;

namespace WebIServices.IServices.HospitalIServices
{
    public interface IPrintRecordServices : IBaseService
    {
        Task<ApiResponse<string>> AddAsync(PrintRecordModel record);
        Task<ApiResponse<string>> UpdateAsync(PrintRecordModel record);
        Task<ApiResponse<string>> DeleteAsync(List<long> ids);
        Task<List<PrintRecordModel>> GetPageAsync(long? examId, int page, int size, RefAsync<int> count);
    }
}
