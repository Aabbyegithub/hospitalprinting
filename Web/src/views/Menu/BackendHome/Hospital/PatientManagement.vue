<template>
  <div class="patient-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchName" placeholder="搜索姓名" class="search-input" @keyup.enter="fetchPatientList" />
          <el-input v-model="searchMedicalNo" placeholder="搜索就诊号" class="search-input" @keyup.enter="fetchPatientList" />
          <el-button class="search-btn" @click="fetchPatientList">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增患者</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="patient-table-view">
      <div class="table-list">
        <el-table
          :data="patientList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="姓名" prop="name" align="center" />
          <el-table-column label="性别" prop="gender" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.gender === '男' ? 'primary' : 'success'">
                {{ scope.row.gender }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="年龄" prop="age" align="center" :formatter="(row) => row.age || '--'" />
          <el-table-column label="身份证号" prop="id_card" align="center" :formatter="(row) => row.id_card || '--'" />
          <el-table-column label="联系方式" prop="contact" align="center" :formatter="(row) => row.contact || '--'" />
          <el-table-column label="就诊号" prop="medical_no" align="center" :formatter="(row) => row.medical_no || '--'" />
          <el-table-column label="创建时间" prop="createtime" align="center" :formatter="formatDate" />
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
    <el-dialog v-model="showEditModal" width="500" :title="editForm.id ? '编辑患者' : '新增患者'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="姓名" required>
          <el-input v-model="editForm.name" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="性别" required>
          <el-select v-model="editForm.gender" placeholder="请选择性别">
            <el-option label="男" value="男" />
            <el-option label="女" value="女" />
          </el-select>
        </el-form-item>
        <el-form-item label="年龄">
          <el-input-number v-model="editForm.age" :min="0" :max="150" placeholder="请输入年龄" />
        </el-form-item>
        <el-form-item label="身份证号">
          <el-input v-model="editForm.id_card" placeholder="请输入身份证号" />
        </el-form-item>
        <el-form-item label="联系方式">
          <el-input v-model="editForm.contact" placeholder="请输入联系方式" />
        </el-form-item>
        <el-form-item label="就诊号">
          <el-input v-model="editForm.medical_no" placeholder="请输入就诊号" />
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
import { ElSelect, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElTag, ElDialog, ElForm, ElFormItem, ElMessage, ElInputNumber } from 'element-plus';
import { addPatientApi, deletePatientApi, editPatientApi, getPatientList } from '../../../../api/patient';

const searchName = ref('');
const searchMedicalNo = ref('');
const patientList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(0);
const selectedRows = ref<any[]>([]);

// 格式化日期
const formatDate = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  return new Date(cellValue).toLocaleString('zh-CN');
};

// 获取患者列表
async function fetchPatientList() {
  await getPatientList(searchName.value, searchMedicalNo.value, pageIndex.value, pageSize.value)
    .then((res: any) => {
      if (res && res.response) {
        patientList.value = res.response;
        total.value = res.count || 0;
      }
    })
    .catch((error) => {
      console.error('获取患者列表失败:', error);
      ElMessage.error('获取患者列表失败');
    });
}

// 保存患者信息
async function handleSave() {
  try {
    if (editForm.id) {
      // 编辑
      await editPatientApi(editForm);
      ElMessage.success('编辑成功');
    } else {
      // 新增
      await addPatientApi(editForm);
      ElMessage.success('新增成功');
    }
    fetchPatientList();
    closeEditModal();
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败');
  }
}

// 删除患者
async function handleDelete(patient: any) {
  try {
    await deletePatientApi([patient.id]);
    ElMessage.success('删除成功');
    fetchPatientList();
  } catch (error) {
    console.error('删除失败:', error);
    ElMessage.error('删除失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  try {
    const ids = selectedRows.value.map(item => item.id);
    await deletePatientApi(ids);
    ElMessage.success('批量删除成功');
    fetchPatientList();
  } catch (error) {
    console.error('批量删除失败:', error);
    ElMessage.error('批量删除失败');
  }
}

// 打开编辑弹窗
function openEditModal(patient?: any) {
  showEditModal.value = true;
  if (patient) {
    Object.keys(patient).forEach(key => {
      editForm[key] = patient[key];
    });
  } else {
    // 重置表单
    Object.keys(editForm).forEach(k => editForm[k] = '');
    editForm.gender = '';
    editForm.age = null;
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
  fetchPatientList();
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchPatientList();
}

onMounted(() => {
  fetchPatientList();
});
</script>

<style scoped>
.patient-management-container {
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

.patient-table-view {
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
