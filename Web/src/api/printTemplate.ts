import axios from '../common/axios';

// 打印模板相关API
export const printTemplateApi = {
  // 获取打印模板列表
  getPrintTemplateList: (name?: string, templateType?: string, page: number = 1, size: number = 10) => {
    return axios.get('/api/PrintTemplate/GetPage', {
      params: { name, templateType, page, size }
    });
  },

  // 添加打印模板
  addPrintTemplate: (data: any) => {
    return axios.post('/api/PrintTemplate/Add', data);
  },

  // 修改打印模板
  updatePrintTemplate: (data: any) => {
    return axios.post('/api/PrintTemplate/Update', data);
  },

  // 删除打印模板
  deletePrintTemplate: (ids: number[]) => {
    return axios.post('/api/PrintTemplate/Delete', ids);
  },

  // 获取模板详情
  getTemplateDetail: (templateId: number) => {
    return axios.get('/api/PrintTemplate/GetDetail', {
      params: { templateId }
    });
  },

  // 设置默认模板
  setDefaultTemplate: (templateId: number) => {
    return axios.post('/api/PrintTemplate/SetDefault', null, {
      params: { templateId }
    });
  },

  // 预览模板
  previewTemplate: (templateId: number, examId: number) => {
    return axios.get('/api/PrintTemplate/Preview', {
      params: { templateId, examId }
    });
  },

  // 打印模板
  printTemplate: (templateId: number, examId: number) => {
    return axios.post('/api/PrintTemplate/Print', null, {
      params: { templateId, examId }
    });
  },

  // 获取可用字段
  getAvailableFields: (templateType: string) => {
    return axios.get('/api/PrintTemplate/GetAvailableFields', {
      params: { templateType }
    });
  }
};
