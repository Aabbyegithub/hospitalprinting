<template>
  <div class="database-config-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchName" placeholder="搜索配置名称" class="search-input" @keyup.enter="onSearch" />
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-group">
          <el-button type="primary" @click="openAddModal">新增配置</el-button>
          <el-button type="danger" @click="batchDelete" :disabled="selectedRows.length === 0">批量删除</el-button>
        </div>
      </div>
    </div>

    <div class="table-container">
      <el-table
        :data="configList"
        v-loading="loading"
        @selection-change="handleSelectionChange"
        stripe
        border
        style="width: 100%;height: 65vh;"
      >
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="ID" prop="id" width="80" align="center" />
        <el-table-column label="配置名称" prop="config_name" align="center" />
        <el-table-column label="服务器IP" prop="server_ip" align="center" />
        <el-table-column label="端口" prop="server_port" align="center" width="80" />
        <el-table-column label="数据库名称" prop="database_name" align="center" />
        <el-table-column label="数据库类型" prop="database_type" align="center" width="100" />
        <el-table-column label="是否启用" prop="is_enabled" align="center" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.is_enabled === 1 ? 'success' : 'danger'">
              {{ scope.row.is_enabled === 1 ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="是否默认" prop="is_default" align="center" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.is_default === 1 ? 'warning' : 'info'">
              {{ scope.row.is_default === 1 ? '默认' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="同步状态" prop="sync_status" align="center" width="100">
          <template #default="scope">
            <el-tag :type="getSyncStatusTagType(scope.row.sync_status)">
              {{ scope.row.sync_status_text }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="最后同步时间" prop="last_sync_time" align="center" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.last_sync_time) }}
          </template>
        </el-table-column>
        <el-table-column label="创建时间" prop="create_time" align="center" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.create_time) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="300" align="center">
          <template #default="scope">
            <el-button type="primary" size="small" @click="editConfig(scope.row)">编辑</el-button>
            <el-button type="success" size="small" @click="testConnection(scope.row)">测试连接</el-button>
            <el-button type="warning" size="small" @click="manualSync(scope.row)">手动同步</el-button>
            <el-button type="info" size="small" @click="viewSyncLogs(scope.row)">同步日志</el-button>
            <el-button type="danger" size="small" @click="deleteConfig(scope.row)">删除</el-button>
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

    <!-- 新增/编辑配置对话框 -->
    <el-dialog
      v-model="showEditModal"
      :title="editForm.id ? '编辑数据库配置' : '新增数据库配置'"
      width="800px"
      @close="closeEditModal"
    >
      <el-form
        ref="editFormRef"
        :model="editForm"
        :rules="editFormRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="配置名称" prop="config_name">
              <el-input v-model="editForm.config_name" placeholder="请输入配置名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据库类型" prop="database_type">
              <el-select v-model="editForm.database_type" placeholder="请选择数据库类型" @change="onDatabaseTypeChange">
                <el-option v-for="(label, value) in databaseTypeOptions" :key="value" :label="label" :value="value" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="服务器IP" prop="server_ip">
              <el-input v-model="editForm.server_ip" placeholder="请输入服务器IP" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="端口" prop="server_port">
              <el-input-number v-model="editForm.server_port" :min="1" :max="65535" placeholder="端口" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="数据库名称" prop="database_name">
              <el-input v-model="editForm.database_name" placeholder="请输入数据库名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="用户名" prop="username">
              <el-input v-model="editForm.username" placeholder="请输入用户名" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="密码" prop="password">
          <el-input v-model="editForm.password" type="password" placeholder="请输入密码" show-password />
        </el-form-item>
        
        <el-form-item label="连接字符串" prop="connection_string">
          <el-input
            v-model="editForm.connection_string"
            type="textarea"
            :rows="3"
            placeholder="请输入连接字符串（可选）"
          />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="是否启用" prop="is_enabled">
              <el-select v-model="editForm.is_enabled" placeholder="请选择">
                <el-option label="启用" :value="1" />
                <el-option label="禁用" :value="0" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="是否默认" prop="is_default">
              <el-select v-model="editForm.is_default" placeholder="请选择">
                <el-option label="是" :value="1" />
                <el-option label="否" :value="0" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="同步间隔(分钟)" prop="sync_interval">
              <el-input-number v-model="editForm.sync_interval" :min="1" :max="1440" placeholder="同步间隔" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="closeEditModal">取消</el-button>
          <el-button type="primary" @click="handleSave">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 同步日志对话框 -->
    <el-dialog
      v-model="showSyncLogModal"
      title="同步日志"
      width="1000px"
      @close="closeSyncLogModal"
    >
      <el-table :data="syncLogList" v-loading="syncLogLoading" stripe border>
        <el-table-column label="ID" prop="id" width="80" align="center" />
        <el-table-column label="同步类型" prop="sync_type_text" align="center" width="100" />
        <el-table-column label="同步状态" prop="sync_status_text" align="center" width="100">
          <template #default="scope">
            <el-tag :type="getSyncStatusTagType(scope.row.sync_status)">
              {{ scope.row.sync_status_text }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="开始时间" prop="start_time" align="center" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.start_time) }}
          </template>
        </el-table-column>
        <el-table-column label="结束时间" prop="end_time" align="center" width="180">
          <template #default="scope">
            {{ formatDate(scope.row.end_time) }}
          </template>
        </el-table-column>
        <el-table-column label="同步记录数" prop="sync_count" align="center" width="120" />
        <el-table-column label="错误信息" prop="error_message" align="center" show-overflow-tooltip />
      </el-table>
      
      <div class="pagination-container" style="margin-top: 16px;">
        <el-pagination
          style="float:right;"
          :total="syncLogTotal"
          :page-size="syncLogPageSize"
          :current-page="syncLogPageIndex"
          @size-change="handleSyncLogSizeChange"
          @current-change="handleSyncLogCurrentChange"
          :page-sizes="[10,20,30,50]"
          layout="prev, pager, next, ->, sizes, jumper"
        />
      </div>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  getDatabaseConfigList, 
  addDatabaseConfig, 
  updateDatabaseConfig, 
  deleteDatabaseConfig,
  testDatabaseConnection,
  manualSyncDatabase,
  getDatabaseSyncLogs,
  getDatabaseTypeOptions
} from '../../../../api/databaseConfig'

// 响应式数据
const loading = ref(false)
const configList = ref([])
const total = ref(0)
const pageIndex = ref(1)
const pageSize = ref(10)
const selectedRows = ref<any[]>([])

// 搜索条件
const searchName = ref('')

// 编辑表单
const showEditModal = ref(false)
const editFormRef = ref()
const editForm: any = reactive({
  id: 0,
  config_name: '',
  server_ip: '',
  server_port: 3306,
  database_name: '',
  username: '',
  password: '',
  database_type: 'MySQL',
  connection_string: '',
  is_enabled: 1,
  is_default: 0,
  sync_interval: 30,
  org_id: 1
})

// 数据库类型选项
const databaseTypeOptions = ref<Record<string, string>>({})

// 同步日志相关
const showSyncLogModal = ref(false)
const syncLogList = ref([])
const syncLogLoading = ref(false)
const syncLogTotal = ref(0)
const syncLogPageIndex = ref(1)
const syncLogPageSize = ref(10)
const currentConfigId = ref(0)

// 表单验证规则
const editFormRules = {
  config_name: [
    { required: true, message: '请输入配置名称', trigger: 'blur' }
  ],
  server_ip: [
    { required: true, message: '请输入服务器IP', trigger: 'blur' }
  ],
  server_port: [
    { required: true, message: '请输入端口', trigger: 'blur' }
  ],
  database_name: [
    { required: true, message: '请输入数据库名称', trigger: 'blur' }
  ],
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' }
  ],
  database_type: [
    { required: true, message: '请选择数据库类型', trigger: 'change' }
  ]
}

// 获取配置列表
async function fetchConfigList(showTip = false) {
  loading.value = true
  try {
    const res: any = await getDatabaseConfigList(searchName.value, 1, pageIndex.value, pageSize.value)
    if (res && res.response) {
      configList.value = res.response
      total.value = res.count || 0
      if (showTip) {
        ElMessage.success('查询成功')
      }
    }
  } catch (error) {
    console.error('获取配置列表失败:', error)
    ElMessage.error('获取配置列表失败')
  } finally {
    loading.value = false
  }
}

// 搜索
function onSearch() {
  pageIndex.value = 1
  fetchConfigList(true)
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val
  pageIndex.value = 1
  fetchConfigList()
}

function handleCurrentChange(val: number) {
  pageIndex.value = val
  fetchConfigList()
}

// 选择处理
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection
}

// 打开新增对话框
function openAddModal() {
  Object.assign(editForm, {
    id: 0,
    config_name: '',
    server_ip: '',
    server_port: 3306,
    database_name: '',
    username: '',
    password: '',
    database_type: 'MySQL',
    connection_string: '',
    is_enabled: 1,
    is_default: 0,
    sync_interval: 30,
    org_id: 1
  })
  showEditModal.value = true
}

// 编辑配置
function editConfig(row: any) {
  Object.assign(editForm, row)
  showEditModal.value = true
}

// 关闭编辑对话框
function closeEditModal() {
  showEditModal.value = false
  editFormRef.value?.resetFields()
}

// 数据库类型变化
function onDatabaseTypeChange(value: string) {
  // 根据数据库类型设置默认端口
  const defaultPorts: Record<string, number> = {
    'MySQL': 3306,
    'SQLServer': 1433,
    'Oracle': 1521,
    'PostgreSQL': 5432
  }
  editForm.server_port = defaultPorts[value] || 3306
}

// 保存配置
async function handleSave() {
  try {
    await editFormRef.value?.validate()
    
    if (editForm.id) {
      await updateDatabaseConfig(editForm)
      ElMessage.success('编辑成功')
    } else {
      await addDatabaseConfig(editForm)
      ElMessage.success('新增成功')
    }
    fetchConfigList()
    closeEditModal()
  } catch (error: any) {
    console.error('保存失败:', error)
    ElMessage.error('保存失败: ' + (error.response?.data?.message || error.message || '未知错误'))
  }
}

// 删除配置
function deleteConfig(row: any) {
  ElMessageBox.confirm('确定要删除该配置吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await deleteDatabaseConfig([row.id])
      ElMessage.success('删除成功')
      fetchConfigList()
    } catch (error: any) {
      console.error('删除失败:', error)
      ElMessage.error('删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 批量删除
function batchDelete() {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('请选择要删除的配置')
    return
  }
  
  ElMessageBox.confirm(`确定要删除选中的 ${selectedRows.value.length} 个配置吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      const ids = selectedRows.value.map((row: any) => row.id)
      await deleteDatabaseConfig(ids)
      ElMessage.success('批量删除成功')
      fetchConfigList()
    } catch (error: any) {
      console.error('批量删除失败:', error)
      ElMessage.error('批量删除失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 测试连接
async function testConnection(row: any) {
  try {
    const testData = {
      server_ip: row.server_ip,
      server_port: row.server_port,
      database_name: row.database_name,
      username: row.username,
      password: row.password,
      database_type: row.database_type
    }
    
    await testDatabaseConnection(testData)
    ElMessage.success('数据库连接测试成功')
  } catch (error: any) {
    console.error('连接测试失败:', error)
    ElMessage.error('连接测试失败: ' + (error.response?.data?.message || error.message || '未知错误'))
  }
}

// 手动同步
function manualSync(row: any) {
  ElMessageBox.confirm('确定要手动同步该配置的数据吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await manualSyncDatabase(row.id)
      ElMessage.success('同步任务已启动')
      fetchConfigList()
    } catch (error: any) {
      console.error('同步失败:', error)
      ElMessage.error('同步失败: ' + (error.response?.data?.message || error.message || '未知错误'))
    }
  })
}

// 查看同步日志
function viewSyncLogs(row: any) {
  currentConfigId.value = row.id
  showSyncLogModal.value = true
  fetchSyncLogs()
}

// 获取同步日志
async function fetchSyncLogs() {
  syncLogLoading.value = true
  try {
    const res: any = await getDatabaseSyncLogs(currentConfigId.value, syncLogPageIndex.value, syncLogPageSize.value)
    if (res && res.response) {
      syncLogList.value = res.response
      syncLogTotal.value = res.count || 0
    }
  } catch (error) {
    console.error('获取同步日志失败:', error)
    ElMessage.error('获取同步日志失败')
  } finally {
    syncLogLoading.value = false
  }
}

// 同步日志分页处理
function handleSyncLogSizeChange(val: number) {
  syncLogPageSize.value = val
  syncLogPageIndex.value = 1
  fetchSyncLogs()
}

function handleSyncLogCurrentChange(val: number) {
  syncLogPageIndex.value = val
  fetchSyncLogs()
}

// 关闭同步日志对话框
function closeSyncLogModal() {
  showSyncLogModal.value = false
  syncLogList.value = []
  syncLogTotal.value = 0
}

// 获取同步状态标签类型
function getSyncStatusTagType(status: number) {
  const typeMap: { [key: number]: 'primary' | 'success' | 'warning' | 'info' | 'danger' } = {
    0: 'info',
    1: 'warning',
    2: 'success',
    3: 'danger'
  }
  return typeMap[status] || 'info'
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
  fetchConfigList()
  // 获取数据库类型选项
  getDatabaseTypeOptions().then((res: any) => {
    if (res && res.response) {
      databaseTypeOptions.value = res.response
    }
  })
})
</script>

<style scoped>
.database-config-container {
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
