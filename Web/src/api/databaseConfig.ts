import axios from '../common/axios'

// 数据库配置相关接口

/**
 * 获取数据库配置列表
 */
export function getDatabaseConfigList(configName: string = '', orgId: number = 1, pageIndex: number = 1, pageSize: number = 10) {
  return axios.get('/api/DatabaseConfig/GetConfigList', {
    params: { configName, orgId, pageIndex, pageSize }
  })
}

/**
 * 获取数据库配置详情
 */
export function getDatabaseConfigById(id: number) {
  return axios.get('/api/DatabaseConfig/GetConfigById', {
    params: { id }
  })
}

/**
 * 新增数据库配置
 */
export function addDatabaseConfig(data: any, userId: number = 1) {
  return axios.post('/api/DatabaseConfig/AddConfig', data, {
    params: { userId }
  })
}

/**
 * 更新数据库配置
 */
export function updateDatabaseConfig(data: any, userId: number = 1) {
  return axios.post('/api/DatabaseConfig/UpdateConfig', data, {
    params: { userId }
  })
}

/**
 * 删除数据库配置
 */
export function deleteDatabaseConfig(ids: number[]) {
  return axios.post('/api/DatabaseConfig/DeleteConfig', ids)
}

/**
 * 测试数据库连接
 */
export function testDatabaseConnection(data: any) {
  return axios.post('/api/DatabaseConfig/TestConnection', data)
}

/**
 * 手动同步数据
 */
export function manualSyncDatabase(configId: number, userId: number = 1) {
  return axios.post('/api/DatabaseConfig/ManualSync', null, {
    params: { configId, userId }
  })
}

/**
 * 获取同步日志
 */
export function getDatabaseSyncLogs(configId: number, pageIndex: number = 1, pageSize: number = 10) {
  return axios.get('/api/DatabaseConfig/GetSyncLogs', {
    params: { configId, pageIndex, pageSize }
  })
}

/**
 * 设置默认配置
 */
export function setDefaultDatabaseConfig(configId: number, orgId: number = 1) {
  return axios.post('/api/DatabaseConfig/SetDefaultConfig', null, {
    params: { configId, orgId }
  })
}

/**
 * 获取数据库类型选项
 */
export function getDatabaseTypeOptions() {
  return axios.get('/api/DatabaseConfig/GetDatabaseTypeOptions')
}
