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
    public class DepartmentServices : IBaseService, IDepartmentServices
    {
        private readonly ISqlHelper _dal;

        public DepartmentServices(ISqlHelper dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public async Task<ApiResponse<string>> AddDepartmentAsync(HolDepartment dept, long orgId)
        {
            try
            {
                dept.orgid_id = orgId;
                dept.createtime = DateTime.Now;
                dept.updatetime = DateTime.Now;
                dept.status = 1;

                var id = await _dal.Db.Insertable(dept).ExecuteReturnBigIdentityAsync();
                return Success(id.ToString(), "科室添加成功");
            }
            catch (Exception)
            {
                return Error<string>("科室保存失败");
            }
        }

        public async Task<ApiResponse<string>> UpdateDepartmentAsync(HolDepartment dept)
        {
            try
            {
                dept.updatetime = DateTime.Now;
                var rows = await _dal.Db.Updateable(dept).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Fail<string>("更新失败");
            }
            catch (Exception)
            {
                return Error<string>("更新失败");
            }
        }

        public async Task<ApiResponse<string>> DeleteDepartmentAsync(List<long> ids)
        {
            try
            {
                var rows = await _dal.Db.Updateable<HolDepartment>()
                    .SetColumns(d => new HolDepartment { status = 0 })
                    .Where(d => ids.Contains(d.id))
                    .ExecuteCommandAsync();
                return rows > 0 ? Success("删除成功") : Fail<string>("删除失败");
            }
            catch (Exception)
            {
                return Error<string>("删除失败");
            }
        }

        public async Task<List<HolDepartmentDto>> GetDepartmentPageAsync(string? name, int page, int size, RefAsync<int> count, long orgId)
        {
            return await _dal.Db.Queryable<HolDepartment>()
                .LeftJoin<sys_orgid>((dep, org) => dep.orgid_id == org.orgid_id) // 关联组织表
                .Where((dep, org) => dep.status == 1 && dep.orgid_id == orgId)
                .WhereIF(!string.IsNullOrEmpty(name), (dep, org) => dep.name.Contains(name))
                .OrderBy((dep, org) => dep.createtime, OrderByType.Desc)
                .Select((dep, org) => new HolDepartmentDto
                {
                    id = dep.id,
                    name = dep.name,
                    code = dep.code,
                    description = dep.description,
                    orgid_id = dep.orgid_id,
                    orgid_name = org.orgid_name,   // 显示组织名
                    status = dep.status,
                    createtime = dep.createtime,
                    updatetime = dep.updatetime
                })
                .ToPageListAsync(page, size, count);
        }


        public async Task<ApiResponse<HolDepartmentDto>> GetDepartmentDetailAsync(long id)
        {
            var data = await _dal.Db.Queryable<HolDepartment>()
                .LeftJoin<sys_orgid>((dep, org) => dep.orgid_id == org.orgid_id)
                .Where((dep, org) => dep.id == id && dep.status == 1)
                .Select((dep, org) => new HolDepartmentDto
                {
                    id = dep.id,
                    name = dep.name,
                    code = dep.code,
                    description = dep.description,
                    orgid_id = dep.orgid_id,
                    orgid_name = org.orgid_name,
                    status = dep.status,
                    createtime = dep.createtime,
                    updatetime = dep.updatetime
                })
                .FirstAsync();

            return data != null ? Success(data) : Fail<HolDepartmentDto>("未找到科室");
        }

    }

}
