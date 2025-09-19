// 环境判断
const ENV = import.meta.env.MODE || 'development'

// API 基础地址
const API_BASE_URL =
  ENV === 'production'
    ? 'http://txsq.eidpb.cn'
    : 'http://localhost:7092'//'http://mpvk8690901.vicp.fun:12575'

const APP_CONFIG = {
  env: ENV,
  apiBaseUrl: API_BASE_URL,
  appName: '九天阁餐厅收银系统',
  version: '1.0.0',
}

export default APP_CONFIG 
