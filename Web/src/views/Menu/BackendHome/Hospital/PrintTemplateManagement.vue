<template>
  <div class="print-template-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchName" placeholder="搜索模板名称" class="search-input" @keyup.enter="onSearch" />
          <el-select v-model="searchTemplateType" placeholder="模板类型" class="search-input" @change="onSearch" clearable>
            <el-option label="全部" value="" />
            <el-option label="胶片模板" value="report" />
            <el-option label="患者模板" value="patient" />
          </el-select>
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
          <el-button class="reset-btn" @click="onReset">重置</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增模板</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>

    <div class="template-table-view">
      <div class="table-list">
        <el-table
          :data="templateList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="模板名称" prop="name" align="center" />
          <el-table-column label="模板类型" prop="template_type" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.template_type === 'report' ? 'primary' : 'success'">
                {{ scope.row.template_type === 'report' ? '胶片模板' : '患者模板' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="条形码类型" prop="barcode_type" align="center" />
          <el-table-column label="条形码数据源" prop="barcode_data_source" align="center" />
          <el-table-column label="是否默认" prop="is_default" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.is_default === 1 ? 'success' : 'info'">
                {{ scope.row.is_default === 1 ? '是' : '否' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="状态" prop="status" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
                {{ scope.row.status === 1 ? '启用' : '禁用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" prop="create_time" width="180" align="center" :formatter="formatDateTime" />
          <el-table-column label="操作" align="center" width="250">
            <template #default="scope">
              <el-button type="text" style="color: #67c23a;" @click="openEditModal(scope.row)">编辑</el-button>
              <el-button 
                v-if="scope.row.is_default !== 1" 
                type="text" 
                style="color: #409eff;" 
                @click="handleSetDefault(scope.row)"
              >
                设为默认
              </el-button>
              <el-button type="text" style="color: #e6a23c;" @click="handlePreview(scope.row)">预览</el-button>
              <el-button type="text" style="color: #f56c6c;" @click="handleDelete(scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="pagination-bar">
        <el-pagination
          style="margin-top:16px;float:right;"
          :total="total"
          :page-size="pageSize"
          :current-page="pageIndex"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
          :page-sizes="[10,20,30,50]"
          layout="prev, pager, next, ->, sizes, jumper"
        />
      </div>
    </div>

    <!-- 编辑/新增弹窗 -->
    <el-dialog v-model="showEditModal" width="800" :title="editForm.id ? '编辑打印模板' : '新增打印模板'">
      <el-form :model="editForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="模板名称" required>
              <el-input v-model="editForm.name" placeholder="请输入模板名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="模板类型" required>
              <el-select v-model="editForm.template_type" placeholder="请选择模板类型" @change="onTemplateTypeChange">
                <el-option label="胶片模板" value="report" />
                <el-option label="患者模板" value="patient" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="模板描述">
          <el-input v-model="editForm.description" type="textarea" placeholder="请输入模板描述" />
        </el-form-item>

        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="条形码类型" required>
              <el-select v-model="editForm.barcode_type" placeholder="请选择条形码类型">
                <el-option label="CODE128" value="CODE128" />
                <el-option label="QR码" value="QR" />
                <el-option label="CODE39" value="CODE39" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="条形码数据源" required>
              <el-select v-model="editForm.barcode_data_source" placeholder="请选择条形码数据源">
                <el-option 
                  v-for="field in availableFields" 
                  :key="field.field" 
                  :label="field.label" 
                  :value="field.field" 
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="显示字段">
          <div class="field-config">
            <div v-for="(field, index) in displayFields" :key="index" class="field-item">
              <el-checkbox v-model="field.show" :label="field.label" />
              <el-input v-model="field.label" placeholder="字段标签" style="width: 120px; margin-left: 10px;" />
            </div>
          </div>
        </el-form-item>

        <!-- 隐藏字段 -->
        <el-form-item v-show="false">
          <el-input v-model="editForm.org_id" />
          <el-input v-model="editForm.status" />
          <el-input v-model="editForm.is_default" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button class="cancel-btn" @click="closeEditModal">取消</el-button>
        <el-button type="primary" class="Btn-Save" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>

    <!-- 预览弹窗 -->
    <el-dialog v-model="showPreviewModal" title="模板预览" width="90%" :close-on-click-modal="false" class="preview-dialog">
      <div class="preview-container">
        <div class="preview-toolbar">
          <div class="toolbar-left">
            <el-select v-model="selectedExamId" placeholder="选择检查记录" style="width: 200px;" @change="onExamChange">
              <el-option label="示例数据" :value="1" />
              <el-option label="检查记录2" :value="2" />
              <el-option label="检查记录3" :value="3" />
            </el-select>
          </div>
          <div class="toolbar-right">
            <el-button type="primary" @click="handlePrint" :icon="Printer">打印</el-button>
            <el-button @click="refreshPreview" :icon="Refresh">刷新</el-button>
          </div>
        </div>
        <div class="preview-content">
          <iframe 
            v-if="previewHtml" 
            :src="previewHtml" 
            width="100%" 
            height="700px"
            frameborder="0"
            class="preview-iframe"
          ></iframe>
          <div v-else class="no-content">
            <el-empty description="暂无预览内容" />
          </div>
        </div>
      </div>
      <template #footer>
        <el-button @click="showPreviewModal = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import { ElSelect, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElTag, ElDialog, ElForm, ElFormItem, ElMessage, ElCheckbox, ElRow, ElCol, ElEmpty } from 'element-plus';
import { Printer, Refresh } from '@element-plus/icons-vue';
import { printTemplateApi } from '../../../../api/printTemplate';

const searchName = ref('');
const searchTemplateType = ref('');
const templateList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(0);
const selectedRows = ref<any[]>([]);

// 预览相关
const showPreviewModal = ref(false);
const previewHtml = ref('');
const currentTemplate = ref<any>(null);
const selectedExamId = ref(1);

// 字段配置
const availableFields = ref<any[]>([]);
const displayFields = ref<any[]>([]);

// 格式化日期时间
const formatDateTime = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  const d = new Date(cellValue);
  if (isNaN(d.getTime())) return '--';
  const y = d.getFullYear();
  const m = d.getMonth() + 1;
  const day = d.getDate();
  const hh = String(d.getHours()).padStart(2, '0');
  const mm = String(d.getMinutes()).padStart(2, '0');
  const ss = String(d.getSeconds()).padStart(2, '0');
  return `${y}-${m}-${day} ${hh}:${mm}:${ss}`;
};

// 获取模板列表
async function fetchTemplateList(showTip: boolean = false) {
  await printTemplateApi.getPrintTemplateList(searchName.value, searchTemplateType.value, pageIndex.value, pageSize.value)
    .then((res: any) => {
      if (res && res.response) {
        templateList.value = res.response;
        total.value = res.count || 0;
        if (showTip) {
          ElMessage.success(`查询成功，匹配到 ${total.value} 条记录`);
        }
      }
    })
    .catch((error) => {
      console.error('获取模板列表失败:', error);
      ElMessage.error('获取模板列表失败');
    });
}

// 主动搜索
function onSearch() {
  pageIndex.value = 1;
  fetchTemplateList(true);
}

// 重置筛选条件
function onReset() {
  searchName.value = '';
  searchTemplateType.value = '';
  pageIndex.value = 1;
  fetchTemplateList(true);
}

// 模板类型变化
async function onTemplateTypeChange(templateType: string) {
  if (templateType) {
    await loadAvailableFields(templateType);
  }
}

// 加载可用字段
async function loadAvailableFields(templateType: string) {
  try {
    const res: any = await printTemplateApi.getAvailableFields(templateType);
    if (res && res.response) {
      availableFields.value = res.response;
      // 初始化显示字段
      displayFields.value = res.response.map((field: any) => ({
        field: field.field,
        label: field.label,
        show: false
      }));
    }
  } catch (error) {
    console.error('获取可用字段失败:', error);
  }
}

// 保存模板
async function handleSave() {
  try {
    // 验证必填字段
    if (!editForm.name || editForm.name.trim() === '') {
      ElMessage.error('模板名称不能为空');
      return;
    }
    
    if (!editForm.template_type || editForm.template_type.trim() === '') {
      ElMessage.error('请选择模板类型');
      return;
    }
    
    if (!editForm.barcode_data_source || editForm.barcode_data_source.trim() === '') {
      ElMessage.error('请选择条形码数据源');
      return;
    }

    // 准备数据
    const formData = {
      id: editForm.id || 0,
      name: editForm.name.trim(),
      description: editForm.description || '',
      template_type: editForm.template_type,
      barcode_data_source: editForm.barcode_data_source,
      barcode_type: editForm.barcode_type || 'CODE128',
      display_fields: JSON.stringify(displayFields.value),
      org_id: Number(editForm.org_id) || 1,
      status: Number(editForm.status) || 1,
      is_default: Number(editForm.is_default) || 0
    };

    if (editForm.id) {
      // 编辑
      await printTemplateApi.updatePrintTemplate(formData);
      ElMessage.success('编辑成功');
    } else {
      // 新增
      await printTemplateApi.addPrintTemplate(formData);
      ElMessage.success('新增成功');
    }
    fetchTemplateList();
    closeEditModal();
  } catch (error: any) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败: ' + (error.response?.data?.message || error.message || '未知错误'));
  }
}

// 删除模板
async function handleDelete(template: any) {
  try {
    await printTemplateApi.deletePrintTemplate([template.id]);
    ElMessage.success('删除成功');
    fetchTemplateList();
  } catch (error) {
    console.error('删除失败:', error);
    ElMessage.error('删除失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  try {
    const ids = selectedRows.value.map(item => item.id);
    await printTemplateApi.deletePrintTemplate(ids);
    ElMessage.success('批量删除成功');
    fetchTemplateList();
  } catch (error) {
    console.error('批量删除失败:', error);
    ElMessage.error('批量删除失败');
  }
}

// 设置默认模板
async function handleSetDefault(template: any) {
  try {
    await printTemplateApi.setDefaultTemplate(template.id);
    ElMessage.success('设置默认模板成功');
    fetchTemplateList();
  } catch (error) {
    console.error('设置默认模板失败:', error);
    ElMessage.error('设置默认模板失败');
  }
}

// 预览模板
async function handlePreview(template: any) {
  try {
    const res: any = await printTemplateApi.previewTemplate(template.id, selectedExamId.value);
    if (res && res.response) {
      previewHtml.value = 'data:text/html;charset=utf-8,' + encodeURIComponent(res.response);
      showPreviewModal.value = true;
      currentTemplate.value = template;
    }
  } catch (error) {
    console.error('预览失败:', error);
    ElMessage.error('预览失败');
  }
}

// 检查记录变化
async function onExamChange() {
  if (currentTemplate.value) {
    await handlePreview(currentTemplate.value);
  }
}

// 刷新预览
async function refreshPreview() {
  if (currentTemplate.value) {
    await handlePreview(currentTemplate.value);
  }
}

// 打印
async function handlePrint() {
  if (!currentTemplate.value) return;
  try {
    const res: any = await printTemplateApi.printTemplate(currentTemplate.value.id, selectedExamId.value);
    if (res && res.response) {
      // 打开新窗口打印
      const printWindow = window.open('', '_blank');
      if (printWindow) {
        printWindow.document.write(res.response);
        printWindow.document.close();
        printWindow.print();
      }
    }
  } catch (error) {
    console.error('打印失败:', error);
    ElMessage.error('打印失败');
  }
}

// 打开编辑弹窗
async function openEditModal(template?: any) {
  showEditModal.value = true;
  if (template) {
    Object.keys(template).forEach(key => {
      editForm[key] = template[key];
    });
    // 解析显示字段
    if (template.display_fields) {
      try {
        displayFields.value = JSON.parse(template.display_fields);
      } catch (e) {
        displayFields.value = [];
      }
    }
    // 加载可用字段
    if (template.template_type) {
      await loadAvailableFields(template.template_type);
    }
  } else {
    // 重置表单
    editForm.id = '';
    editForm.name = '';
    editForm.description = '';
    editForm.template_type = '';
    editForm.barcode_data_source = '';
    editForm.barcode_type = 'CODE128';
    editForm.org_id = 1;
    editForm.status = 1;
    editForm.is_default = 0;
    displayFields.value = [];
    availableFields.value = [];
  }
}

// 关闭编辑弹窗
function closeEditModal() {
  showEditModal.value = false;
  // 重置表单
  editForm.id = '';
  editForm.name = '';
  editForm.description = '';
  editForm.template_type = '';
  editForm.barcode_data_source = '';
  editForm.barcode_type = 'CODE128';
  editForm.org_id = 1;
  editForm.status = 1;
  editForm.is_default = 0;
  displayFields.value = [];
  availableFields.value = [];
}

// 处理选择变化
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection;
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchTemplateList(false);
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchTemplateList(false);
}

onMounted(() => {
  fetchTemplateList(false);
});
</script>

<style scoped>
.print-template-management-container {
  padding: 10px;
}

.page-header {
  margin-bottom: 24px;
}

.operation-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 12px;
}

.search-input {
  width: 180px;
}

.search-btn {
  background-color: #22a2b6;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 4px 14px;
  font-size: 14px;
  cursor: pointer;
}

.reset-btn {
  background-color: #f5f7fa;
  color: #606266;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 4px 14px;
  font-size: 14px;
  cursor: pointer;
}

.reset-btn:hover {
  background-color: #ecf5ff;
  color: #409eff;
  border-color: #c6e2ff;
}

.action-buttons {
  display: flex;
  gap: 10px;
}

.primary-btn {
  background-color: #22a2b6 !important;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 4px 14px;
  font-size: 14px;
  cursor: pointer;
}

.primary-btn:hover {
  background-color: #0E42D2;
}

.danger-btn {
  background-color: #F53F3F;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 4px 14px;
  font-size: 14px;
  cursor: pointer;
}

.danger-btn:disabled {
  background-color: #F5F7FA;
  color: #C9CDD4;
  cursor: not-allowed;
}

.template-table-view {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  padding: 0 0 16px 0;
}

.table-list {
  margin-bottom: 20px;
}

.pagination-bar {
  text-align: right;
}

.Btn-Save {
  background-color: #22a2b6;
  border-color: #22a2b6;
}

.cancel-btn {
  background-color: #fff;
  border-color: #6b5d5d;
  color: #000;
  margin-right: 10px;
}

.field-config {
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  padding: 10px;
  max-height: 200px;
  overflow-y: auto;
}

.field-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.field-item:last-child {
  margin-bottom: 0;
}

.preview-dialog .el-dialog__body {
  padding: 0;
}

.preview-container {
  display: flex;
  flex-direction: column;
  height: 80vh;
}

.preview-toolbar {
  padding: 15px 20px;
  background: #f5f7fa;
  border-bottom: 1px solid #e4e7ed;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.toolbar-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.toolbar-right {
  display: flex;
  align-items: center;
  gap: 10px;
}

.preview-content {
  flex: 1;
  overflow: hidden;
  position: relative;
}

.preview-iframe {
  border: none;
  width: 100%;
  height: 100%;
  background: white;
}

.no-content {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  background: #f5f7fa;
}
</style>
