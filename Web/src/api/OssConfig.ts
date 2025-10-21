import axios from '../common/axios'

export function getOssConfigApi() {
  return axios.get('/api/OssConfig/GetConfig')
}

export function saveOssConfigApi(data: any) {
  return axios.post('/api/OssConfig/SaveConfig', data)
}

export function testOssConnectionApi(data: any) {
  return axios.post('/api/OssConfig/TestConnection', data)
}

export function uploadFileToOssApi(examId: number, filePath: string, fileName: string) {
  return axios.post('/api/OssConfig/UploadFile', null, { 
    params: { examId, filePath, fileName } 
  })
}
