import axios, { type AxiosRequestConfig, type AxiosResponse } from 'axios'
import { ElMessage } from 'element-plus'
import API_BASE_URL from '../../public/config'
import router from '../router/index'

// 创建 axios 实例
const service = axios.create({
  baseURL: API_BASE_URL.apiBaseUrl, 
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// 请求拦截
service.interceptors.request.use(
  config => {
    // 自动携带 token（如有）
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  }
)

// 响应拦截
service.interceptors.response.use(
  (response: AxiosResponse) => {
    // 统一处理后端返回格式
    if (response.data && response.data.start == 200) {
       return response.data
    }else{
       ElMessage.error(response.data.message || '请求错误')
      return Promise.reject(new Error(response.data.message || '请求错误'))
    }
  },
  error => {
    console.log(error)
   if(error.response?.status === 401){
       ElMessage.error(error.response?.data?.message || '登陆已过期，请重新登录')
       localStorage.removeItem('token'); 
        router.replace('/login');
      return Promise.reject(error.response?.data?.message || '请求错误')
    }else{
      //  localStorage.removeItem('token'); 
      // router.replace('/login');
      // 网络或服务器错误统一提示
      ElMessage.error(error.response?.data?.message || '服务器错误')
      return Promise.reject(error)
  }}
)

// 通用 GET 方法
export function get<T = any>(url: string, params?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.get(url, { params, ...config })
}

// 通用 POST 方法
export function post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
  return service.post(url, data, config)
}

export default service