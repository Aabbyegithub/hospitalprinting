using SQLitePCL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using MyNamespace;
using ModelClassLibrary.Model.HolModel;
using System.Runtime.CompilerServices;

namespace WebServiceClass.Services.HospitalServices
{
    public class ExaminationServices(ISqlHelper dal) : IBaseService, IExaminationServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<string>> AddExaminationAsync(HolExamination exam, long OrgId, long UserId)
        {
            try
            {
                // 验证必填字段
                if (string.IsNullOrEmpty(exam.exam_no))
                    return Error<string>("检查号不能为空");

                if (exam.patient_id <= 0)
                    return Error<string>("患者ID不能为空");
                exam.org_id = OrgId;
                exam.create_time = DateTime.Now;
                exam.update_time = DateTime.Now;
                var id = await _dal.Db.Insertable(exam).ExecuteReturnBigIdentityAsync();
                return id > 0 ? Success("添加成功") : Error<string>("保存失败");
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<List<HolExamination>> GetExaminationPageAsync(string? examNo, string? examType, string? patientName, int? isPrinted, DateTime? examDate,
            int page, int size, RefAsync<int> count, long OrgId)
        {
            return await _dal.Db.Queryable<HolExamination>()
                .Includes(a => a.patient)
                .Includes(a => a.doctor, then=>then.holdepartment)  
                .Where(a => a.status == 1)
                //.Where(a => a.org_id == OrgId && a.status == 1)
                .WhereIF(!string.IsNullOrEmpty(examNo), a => a.exam_no.Contains(examNo))
                .WhereIF(!string.IsNullOrEmpty(examType), a => a.exam_type == examType)
                .WhereIF(!string.IsNullOrEmpty(patientName), a => a.patient.name.Contains(patientName))
                .WhereIF(examDate != null, a => a.exam_date.Date == examDate.Value.Date)
                .WhereIF(isPrinted.HasValue, a => a.is_printed == isPrinted)
                .ToPageListAsync(page, size, count);
        }

        public async Task<ApiResponse<string>> UpdateExaminationAsync(HolExamination exam, long UserId)
        {
            try
            {
                exam.update_time = DateTime.Now;
                var rows = await _dal.Db.Updateable(exam).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteExaminationAsync(List<long> ids, long UserId, long OrgId)
        {
            try
            {
                // 逻辑删除
                await _dal.Db.Updateable<HolExamination>()
                    .SetColumns(a => new HolExamination { status = 0, update_time = DateTime.Now })
                    .Where(a => ids.Contains(a.id))
                    .ExecuteCommandAsync();

                // 写入日志表
                //var logs = ids.Select(id => new delete_log
                //{
                //    table_name = "HolExamination",
                //    record_id = id,
                //    deleted_by = UserId,
                //    deleted_time = DateTime.Now,
                //    reason = "用户删除检查数据",
                //    org_id = OrgId
                //}).ToList();

                //await _dal.Db.Insertable(logs).ExecuteCommandAsync();
                return Success("删除成功");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }
        public async Task<ApiResponse<string>> PrintExaminationAsync(long examId)
        {
            try
            {
                var exam = await _dal.Db.Queryable<HolExamination>().FirstAsync(a => a.id == examId && a.status == 1);
                if (exam == null)
                    return Fail<string>("检查记录不存在");

                if (exam.is_printed == 1)
                    return Fail<string>("该检查已打印，无法重复打印");

                // 开始事务
                _dal.Db.Ado.BeginTran();
                try
                {
                    // 更新检查记录打印状态
                    await _dal.Db.Updateable<HolExamination>()
                        .SetColumns(a => new HolExamination
                        {
                            is_printed = 1,
                            update_time = DateTime.Now
                        })
                        .Where(a => a.id == examId)
                        .ExecuteCommandAsync();

                    // 插入打印记录
                    await _dal.Db.Insertable(new PrintRecordModel
                    {
                        exam_id = exam.id,
                        patient_id = exam.patient_id,
                        printed_by = exam.patient_id,
                        print_time = DateTime.Now,
                        status = 1,
                        create_time = DateTime.Now,
                        update_time = DateTime.Now
                    }).ExecuteCommandAsync();

                    _dal.Db.Ado.CommitTran();
                    return Success("打印成功");
                }
                catch (Exception e)
                {
                    _dal.Db.Ado.RollbackTran();
                    return Error<string>($"打印失败:{e.Message}");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"打印失败:{e.Message}");
            }
        }


        public async Task<ApiResponse<string>> UnlockPrintAsync(long examId)
        {
            try
            {
                _dal.Db.Ado.BeginTran();
                try
                {
                    await _dal.Db.Updateable<HolExamination>()
                        .SetColumns(a => new HolExamination
                        {
                            is_printed = 0,
                            update_time = DateTime.Now
                        })
                        .Where(a => a.id == examId)
                        .ExecuteCommandAsync();

                    await _dal.Db.Deleteable<PrintRecordModel>()
                        .Where(a => a.exam_id == examId)
                        .ExecuteCommandAsync();

                    _dal.Db.Ado.CommitTran();
                    return Success("打印状态已解锁，可重新打印");
                }
                catch (Exception e)
                {
                    _dal.Db.Ado.RollbackTran();
                    return Error<string>($"解锁失败:{e.Message}");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"解锁失败:{e.Message}");
            }
        }

    }
}
