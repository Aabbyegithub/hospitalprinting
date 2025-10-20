using FellowOakDicom.Tools;
using ModelClassLibrary.Model.HolModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.EquipmentIServices;
using static WebProjectTest.Common.Message;

namespace WebServiceClass.Services.EquipmentServices
{
    public class EquipmentServices:IEquipmentServices,IBaseService
    {
        private readonly ISqlHelper _dal;
        public EquipmentServices(ISqlHelper dal)
        {
            _dal = dal;
        }

        /// <summary>
        /// 获取所有打印机
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<List<HolPrinter>>> GetDepartmentAsync()
        {
            var res = await _dal.Db.Queryable<HolPrinter>().Where(a => a.type != 4).ToListAsync();
            return Success(res);
        }

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ApiResponse<List<HolPrinter>>> GetPrintConfigAsync(long PrinterId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="holPrinterConfigs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId, List<HolPrinterConfig> holPrinterConfigs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新设备运行状态
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<ApiResponse<bool>> UpdatePrintStausAsync(long PrinterId, int status)
        {
            await _dal.Db.Updateable<HolPrinter>().SetColumns(a => a.status == status).Where(a => a.id == PrinterId).ExecuteCommandAsync();
            return Success(true);
        }
    }
}
