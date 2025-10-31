using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
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
        /// 获取激光相机
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<HolPrinter>>> GetLaserCameraAsync()
        {
            return await _equipmentServices.GetLaserCameraAsync();
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
        public async Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId,int Type, int Action, HolPrinterConfig holPrinterConfigs)
        {
            return await _equipmentServices.SavePrintConfigAsync(PrinterId,Type, Action, holPrinterConfigs);
        }

        /// <summary>
        /// 获取患者报告
        /// </summary>
        /// <param name="examNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async  Task<ApiResponse<List<HolExamination>>> GetByExamNoAsync(string examNo)
        {
            return await _equipmentServices.GetByExamNoAsync(examNo);
        }

        /// <summary>
        /// 保存打印记录
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<bool>> SavePrintRecordAsync([FromBody] PrintRecordModel print)
        {
            return await _equipmentServices.SavePrintRecordAsync(print);
        }

        /// <summary>
        /// 获取患者队列
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<List<HolExamination>>> GetAllUserAsync()
        {
            return await _equipmentServices.GetAllUserAsync();
        }

        /// <summary>
        /// 获取Ai配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponse<HolAiConfig>> GetAIConfigAsync()
        {
            return await _equipmentServices.GetAIConfigAsync();
        }
    }
}
