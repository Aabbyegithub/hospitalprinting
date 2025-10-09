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

namespace WebServiceClass.Services.HospitalServices
{
    public class PrinterServices(ISqlHelper dal) : IBaseService, IPrinterServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<string>> AddPrinterAsync(HolPrinter printer, long OrgId, long UserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(printer.name)) return Error<string>("打印机名称不能为空");
                if (printer.type is < 1 or > 3) return Error<string>("打印机类型不合法");

                printer.org_id = OrgId;
                printer.create_time = DateTime.Now;
                printer.update_time = DateTime.Now;
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
    }
}
