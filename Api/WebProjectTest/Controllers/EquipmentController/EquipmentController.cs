using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using WebIServices.IBase;
using WebIServices.IServices.EquipmentIServices;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.EquipmentController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EquipmentController(IEquipmentServices _equipmentServices) : ControllerBase
    {
        /// <summary>
        /// 获取所有打印机
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async  Task<ApiResponse<List<HolPrinter>>> GetDepartmentAsync()
        {
            return await _equipmentServices.GetDepartmentAsync();
        }

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<HolPrinterConfig>>> GetPrintConfigAsync(long PrinterId)
        {
            return await _equipmentServices.GetPrintConfigAsync(PrinterId);
        }

        /// <summary>
        /// 更新打印机运行状态
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<bool>> UpdatePrintStausAsync(long PrinterId,int status)
        {
            return await _equipmentServices.UpdatePrintStausAsync(PrinterId,status);
        }

        /// <summary>
        /// 保存打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="holPrinterConfigs"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId ,List<HolPrinterConfig> holPrinterConfigs)
        {
            return await _equipmentServices.SavePrintConfigAsync(PrinterId,holPrinterConfigs);
        }
    }
}
