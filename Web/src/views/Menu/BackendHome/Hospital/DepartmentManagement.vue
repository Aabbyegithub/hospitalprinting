<template>
  <div class="department-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchName" placeholder="搜索科室名称" class="search-input" @keyup.enter="onSearch" />
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-group">
          <el-button type="primary" @click="openAddModal">新增科室</el-button>
          <el-button type="danger" @click="batchDelete" :disabled="selectedRows.length === 0">批量删除</el-button>
        </div>
      </div>
    </div>

    <div class="table-container">
      <el-table
        :data="departmentList"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        stripe
        border
        style="width: 100%;height: 65vh;"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="ID" prop="id" width="80" align="center" />
        <el-table-column label="科室名称" prop="name" align="center" />
        <el-table-column label="科室编码" prop="code" align="center" />
        <el-table-column label="科室简介" prop="description" align="center" show-overflow-tooltip />
        <el-table-column label="状态" prop="status" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
              {{ scope.row.status === 1 ? '有效' : '删除' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" prop="createtime" align="center">
          <template #default="scope">
            {{ formatDate(scope.row.createtime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" align="center">
          <template #default="scope">
            <el-button type="primary" size="small" @click="editDepartment(scope.row)">编辑</el-button>
            <el-button type="danger" size="small" @click="deleteDepartment(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <div class="pagination-container">
      <el-pagination
        style="margin-top:16px;float:right;"
        :total="total"
        :page-size="pageSize"
        :current-page="pageIndex"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :page-sizes="[10,20,30,50]"
        layout="prev, pager, next, ->, sizes, jumper"
      />
    </div>

    <!-- 新增/编辑科室对话框 -->
    <el-dialog
      v-model="showEditModal"
      :title="editForm.id ? '编辑科室' : '新增科室'"
      width="600px"
      @close="closeEditModal"
    >
      <el-form
        ref="editFormRef"
        :model="editForm"
        :rules="editFormRules"
        label-width="100px"
      >
        <el-form-item label="科室名称" prop="name">
          <el-input v-model="editForm.name" placeholder="请输入科室名称" />
        </el-form-item>
        <el-form-item label="科室编码" prop="code">
          <el-input v-model="editForm.code" placeholder="请输入科室编码" />
        </el-form-item>
        <el-form-item label="科室简介" prop="description">
          <el-input
            v-model="editForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入科室简介"
          />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-select v-model="editForm.status" placeholder="请选择状态">
            <el-option label="有效" :value="1" />
            <el-option label="删除" :value="0" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="closeEditModal">取消</el-button>
          <el-button type="primary" @click="handleSave">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getDepartmentList, addDepartmentApi, editDepartmentApi, deleteDepartmentApi } from '../../../../api/department'

// 响应式数据
const loading = ref(false)
const departmentList = ref([])
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)
const selectedRows = ref<any[]>([])

// 搜索条件
const searchName = ref('')

// 编辑表单
const showEditModal = ref(false)
const editFormRef = ref()
const editForm = reactive({
  id: 0,
  name: '',
  code: '',
  description: '',
  orgid_id: 1,
  status: 1
})

// 表单验证规则
const editFormRules = {
  name: [
    { required: true, message: '请输入科室名称', trigger: 'blur' }
  ]
}

// 获取科室列表
async function fetchDepartmentList(showTip = false) {
  loading.value = true
  try {
    const res:any = await getDepartmentList(searchName.value, pageIndex.value, pageSize.value)
    if (res && res.response) {
      departmentList.value = res.response
      total.value = res.count || 0
      if (showTip) {
        ElMessage.success('查询成功')
      }
    }
  } catch (error) {
    console.error('获取科室列表失败:', error)
    ElMessage.error('获取科室列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
function onSearch() {
  pageIndex.value = 1
  fetchDepartmentList(true)
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val
  pageIndex.value = 1
  fetchDepartmentList()
}

function handleCurrentChange(val: number) {
  pageIndex.value = val
  fetchDepartmentList()
}

// 选择处理
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection
}

// 打开新增对话框
function openAddModal() {
  Object.assign(editForm, {
    id: 0,
    name: '',
    code: '',
    description: '',
    orgid_id: 1,
    status: 1
  })
  showEditModal.value = true
}

// 编辑科室
function editDepartment(row: any) {
  Object.assign(editForm, row)
  showEditModal.value = true
}

// 关闭编辑对话框
function closeEditModal() {
  showEditModal.value = false
  editFormRef.value?.resetFields()
}

// 保存科室
async function handleSave() {
  try {
    await editFormRef.value?.validate()
    
    const formData = {
      ...editForm,
      orgid_id: Number(editForm.orgid_id) || 1,
      status: Number(editForm.status) || 1
    }

    if (editForm.id) {
      await editDepartmentApi(formData)
      ElMessage.success('编辑成功')
    } else {
      await addDepartmentApi(formData)
      ElMessage.success('新增成功')
    }
    fetchDepartmentList()
    closeEditModal()
  } catch (error: any) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败: ' + (error.response?.data?.message || error.message || '未知错误'))
  }
}

// 删除科室
function deleteDepartment(row: any) {
  ElMessageBox.confirm('确定要删除该科室吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteDepartmentApi([row.id])
      ElMessage.success('删除成功')
      fetchDepartmentList()
    } catch (error: any) {
      console.error('删除失败:', error)
      ElMessage.error('删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 批量删除
function batchDelete() {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('请选择要删除的科室')
    return
  }
  
  ElMessageBox.confirm(`确定要删除选中的 ${selectedRows.value.length} 个科室吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const ids = selectedRows.value.map((row: any) => row.id)
      await deleteDepartmentApi(ids)
      ElMessage.success('批量删除成功')
      fetchDepartmentList()
    } catch (error: any) {
      console.error('批量删除失败:', error)
      ElMessage.error('批量删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 格式化日期
function formatDate(dateStr: string) {
  if (!dateStr) return '--'
  const d = new Date(dateStr)
  if (isNaN(d.getTime())) return '--'
  const y = d.getFullYear()
  const m = d.getMonth() + 1
  const day = d.getDate()
  const hh = String(d.getHours()).padStart(2, '0')
  const mm = String(d.getMinutes()).padStart(2, '0')
  const ss = String(d.getSeconds()).padStart(2, '0')
  return `${y}-${m}-${day} ${hh}:${mm}:${ss}`
}

// 初始化
onMounted(() => {
  fetchDepartmentList()
})
</script>

<style scoped>
.department-management-container {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.operation-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.filter-group {
  display: flex;
  gap: 10px;
  align-items: center;
}

.search-input {
  width: 200px;
}

.search-btn {
  background-color: #409eff;
  color: white;
  border: none;
}

.action-group {
  display: flex;
  gap: 10px;
}

.table-container {
  margin-bottom: 20px;
}

.pagination-container {
  text-align: right;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>
