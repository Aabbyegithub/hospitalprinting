import axios from '../common/axios'

// 获取患者列表
export function getPatientList(name: string, medicalNo: string, page = 1, size = 10) {
    return axios.get('/api/Patient/GetPatientPage', { 
        params: { name, medicalNo, page, size } 
    })
}

// 添加患者
export function addPatientApi(patient: any) {
    return axios.post('/api/Patient/AddPatient', patient)
}

// 编辑患者
export function editPatientApi(patient: any) {
    return axios.post('/api/Patient/UpdatePatient', patient)
}

// 删除患者
export function deletePatientApi(ids: number[]) {
    return axios.post('/api/Patient/DeletePatient', ids)
}

// 获取患者详情
export function getPatientDetailApi(id: number) {
    return axios.get('/api/Patient/GetPatientDetail', { 
        params: { id } 
    })
}
