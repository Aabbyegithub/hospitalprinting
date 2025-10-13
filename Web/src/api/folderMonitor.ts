import axios from '../common/axios'

// 获取文件夹监听配置列表
export function getFolderMonitorListApi(name: string, ipAddress: string, page = 1, size = 10) {
  return axios.get('/api/FolderMonitor/GetPage', { 
    params: { name, ipAddress, page, size } 
  })
}

// 添加文件夹监听配置
export function addFolderMonitorApi(data: any) {
  return axios.post('/api/FolderMonitor/Add', data)
}

// 修改文件夹监听配置
export function updateFolderMonitorApi(data: any) {
  return axios.post('/api/FolderMonitor/Update', data)
}

// 删除文件夹监听配置
export function deleteFolderMonitorApi(ids: number[]) {
  return axios.post('/api/FolderMonitor/Delete', ids)
}

// 切换状态
export function toggleFolderMonitorStatusApi(id: number) {
  return axios.post('/api/FolderMonitor/ToggleStatus', null, { params: { id } })
}
