import axios from '../common/axios'

// 获取AI配置
export function getAiConfigApi() {
  return axios.get('/api/AiConfig/GetConfig')
}

// 保存AI配置
export function saveAiConfigApi(data: any) {
  return axios.post('/api/AiConfig/SaveConfig', data)
}

// 测试AI连接
export function testAiConnectionApi(data: any) {
  return axios.post('/api/AiConfig/TestConnection', data)
}

