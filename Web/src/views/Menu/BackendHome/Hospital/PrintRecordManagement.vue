<template>
  <div class="print-record-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchExamId" placeholder="搜索检查ID" class="search-input" @keyup.enter="onSearch" />
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增打印记录</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="print-record-table-view">
      <div class="table-list">
        <el-table
          :data="printRecordList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="记录ID" prop="id" align="center" width="80" />
          <el-table-column label="检查ID" align="center" width="120">
            <template #default="scope">
              <el-button type="text" style="color:#409eff;" @click="showExam(scope.row.holExamination)">
                {{ scope.row.exam_id }}
              </el-button>
            </template>
          </el-table-column>
          <el-table-column label="患者姓名" align="center" width="140">
            <template #default="scope">
              <el-button
                v-if="scope.row.holPatient && scope.row.holPatient.name"
                type="text"
                style="color:#409eff;"
                @click="showPatient(scope.row.holPatient)"
              >
                {{ scope.row.holPatient.name }}
              </el-button>
              <span v-else>--</span>
            </template>
          </el-table-column>
          <el-table-column label="打印时间" prop="print_time" align="center" :formatter="formatDateTime" />
          <el-table-column label="打印人ID" prop="printed_by" align="center" width="100" />
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
          style="margin-top:16px;float:right;"
          :total="total"
          :page-size="pageSize"
          :current-page="pageIndex"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
          :page-sizes="[10,20,30,50]"
          layout="prev, pager, next, ->, sizes, jumper"
        />
      </div>
    </div>
    <!-- 编辑/新增弹窗 -->
    <el-dialog v-model="showEditModal" width="600" :title="editForm.id ? '编辑打印记录' : '新增打印记录'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="检查记录" required>
          <el-select
            v-model="editForm.exam_id"
            placeholder="搜索并选择检查记录（按检查号/患者名）"
            filterable
            remote
            :remote-method="searchExaminations"
            :loading="examLoading"
            style="width:100%"
          >
            <el-option
              v-for="exam in examOptions"
              :key="exam.id"
              :label="`${exam.exam_no}（${exam.patient?.name || '--'}，${exam.exam_type || ''}）`
                "
              :value="exam.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="患者" required>
          <el-select
            v-model="editForm.patient_id"
            placeholder="搜索并选择患者"
            filterable
            remote
            :remote-method="searchPatients"
            :loading="patientLoading"
            style="width:100%"
          >
            <el-option
              v-for="p in patientOptions"
              :key="p.id"
              :label="`${p.name}（${p.medical_no || '无就诊号'}）`"
              :value="p.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="打印人ID" required>
          <el-input-number v-model="editForm.printed_by" :min="1" placeholder="请输入打印人ID" style="width: 100%" />
        </el-form-item>
        <el-form-item label="打印时间">
          <el-date-picker
            v-model="editForm.print_time"
            type="datetime"
            placeholder="选择打印时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button class="cancel-btn" @click="closeEditModal">取消</el-button>
        <el-button type="primary" class="Btn-Save" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>
    <!-- 患者信息弹窗 -->
    <el-dialog v-model="showPatientModal" title="患者信息" width="500px" :close-on-click-modal="false">
      <div v-if="currentPatient" class="patient-info">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="姓名">{{ currentPatient.name || '--' }}</el-descriptions-item>
          <el-descriptions-item label="性别">{{ currentPatient.gender === 1 || currentPatient.gender === '男' ? '男' : (currentPatient.gender === 2 || currentPatient.gender === '女' ? '女' : '未知') }}</el-descriptions-item>
          <el-descriptions-item label="年龄">{{ currentPatient.age ?? '--' }}</el-descriptions-item>
          <el-descriptions-item label="身份证号">{{ currentPatient.id_card || '--' }}</el-descriptions-item>
          <el-descriptions-item label="联系方式">{{ currentPatient.contact || '--' }}</el-descriptions-item>
          <el-descriptions-item label="就诊号">{{ currentPatient.medical_no || '--' }}</el-descriptions-item>
        </el-descriptions>
      </div>
      <template #footer>
        <el-button type="primary" @click="showPatientModal = false">关闭</el-button>
      </template>
    </el-dialog>

    <!-- 检查信息弹窗 -->
    <el-dialog v-model="showExamModal" title="检查信息" width="600px" :close-on-click-modal="false">
      <div v-if="currentExam" class="exam-info">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="检查号">{{ currentExam.exam_no || '--' }}</el-descriptions-item>
          <el-descriptions-item label="检查类型">{{ currentExam.exam_type || '--' }}</el-descriptions-item>
          <el-descriptions-item label="检查日期">{{ formatDateTime({}, {}, currentExam.exam_date) }}</el-descriptions-item>
          <el-descriptions-item label="打印状态">{{ currentExam.is_printed === 1 ? '已打印' : '未打印' }}</el-descriptions-item>
          <el-descriptions-item label="报告编号">{{ currentExam.report_no || '--' }}</el-descriptions-item>
          <el-descriptions-item label="胶片检查号">{{ currentExam.image_no || '--' }}</el-descriptions-item>
          <el-descriptions-item label="报告文件"><span>{{ currentExam.report_path || '--' }}</span></el-descriptions-item>
          <el-descriptions-item label="电子胶片"><span>{{ currentExam.image_path || '--' }}</span></el-descriptions-item>
          <el-descriptions-item label="诊断医生">{{ currentExam.doctor?.name || '--' }}</el-descriptions-item>
        </el-descriptions>
      </div>
      <template #footer>
        <el-button type="primary" @click="showExamModal = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import { ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElDialog, ElForm, ElFormItem, ElMessage, ElInputNumber, ElDatePicker } from 'element-plus';
import { addPrintRecordApi, deletePrintRecordApi, editPrintRecordApi, getPrintRecordList } from '../../../../api/printRecord';
import { getPatientList } from '../../../../api/patient';
import { getExaminationList } from '../../../../api/examination';

const searchExamId = ref('');
const printRecordList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(0);
const selectedRows = ref<any[]>([]);

// 弹窗状态
const showPatientModal = ref(false);
const currentPatient = ref<any>(null);
const showExamModal = ref(false);
const currentExam = ref<any>(null);

// 远程搜索选项
const patientOptions = ref<any[]>([]);
const patientLoading = ref(false);
const examOptions = ref<any[]>([]);
const examLoading = ref(false);

// 格式化日期时间
const formatDateTime = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  const d = new Date(cellValue);
  if (isNaN(d.getTime())) return '--';
  const y = d.getFullYear();
  const m = d.getMonth() + 1;
  const day = d.getDate();
  const hh = String(d.getHours()).padStart(2, '0');
  const mm = String(d.getMinutes()).padStart(2, '0');
  const ss = String(d.getSeconds()).padStart(2, '0');
  return `${y}-${m}-${day} ${hh}:${mm}:${ss}`;
};

// 获取打印记录列表
async function fetchPrintRecordList(showTip: boolean = false) {
  const examId = searchExamId.value ? parseInt(searchExamId.value) : null;
  await getPrintRecordList(examId, pageIndex.value, pageSize.value)
    .then((res: any) => {
      if (res && res.response) {
        printRecordList.value = res.response;
        total.value = res.count || 0;
        if (showTip) {
          ElMessage.success(`查询成功，匹配到 ${total.value} 条记录`);
        }
      }
    })
    .catch((error) => {
      console.error('获取打印记录列表失败:', error);
      ElMessage.error('获取打印记录列表失败');
    });
}

function onSearch() {
  pageIndex.value = 1;
  fetchPrintRecordList(true);
}

// 远程搜索：患者
async function searchPatients(query: string) {
  if (!query) {
    patientOptions.value = [];
    return;
  }
  patientLoading.value = true;
  try {
    const res: any = await getPatientList(query, '', 1, 20);
    if (res && res.response) patientOptions.value = res.response;
  } catch (e) {
    console.error('搜索患者失败:', e);
  } finally {
    patientLoading.value = false;
  }
}

// 远程搜索：检查（按检查号/患者名）
async function searchExaminations(query: string) {
  if (!query) {
    examOptions.value = [];
    return;
  }
  examLoading.value = true;
  try {
    // 这里用后端列表接口进行模糊查询（第三个参数为日期，这里传空字符串）
    const res: any = await getExaminationList(query, query, '', 1, 20);
    if (res && res.response) examOptions.value = res.response;
  } catch (e) {
    console.error('搜索检查失败:', e);
  } finally {
    examLoading.value = false;
  }
}

// 保存打印记录
async function handleSave() {
  try {
    // 规范化提交数据，确保类型正确，并使用 ISO 日期格式
    const payload: any = {
      id: editForm.id || 0,
      exam_id: Number(editForm.exam_id),
      patient_id: Number(editForm.patient_id),
      printed_by: Number(editForm.printed_by)
    };
    if (editForm.print_time) {
      payload.print_time = new Date(editForm.print_time).toISOString();
    }
    if (editForm.id) {
      // 编辑
      await editPrintRecordApi(payload);
      ElMessage.success('编辑成功');
    } else {
      // 新增
      await addPrintRecordApi(payload);
      ElMessage.success('新增成功');
    }
    fetchPrintRecordList();
    closeEditModal();
  } catch (error) {
    console.error('保存失败:', error);
    ElMessage.error('保存失败');
  }
}

// 删除打印记录
async function handleDelete(record: any) {
  try {
    await deletePrintRecordApi([record.id]);
    ElMessage.success('删除成功');
    fetchPrintRecordList();
  } catch (error) {
    console.error('删除失败:', error);
    ElMessage.error('删除失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  try {
    const ids = selectedRows.value.map(item => item.id);
    await deletePrintRecordApi(ids);
    ElMessage.success('批量删除成功');
    fetchPrintRecordList();
  } catch (error) {
    console.error('批量删除失败:', error);
    ElMessage.error('批量删除失败');
  }
}

// 打开编辑弹窗
function openEditModal(record?: any) {
  showEditModal.value = true;
  if (record) {
    Object.keys(record).forEach(key => {
      editForm[key] = record[key];
    });
    // 预填可选项
    if (record.holPatient) {
      patientOptions.value = [record.holPatient];
    }
    if (record.holExamination) {
      examOptions.value = [record.holExamination];
    }
  } else {
    // 重置表单
    Object.keys(editForm).forEach(k => editForm[k] = '');
    editForm.print_time = new Date().toISOString().slice(0, 19).replace('T', ' ');
    patientOptions.value = [];
    examOptions.value = [];
  }
}

// 关闭编辑弹窗
function closeEditModal() {
  showEditModal.value = false;
  Object.keys(editForm).forEach(k => editForm[k] = '');
}

// 处理选择变化
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection;
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchPrintRecordList(false);
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchPrintRecordList(false);
}

onMounted(() => {
  fetchPrintRecordList(false);
});

// 显示患者信息
function showPatient(patient: any) {
  currentPatient.value = patient;
  showPatientModal.value = true;
}

// 显示检查信息
function showExam(exam: any) {
  currentExam.value = exam;
  showExamModal.value = true;
}
</script>

<style scoped>
.print-record-management-container {
  padding: 10px;
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
  width: 180px;
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
  background-color: #22a2b6 !important;
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

.print-record-table-view {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  padding: 0 0 16px 0;
}

.table-list {
  margin-bottom: 20px;
}

.pagination-bar {
  text-align: right;
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
