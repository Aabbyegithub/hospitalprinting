import axios from '../common/axios'

// 获取打印记录列表
export function getPrintRecordList(examId: number | null, page = 1, size = 10) {
    return axios.get('/api/PrintRecord/GetPage', { 
        params: { examId, page, size } 
    })
}

// 添加打印记录
export function addPrintRecordApi(record: any) {
    return axios.post('/api/PrintRecord/Add', record)
}

// 编辑打印记录
export function editPrintRecordApi(record: any) {
    return axios.post('/api/PrintRecord/Update', record)
}

// 删除打印记录
export function deletePrintRecordApi(ids: number[]) {
    return axios.post('/api/PrintRecord/Delete', ids)
}
