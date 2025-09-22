<template>
  <div class="print-record-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchExamId" placeholder="搜索检查ID" class="search-input" @keyup.enter="onSearch" />
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增打印记录</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="print-record-table-view">
      <div class="table-list">
        <el-table
          :data="printRecordList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="记录ID" prop="id" align="center" width="80" />
          <el-table-column label="检查ID" prop="exam_id" align="center" width="100" />
          <el-table-column label="患者ID" prop="patient_id" align="center" width="100" />
          <el-table-column label="打印时间" prop="print_time" align="center" :formatter="formatDateTime" />
          <el-table-column label="打印人ID" prop="printed_by" align="center" width="100" />
          <el-table-column label="操作" align="center" width="180">
            <template #default="scope">
              <el-button type="text" style="color: #67c23a;" @click="openEditModal(scope.row)">编辑</el-button>
              <el-button type="text" style="color: #f56c6c;" @click="handleDelete(scope.row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="pagination-bar">
        <el-pagination
          layout="prev, pager, next, ->, sizes, jumper"
          :total="total"
          :page-size="pageSize"
          :current-page="pageIndex"
          :prev-text="'<'"
          :next-text="'>'"
          :page-sizes="[10, 20, 30, 40, 50]"
          :display-page-count="5"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </div>
    <!-- 编辑/新增弹窗 -->
    <el-dialog v-model="showEditModal" width="500" :title="editForm.id ? '编辑打印记录' : '新增打印记录'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="检查ID" required>
          <el-input-number v-model="editForm.exam_id" :min="1" placeholder="请输入检查ID" style="width: 100%" />
        </el-form-item>
        <el-form-item label="患者ID" required>
          <el-input-number v-model="editForm.patient_id" :min="1" placeholder="请输入患者ID" style="width: 100%" />
        </el-form-item>
        <el-form-item label="打印人ID" required>
          <el-input-number v-model="editForm.printed_by" :min="1" placeholder="请输入打印人ID" style="width: 100%" />
        </el-form-item>
        <el-form-item label="打印时间">
          <el-date-picker
            v-model="editForm.print_time"
            type="datetime"
            placeholder="选择打印时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button class="cancel-btn" @click="closeEditModal">取消</el-button>
        <el-button type="primary" class="Btn-Save" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import { ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElDialog, ElForm, ElFormItem, ElMessage, ElInputNumber, ElDatePicker } from 'element-plus';
import { addPrintRecordApi, deletePrintRecordApi, editPrintRecordApi, getPrintRecordList } from '../../../../api/printRecord';

const searchExamId = ref('');
const printRecordList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(0);
const selectedRows = ref<any[]>([]);

// 格式化日期时间
const formatDateTime = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  return new Date(cellValue).toLocaleString('zh-CN');
};

// 获取打印记录列表
async function fetchPrintRecordList(showTip: boolean = false) {
  const examId = searchExamId.value ? parseInt(searchExamId.value) : null;
  await getPrintRecordList(examId, pageIndex.value, pageSize.value)
    .then((res: any) => {
      if (res && res.response) {
        printRecordList.value = res.response;
        total.value = res.count || 0;
        if (showTip) {
          ElMessage.success(`查询成功，匹配到 ${total.value} 条记录`);
        }
      }
    })
    .catch((error) => {
      console.error('获取打印记录列表失败:', error);
      ElMessage.error('获取打印记录列表失败');
    });
}

function onSearch() {
  pageIndex.value = 1;
  fetchPrintRecordList(true);
}

// 保存打印记录
async function handleSave() {
  try {
    // 规范化提交数据，确保类型正确，并使用 ISO 日期格式
    const payload: any = {
      id: editForm.id || 0,
      exam_id: Number(editForm.exam_id),
      patient_id: Number(editForm.patient_id),
      printed_by: Number(editForm.printed_by)
    };
    if (editForm.print_time) {
      payload.print_time = new Date(editForm.print_time).toISOString();
    }
    if (editForm.id) {
      // 编辑
      await editPrintRecordApi(payload);
      ElMessage.success('编辑成功');
    } else {
      // 新增
      await addPrintRecordApi(payload);
      ElMessage.success('新增成功');
    }
    fetchPrintRecordList();
    closeEditModal();
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败');
  }
}

// 删除打印记录
async function handleDelete(record: any) {
  try {
    await deletePrintRecordApi([record.id]);
    ElMessage.success('删除成功');
    fetchPrintRecordList();
  } catch (error) {
    console.error('删除失败:', error);
    ElMessage.error('删除失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  try {
    const ids = selectedRows.value.map(item => item.id);
    await deletePrintRecordApi(ids);
    ElMessage.success('批量删除成功');
    fetchPrintRecordList();
  } catch (error) {
    console.error('批量删除失败:', error);
    ElMessage.error('批量删除失败');
  }
}

// 打开编辑弹窗
function openEditModal(record?: any) {
  showEditModal.value = true;
  if (record) {
    Object.keys(record).forEach(key => {
      editForm[key] = record[key];
    });
  } else {
    // 重置表单
    Object.keys(editForm).forEach(k => editForm[k] = '');
    editForm.print_time = new Date().toISOString().slice(0, 19).replace('T', ' ');
  }
}

// 关闭编辑弹窗
function closeEditModal() {
  showEditModal.value = false;
  Object.keys(editForm).forEach(k => editForm[k] = '');
}

// 处理选择变化
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection;
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchPrintRecordList(false);
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchPrintRecordList(false);
}

onMounted(() => {
  fetchPrintRecordList(false);
});
</script>

<style scoped>
.print-record-management-container {
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

.print-record-table-view {
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
</style>
