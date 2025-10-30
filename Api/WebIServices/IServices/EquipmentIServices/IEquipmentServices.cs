using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;

namespace WebIServices.IServices.EquipmentIServices
{
    public interface IEquipmentServices
    {
        /// <summary>
        /// 获取所有打印机
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<List<HolPrinter>>> GetDepartmentAsync();
        
        /// <summary>
        /// 获取激光相机
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<List<HolPrinter>>> GetLaserCameraAsync();

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <returns></returns>
        Task<ApiResponse<List<HolPrinterConfig>>> GetPrintConfigAsync(long PrinterId);

        /// <summary>
        /// 更新打印机使用状态
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> UpdatePrintStausAsync(long PrinterId,int status);

        /// <summary>
        /// 修改打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="holPrinterConfigs"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId ,int Type, int Action, HolPrinterConfig holPrinterConfigs);

        /// <summary>
        /// 获取打印数据
        /// </summary>
        /// <param name="examNo"></param>
        /// <returns></returns>
        Task<ApiResponse<HolExamination>> GetByExamNoAsync(string examNo);

        /// <summary>
        /// 保存打印记录
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        Task<ApiResponse<bool>> SavePrintRecordAsync(PrintRecordModel print);
        /// <summary>
        /// 获取未打印的患者
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<List<HolExamination>>> GetAllUserAsync();

    }
}
