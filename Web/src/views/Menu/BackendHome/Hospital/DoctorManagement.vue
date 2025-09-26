<template>
  <div class="doctor-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchName" placeholder="搜索医生姓名" class="search-input" @keyup.enter="onSearch" />
          <el-select v-model="searchDepartmentId" placeholder="搜索科室" class="search-input" filterable clearable @change="onSearch">
            <el-option v-for="d in departmentOptions" :key="d.id" :label="d.name" :value="d.id" />
          </el-select>
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-group">
          <el-button type="primary" @click="openAddModal">新增医生</el-button>
          <el-button type="danger" @click="batchDelete" :disabled="selectedRows.length === 0">批量删除</el-button>
        </div>
      </div>
    </div>

    <div class="table-container">
      <el-table
        :data="doctorList"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        stripe
        border
        style="width: 100%;height: 65vh;"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="ID" prop="id" width="80" align="center" />
        <el-table-column label="医生姓名" prop="name" align="center" />
        <el-table-column label="性别" prop="gender" align="center">
          <template #default="scope">
            <el-tag :type="getGenderTagType(scope.row.gender)">
              {{ getGenderText(scope.row.gender) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="联系电话" prop="phone" align="center" />
        <el-table-column label="所属科室" prop="department_name" align="center" />
        <el-table-column label="职称" prop="title" align="center" />
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
            <el-button type="primary" size="small" @click="editDoctor(scope.row)">编辑</el-button>
            <el-button type="danger" size="small" @click="deleteDoctor(scope.row)">删除</el-button>
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

    <!-- 新增/编辑医生对话框 -->
    <el-dialog
      v-model="showEditModal"
      :title="editForm.id ? '编辑医生' : '新增医生'"
      width="600px"
      @close="closeEditModal"
    >
      <el-form
        ref="editFormRef"
        :model="editForm"
        :rules="editFormRules"
        label-width="100px"
      >
        <el-form-item label="医生姓名" prop="name">
          <el-input v-model="editForm.name" placeholder="请输入医生姓名" />
        </el-form-item>
        <el-form-item label="性别" prop="gender">
          <el-select v-model="editForm.gender" placeholder="请选择性别">
            <el-option label="未知" :value="0" />
            <el-option label="男" :value="1" />
            <el-option label="女" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="联系电话" prop="phone">
          <el-input v-model="editForm.phone" placeholder="请输入联系电话" />
        </el-form-item>
        <el-form-item label="所属科室" prop="department_id">
          <el-select v-model="editForm.department_id" placeholder="请选择所属科室" filterable>
            <el-option v-for="d in departmentOptions" :key="d.id" :label="d.name" :value="d.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="所属机构" prop="orgid_id">
          <el-select v-model="editForm.orgid_id" placeholder="请选择机构" filterable>
            <el-option v-for="o in orgOptions" :key="o.orgid_id" :label="o.orgid_name" :value="o.orgid_id" />
          </el-select>
        </el-form-item>
        <el-form-item label="职称" prop="title">
          <el-input v-model="editForm.title" placeholder="请输入职称" />
        </el-form-item>
        <el-form-item label="医生简介" prop="introduction">
          <el-input
            v-model="editForm.introduction"
            type="textarea"
            :rows="3"
            placeholder="请输入医生简介"
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
import { getDoctorList, addDoctorApi, editDoctorApi, deleteDoctorApi } from '../../../../api/doctor'
import { getDepartmentList } from '../../../../api/department'
import { getorgidList } from '../../../../api/orgapi'

// 响应式数据
const loading = ref(false)
const doctorList = ref([])
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)
const selectedRows = ref<any[]>([])

// 搜索条件
const searchName = ref('')
// 搜索改为按科室ID（后端需要 departmentId）
const searchDepartmentId = ref<number | null>(null)
const departmentOptions = ref<any[]>([])
const orgOptions = ref<any[]>([])

// 编辑表单
const showEditModal = ref(false)
const editFormRef = ref()
const editForm:any = reactive({
  id: 0,
  name: '',
  gender: 1,
  phone: '',
  department: '',
  title: '',
  introduction: '',
  orgid_id: 1,
  status: 1
})

// 表单验证规则
const editFormRules = {
  name: [
    { required: true, message: '请输入医生姓名', trigger: 'blur' }
  ],
  gender: [
    { required: true, message: '请选择性别', trigger: 'change' }
  ]
}

// 获取医生列表
async function fetchDoctorList(showTip = false) {
  loading.value = true
  try {
    const res:any = await getDoctorList(searchName.value, searchDepartmentId.value, pageIndex.value, pageSize.value)
    if (res && res.response) {
      doctorList.value = res.response
      total.value = res.count || 0
      if (showTip) {
        ElMessage.success('查询成功')
      }
    }
  } catch (error) {
    console.error('获取医生列表失败:', error)
    ElMessage.error('获取医生列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
function onSearch() {
  pageIndex.value = 1
  fetchDoctorList(true)
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val
  pageIndex.value = 1
  fetchDoctorList()
}

function handleCurrentChange(val: number) {
  pageIndex.value = val
  fetchDoctorList()
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
    gender: 1,
    phone: '',
    department: '',
    title: '',
    introduction: '',
    orgid_id: 1,
    status: 1
  })
  showEditModal.value = true
}

// 编辑医生
function editDoctor(row: any) {
  Object.assign(editForm, row)
  showEditModal.value = true
}

// 关闭编辑对话框
function closeEditModal() {
  showEditModal.value = false
  editFormRef.value?.resetFields()
}

// 保存医生
async function handleSave() {
  try {
    await editFormRef.value?.validate()
    
    const formData = {
      ...editForm,
      orgid_id: Number(editForm.orgid_id) || 1,
      status: Number(editForm.status) || 1
    }

    if (editForm.id) {
      await editDoctorApi(formData)
      ElMessage.success('编辑成功')
    } else {
      await addDoctorApi(formData)
      ElMessage.success('新增成功')
    }
    fetchDoctorList()
    closeEditModal()
  } catch (error: any) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败: ' + (error.response?.data?.message || error.message || '未知错误'))
  }
}

// 删除医生
function deleteDoctor(row: any) {
  ElMessageBox.confirm('确定要删除该医生吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteDoctorApi([row.id])
      ElMessage.success('删除成功')
      fetchDoctorList()
    } catch (error: any) {
      console.error('删除失败:', error)
      ElMessage.error('删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 批量删除
function batchDelete() {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('请选择要删除的医生')
    return
  }
  
  ElMessageBox.confirm(`确定要删除选中的 ${selectedRows.value.length} 个医生吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const ids = selectedRows.value.map((row: any) => row.id)
      await deleteDoctorApi(ids)
      ElMessage.success('批量删除成功')
      fetchDoctorList()
    } catch (error: any) {
      console.error('批量删除失败:', error)
      ElMessage.error('批量删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 获取性别文本
function getGenderText(gender: number) {
  const genderMap: { [key: number]: string } = {
    0: '未知',
    1: '男',
    2: '女'
  }
  return genderMap[gender] || '未知'
}

// 获取性别标签类型
function getGenderTagType(gender: number) {
  const typeMap: { [key: number]: 'primary' | 'success' | 'warning' | 'info' | 'danger' } = {
    0: 'info',
    1: 'primary',
    2: 'success'
  }
  return typeMap[gender] || 'info'
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
  fetchDoctorList()
  // 预加载科室与机构
  getDepartmentList('', 1, 1000).then((res:any) => {
    if (res && res.response) departmentOptions.value = res.response
  })
  getorgidList('', '', '', 1, 1000).then((res:any) => {
    if (res && res.response) orgOptions.value = res.response
  })
})
</script>

<style scoped>
.doctor-management-container {
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
