import axios from '../common/axios'

// 打印机 - 分页查询
export function getPrinterPage(name?: string, type?: number, status?: number, page = 1, size = 10) {
  return axios.get('/api/Printer/GetPage', {
    params: { name, type, status, page, size }
  })
}

// 新增打印机
export function addPrinterApi(data: any) {
  return axios.post('/api/Printer/Add', data)
}

// 编辑打印机
export function editPrinterApi(data: any) {
  return axios.post('/api/Printer/Update', data)
}

// 删除打印机
export function deletePrinterApi(ids: number[]) {
  return axios.post('/api/Printer/Delete', ids)
}

// 启用停用
export function togglePrinterStatusApi(id: number, status: number) {
  return axios.post('/api/Printer/ToggleStatus', null, { params: { id, status } })
}

// 测试打印
export function testPrintApi(id: number) {
  return axios.post('/api/Printer/TestPrint', null, { params: { id } })
}


