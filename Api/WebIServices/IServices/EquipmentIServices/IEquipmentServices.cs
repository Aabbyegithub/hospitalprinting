using ModelClassLibrary.Model.HolModel;
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
        /// 获取打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <returns></returns>
        Task<ApiResponse<List<HolPrinter>>> GetPrintConfigAsync(long PrinterId);

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
        Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId ,List<HolPrinterConfig> holPrinterConfigs);

    }
}
