import axios from '../common/axios'

//获取角色权限列表
export function getRolePermissionList(RoleName:string='',page:number=1,size:number=10) {
    return axios.get('/api/Role/GetRoleList',{params: { RoleName,page,size } })
}
//添加角色权限
export function addRolePermissionApi(role_name:string,description:string='') {
    return axios.post('/api/Role/AddRole',{ role_name,description })
}
//编辑角色权限
export function editRolePermissionApi(role_id:string,role_name:string,description:string='') {
    return axios.post('/api/Role/UpdateRole',{ role_id,role_name,description })
}
//删除角色权限
export function deleteRolePermissionApi( roleIds:number ) {
    return axios.delete(`/api/Role/DeleteRole/${roleIds}` )
}
//获取所有权限
export function getAllPermissions(role_id:string) {
    return axios.get('/api/Role/GetUserPermissions',{ params: { role_id } })
}
//获取角色详情
export function getRoleDetail(roleId:string) {
    return axios.get('/api/RolePermission/GetRoleDetail',{ params: { roleId } })
}
//保存角色权限
export function saveRolePermissions(role_id:string,permissions:number[]) {
    return axios.post(`/api/Role/UpdateRolePermissions?roleId=${role_id}`, permissions )
}