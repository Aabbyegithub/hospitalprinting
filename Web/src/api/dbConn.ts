import axios from '../common/axios'

// 最小化数据库连接配置 CRUD

export function listDbConn(configName: string = '', orgId: number = 1, pageIndex: number = 1, pageSize: number = 10) {
  return axios.get('/api/DbConn/GetConfigList', { params: { configName, orgId, pageIndex, pageSize } })
}

export function getDbConn(id: number) {
  return axios.get('/api/DbConn/GetConfigById', { params: { id } })
}

export function addDbConn(data: any) {
  return axios.post('/api/DbConn/AddConfig', data)
}

export function updateDbConn(data: any) {
  return axios.post('/api/DbConn/UpdateConfig', data)
}

export function deleteDbConn(ids: number[]) {
  return axios.post('/api/DbConn/DeleteConfig', ids)
}


