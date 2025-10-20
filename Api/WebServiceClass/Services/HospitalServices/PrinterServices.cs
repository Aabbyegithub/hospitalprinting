using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using ModelClassLibrary.Model.HolModel;
using ModelClassLibrary.Model.Dto.HolDto;
using Microsoft.Extensions.Configuration;

namespace WebServiceClass.Services.HospitalServices
{
    public class PrinterServices(ISqlHelper dal, IConfiguration config) : IBaseService, IPrinterServices
    {
        private readonly ISqlHelper _dal = dal;
        private readonly IConfiguration _config = config;

        public async Task<ApiResponse<string>> AddPrinterAsync(HolPrinter printer, long OrgId, long UserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(printer.name)) return Error<string>("打印机名称不能为空");
                if (printer.type is < 1 or > 4) return Error<string>("打印机类型不合法");

                printer.org_id = OrgId;
                printer.create_time = DateTime.Now;
                printer.update_time = DateTime.Now;

                // 如果没有设置 server_url，自动获取当前服务器地址
                if (string.IsNullOrWhiteSpace(printer.server_url))
                {
                    //printer.server_url = "http://localhost:7092"; // 默认值，实际应从配置获取
                    printer.server_url = _config["Printer:ServerUrl"];
                }
                var id = await _dal.Db.Insertable(printer).ExecuteReturnBigIdentityAsync();
                return id > 0 ? Success("添加成功") : Error<string>("保存失败");
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> UpdatePrinterAsync(HolPrinter printer, long UserId)
        {
            try
            {
                printer.update_time = DateTime.Now;
                var rows = await _dal.Db.Updateable(printer).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeletePrinterAsync(List<long> ids, long UserId, long OrgId)
        {
            try
            {
                var rows = await _dal.Db.Deleteable<HolPrinter>().In(ids).ExecuteCommandAsync();
                return rows > 0 ? Success("删除成功") : Error<string>("删除失败");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }

        public async Task<List<HolPrinter>> GetPrinterPageAsync(string? name, int? type, int? status, int page, int size, RefAsync<int> count, long OrgId)
        {
            return await _dal.Db.Queryable<HolPrinter>()
                //.Where(a => a.org_id == OrgId) // 如果需要按机构隔离
                .WhereIF(!string.IsNullOrWhiteSpace(name), a => a.name.Contains(name!))
                .WhereIF(type.HasValue, a => a.type == type.Value)
                .WhereIF(status.HasValue, a => a.status == status.Value)
                .OrderBy(a => a.update_time, OrderByType.Desc)
                .ToPageListAsync(page, size, count);
        }
        /// <summary>
        /// 切换打印机启用状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> TogglePrinterStatusAsync(long id, int status, long UserId)
        {
            try
            {
                var rows = await _dal.Db.Updateable<HolPrinter>()
                    .SetColumns(a => new HolPrinter { status = status, update_time = DateTime.Now })
                    .Where(a => a.id == id)
                    .ExecuteCommandAsync();
                return rows > 0 ? Success("状态已更新") : Error<string>("状态更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"状态更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> TestPrintAsync(long id, long UserId)
        {
            // 这里仅做占位，真实打印逻辑按你现有打印服务对接
            var exists = await _dal.Db.Queryable<HolPrinter>().AnyAsync(a => a.id == id);
            return exists ? Success("测试打印任务已下发") : Error<string>("打印机不存在");
        }

        /// <summary>
        /// 获取打印机配置
        /// </summary>
        /// <param name="printerId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public async Task<HolPrinterConfigDto?> GetConfigAsync(long printerId, long OrgId)
        {
            var cfg = await _dal.Db.Queryable<HolPrinterConfig>()
                //.Where(a => a.org_id == OrgId)
                .FirstAsync(a => a.printer_id == printerId);

            if (cfg == null) return null;

            var dto = new HolPrinterConfigDto
            {
                id = cfg.id,
                printer_id = cfg.printer_id,
                mask_mode = cfg.mask_mode ,
                limit_days = cfg.limit_days,
                allowed_exam_types = string.IsNullOrWhiteSpace(cfg.allowed_exam_types)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(cfg.allowed_exam_types!) ?? new List<string>(),
                only_unprinted = cfg.only_unprinted ,
                laser_printer_id = cfg.laser_printer_id,
                remark = cfg.remark
            };
            return dto;
        }
        /// <summary>
        /// 保存打印机配置
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OrgId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> SaveConfigAsync(HolPrinterConfigDto dto, long OrgId, long UserId)
        {
            try
            {
                var now = DateTime.Now;
                var json = System.Text.Json.JsonSerializer.Serialize(dto.allowed_exam_types ?? new List<string>());

                // UPSERT by printer_id
                var exists = await _dal.Db.Queryable<HolPrinterConfig>()
                    .AnyAsync(a => a.printer_id == dto.printer_id /* && a.org_id == OrgId */);

                if (exists)
                {
                    var rows = await _dal.Db.Updateable<HolPrinterConfig>()
                        .SetColumns(a => new HolPrinterConfig
                        {
                            mask_mode = dto.mask_mode ,
                            limit_days = dto.limit_days,
                            allowed_exam_types = json,
                            laser_printer_id = dto.laser_printer_id,
                            only_unprinted = dto.only_unprinted,
                            remark = dto.remark,
                            update_time = now
                        })
                        .Where(a => a.printer_id == dto.printer_id /* && a.org_id == OrgId */)
                        .ExecuteCommandAsync();

                    return rows > 0 ? Success("配置已更新") : Error<string>("配置更新失败");
                }
                else
                {
                    var cfg = new HolPrinterConfig
                    {
                        printer_id = dto.printer_id,
                        mask_mode = dto.mask_mode,
                        limit_days = dto.limit_days,
                        allowed_exam_types = json,
                        only_unprinted = dto.only_unprinted,
                        laser_printer_id = dto.laser_printer_id,
                        remark = dto.remark,
                        org_id = OrgId,
                        create_time = now,
                        update_time = now
                    };
                    var id = await _dal.Db.Insertable(cfg).ExecuteReturnBigIdentityAsync();
                    return id > 0 ? Success("配置已保存") : Error<string>("配置保存失败");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<List<HolPrinterConfigDto>> GetFilmSizeConfigsAsync(long printerId, long OrgId)
        {
            var configs = await _dal.Db.Queryable<HolPrinterConfig>()
                .Includes(a => a.laser_printer)
                .Where(a => a.printer_id == printerId && !string.IsNullOrEmpty(a.film_size))
                .OrderBy(a => a.film_size)
                .ToListAsync();

            return configs.Select(c => new HolPrinterConfigDto
            {
                id = c.id,
                printer_id = c.printer_id,
                mask_mode = c.mask_mode,
                limit_days = c.limit_days,
                allowed_exam_types = string.IsNullOrWhiteSpace(c.allowed_exam_types)
                    ? new List<string>()
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.allowed_exam_types!) ?? new List<string>(),
                only_unprinted = c.only_unprinted,
                laser_printer_id = c.laser_printer_id,
                film_size = c.film_size,
                available_count = c.available_count,
                print_time_seconds = c.print_time_seconds,
                remark = c.remark
            }).ToList();
        }

        /// <summary>
        /// 保存打印机尺寸设置
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="OrgId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<string>> SaveFilmSizeConfigAsync(HolPrinterConfigDto dto, long OrgId, long UserId)
        {
            try
            {
                var now = DateTime.Now;
                var json = System.Text.Json.JsonSerializer.Serialize(dto.allowed_exam_types ?? new List<string>());

                if (dto.id > 0)
                {
                    // 更新现有配置
                    var rows = await _dal.Db.Updateable<HolPrinterConfig>()
                        .SetColumns(a => new HolPrinterConfig
                        {
                            mask_mode = dto.mask_mode,
                            limit_days = dto.limit_days,
                            allowed_exam_types = json,
                            only_unprinted = dto.only_unprinted,
                            laser_printer_id = dto.laser_printer_id,
                            available_count = dto.available_count,
                            print_time_seconds = dto.print_time_seconds,
                            remark = dto.remark,
                            update_time = now
                        })
                        .Where(a => a.id == dto.id)
                        .ExecuteCommandAsync();

                    return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
                }
                else
                {
                    // 新增配置（允许重复）
                    var cfg = new HolPrinterConfig
                    {
                        printer_id = dto.printer_id,
                        mask_mode = dto.mask_mode,
                        limit_days = dto.limit_days,
                        allowed_exam_types = json,
                        only_unprinted = dto.only_unprinted,
                        laser_printer_id = dto.laser_printer_id,
                        film_size = dto.film_size,
                        available_count = dto.available_count,
                        print_time_seconds = dto.print_time_seconds,
                        remark = dto.remark,
                        org_id = OrgId,
                        create_time = now,
                        update_time = now
                    };
                    var id = await _dal.Db.Insertable(cfg).ExecuteReturnBigIdentityAsync();
                    return id > 0 ? Success("添加成功") : Error<string>("添加失败");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteAllFilmSizeConfigsAsync(long printerId, long UserId, long OrgId)
        {
            try
            {
                var rows = await _dal.Db.Deleteable<HolPrinterConfig>()
                    .Where(a => a.printer_id == printerId )
                    .ExecuteCommandAsync();

                return rows > 0 ? Success("删除成功") : Error<string>("删除失败");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }
    }
}
