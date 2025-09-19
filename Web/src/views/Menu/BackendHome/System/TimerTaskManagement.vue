<template>
  <div class="timer-task-container">
    <!-- 筛选区 -->
    <div class="filter-bar">
      <el-input v-model="searchName" placeholder="定时器名称" style="width:200px;margin-right:16px;" clearable />
      <el-select v-model="searchStatus" placeholder="运行状态" style="width:140px;margin-right:16px;" clearable>
        <el-option label="全部" :value="''" />
        <el-option label="未启动" :value="0" />
        <el-option label="运行中" :value="1" />
        <el-option label="暂停" :value="2" />
      </el-select>
      <el-button type="primary" @click="fetchList">查询</el-button>
      <el-button type="success" @click="AllTaskStart">全部启动</el-button>
      <el-button type="danger" @click="AllTaskStop">全部停止</el-button>
      <el-button @click="openAddDialog" style="margin-left:auto;">新增定时任务</el-button>
    </div>
    <!-- 列表区 -->
    <el-table :data="taskList" style="margin-top:18px; height: 68vh;" border>
      <el-table-column prop="timerName" label="定时器名称"/>
      <el-table-column prop="timerClass" label="服务类" width="200"/>
      <el-table-column prop="corn" label="运行时段" width="150"/>
      <el-table-column prop="startNumber" label="运行次数" width="100"/>
      <el-table-column prop="lastRunTime" label="最后运行时间" width="160"/>
      <el-table-column prop="isStart" label="状态" width="100">
        <template #default="scope">
          <el-tag :type="statusTagType(scope.row.isStart)">{{ statusText(scope.row.isStart) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="160"/>
      <el-table-column label="操作" width="330">
        <template #default="scope">
          <el-button size="small" @click="openDetailDialog(scope.row)">详情</el-button>
          <el-button size="small" @click="openEditDialog(scope.row)">编辑</el-button>
          <el-button size="small" type="danger" @click="handleDelete(scope.row)">删除</el-button>
          <el-button size="small" v-if="scope.row.isStart===0" type="success" @click="handleStart(scope.row)">启动</el-button>
          <el-button size="small" v-if="scope.row.isStart===1" type="warning" @click="handlePause(scope.row)">暂停</el-button>
          <el-button size="small" v-if="scope.row.isStart===2" type="primary" @click="handleResume(scope.row)">恢复</el-button>
          <el-button size="small" v-if="scope.row.isStart===1" type="info" @click="handleStop(scope.row)">停止</el-button>
        </template>
      </el-table-column>
    </el-table>
    <!-- 分页 -->
    <el-pagination
      style="margin-top:16px;float:right;"
      :total="total"
      :page-size="pageSize"
      :current-page="currentPage"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
      :page-sizes="[10,20,30,50]"
      layout="prev, pager, next, ->, sizes, jumper"
    />
    <!-- 新增/编辑弹窗 -->
    <el-dialog v-model="editDialogVisible" :title="editMode==='add'?'新增定时任务':'编辑定时任务'" width="500">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="定时器名称">
          <el-input v-model="editForm.timerName" />
        </el-form-item>
        <el-form-item label="服务类">
          <el-input v-model="editForm.timerClass" />
        </el-form-item>
        <el-form-item label="Corn表达式">
          <el-input v-model="editForm.corn" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialogVisible=false">取消</el-button>
        <el-button type="primary" @click="handleEditConfirm">保存</el-button>
      </template>
    </el-dialog>
    <!-- 详情弹窗 -->
    <el-dialog v-model="detailDialogVisible" title="定时任务详情" width="500">
      <el-descriptions :column="1" border>
        <el-descriptions-item label="定时器名称">{{ detail.timerName }}</el-descriptions-item>
        <el-descriptions-item label="服务类">{{ detail.timerClass }}</el-descriptions-item>
        <el-descriptions-item label="Corn表达式">{{ detail.corn }}</el-descriptions-item>
        <el-descriptions-item label="运行次数">{{ detail.startNumber }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ statusText(detail.isStart) }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ detail.createTime }}</el-descriptions-item>
        <el-descriptions-item label="开始时间">{{ detail.beginTime }}</el-descriptions-item>
        <el-descriptions-item label="结束时间">{{ detail.endTime }}</el-descriptions-item>
        <el-descriptions-item label="组织ID">{{ detail.orgId }}</el-descriptions-item>
        <el-descriptions-item label="创建人">{{ detail.addUser }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailDialogVisible=false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { dayjs, ElMessage } from 'element-plus';
import {
  getTimerTaskList,
  getTimerTaskDetail,
  addTimerTask,
  updateTimerTask,
  deleteTimerTask,
  startTask,
  stopTask,
  pauseJob,
  resumeJob,
  AddTask,
  removeJob
} from '../../../../api/timertask';
const searchName = ref('');
const searchStatus = ref('');
const taskList = ref<any[]>([]);
const total = ref(0);
const pageSize = ref(10);
const currentPage = ref(1);
const editDialogVisible = ref(false);
const editMode = ref<'add'|'edit'>('add');
const editForm = ref<any>({
  id: 0,
  timerName: '',
  timerClass: '',
  corn: '',
  startNumber: 0
});
const detailDialogVisible = ref(false);
const detail = ref<any>({});

function statusText(val:number) {
  if(val===0) return '未启动';
  if(val===1) return '运行中';
  if(val===2) return '暂停';
  return '未知';
}
function statusTagType(val:number) {
  if(val===0) return 'info';
  if(val===1) return 'success';
  if(val===2) return 'warning';
  return 'default';
}

async function fetchList() {
  const params:any = {
    pageIndex: currentPage.value,
    pageSize: pageSize.value,
    jobName: searchName.value
  };
  const res:any = await getTimerTaskList(params);
  if(res?.success) {
    taskList.value = res.response || [];
    taskList.value.forEach(item=>{
      item.createTime = dayjs(item.createTime).format('YYYY-MM-DD HH:mm:ss');
      item.lastRunTime = item.lastRunTime ? dayjs(item.lastRunTime).format('YYYY-MM-DD HH:mm:ss') : '-';
    });
    total.value = res.count || 0;
  }
}
function handleSizeChange(val:number) {
  pageSize.value = val;
  fetchList();
}
function handlePageChange(val:number) {
  currentPage.value = val;
  fetchList();
}
function openAddDialog() {
  editMode.value = 'add';
  editForm.value = {
    id: 0,
    timerName: '',
    timerClass: '',
    corn: '',
    startNumber: 0
  };
  editDialogVisible.value = true;
}
function openEditDialog(row:any) {
  editMode.value = 'edit';
  editForm.value = { ...row };
  editDialogVisible.value = true;
}
async function AllTaskStart() {
  const res:any = await startTask();
  if(res?.success) {
    ElMessage.success('全部启动成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'全部启动失败');
  }
}

async function AllTaskStop() {
  const res:any = await stopTask();
  if(res?.success) {
    ElMessage.success('全部停止成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'全部停止失败');
  }
}

async function handleEditConfirm() {
  if(editMode.value==='add') {
    const res:any = await addTimerTask(editForm.value);
    if(res?.success) {
      ElMessage.success('新增成功');
      editDialogVisible.value = false;
      fetchList();
    } else {
      ElMessage.error(res?.message||'新增失败');
    }
  } else {
    const play = {
      id: editForm.value.id,
      timerName : editForm.value.timerName,
      timerClass : editForm.value.timerClass,
      corn : editForm.value.corn
    }
    const res:any = await updateTimerTask(play);
    if(res?.success) {
      ElMessage.success('编辑成功');
      editDialogVisible.value = false;
      fetchList();
    } else {
      ElMessage.error(res?.message||'编辑失败');
    }
  }
}
function openDetailDialog(row:any) {
  getTimerTaskDetail(row.id).then((res:any)=>{
    if(res?.success) {
      detail.value = res.response;
      detail.value.createTime = dayjs(detail.value.createTime).format('YYYY-MM-DD HH:mm:ss');
      detail.value.beginTime = dayjs(detail.value.beginTime).format('YYYY-MM-DD HH:mm:ss');
      detail.value.endTime = dayjs(detail.value.endTime).format('YYYY-MM-DD HH:mm:ss');
      detailDialogVisible.value = true;
    }
  });
}
async function handleDelete(row:any) {
  const res:any = await deleteTimerTask(row.id);
  if(res?.success) {
    ElMessage.success('删除成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'删除失败');
  }
}
async function handleStart(row:any) {
  const res:any = await AddTask(row.id,row.timerClass,row.corn);
  if(res?.success) {
    ElMessage.success('启动成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'启动失败');
  }
}
async function handlePause(row:any) {
  const res:any = await pauseJob(row.id);
  if(res?.success) {
    ElMessage.success('暂停成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'暂停失败');
  }
}
async function handleResume(row:any) {
  const res:any = await resumeJob(row.id);
  if(res?.success) {
    ElMessage.success('恢复成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'恢复失败');
  }
}
async function handleStop(row:any) {
  const res:any = await removeJob(row.id);
  if(res?.success) {
    ElMessage.success('停止成功');
    fetchList();
  } else {
    ElMessage.error(res?.message||'停止失败');
  }
}
onMounted(()=>{
  fetchList();
});
</script>

<style scoped>
.timer-task-container {
  padding: 20px;
}
.filter-bar {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}
</style>
