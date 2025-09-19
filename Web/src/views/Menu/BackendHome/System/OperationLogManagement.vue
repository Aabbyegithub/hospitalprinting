<template>
  <div class="operation-log-container">
    <!-- 筛选区 -->
    <div class="filter-bar">
      <el-input v-model="searchUser" placeholder="操作人" style="width:160px;margin-right:12px;" clearable />
      <el-input v-model="searchModule" placeholder="模块名称" style="width:160px;margin-right:12px;" clearable />
      <el-select v-model="searchActionType" placeholder="操作类型" style="width:140px;margin-right:12px;" clearable>
        <el-option label="全部" :value="''" />
        <el-option v-for="(label, key) in ActionTypeMap" :key="key" :label="label" :value="Number(key)" />
      </el-select>
      <el-date-picker v-model="searchStartTime" type="datetime" placeholder="开始时间" style="width:180px;margin-right:12px;" clearable />
      <el-date-picker v-model="searchEndTime" type="datetime" placeholder="结束时间" style="width:180px;margin-right:12px;" clearable />
      <el-button type="primary"style="background-color: #22A2B6;" @click="fetchList">查询</el-button>
    </div>
    <!-- 列表区 -->
    <el-table :data="logList" style="margin-top:18px; height: 68vh;" border>
      <el-table-column prop="addUser" label="操作人" width="100"  align="center"/>
      <el-table-column prop="actionType" label="操作类型" width="100"  align="center">
        <template #default="scope">
          <el-tag>{{ ActionTypeMap[scope.row.actionType] || scope.row.actionType }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="moduleName" label="模块名称"/>
      <el-table-column prop="description" label="操作描述" />
      <el-table-column prop="ActionTime" label="操作时间" width="200"  align="center"/>
      <el-table-column prop="actionContent" label="操作内容" show-overflow-tooltip />
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
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { dayjs } from 'element-plus';
import { getOperationLog, ActionTypeMap, type OperationLog } from '../../../../api/operationlog';

const searchUser = ref('');
const searchModule = ref('');
const searchActionType = ref('');
const searchStartTime = ref('');
const searchEndTime = ref('');
const logList = ref<OperationLog[]>([]);
const total = ref(0);
const pageSize = ref(10);
const currentPage = ref(1);

async function fetchList() {
  const params:any = {
    Page: currentPage.value,
    Size: pageSize.value,
    User: searchUser.value,
    ActionModel: searchModule.value,
    StartTime: searchStartTime.value ? dayjs(searchStartTime.value).format('YYYY-MM-DD HH:mm:ss') : '',
    EndTime: searchEndTime.value ? dayjs(searchEndTime.value).format('YYYY-MM-DD HH:mm:ss') : '',
    actionType: searchActionType.value
  };
  const res:any = await getOperationLog(params);
  if(res?.success) {
    logList.value = res.response || [];
    logList.value.forEach(item=>{
      item.ActionTime = dayjs(item.ActionTime).format('YYYY-MM-DD HH:mm:ss');
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
onMounted(()=>{
  fetchList();
});
</script>

<style scoped>
.operation-log-container {
  padding: 20px;
}
.filter-bar {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}
</style>
