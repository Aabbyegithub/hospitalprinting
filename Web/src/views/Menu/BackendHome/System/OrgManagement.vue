<template>
  <div class="orgid-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchorgidName" placeholder="搜索组织名称" class="search-input" @keyup.enter="fetchorgidList" />
          <el-input v-model="searchaddress" placeholder="搜索组织地址" class="search-input" @keyup.enter="fetchorgidList" />
          <el-button class="search-btn" @click="fetchorgidList">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增组织</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="orgid-table-view">
      <div class="table-list">
        <el-table
          :data="orgidList"
          border
          style="width: 100%;height: 68vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="组织编码" prop="orgid_code" align="center" />
          <el-table-column label="组织名称" prop="orgid_name" align="center" />
          <el-table-column label="地址" prop="address" align="center" />
          <el-table-column label="状态" prop="status" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.status === 1 ? 'success' : 'info'" @click="toggleStatus(scope.row)">
                {{ scope.row.status === 1 ? '启用' : '停用' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" prop="created_at" align="center" />
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
    <el-dialog v-model="showEditModal" width="500" :title="editForm.orgid_id ? '编辑组织' : '新增组织'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="组织名称">
          <el-input v-model="editForm.orgid_name" placeholder="请输入组织名称" />
        </el-form-item>
        <el-form-item label="地址">
          <el-input v-model="editForm.address" placeholder="请输入组织地址" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="editForm.status">
            <el-option label="启用" :value="1" />
            <el-option label="停用" :value="0" />
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
import { ref, reactive, computed, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { addorgidApi, deleteorgidApi, editorgidApi, getorgidList, updateorgidStatusApi } from '../../../../api/orgapi';
import dayjs from 'dayjs';

const orgidList = ref<any[]>([]);
const searchorgidName = ref('')
const searchphone = ref('');
const searchaddress= ref('');
const pageIndex = ref(1);
const pageSize = ref(10);
const total = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const selectedRows = ref<any[]>([]);

async function fetchorgidList() {
  await getorgidList(searchorgidName.value,searchphone.value,searchaddress.value,pageIndex.value,pageSize.value).then((res: any) => {
    if (res && res.response) {
      orgidList.value = res.response.map((item: any) => ({
        orgid_id: item.orgid_id,
        orgid_name: item.orgid_name,
        address: item.address,
        status: item.status,
        created_at:dayjs(item.created_at).format('YYYY-MM-DD HH:mm:ss'),
        orgid_code:item.orgid_code
      }));
      total.value = res.count || 0;
    } else {
      ElMessage.error('获取组织列表失败');
    }
  }).catch(() => {
     ElMessage.error('获取组织列表失败');
  });
}

async function handleSave() {
  // TODO: 保存组织信息（新增或编辑）
  if (editForm.orgid_id) {
    // 编辑
    const idx = orgidList.value.findIndex(item => item.orgid_id === editForm.orgid_id);
    if (idx > -1) {
      await editorgidApi(editForm.orgid_id,editForm.orgid_name, editForm.phone, editForm.address,editForm.status,editForm.orgid_code)
      ElMessage.success('编辑成功');
    }
  } else {
    // 新增
    await addorgidApi(editForm.orgid_name, editForm.phone, editForm.address,editForm.status)
    ElMessage.success('新增成功');
  }
  fetchorgidList();
  closeEditModal();
}

async function handleDelete(orgid: any) {
  // TODO: 删除组织
  await deleteorgidApi([orgid.orgid_id]);
  ElMessage.success('删除成功');
  fetchorgidList();
}

async function handleBatchDelete() {
  // TODO: 批量删除
  const ids = selectedRows.value.map(item => item.orgid_id);
  await deleteorgidApi(ids);
  ElMessage.success('批量删除成功');
  selectedRows.value = [];
  fetchorgidList();
}

async function toggleStatus(orgid: any) {
  // TODO: 切换组织状态
  await updateorgidStatusApi(orgid.orgid_id, orgid.status === 1 ? 0 : 1);
  ElMessage.success('状态已切换');
  fetchorgidList();
}

function openEditModal(orgid?: any) {
  showEditModal.value = true;
  if (orgid) {
    Object.keys(orgid).forEach(key => {
      editForm[key] = orgid[key];
    });
  } else {
    editForm.orgid_id = '';
    editForm.orgid_name = '';
    editForm.orgid_code = '';
    editForm.address = '';
    editForm.status = 1;
  }
}

function closeEditModal() {
  showEditModal.value = false;
  Object.keys(editForm).forEach(k => editForm[k] = '');
}

function handleSelectionChange(val: any[]) {
  selectedRows.value = val;
}

function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchorgidList();
}
function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchorgidList();
}

onMounted(() => {
  fetchorgidList();
});
</script>

<style scoped>
/* 可复用员工管理的样式，或根据实际需求调整 */
.orgid-management-container {
  padding: 10px;
  /* background-color: #F5F7FA; */
  /* min-height: 100%; */
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
  /* padding: 4px 10px;
  border-radius: 4px;
  border: 1px solid #DCDFE6;
  font-size: 14px; */
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
  background-color: #22a2b6;
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
.orgid-table-view {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  padding: 0 0 16px 0;
}
.pagination-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 12px;
  padding: 12px 16px 0 0;
  font-size: 14px;
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