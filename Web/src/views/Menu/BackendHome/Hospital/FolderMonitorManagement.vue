<template>
  <div class="folder-monitor-management">
    <!-- 搜索区域 -->
    <div class="search-bar">
      <el-input
        v-model="searchName"
        placeholder="配置名称"
        class="search-input"
        @keyup.enter="onSearch"
        clearable
      />
      <el-input
        v-model="searchIpAddress"
        placeholder="IP地址"
        class="search-input"
        @keyup.enter="onSearch"
        clearable
      />
      <el-button type="primary" @click="onSearch">搜索</el-button>
      <el-button @click="onReset">重置</el-button>
    </div>

    <!-- 操作按钮 -->
    <div class="action-bar">
      <el-button type="primary" @click="openAdd">新增配置</el-button>
    </div>

    <!-- 表格 -->
    <el-table
      :data="tableData"
      border
      style="width: 100%"
      height="65vh"
      v-loading="loading"
    >
      <el-table-column prop="name" label="配置名称" align="center" />
      <el-table-column prop="ip_address" label="IP地址" align="center" />
      <el-table-column label="文件夹路径" align="center" min-width="300">
        <template #default="scope">
          <div v-if="scope.row.folder_paths">
            <el-tag
              v-for="(path, index) in getFolderPaths(scope.row.folder_paths)"
              :key="index"
              size="small"
              style="margin: 2px; display: block; text-align: left;"
            >
              {{ path }}
            </el-tag>
          </div>
          <span v-else>--</span>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="状态" align="center" width="100">
        <template #default="scope">
          <el-tag :type="scope.row.status === 1 ? 'success' : 'danger'">
            {{ scope.row.status === 1 ? '启用' : '停用' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="remark" label="备注" align="center" />
      <el-table-column prop="create_time" label="创建时间" align="center" width="180">
        <template #default="scope">
          {{ formatDate(scope.row.create_time) }}
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" width="200">
        <template #default="scope">
          <el-button type="primary" size="small" @click="openEdit(scope.row)">编辑</el-button>
          <el-button
            :type="scope.row.status === 1 ? 'warning' : 'success'"
            size="small"
            @click="toggleStatus(scope.row)"
          >
            {{ scope.row.status === 1 ? '停用' : '启用' }}
          </el-button>
          <el-button type="danger" size="small" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <div class="pagination-bar">
      <el-pagination
        v-model:current-page="pageIndex"
        v-model:page-size="pageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="total"
        layout="prev, pager, next, ->, sizes, jumper"
        @size-change="onSizeChange"
        @current-change="onCurrentChange"
        style="margin-top:16px;float:right;"
      />
    </div>

    <!-- 新增/编辑弹窗 -->
    <el-dialog
      v-model="showEditModal"
      :title="isEdit ? '编辑配置' : '新增配置'"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form :model="editForm" :rules="editRules" ref="editFormRef" label-width="120px">
        <el-form-item label="配置名称" prop="name">
          <el-input v-model="editForm.name" placeholder="请输入配置名称" />
        </el-form-item>
        <el-form-item label="IP地址" prop="ip_address">
          <el-input v-model="editForm.ip_address" placeholder="请输入IP地址" />
        </el-form-item>
        <el-form-item label="文件夹路径" prop="folderPathsText">
          <el-input
            v-model="editForm.folderPathsText"
            type="textarea"
            :rows="4"
            placeholder="请输入文件夹路径，多个路径用##分隔&#10;例如：&#10;192.168.1.100\Hospital\ElectronicReports\PDF##&#10;192.168.1.101\Reports\Images"
          />
          <div class="path-tip">
            <el-text type="info" size="small">
              提示：多个路径请用 ## 分隔，每行一个路径
            </el-text>
          </div>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="editForm.remark" type="textarea" :rows="2" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showEditModal = false">取消</el-button>
        <el-button type="primary" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {getFolderMonitorListApi,addFolderMonitorApi,updateFolderMonitorApi, deleteFolderMonitorApi,toggleFolderMonitorStatusApi} from '../../../../api/folderMonitor'

// 搜索条件
const searchName = ref('')
const searchIpAddress = ref('')

// 表格数据
const tableData = ref([])
const loading = ref(false)
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)

// 编辑弹窗
const showEditModal = ref(false)
const isEdit = ref(false)
const editFormRef = ref()
const editForm = reactive({
  id: 0,
  name: '',
  ip_address: '',
  folder_paths: '',
  folderPathsText: '',
  remark: ''
})

// 表单验证规则
const editRules = {
  name: [{ required: true, message: '请输入配置名称', trigger: 'blur' }],
  ip_address: [{ required: true, message: '请输入IP地址', trigger: 'blur' }],
  folderPathsText: [{ required: true, message: '请输入文件夹路径', trigger: 'blur' }]
}

// 获取文件夹路径数组
function getFolderPaths(paths: string): string[] {
  if (!paths) return []
  return paths.split('##').filter(path => path.trim())
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

// 搜索
function onSearch() {
  pageIndex.value = 1
  fetchData()
}

// 重置
function onReset() {
  searchName.value = ''
  searchIpAddress.value = ''
  pageIndex.value = 1
  fetchData()
}

// 获取数据
async function fetchData() {
  loading.value = true
  try {
    const res:any = await getFolderMonitorListApi(
      searchName.value,
      searchIpAddress.value,
      pageIndex.value,
      pageSize.value
    )
    if (res && res.response) {
      tableData.value = res.response || []
      total.value = res.count || 0
    } else {
      ElMessage.error(res.message || '获取数据失败')
    }
  } catch (error) {
    ElMessage.error('获取数据失败')
  } finally {
    loading.value = false
  }
}

// 分页大小改变
function onSizeChange(size: number) {
  pageSize.value = size
  pageIndex.value = 1
  fetchData()
}

// 当前页改变
function onCurrentChange(page: number) {
  pageIndex.value = page
  fetchData()
}

// 打开新增
function openAdd() {
  isEdit.value = false
  editForm.id = 0
  editForm.name = ''
  editForm.ip_address = ''
  editForm.folder_paths = ''
  editForm.folderPathsText = ''
  editForm.remark = ''
  showEditModal.value = true
}

// 打开编辑
function openEdit(row: any) {
  isEdit.value = true
  editForm.id = row.id
  editForm.name = row.name
  editForm.ip_address = row.ip_address
  editForm.folder_paths = row.folder_paths
  editForm.folderPathsText = getFolderPaths(row.folder_paths).join('\n')
  editForm.remark = row.remark || ''
  showEditModal.value = true
}

// 保存
async function handleSave() {
  if (!editFormRef.value) return
  
  await editFormRef.value.validate()
  
  // 将文本转换为路径字符串
  const paths = editForm.folderPathsText
    .split('\n')
    .map(path => path.trim())
    .filter(path => path)
    .join('##')
  
  editForm.folder_paths = paths
  
  try {
    if (isEdit.value) {
      await updateFolderMonitorApi(editForm)
      ElMessage.success('更新成功')
    } else {
      await addFolderMonitorApi(editForm)
      ElMessage.success('添加成功')
    }
    showEditModal.value = false
    fetchData()
  } catch (error) {
    ElMessage.error(isEdit.value ? '更新失败' : '添加失败')
  }
}

// 切换状态
async function toggleStatus(row: any) {
  try {
    await toggleFolderMonitorStatusApi(row.id)
    ElMessage.success('状态切换成功')
    fetchData()
  } catch (error) {
    ElMessage.error('状态切换失败')
  }
}

// 删除
async function handleDelete(row: any) {
  try {
    await ElMessageBox.confirm('确定要删除这个配置吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    await deleteFolderMonitorApi([row.id])
    ElMessage.success('删除成功')
    fetchData()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
    }
  }
}

// 初始化
onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.folder-monitor-management {
  padding: 20px;
}

.search-bar {
  margin-bottom: 20px;
  display: flex;
  gap: 10px;
  align-items: center;
}

.search-input {
  width: 200px;
}

.action-bar {
  margin-bottom: 20px;
}

.pagination-bar {
  text-align: right;
}

.path-tip {
  margin-top: 8px;
}

.w-160{ width:160px; }
.btn{ background:#22a2b6; color:#fff; border:none; padding:4px 12px; border-radius:4px; }
.btn.ghost{ background:#f5f7fa; color:#606266; border:1px solid #dcdfe6; }
</style>
