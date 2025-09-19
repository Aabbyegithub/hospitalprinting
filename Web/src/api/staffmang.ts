import axios from '../common/axios'

// 获取员工列表
export function getStaffList(name:string,username:string,phone:string, page = 1, size = 10) {
    return axios.get('/api/User/GetUser', { params: {name,username,phone, page, size} })
}
// 添加员工
export function addStaffApi(name:string, username:string, password:string, phone:string, position:string, orgid_id:string,status:number,roleId:number) {
    return axios.post('/api/User/AddUser', {name,username,password,phone,position,orgid_id,status,roleId})
}
// 编辑员工
export function editStaffApi(staff_id:number,name:string, username:string, password:string, phone:string, position:string, orgid_id:string,status:number,roleId:number) {
    return axios.post(`/api/User/UpUser`,  {staff_id,name,username,password,phone,position,orgid_id,status,roleId})
}
// 删除员工
export function deleteStaffApi(staffIds: number[]) {
    return axios.post(`/api/User/Delete`,staffIds)
}

//获取角色列表
export function getRoleList() {
    return axios.get('/api/Role/GetAllRoleList')
}