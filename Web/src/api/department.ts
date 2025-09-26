import axios from '../common/axios'

export function getDepartmentList(name: string, page = 1, size = 10) {
    return axios.get('/api/Department/GetDepartmentPage', {
        params: { name, page, size }
    })
}

export function addDepartmentApi(department: any) {
    return axios.post('/api/Department/AddDepartment', department)
}

export function editDepartmentApi(department: any) {
    return axios.post('/api/Department/UpdateDepartment', department)
}

export function deleteDepartmentApi(ids: number[]) {
    return axios.post('/api/Department/DeleteDepartment', ids)
}

export function getDepartmentDetailApi(id: number) {
    return axios.get('/api/Department/GetDepartmentDetail', {
        params: { id }
    })
}
