import axios from '../common/axios';

export interface OperationLog {
  Id: number;
  UserId: number;
  ActionType: number;
  ModuleName: string;
  Description: string;
  ActionTime: string;
  ActionContent: string;
  OrgId: number;
  AddUserId: number;
  AddTime: string;
  UpUserId: number;
  UpTime: string;
}

export interface OperationLogParams {
  Page: number;
  Size: number;
  User?: string;
  ActionModel?: string;
  StartTime?: string;
  EndTime?: string;
  actionType?: number;
}

export function getOperationLog(params: OperationLogParams) {
  return axios.get('/api/Operlog/GetOperationlog', { params });
}

export const ActionTypeMap: Record<number, string> = {
  1: '编辑',
  2: '删除',
  3: '新增',
  4: '查询',
  5: '提交',
  6: '导出',
  7: '导入',
  8: '上传',
  9: '下载',
  10: '登陆',
  11: '退出',
  12: '启动',
  13: '关闭',
  14: '移除',
  15: '暂停'
};
