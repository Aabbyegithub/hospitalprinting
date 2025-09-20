import axios from '../common/axios'

// 获取检查列表
export function getExaminationList(examNo: string, patientName: string, examDate: string, page = 1, size = 10) {
    return axios.get('/api/Examination/GetPage', { 
        params: { examNo, patientName, examDate, page, size } 
    })
}

// 添加检查记录
export function addExaminationApi(exam: any) {
    return axios.post('/api/Examination/Add', exam)
}

// 编辑检查记录
export function editExaminationApi(examination: any) {
    return axios.post('/api/Examination/Update', examination)
}

// 删除检查记录
export function deleteExaminationApi(ids: number[]) {
    return axios.post('/api/Examination/Delete', ids)
}

// 打印检查报告
export function printExaminationApi(examId: number) {
    return axios.post('/api/Examination/Print', null, { 
        params: { examId } 
    })
}

// 解锁打印状态
export function unlockPrintApi(examId: number) {
    return axios.post('/api/Examination/UnlockPrint', null, { 
        params: { examId } 
    })
}
