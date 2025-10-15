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
    public interface IExaminationServices : IBaseService
    {
        Task<ApiResponse<string>> AddExaminationAsync(HolExamination exam, long OrgId, long UserId);

        Task<List<HolExamination>> GetExaminationPageAsync(string? examNo, string? examType, string? patientName, int? isPrinted, DateTime? examDate,
            int page, int size, RefAsync<int> count, long OrgId);

        Task<ApiResponse<string>> UpdateExaminationAsync(HolExamination exam, long UserId);

        Task<ApiResponse<string>> DeleteExaminationAsync(List<long> ids, long UserId, long OrgId);

        /// <summary>
        /// 打印检查报告（仅支持单次打印）
        /// </summary>
        Task<ApiResponse<string>> PrintExaminationAsync(long examId);

        /// <summary>
        /// 手动解锁打印状态
        /// </summary>
        Task<ApiResponse<string>> UnlockPrintAsync(long examId);
    }
}
