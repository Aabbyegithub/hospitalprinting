import axios from '../common/axios'

// 获取OCR配置
export function getOcrConfigApi() {
  return axios.get('/api/OcrConfig/GetConfig')
}

// 保存OCR配置
export function saveOcrConfigApi(data: any) {
  return axios.post('/api/OcrConfig/SaveConfig', data)
}

// 测试OCR连接
export function testOcrConnectionApi(data: any) {
  return axios.post('/api/OcrConfig/TestConnection', data)
}
