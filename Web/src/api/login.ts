import axios from '../common/axios'
export function loginApi(UserName: string, PassWord: string) {
    return axios.get('/api/User/Login', {params: { UserName, PassWord } })
}

//获取门店列表
export function getStoreList() {
    return axios.get('/api/Store/GetAllStoreList')
}

//退出系统
export function logoutApi() {
    return axios.get('/api/User/logout')
}
//获取菜单
export function getMenuList() {
    return axios.get('/api/User/GetUserMenu')
}

export function GetUserDetial() {
    return axios.get('/api/User/GetUserDetial')
}