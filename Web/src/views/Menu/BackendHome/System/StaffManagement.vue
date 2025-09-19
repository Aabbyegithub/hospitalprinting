<template>
  <div class="staff-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchname" placeholder="搜索姓名" class="search-input" @keyup.enter="fetchStaffList" />
          <el-input v-model="searchusername" placeholder="搜索账号" class="search-input" @keyup.enter="fetchStaffList" />
          <el-input v-model="searchphone" placeholder="搜索手机号" class="search-input" @keyup.enter="fetchStaffList" />
          <el-button class="search-btn" @click="fetchStaffList">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增员工</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="staff-table-view">
      <div class="table-list">
        <el-table
          :data="staffList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="姓名" prop="name" align="center" />
          <el-table-column label="账号" prop="username" align="center" />
          <el-table-column label="手机号" prop="phone" align="center" :formatter="(row) => row.phone || '--'" />
          <el-table-column label="职位" prop="position" align="center" />
          <el-table-column label="状态" prop="status" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.status === 1 ? 'success' : 'info'" @click="toggleStatus(scope.row)">
                {{ scope.row.status === 1 ? '在职' : '离职' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="角色" prop="user_role" align="center">
            <template #default="scope">
              {{ roleList.find(cat => cat.id === scope.row.user_role?.role_id)?.name || '' }}
            </template>
          </el-table-column>
          <el-table-column label="最后登录" prop="last_login_time" align="center" :formatter="(row) => row.last_login_time || '--'" />
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
    <el-dialog v-model="showEditModal" width="500" :title="editForm.user_id ? '编辑员工' : '新增员工'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="姓名" required>
          <el-input v-model="editForm.name" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="账号" required>
          <el-input v-model="editForm.username" placeholder="请输入登录账号" :disabled="!!editForm.staff_id" />
        </el-form-item>
        <el-form-item label="手机号" required>
          <el-input v-model="editForm.phone" placeholder="请输入手机号" />
        </el-form-item>
        <el-form-item label="职位" required>
          <el-select v-model="editForm.position" placeholder="请选择职位">
            <el-option label="请选择职位" value="" />
          </el-select>
        </el-form-item>
        <el-form-item label="角色" required>
          <el-select v-model="editForm.roleId" placeholder="请选择权限角色">
            <el-option v-for="role in roleList" :key="role.id" :value="role.id" :label="role.name">{{ role.name }}</el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="密码" v-if="!editForm.staff_id" required>
          <el-input v-model="editForm.password" type="password" placeholder="请输入密码" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="editForm.status">
            <el-option label="在职" :value="1" />
            <el-option label="离职" :value="0" />
          </el-select>
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
import { ElSelect, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElTag, ElDialog, ElForm, ElFormItem, ElMessage } from 'element-plus';
import { addStaffApi, deleteStaffApi, editStaffApi, getRoleList, getStaffList } from '../../../../api/staffmang';
const searchname = ref('');
const searchusername = ref('');
const searchphone = ref('');
const staffList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(10)
const roleList = ref<any[]>([]);

// 新增缺失的参数和接口方法
const selectedRows = ref<any[]>([]);

async function fetchStaffList() {
  await getStaffList(searchname.value,searchusername.value,searchphone.value,pageIndex.value,pageSize.value)
  .then((res:any)=>{
    if (res && res.response) {
      staffList.value = res.response;
      total.value = res.count || 0;
    }
  })
}
async function handleSave() {
  // TODO: 这里预留API调用，保存员工信息
  if (editForm.user_id) {
    // 编辑
    const idx = staffList.value.findIndex(item => item.user_id === editForm.user_id);
    if (idx > -1) {
      await editStaffApi(editForm.user_id,editForm.name,editForm.username,editForm.password,editForm.phone,editForm.position,editForm.orgid_id,editForm.status,editForm.roleId)
      ElMessage.success('编辑成功');
    }
  } else {
    // 新增
    await addStaffApi(editForm.name,editForm.username,editForm.password,editForm.phone,editForm.position,editForm.orgid_id,editForm.status,editForm.roleId)
    ElMessage.success('新增成功');
  }
  fetchStaffList()
  closeEditModal();
}
async function handleDelete(staff:any) {
  // TODO: 这里预留API调用，删除员工
    await deleteStaffApi([staff.staff_id])
     fetchStaffList()
}
function toggleStatus(staff:any) {
  // TODO: 这里预留API调用，切换员工状态
  staff.status = staff.status === 1 ? 0 : 1;
}
function openEditModal(staff?:any) {
  showEditModal.value = true;
  if (staff) {
    Object.keys(staff).forEach(key => {
      editForm[key] = staff[key];
    });
    editForm.password = '';
    // 修正角色id类型，防止下拉框映射不上
    if (!roleList.value.some(role => role.id === staff.user_role?.role_id)) {
      editForm.roleId = '';
    }else
      editForm.roleId = staff.user_role?.role_id;
  } else {
    editForm.staff_id = '';
    editForm.username = '';
    editForm.password = '';
    editForm.name = '';
    editForm.phone = '';
    editForm.position = '';
    editForm.status = 1;
    editForm.roleId = '';
  }
}
function closeEditModal() {
  showEditModal.value = false;
  Object.keys(editForm).forEach(k => editForm[k] = '');
}
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchStaffList();
}
function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchStaffList();
}


async function handleBatchDelete() {
  // TODO: 批量删除逻辑
  const ids = selectedRows.value.map(item => item.staff_id);
  await deleteStaffApi(ids)
   fetchStaffList()
}

onMounted(() => {
  fetchStaffList();
  fetchRoleList();
});

async function fetchRoleList() {
  // 获取角色列表
 await getRoleList().then((res:any) => {
    if (res && res.response) {
      roleList.value = res.response.map((item: any) => ({
        id: item.role_id,
        name: item.role_name
      }));
    }
  });
}
</script>

<style scoped>
.staff-management-container {
  padding: 10px;
  /* background-color: #F5F7FA;
  min-height: 100%; */
}
.filter-bar {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}
.filter-item {
  margin-right: 20px;
  width: 180px;
}
.table-list {
  margin-bottom: 20px;
}
.pagination-bar {
  text-align: right;
}
.Btn-Save{
  background-color: #22a2b6;
  border-color: #22a2b6;
}
.cancel-btn {
  background-color: #fff;
  border-color: #6b5d5d;
  color: #000;
  margin-right: 10px;
}
.page-header {
  margin-bottom: 24px;
}
.page-title {
  font-size: 20px;
  color: #1D2129;
  margin: 0 0 16px 0;
  font-weight: 600;
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
.filter-label {
  font-size: 14px;
  width: 180px;
  color: #4E5969;
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
.staff-table-view {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  padding: 0 0 16px 0;
}
.table-list-header, .table-list-item {
  display: flex;
  align-items: center;
  border-bottom: 1px solid #F2F3F5;
  padding: 0 16px;
  min-height: 44px;
}
.table-list-header {
  background: #F5F7FA;
  font-weight: 500;
}
.list-column {
  flex: 1;
  font-size: 14px;
  color: #1D2129;
  padding: 0 8px;
  text-align: left;
}
.checkbox-col {
  flex: 0 0 36px;
  text-align: center;
}
.status-tag {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}
.status-tag.active {
  background-color: #E8F7FF;
  color: #165DFF;
}
.status-tag.inactive {
  background-color: #F5F7FA;
  color: #C9CDD4;
}
.action-btn {
  padding: 4px 10px;
  border-radius: 4px;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
}
.view-details {
  background-color: #F5F7FA;
  color: #4E5969;
}
.view-details:hover {
  background-color: #E8EBF0;
}
.danger-btn {
  background-color: #F53F3F;
  color: #fff;
}
.danger-btn:hover {
  background-color: #CB2634;
}
.no-record {
  text-align: center;
  padding: 20px;
  color: #86909C;
  font-size: 14px;
}
.pagination-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  padding: 12px 16px 0 0;
  font-size: 14px;
}
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-dialog {
  width: 420px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  overflow: hidden;
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  border-bottom: 1px solid #F2F3F5;
}
.modal-title {
  font-size: 16px;
  color: #1D2129;
  margin: 0;
  font-weight: 600;
}
.modal-close {
  background: none;
  border: none;
  font-size: 18px;
  color: #86909C;
  cursor: pointer;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}
.modal-close:hover {
  background-color: #F5F7FA;
  color: #1D2129;
}
.modal-body {
  padding: 20px;
  max-height: 400px;
  overflow-y: auto;
}
.form-row {
  display: flex;
  align-items: center;
  margin-bottom: 16px;
}
.form-row label {
  width: 80px;
  color: #4E5969;
  font-size: 14px;
  flex-shrink: 0;
}
.form-row input, .form-row select {
  flex: 1;
  padding: 4px 10px;
  border-radius: 4px;
  border: 1px solid #DCDFE6;
  font-size: 14px;
}
.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 16px 20px;
  border-top: 1px solid #F2F3F5;
}
.btn {
  padding: 8px 16px;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}
.cancel-btn {
  background-color: #fff;
  color: #1D2129;
  border: 1px solid #DCDFE6;
}
.cancel-btn:hover {
  background-color: #F5F7FA;
}
.primary-btn {
  background-color: #165DFF;
  color: #fff;
  border: 1px solid #165DFF;
}
.primary-btn:hover {
  background-color: #0E42D2;
}
</style>
