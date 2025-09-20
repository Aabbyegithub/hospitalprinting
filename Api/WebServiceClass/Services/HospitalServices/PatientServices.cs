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

namespace WebServiceClass.Services.HospitalServices
{
    public class PatientServices : IBaseService, IPatientServices
    {
        private readonly ISqlHelper _dal;

        public PatientServices(ISqlHelper dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public async Task<ApiResponse<string>> AddPatientAsync(HolPatient HolPatient, long orgId)
        {
            try
            {
                //HolPatient.orgid_id = orgId;
                HolPatient.createtime = DateTime.Now;
                HolPatient.updatetime = DateTime.Now;
                HolPatient.status = 1;
                var id = await _dal.Db.Insertable(HolPatient).ExecuteReturnBigIdentityAsync();
                return Success<string>(id.ToString(), "患者添加成功");
            }
            catch (Exception)
            {
                return Error<string>("患者保存失败");
            }
        }

        public async Task<ApiResponse<string>> UpdatePatientAsync(HolPatient HolPatient)
        {
            try
            {
                HolPatient.updatetime = DateTime.Now;
                var rows = await _dal.Db.Updateable(HolPatient).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Fail<string>("更新失败");
            }
            catch (Exception)
            {
                return Error<string>("更新失败");
            }
        }

        public async Task<ApiResponse<string>> DeletePatientAsync(List<long> ids)
        {
            try
            {
                var rows = await _dal.Db.Updateable<HolPatient>()
                    .SetColumns(p => new HolPatient { status = 0 })
                    .Where(p => ids.Contains(p.id))
                    .ExecuteCommandAsync();
                return rows > 0 ? Success("删除成功") : Fail<string>("删除失败");
            }
            catch (Exception)
            {
                return Error<string>("删除失败");
            }
        }

        public async Task<List<HolPatient>> GetPatientPageAsync(string? name, string? medicalNo, int page, int size, RefAsync<int> count, long orgId)
        {
            return await _dal.Db.Queryable<HolPatient>()
                .Where(p=>p.status==1)
                //.Where(p => p.orgid_id == orgId && p.status == 1)
                .WhereIF(!string.IsNullOrEmpty(name), p => p.name.Contains(name))
                .WhereIF(!string.IsNullOrEmpty(medicalNo), p => p.medical_no.Contains(medicalNo))
                .OrderBy(p => p.createtime, OrderByType.Desc)
                .ToPageListAsync(page, size, count);
        }

        public async Task<ApiResponse<HolPatient>> GetPatientDetailAsync(long id)
        {
            var data = await _dal.Db.Queryable<HolPatient>().FirstAsync(p => p.id == id && p.status == 1);
            return data != null ? Success(data) : Fail<HolPatient>("未找到患者");
        }
    }
}
