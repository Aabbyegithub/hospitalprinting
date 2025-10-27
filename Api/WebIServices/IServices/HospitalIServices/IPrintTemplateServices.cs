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
    /// <summary>
    /// 打印模板服务接口
    /// </summary>
    public interface IPrintTemplateServices : IBaseService
    {
        /// <summary>
        /// 添加打印模板
        /// </summary>
        Task<ApiResponse<string>> AddPrintTemplateAsync(HolPrintTemplate template, long OrgId, long UserId);

        /// <summary>
        /// 分页查询打印模板
        /// </summary>
        Task<List<HolPrintTemplate>> GetPrintTemplatePageAsync(string? name, string? templateType, int page, int size, RefAsync<int> count, long OrgId);

        /// <summary>
        /// 修改打印模板
        /// </summary>
        Task<ApiResponse<string>> UpdatePrintTemplateAsync(HolPrintTemplate template, long UserId);

        /// <summary>
        /// 删除打印模板
        /// </summary>
        Task<ApiResponse<string>> DeletePrintTemplateAsync(List<long> ids, long UserId, long OrgId);

        /// <summary>
        /// 获取模板详情
        /// </summary>
        Task<ApiResponse<HolPrintTemplate>> GetTemplateDetailAsync(long templateId);

        /// <summary>
        /// 设置默认模板
        /// </summary>
        Task<ApiResponse<string>> SetDefaultTemplateAsync(long templateId, long OrgId, long UserId);

        /// <summary>
        /// 预览模板
        /// </summary>
        Task<ApiResponse<string>> PreviewTemplateAsync(long templateId, long examId);

        /// <summary>
        /// 打印模板
        /// </summary>
        Task<ApiResponse<string>> PrintTemplateAsync(long templateId, long examId, long UserId, long OrgId);

        /// <summary>
        /// 获取可用的数据字段
        /// </summary>
        Task<ApiResponse<List<object>>> GetAvailableFieldsAsync(string templateType);
    }
}
