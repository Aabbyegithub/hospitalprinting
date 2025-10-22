<template>
    <div class="db-conn-container">
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
  
      <el-table :data="configList" v-loading="loading" @selection-change="handleSelectionChange" stripe border>
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="ID" prop="id" width="90" align="center" />
        <el-table-column label="配置名称" prop="config_name" align="center" />
        <el-table-column label="数据库类型" prop="database_type" align="center" />
        <el-table-column label="服务器IP" prop="server_ip" align="center" />
        <el-table-column label="端口" prop="server_port" width="100" align="center" />
        <el-table-column label="数据库" prop="database_name" align="center" />
        <el-table-column label="用户名" prop="username" align="center" />
        <el-table-column label="创建时间" prop="create_time" width="180" align="center">
          <template #default="scope">{{ formatDate(scope.row.create_time) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="200" align="center">
          <template #default="scope">
            <el-button type="primary" size="small" @click="editConfig(scope.row)">编辑</el-button>
            <el-button type="danger" size="small" @click="deleteOne(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
  
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
  
      <el-dialog v-model="showEditModal" :title="editForm.id ? '编辑配置' : '新增配置'" width="650px" @close="closeEditModal">
        <el-form ref="editFormRef" :model="editForm" :rules="editFormRules" label-width="130px">
          <el-form-item label="配置名称" prop="config_name">
            <el-input v-model="editForm.config_name" placeholder="请输入配置名称" />
          </el-form-item>
          <el-form-item label="数据库类型" prop="database_type">
          <el-select v-model="editForm.database_type" placeholder="请选择数据库类型" style="width: 100%">
            <el-option label="MySQL" value="MySQL" />
            <el-option label="SQL Server" value="SQLServer" />
            <el-option label="Oracle" value="Oracle" />
            <el-option label="PostgreSQL" value="PostgreSQL" />
          </el-select>
        </el-form-item>
          <el-row :gutter="20">
            <el-col :span="14">
              <el-form-item label="服务器IP/域名" prop="server_ip">
                <el-input v-model="editForm.server_ip" placeholder="例如 127.0.0.1 或 db.example.com" />
              </el-form-item>
            </el-col>
            <el-col :span="10">
              <el-form-item label="端口" prop="server_port">
                <el-input-number v-model="editForm.server_port" :min="1" :max="65535" style="width: 100%"
                :controls="true"/>
              </el-form-item>
            </el-col>
          </el-row>
          <el-form-item label="数据库名称" prop="database_name">
            <el-input v-model="editForm.database_name" placeholder="请输入数据库名称" />
          </el-form-item>
          <el-row :gutter="12">
            <el-col :span="12">
              <el-form-item label="用户名" prop="username">
                <el-input v-model="editForm.username" placeholder="请输入用户名" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="密码" prop="password">
                <el-input v-model="editForm.password" type="password" show-password placeholder="请输入密码" />
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
    </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, reactive, onMounted } from 'vue'
  import { ElMessage, ElMessageBox } from 'element-plus'
  import { listDbConn, addDbConn, updateDbConn, deleteDbConn, getDbConn } from '../../../../api/dbConn'
  
  const loading = ref(false)
  const configList = ref([])
  const total = ref(0)
  const pageIndex = ref(1)
  const pageSize = ref(10)
  const selectedRows = ref<any[]>([])
  const searchName = ref('')
  
  const showEditModal = ref(false)
  const editFormRef = ref()
  const editForm:any = reactive({
    id: 0,
    config_name: '',
    server_ip: '',
    server_port: 3306,
    database_name: '',
    username: '',
    password: '',
    database_type: 'MySQL'
  })
  
  const editFormRules = {
    config_name: [{ required: true, message: '请输入配置名称', trigger: 'blur' }],
    server_ip: [{ required: true, message: '请输入服务器IP/域名', trigger: 'blur' }],
    server_port: [{ required: true, message: '请输入端口', trigger: 'blur' }],
    database_name: [{ required: true, message: '请输入数据库名称', trigger: 'blur' }],
    username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
    password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
    database_type: [{ required: true, message: '请选择数据库类型', trigger: 'change' }]
  }
  
  async function fetchList(showTip = false) {
    loading.value = true
    try {
      const res:any = await listDbConn(searchName.value, 1, pageIndex.value, pageSize.value)
      if (res && res.response) {
        configList.value = res.response
        total.value = res.count || 0
        if (showTip) ElMessage.success('查询成功')
      }
    } catch (e) {
      ElMessage.error('获取列表失败')
    } finally {
      loading.value = false
    }
  }
  
  function onSearch() {
    pageIndex.value = 1
    fetchList(true)
  }
  
  function handleSizeChange(val:number) {
    pageSize.value = val
    pageIndex.value = 1
    fetchList()
  }
  
  function handleCurrentChange(val:number) {
    pageIndex.value = val
    fetchList()
  }
  
  function handleSelectionChange(selection:any[]) {
    selectedRows.value = selection
  }
  
  function openAddModal() {
    Object.assign(editForm, { id: 0, config_name: '', server_ip: '', server_port: 3306, database_name: '', username: '', password: '', database_type: 'MySQL' })
    showEditModal.value = true
  }
  
  async function editConfig(row:any) {
    try {
      const res:any = await getDbConn(row.id)
      if (res && res.response) {
        Object.assign(editForm, res.response)
        // 出于安全考虑，密码不回显明文
        editForm.password = ''
        showEditModal.value = true
      }
    } catch (e) {
      ElMessage.error('获取配置失败')
    }
  }
  
  function closeEditModal() {
    showEditModal.value = false
    editFormRef.value?.resetFields()
  }
  
  async function handleSave() {
    try {
      await editFormRef.value?.validate()
      if (editForm.id) {
        await updateDbConn(editForm)
        ElMessage.success('编辑成功')
      } else {
        await addDbConn(editForm)
        ElMessage.success('新增成功')
      }
      fetchList()
      closeEditModal()
    } catch (e:any) {
      ElMessage.error('保存失败: ' + (e.response?.data?.message || e.message || '未知错误'))
    }
  }
  
  function deleteOne(row:any) {
    ElMessageBox.confirm('确定要删除该配置吗？', '提示', { type: 'warning' }).then(async () => {
      try {
        await deleteDbConn([row.id])
        ElMessage.success('删除成功')
        fetchList()
      } catch (e:any) {
        ElMessage.error('删除失败: ' + (e.response?.data?.message || e.message || '未知错误'))
      }
    })
  }
  
  function batchDelete() {
    if (selectedRows.value.length === 0) {
      ElMessage.warning('请选择要删除的配置')
      return
    }
    ElMessageBox.confirm(`确定要删除选中的 ${selectedRows.value.length} 个配置吗？`, '提示', { type: 'warning' }).then(async () => {
      try {
        const ids = selectedRows.value.map((r:any) => r.id)
        await deleteDbConn(ids)
        ElMessage.success('批量删除成功')
        fetchList()
      } catch (e:any) {
        ElMessage.error('批量删除失败: ' + (e.response?.data?.message || e.message || '未知错误'))
      }
    })
  }
  
  function formatDate(dateStr:string) {
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
  
  onMounted(() => {
    fetchList()
  })
  </script>
  
  <style scoped>
  .db-conn-container { padding: 20px; }
  .operation-bar { display:flex; justify-content: space-between; align-items:center; margin-bottom: 20px; }
  .filter-group { display:flex; gap:10px; align-items:center; }
  .search-input { width: 220px; }
  .search-btn { background:#409eff; color:#fff; border:none; }
  .action-group { display:flex; gap:10px; }
  .pagination-container { text-align:right; }
  .dialog-footer { display:flex; justify-content:flex-end; gap:10px; }
  </style>
  
  