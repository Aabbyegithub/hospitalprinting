using Dm.util;
using FellowOakDicom.Tools;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
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
        /// 获取所有患者信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<List<HolExamination>>> GetAllUserAsync()
        {
            var res = await _dal.Db.Queryable<HolExamination>().Includes(a => a.patient).Where(a => a.is_printed == 0 && a.isfees == 1).ToListAsync();
            return Success(res);
        }

        /// <summary>
        /// 获取打印数据
        /// </summary>
        /// <param name="examNo"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<List<HolExamination>>> GetByExamNoAsync(string examNo)
        {
            var res = await _dal.Db.Queryable<HolExamination>().Includes(a => a.patient).Where(a => a.exam_no == examNo).ToListAsync();
            return Success(res);
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

        public async Task<ApiResponse<List<HolPrinter>>> GetLaserCameraAsync()
        {
            var res = await _dal.Db.Queryable<HolPrinter>().Where(a => a.type == 4).ToListAsync();
            return Success(res);
        }

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<List<HolPrinterConfig>>> GetPrintConfigAsync(long PrinterId)
        {
            var res = await _dal.Db.Queryable<HolPrinterConfig>().Where(a=>a.printer_id == PrinterId).ToListAsync();
            return Success(res);
        }

        /// <summary>
        /// 修改打印机配置
        /// </summary>
        /// <param name="PrinterId"></param>
        /// <param name="holPrinterConfigs"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ApiResponse<bool>> SavePrintConfigAsync(long PrinterId,int Type,int Action, HolPrinterConfig holPrinterConfigs)
        {
            try
            {
                var dataList = await _dal.Db
                    .Queryable<HolPrinterConfig>()
                    .Where(a => a.printer_id == PrinterId)
                    .ToListAsync();

                var data = dataList.FirstOrDefault(a => a.film_size == holPrinterConfigs.film_size);

                // Action: 1=添加(存在则更新局部字段)，2=修改(必须存在)，3=删除(存在则删除)
                if (Action == 3)
                {
                    if (data != null)
                    {
                        if (Type == 1)
                            data.print_time_seconds = 0;
                        if (Type == 2)
                            data.available_count = 0;
                        await _dal.Db.Updateable(data).ExecuteCommandAsync();
                    }
                    return Success(true);
                }

                // 确保存在记录；添加时如果不存在则插入
                if (data == null)
                {
                    if (Action == 2)
                    {
                        return Fail<bool>("配置不存在，无法修改！");
                    }
                    var toInsert = new HolPrinterConfig();
                    if (dataList.Count >0)
                    {
                        toInsert = dataList.First();
                        toInsert.id = 0;
                        toInsert.film_size = holPrinterConfigs.film_size;
                        toInsert.create_time = DateTime.Now;
                        toInsert.update_time = DateTime.Now;
                    }
                    else
                    {
                         toInsert = new HolPrinterConfig
                        {
                            printer_id = (int)PrinterId,
                            mask_mode = 1,
                            limit_days = 15,
                            only_unprinted = 1,
                            remark = "一体机添加",
                            film_size = holPrinterConfigs.film_size,
                            org_id = holPrinterConfigs.org_id,
                            create_time = DateTime.Now,
                            update_time = DateTime.Now
                        };
                    }


                    // 按类型设置字段
                    if (Type == 1)
                    {
                        toInsert.print_time_seconds = holPrinterConfigs.print_time_seconds;
                    }
                    else if (Type == 2)
                    {
                        toInsert.available_count = holPrinterConfigs.available_count;
                    }
                    else // Type == 3 或 其他：更新通用参数
                    {
                        toInsert.mask_mode = holPrinterConfigs.mask_mode;
                        toInsert.limit_days = holPrinterConfigs.limit_days;
                        toInsert.allowed_exam_types = holPrinterConfigs.allowed_exam_types;
                        toInsert.laser_printer_id = holPrinterConfigs.laser_printer_id;
                        toInsert.only_unprinted = holPrinterConfigs.only_unprinted;
                        toInsert.remark = holPrinterConfigs.remark;
                    }

                    await _dal.Db.Insertable(toInsert).ExecuteCommandAsync();
                    return Success(true);
                }

                // 走到这里表示需要更新已有记录（Action=1 或 2）
                if (Type == 1) // 等待时间
                {
                    data.print_time_seconds = holPrinterConfigs.print_time_seconds;
                }
                else if (Type == 2) // 数量
                {
                    data.available_count = holPrinterConfigs.available_count;
                }
                else // Type == 3 或其他：更新通用参数
                {
                    data.mask_mode = holPrinterConfigs.mask_mode;
                    data.limit_days = holPrinterConfigs.limit_days;
                    data.allowed_exam_types = holPrinterConfigs.allowed_exam_types;
                    data.laser_printer_id = holPrinterConfigs.laser_printer_id;
                    data.only_unprinted = holPrinterConfigs.only_unprinted;
                    data.remark = holPrinterConfigs.remark;
                }

                data.update_time = DateTime.Now;
                await _dal.Db.Updateable(data).ExecuteCommandAsync();
                return Success(true);
            }
            catch (Exception)
            {
                return Fail<bool>("保存失败！");
            }
        }

        /// <summary>
        /// 保存打印记录
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public async Task<ApiResponse<bool>> SavePrintRecordAsync(PrintRecordModel print)
        {
            await _dal.Db.Insertable(print).ExecuteCommandAsync();
            return Success(true);
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
