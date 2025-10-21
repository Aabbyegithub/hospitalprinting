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
      <el-table-column prop="server_url" label="服务器URL" align="center" width="180" />
      <!-- <el-table-column v-if="props.type === 2" prop="film_size" label="胶片尺寸" align="center" /> -->
      <el-table-column v-if="props.type !== 2" prop="resolution" label="胶片尺寸" align="center" />
      <!-- <el-table-column v-if="props.type === 2" prop="available_count" label="可用数量" align="center" /> -->
      <el-table-column v-if="props.type !== 2" prop="paper_size" label="纸张" align="center" />
      <el-table-column label="运行状态" align="center" width="100">
        <template #default="scope">
          <el-tag :type="scope.row.status === 1 ? 'success' : 'info'">{{ scope.row.status === 1 ? '运行中' : '未运行' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="location" label="位置" align="center" />
      <el-table-column prop="update_time" label="更新时间" align="center" width="180" :formatter="formatDateTime" />
      <el-table-column label="操作" align="center" :width="props.type === 4 ? '520' : '440'">
        <template #default="scope">
          <el-button type="text" style="color:#67c23a" @click="openEdit(scope.row)">编辑</el-button>
          <el-button type="text" style="color:#67c23a" @click="openConfig(scope.row)">配置</el-button>
          <!-- <el-button type="text" style="color:#409eff" @click="toggleStatus(scope.row)">{{ scope.row.status === 1 ? '停用' : '启用' }}</el-button> -->
          <!-- <el-button type="text" style="color:#409eff" @click="testPrint(scope.row)">测试打印</el-button> -->
          <el-button v-if="props.type === 4" type="text" style="color:#e6a23c" @click="testConnectivity(scope.row)">测试连通</el-button>
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
            <el-option label="激光打印机" :value="4" />
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
        <el-form-item label="服务器URL">
          <el-input v-model="editForm.server_url" placeholder="自动获取" readonly />
        </el-form-item>
        <el-form-item v-if="props.type === 2" label="胶片尺寸" required>
          <el-select v-model="editForm.film_size" placeholder="请选择胶片尺寸">
            <el-option label="14IN×17IN" value="14IN×17IN" />
            <el-option label="14IN×14IN" value="14IN×14IN" />
            <el-option label="11IN×14IN" value="11IN×14IN" />
            <el-option label="10IN×12IN" value="10IN×12IN" />
            <el-option label="8IN×10IN" value="8IN×10IN" />
            <el-option label="A3" value="A3" />
            <el-option label="A4" value="A4" />
          </el-select>
        </el-form-item>
        <!-- <el-form-item v-if="props.type === 2" label="可用数量" required>
          <el-input-number v-model="editForm.available_count" :min="0" />
        </el-form-item>
        <el-form-item v-if="props.type === 2" label="打印时间(秒)" required>
          <el-input-number v-model="editForm.print_time_seconds" :min="0" />
        </el-form-item>
        <el-form-item v-if="props.type !== 2" label="分辨率">
          <el-input v-model="editForm.resolution" />
        </el-form-item>
        <el-form-item v-if="props.type !== 2" label="纸张">
          <el-input v-model="editForm.paper_size" />
        </el-form-item> -->
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

    <!-- 配置弹窗 -->
    <el-dialog v-model="showConfig" title="打印机配置" width="1000px">
      <el-form :model="configForm" label-width="160px">
        <el-form-item label="姓名屏蔽方式">
          <el-select v-model="configForm.mask_mode" placeholder="请选择">
            <el-option label="不屏蔽" :value="0" />
            <el-option label="终端显示屏蔽中间字" :value="1" />
            <el-option label="终端显示屏蔽末尾字" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item label="限制显示天数">
          <el-input-number v-model="configForm.limit_days" :min="0" :max="365" />
          <span style="margin-left:8px;color:#909399;">0 表示不限制</span>
        </el-form-item>
        <el-form-item label="允许的检查类型">
          <el-select v-model="configForm.allowed_exam_types" multiple filterable placeholder="请选择检查类型">
            <el-option v-for="opt in examTypeOptions" :key="opt" :label="opt" :value="opt" />
          </el-select>
        </el-form-item>
        <el-form-item label="仅显示未打印">
          <el-switch v-model="configForm.only_unprinted" :active-value="1" :inactive-value="0" />
        </el-form-item>
        <el-form-item v-if="props.type === 2" label="连接激光打印机">
          <el-select v-model="configForm.laser_printer_id" placeholder="请选择激光打印机" clearable>
            <el-option
              v-for="printer in laserPrinterOptions"
              :key="printer.id"
              :label="printer.name"
              :value="printer.id"
            />
          </el-select>
        </el-form-item>
        
        <!-- 胶片尺寸配置列表 -->
 <el-form-item v-if="props.type === 2" label="胶片尺寸配置">
   <div class="film-size-configs">
     <div v-for="(config, index) in filmSizeConfigs" :key="index" class="config-item">
       <el-row :gutter="8">
         <el-col :span="5">
           <el-select v-model="config.film_size" placeholder="胶片尺寸" style="width: 100%" @change="onFilmSizeChange(config, index)">
             <el-option label="14IN×17IN" value="14IN×17IN" />
             <el-option label="14IN×14IN" value="14IN×14IN" />
             <el-option label="11IN×14IN" value="11IN×14IN" />
             <el-option label="10IN×12IN" value="10IN×12IN" />
             <el-option label="8IN×10IN" value="8IN×10IN" />
             <el-option label="A3" value="A3" />
             <el-option label="A4" value="A4" />
           </el-select>
         </el-col>
         <el-col :span="5">
          <el-select v-model="config.output_file_size" placeholder="输出胶片尺寸" style="width: 100%">
             <el-option label="14IN×17IN" value="14IN×17IN" />
             <el-option label="14IN×14IN" value="14IN×14IN" />
             <el-option label="11IN×14IN" value="11IN×14IN" />
             <el-option label="10IN×12IN" value="10IN×12IN" />
             <el-option label="8IN×10IN" value="8IN×10IN" />
             <el-option label="A3" value="A3" />
             <el-option label="A4" value="A4" />
           </el-select>
         </el-col>
         <el-col :span="3">
           <el-input-number v-model="config.available_count" :min="0" placeholder="数量" style="width: 100%" />
         </el-col>
         <el-col :span="3">
           <el-input-number v-model="config.print_time_seconds" :min="0" placeholder="打印时间" style="width: 100%" />
         </el-col>
         <el-col :span="5">
           <el-select v-model="config.laser_printer_id" placeholder="激光打印机" clearable style="width: 100%">
             <el-option
               v-for="printer in laserPrinterOptions"
               :key="printer.id"
               :label="printer.name"
               :value="printer.id"
             />
           </el-select>
         </el-col>
         <el-col :span="3">
           <el-button type="danger" size="small" @click="removeFilmSizeConfig(index)" style="width: 100%">删除</el-button>
         </el-col>
       </el-row>
     </div>
     <div class="add-config-btn">
       <el-button type="primary" size="small" @click="addFilmSizeConfig">添加尺寸配置</el-button>
     </div>
     <div class="config-tip">
       <el-text type="info" size="small">
         提示：一台打印机的一种胶片尺寸只能添加一条记录
       </el-text>
     </div>
   </div>
 </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="configForm.remark" type="textarea" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showConfig=false">取消</el-button>
        <el-button type="primary" @click="saveConfig">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, watch, onMounted } from 'vue'
import { getPrinterPage, addPrinterApi, editPrinterApi, deletePrinterApi, togglePrinterStatusApi, testPrintApi, testConnectivityApi, getPrinterConfigApi, savePrinterConfigApi, getFilmSizeConfigsApi, saveFilmSizeConfigApi, deleteFilmSizeConfigApi, deleteAllFilmSizeConfigsApi } from '../../../../api/printer'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '../../../../common/axios'

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
const showConfig = ref(false)
const configForm = reactive<any>({})
const examTypeOptions = ['CT','MRI','超声','X光','心电图','其他']
const laserPrinterOptions = ref<any[]>([])
const filmSizeConfigs = ref<any[]>([])

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

// 获取后端API服务器URL
function getBackendApiUrl() {
  // 从axios实例中获取baseURL，这样更灵活
  return axios.defaults.baseURL || 'http://localhost:7092'
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
    // 自动获取后端API服务器URL
    const serverUrl = getBackendApiUrl()
    Object.assign(editForm, { 
      id: 0, 
      name: '', 
      type: props.type, 
      status: 1, 
      model: '', 
      ip_address: '', 
      port: null, 
      server_url: serverUrl,
      resolution: '', 
      paper_size: '', 
      film_size: '',
      available_count: 0,
      print_time_seconds: 0,
      location: '', 
      remark: '' 
    })
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

async function openConfig(row:any){
  try{
    const res:any = await getPrinterConfigApi(row.id)
    const cfg = res?.response || {}
    Object.assign(configForm, {
      id: cfg.id || 0,
      printer_id: row.id,
      mask_mode: cfg.mask_mode ?? 0,
      limit_days: cfg.limit_days ?? 0,
      allowed_exam_types: cfg.allowed_exam_types || [],
      only_unprinted: typeof cfg.only_unprinted === 'number' ? cfg.only_unprinted : 0,
      laser_printer_id: cfg.laser_printer_id || null,
      remark: cfg.remark || ''
    })
    
    // 如果是胶片打印机，获取激光打印机列表和胶片尺寸配置
    if (props.type === 2) {
      await fetchLaserPrinters()
      await fetchFilmSizeConfigs(row.id)
    }
    
    showConfig.value = true
  }catch(e){
    // 无配置也允许打开
    Object.assign(configForm, { 
      id:0, 
      printer_id: row.id, 
      mask_mode:0, 
      limit_days:0, 
      allowed_exam_types:[], 
      only_unprinted:0, 
      laser_printer_id: null,
      remark:'' 
    })
    
    if (props.type === 2) {
      await fetchLaserPrinters()
      filmSizeConfigs.value = []
    }
    
    showConfig.value = true
  }
}

async function saveConfig(){
  try{
    // 保存基础配置
    await savePrinterConfigApi({ ...configForm })
    
    // 如果是胶片打印机，保存胶片尺寸配置
    if (props.type === 2) {
      await saveFilmSizeConfigs()
    }
    
    ElMessage.success('配置已保存')
    showConfig.value = false
  }catch(e){
    ElMessage.error('保存配置失败')
  }
}

// 胶片尺寸配置相关函数
function addFilmSizeConfig() {
  filmSizeConfigs.value.push({
    id: 0,
    film_size: '',
    output_file_size: '',
    available_count: 0,
    print_time_seconds: 0,
    laser_printer_id: null
  })
}

function removeFilmSizeConfig(index: number) {
  filmSizeConfigs.value.splice(index, 1)
}

// 检查胶片尺寸是否重复
function onFilmSizeChange(config: any, currentIndex: number) {
  if (!config.film_size) return
  
  // 检查是否有重复的胶片尺寸
  const duplicateIndex = filmSizeConfigs.value.findIndex((item, index) => 
    index !== currentIndex && item.film_size === config.film_size
  )
  
  if (duplicateIndex !== -1) {
    ElMessage.warning('该胶片尺寸已存在，请选择其他尺寸')
    config.film_size = ''
    return
  }
}

async function fetchFilmSizeConfigs(printerId: number) {
  try {
    const res: any = await getFilmSizeConfigsApi(printerId)
    if (res && res.response) {
      filmSizeConfigs.value = res.response.map((item: any) => ({
        id: item.id,
        film_size: item.film_size,
        output_file_size: item.output_file_size || '',
        available_count: item.available_count,
        print_time_seconds: item.print_time_seconds,
        laser_printer_id: item.laser_printer_id
      }))
    } else {
      filmSizeConfigs.value = []
    }
  } catch (error) {
    console.error('获取胶片尺寸配置失败:', error)
    filmSizeConfigs.value = []
  }
}

async function saveFilmSizeConfigs() {
  try {
    // 检查是否有重复的胶片尺寸
    const filmSizes = filmSizeConfigs.value.map(config => config.film_size).filter(size => size)
    const uniqueFilmSizes = [...new Set(filmSizes)]
    
    if (filmSizes.length !== uniqueFilmSizes.length) {
      ElMessage.error('存在重复的胶片尺寸，请检查后重试')
      return
    }
    
    // 先删除该打印机的所有胶片尺寸配置
    await deleteAllFilmSizeConfigsApi(configForm.printer_id)
    
    // 保存每个胶片尺寸配置
    for (const config of filmSizeConfigs.value) {
      if (!config.film_size) {
        ElMessage.warning('请选择胶片尺寸')
        continue
      }
      
      const configData = {
        id: 0, // 新增时ID设为0
        printer_id: configForm.printer_id,
        mask_mode: configForm.mask_mode,
        limit_days: configForm.limit_days,
        allowed_exam_types: configForm.allowed_exam_types,
        only_unprinted: configForm.only_unprinted,
        laser_printer_id: config.laser_printer_id,
        film_size: config.film_size,
        output_file_size: config.output_file_size || '',
        available_count: config.available_count,
        print_time_seconds: config.print_time_seconds,
        remark: configForm.remark
      }
      
      await saveFilmSizeConfigApi(configData)
    }
    
    console.log('保存胶片尺寸配置成功:', filmSizeConfigs.value)
  } catch (error) {
    console.error('保存胶片尺寸配置失败:', error)
    throw error
  }
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

async function testConnectivity(row:any){
  try {
    await testConnectivityApi(row.id)
    ElMessage.success('连通性测试成功')
  } catch (error) {
    ElMessage.error('连通性测试失败')
  }
}

async function fetchLaserPrinters(){
  try {
    const res:any = await getPrinterPage('', 4, 1, 1, 100) // type=4 激光打印机
    if (res && res.response) {
      laserPrinterOptions.value = res.response
    }
  } catch (error) {
    console.error('获取激光打印机列表失败:', error)
  }
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

.film-size-configs {
  width: 100%;
}

.config-item {
  margin-bottom: 12px;
  padding: 12px;
  border: 1px solid #e4e7ed;
  border-radius: 4px;
  background-color: #fafafa;
}

.config-item:hover {
  border-color: #409eff;
  background-color: #f0f9ff;
}

.config-item .el-row {
  margin-bottom: 0;
}

.config-item .el-col {
  margin-bottom: 0;
}

.add-config-btn {
  margin-top: 12px;
  text-align: center;
  padding: 8px 0;
}

.config-tip {
  margin-top: 12px;
  text-align: center;
  padding: 8px;
  background-color: #f8f9fa;
  border-radius: 4px;
  border: 1px solid #e9ecef;
}
</style>


