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
using ModelClassLibrary.Model.Dto.HolDto;
using MyNamespace;

namespace WebServiceClass.Services.HospitalServices
{
    public class DoctorServices : IBaseService, IDoctorServices
    {
        private readonly ISqlHelper _dal;

        public DoctorServices(ISqlHelper dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public async Task<ApiResponse<string>> AddDoctorAsync(HolDoctor doctor, long orgId)
        {
            try
            {
                doctor.orgid_id = orgId;
                doctor.createtime = DateTime.Now;
                doctor.updatetime = DateTime.Now;
                doctor.status = 1;

                var id = await _dal.Db.Insertable(doctor).ExecuteReturnBigIdentityAsync();
                return Success(id.ToString(), "医生添加成功");
            }
            catch (Exception)
            {
                return Error<string>("医生保存失败");
            }
        }

        public async Task<ApiResponse<string>> UpdateDoctorAsync(HolDoctor doctor)
        {
            try
            {
                doctor.updatetime = DateTime.Now;
                var rows = await _dal.Db.Updateable(doctor).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Fail<string>("更新失败");
            }
            catch (Exception)
            {
                return Error<string>("更新失败");
            }
        }

        public async Task<ApiResponse<string>> DeleteDoctorAsync(List<long> ids)
        {
            try
            {
                var rows = await _dal.Db.Updateable<HolDoctor>()
                    .SetColumns(d => new HolDoctor { status = 0 })
                    .Where(d => ids.Contains(d.id))
                    .ExecuteCommandAsync();
                return rows > 0 ? Success("删除成功") : Fail<string>("删除失败");
            }
            catch (Exception)
            {
                return Error<string>("删除失败");
            }
        }

        public async Task<List<HolDoctorDto>> GetDoctorPageAsync(string? name, long? departmentId, int page, int size, RefAsync<int> count, long orgId)
        {
            return await _dal.Db.Queryable<HolDoctor>()
                .Includes(doc=>doc.holdepartment)
                .LeftJoin<HolDepartment>((doc, dep) => doc.department_id == dep.id)
                .LeftJoin<sys_orgid>((doc, dep, org) => org.orgid_id == doc.orgid_id)
                .Where((doc, dep) => doc.status == 1 && doc.orgid_id == orgId && dep.status == 1)
                .WhereIF(!string.IsNullOrEmpty(name), (doc, dep) => doc.name.Contains(name))
                .WhereIF(departmentId.HasValue, (doc, dep) => doc.department_id == departmentId.Value)
                .OrderBy((doc, dep) => doc.createtime, OrderByType.Desc)
                .Select((doc, dep,org) => new HolDoctorDto
                {
                    id = doc.id,
                    name = doc.name,
                    gender = doc.gender,
                    phone = doc.phone,
                    department_id = doc.department_id,
                    department_name = dep.name,   // 取科室名
                    title = doc.title,
                    introduction = doc.introduction,
                    orgid_id = doc.orgid_id,
                    orgid_name = org.orgid_name,
                    status = doc.status,
                    createtime = doc.createtime,
                    updatetime = doc.updatetime
                })
                .ToPageListAsync(page, size, count);
        }

        public async Task<ApiResponse<HolDoctorDto>> GetDoctorDetailAsync(long id)
        {
            var data = await _dal.Db.Queryable<HolDoctor>()
                .LeftJoin<HolDepartment>((doc, dep) => doc.department_id == dep.id)
                .Where((doc, dep) => doc.id == id && doc.status == 1 && dep.status == 1)
                .Select((doc, dep) => new HolDoctorDto
                {
                    id = doc.id,
                    name = doc.name,
                    gender = doc.gender,
                    phone = doc.phone,
                    department_id = doc.department_id,
                    department_name = dep.name,
                    title = doc.title,
                    introduction = doc.introduction,
                    orgid_id = doc.orgid_id,
                    status = doc.status,
                    createtime = doc.createtime,
                    updatetime = doc.updatetime
                })
                .FirstAsync();

            return data != null ? Success(data) : Fail<HolDoctorDto>("未找到医生");
        }
    }

}
