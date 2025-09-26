import axios from '../common/axios'

// departmentId 与后端保持一致
export function getDoctorList(name: string, departmentId: number | null, page = 1, size = 10) {
    return axios.get('/api/Doctor/GetDoctorPage', {
        params: { name, departmentId, page, size }
    })
}

export function addDoctorApi(doctor: any) {
    return axios.post('/api/Doctor/AddDoctor', doctor)
}

export function editDoctorApi(doctor: any) {
    return axios.post('/api/Doctor/UpdateDoctor', doctor)
}

export function deleteDoctorApi(ids: number[]) {
    return axios.post('/api/Doctor/DeleteDoctor', ids)
}

export function getDoctorDetailApi(id: number) {
    return axios.get('/api/Doctor/GetDoctorDetail', {
        params: { id }
    })
}
