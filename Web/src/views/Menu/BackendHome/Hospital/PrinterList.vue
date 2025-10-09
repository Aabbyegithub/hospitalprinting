<template>
  <div class="printer-list-container">
    <div class="toolbar">
      <div class="filters">
        <el-input v-model="searchName" placeholder="搜索名称" class="w-200" @keyup.enter="fetchList(true)" />
        <el-select v-model="searchStatus" placeholder="状态" class="w-160" clearable @change="fetchList(true)">
          <el-option label="全部" :value="undefined" />
          <el-option label="启用" :value="1" />
          <el-option label="停用" :value="0" />
        </el-select>
        <el-button class="btn" @click="fetchList(true)">搜索</el-button>
        <el-button class="btn ghost" @click="resetFilters">重置</el-button>
      </div>
      <div class="actions">
        <el-button type="primary" @click="openEdit()">新增打印机</el-button>
        <el-button type="danger" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
      </div>
    </div>

    <el-table :data="tableData" border style="width:100%;height:65vh" @selection-change="onSelectionChange">
      <el-table-column type="selection" width="55" />
      <el-table-column prop="name" label="名称" align="center" />
      <el-table-column prop="model" label="型号" align="center" />
      <el-table-column prop="ip_address" label="IP/主机名" align="center" />
      <el-table-column prop="port" label="端口" width="80" align="center" />
      <el-table-column prop="resolution" label="分辨率" align="center" />
      <el-table-column prop="paper_size" label="纸张" align="center" />
      <el-table-column label="状态" align="center" width="100">
        <template #default="scope">
          <el-tag :type="scope.row.status === 1 ? 'success' : 'info'">{{ scope.row.status === 1 ? '启用' : '停用' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="location" label="位置" align="center" />
      <el-table-column prop="update_time" label="更新时间" align="center" :formatter="formatDateTime" />
      <el-table-column label="操作" align="center" width="320">
        <template #default="scope">
          <el-button type="text" style="color:#67c23a" @click="openEdit(scope.row)">编辑</el-button>
          <el-button type="text" style="color:#409eff" @click="toggleStatus(scope.row)">{{ scope.row.status === 1 ? '停用' : '启用' }}</el-button>
          <el-button type="text" style="color:#409eff" @click="testPrint(scope.row)">测试打印</el-button>
          <el-button type="text" style="color:#f56c6c" @click="handleDelete(scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <div class="pagination-bar">
      <el-pagination
        style="margin-top:16px;float:right;"
        :total="total"
        :page-size="pageSize"
        :current-page="pageIndex"
        @size-change="onSizeChange"
        @current-change="onPageChange"
        :page-sizes="[10,20,30,50]"
        layout="prev, pager, next, ->, sizes, jumper"
      />
    </div>

    <el-dialog v-model="showEdit" :title="editForm.id ? '编辑打印机' : '新增打印机'" width="640px">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="名称" required>
          <el-input v-model="editForm.name" />
        </el-form-item>
        <el-form-item label="类型" required>
          <el-select v-model="editForm.type" :disabled="lockType">
            <el-option label="自助打印机" :value="1" />
            <el-option label="胶片打印机" :value="2" />
            <el-option label="报告打印机" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="型号">
          <el-input v-model="editForm.model" />
        </el-form-item>
        <el-form-item label="IP/主机名">
          <el-input v-model="editForm.ip_address" />
        </el-form-item>
        <el-form-item label="端口">
          <el-input v-model.number="editForm.port" />
        </el-form-item>
        <el-form-item label="分辨率">
          <el-input v-model="editForm.resolution" />
        </el-form-item>
        <el-form-item label="纸张">
          <el-input v-model="editForm.paper_size" />
        </el-form-item>
        <el-form-item label="位置">
          <el-input v-model="editForm.location" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="editForm.remark" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showEdit=false">取消</el-button>
        <el-button type="primary" @click="save">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, watch, onMounted } from 'vue'
import { getPrinterPage, addPrinterApi, editPrinterApi, deletePrinterApi, togglePrinterStatusApi, testPrintApi } from '../../../../api/printer'
import { ElMessage, ElMessageBox } from 'element-plus'

const props = defineProps<{ type: number }>()

const searchName = ref('')
const searchStatus = ref<number | undefined>(undefined)
const pageIndex = ref(1)
const pageSize = ref(10)
const total = ref(0)
const tableData = ref<any[]>([])
const selectedRows = ref<any[]>([])

const showEdit = ref(false)
const editForm = reactive<any>({})
const lockType = ref(false)

function formatDateTime(row: any, column: any, value: any) {
  if (!value) return '--'
  const d = new Date(value)
  if (isNaN(d.getTime())) return '--'
  const y = d.getFullYear()
  const m = String(d.getMonth()+1).padStart(2,'0')
  const da = String(d.getDate()).padStart(2,'0')
  const hh = String(d.getHours()).padStart(2,'0')
  const mm = String(d.getMinutes()).padStart(2,'0')
  const ss = String(d.getSeconds()).padStart(2,'0')
  return `${y}-${m}-${da} ${hh}:${mm}:${ss}`
}

async function fetchList(showTip=false) {
  await getPrinterPage(searchName.value, props.type, searchStatus.value, pageIndex.value, pageSize.value)
    .then((res:any)=>{
      if (res && res.response) {
        tableData.value = res.response
        total.value = res.count || 0
        if (showTip) ElMessage.success('查询成功')
      }
    }).catch(()=> ElMessage.error('查询失败'))
}

function resetFilters(){
  searchName.value=''
  searchStatus.value=undefined
  pageIndex.value=1
  fetchList(true)
}

function onSelectionChange(rows:any[]){ selectedRows.value = rows }
function onSizeChange(val:number){ pageSize.value = val; fetchList() }
function onPageChange(val:number){ pageIndex.value = val; fetchList() }

function openEdit(row?:any){
  showEdit.value = true
  lockType.value = true
  if (row) {
    Object.assign(editForm, row)
  } else {
    Object.assign(editForm, { id: 0, name: '', type: props.type, status: 1, model: '', ip_address: '', port: null, resolution: '', paper_size: '', location: '', remark: '' })
  }
}

async function save(){
  if (!editForm.name || !editForm.type){ ElMessage.error('名称与类型必填'); return }
  const payload = { ...editForm }
  try{
    if (payload.id && payload.id>0){
      await editPrinterApi(payload)
      ElMessage.success('编辑成功')
    } else {
      await addPrinterApi(payload)
      ElMessage.success('新增成功')
    }
    showEdit.value=false
    fetchList()
  }catch(e){ ElMessage.error('保存失败') }
}

async function handleDelete(row:any){
  await ElMessageBox.confirm('确认删除该打印机吗？','提示')
  await deletePrinterApi([row.id])
  ElMessage.success('删除成功')
  fetchList()
}

async function handleBatchDelete(){
  if (!selectedRows.value.length) return
  await ElMessageBox.confirm(`确认删除选中的 ${selectedRows.value.length} 条吗？`,'提示')
  await deletePrinterApi(selectedRows.value.map(i=>i.id))
  ElMessage.success('删除成功')
  fetchList()
}

async function toggleStatus(row:any){
  const to = row.status === 1 ? 0 : 1
  await togglePrinterStatusApi(row.id, to)
  ElMessage.success('状态已更新')
  fetchList()
}

async function testPrint(row:any){
  await testPrintApi(row.id)
  ElMessage.success('测试打印任务已下发')
}

onMounted(()=> fetchList())
watch(()=>props.type, ()=>{ resetFilters() })
</script>

<style scoped>
.printer-list-container{ padding:10px; }
.toolbar{ display:flex; justify-content:space-between; align-items:center; margin-bottom:16px; flex-wrap:wrap; gap:12px; }
.filters{ display:flex; align-items:center; gap:12px; }
.w-200{ width:200px; }
.w-160{ width:160px; }
.btn{ background:#22a2b6; color:#fff; border:none; padding:4px 12px; border-radius:4px; }
.btn.ghost{ background:#f5f7fa; color:#606266; border:1px solid #dcdfe6; }
.pagination-bar{ text-align:right; }
</style>


