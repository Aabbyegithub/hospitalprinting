import axios from '../common/axios'
//获取门店列表
export function getorgidList(StoreName:string='',phone:string='',address:string='',page:number=1,size:number=10) {
    return axios.get('/api/Org/GetStoreList',{params: { StoreName,phone,address,page,size } })
}

//添加门店
export function addorgidApi(orgid_name:string,phone:string,address:string,status:number=1) {
    return axios.post('/api/Org/AddStore',{ orgid_name,phone,address,status})
}

//编辑门店
export function editorgidApi(orgid_id:string,orgid_name:string,phone:string,address:string,status:number=1,orgid_code?:string) {
    return axios.post('/api/Org/UpdateStore',{ orgid_id,orgid_name,phone,address,status,orgid_code })
}

//删除门店
export function deleteorgidApi( storeIds:number[] ) {
    return axios.post('/api/Org/DeleteStore', storeIds )
}

//更新门店状态
export function updateorgidStatusApi(storeId:string,status:number) {
    return axios.get('/api/Org/UpdateStoreStatus',{ params: { storeId, status } })
}

export function getDashboardData() {
  return axios.get('/api/CoreKPIReport/GetDashboardData');
}