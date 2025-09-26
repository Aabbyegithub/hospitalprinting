using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;

namespace WebServiceClass.Services.HospitalServices
{
    public class PrintRecordServices(ISqlHelper dal) : IBaseService, IPrintRecordServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<string>> AddAsync(PrintRecordModel record)
        {
            try
            {
                record.print_time = DateTime.Now;
                var id = await _dal.Db.Insertable(record).ExecuteReturnBigIdentityAsync();
                return id > 0 ? Success("添加成功") : Error<string>("添加失败");
            }
            catch (Exception e)
            {
                return Error<string>($"添加失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> UpdateAsync(PrintRecordModel record)
        {
            try
            {
                record.print_time = DateTime.Now;
                var rows = await _dal.Db.Updateable(record).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(List<long> ids)
        {
            try
            {
                var rows = await _dal.Db.Updateable<PrintRecordModel>().SetColumns(a => new PrintRecordModel { status = 0, update_time = DateTime.Now })
                    .Where(a => ids.Contains(a.id))
                    .ExecuteCommandAsync();
                return rows > 0 ? Success("删除成功") : Error<string>("删除失败");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }

        public async Task<List<PrintRecordModel>> GetPageAsync(long? examId, int page, int size, RefAsync<int> count)
        {
            return await _dal.Db.Queryable<PrintRecordModel>().Includes(a=>a.holExamination,then=>then.doctor).Includes(a => a.holPatient).Where(a=>a.status==1)
                .WhereIF(examId != null, a => a.exam_id == examId)
                .ToPageListAsync(page, size, count);
        }
    }
}
