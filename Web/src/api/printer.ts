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

// 测试连通性
export function testConnectivityApi(id: number) {
  return axios.post('/api/Printer/TestConnectivity', null, { params: { id } })
}

// 获取胶片尺寸配置
export function getFilmSizeConfigsApi(printerId: number) {
  return axios.get('/api/Printer/GetFilmSizeConfigs', { params: { printerId } })
}

// 保存胶片尺寸配置
export function saveFilmSizeConfigApi(data: any) {
  return axios.post('/api/Printer/SaveFilmSizeConfig', data)
}

// 删除胶片尺寸配置
export function deleteFilmSizeConfigApi(id: number) {
  return axios.post('/api/Printer/DeleteFilmSizeConfig', null, { params: { id } })
}

// 删除打印机的所有胶片尺寸配置
export function deleteAllFilmSizeConfigsApi(printerId: number) {
  return axios.post('/api/Printer/DeleteAllFilmSizeConfigs', null, { params: { printerId } })
}


// 获取打印机配置
export function getPrinterConfigApi(printerId: number) {
  return axios.get('/api/Printer/GetConfig', { params: { printerId } })
}

// 保存打印机配置
export function savePrinterConfigApi(data: any) {
  return axios.post('/api/Printer/SaveConfig', data)
}


