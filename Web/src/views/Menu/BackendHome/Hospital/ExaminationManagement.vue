<template>
  <div class="examination-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchExamNo" placeholder="搜索检查号" class="search-input" @keyup.enter="onSearch" />
          <el-input v-model="searchPatientName" placeholder="搜索患者姓名" class="search-input" @keyup.enter="onSearch" />
          <el-date-picker
            v-model="searchExamDate"
            type="date"
            placeholder="选择检查日期"
            class="search-input"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="onSearch"
          />
          <el-button class="search-btn" @click="onSearch">搜索</el-button>
        </div>
        <div class="action-buttons">
          <el-button class="primary-btn" @click="openEditModal()">新增检查</el-button>
          <el-button class="danger-btn" :disabled="!selectedRows.length" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>
    </div>
    <div class="examination-table-view">
      <div class="table-list">
        <el-table
          :data="examinationList"
          border
          style="width: 100%;height: 65vh;"
          :header-cell-style="{ background: '#f8f9fa', color: '#606266' }"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
          <el-table-column label="检查号" prop="exam_no" align="center" />
          <el-table-column label="患者姓名" align="center">
            <template #default="scope">
              <el-button
                v-if="scope.row.patient && scope.row.patient.name"
                type="text"
                style="color:#409eff;"
                @click="showPatient(scope.row.patient)"
              >
                {{ scope.row.patient.name }}
              </el-button>
              <span v-else>--</span>
            </template>
          </el-table-column>
          <el-table-column label="诊断医生" align="center">
            <template #default="scope">
              <el-button
                v-if="scope.row.doctor && scope.row.doctor.name"
                type="text"
                style="color:#409eff;"
                @click="showDoctor(scope.row.doctor)"
              >
                {{ scope.row.doctor.name }}
              </el-button>
              <span v-else>--</span>
            </template>
          </el-table-column>
          <el-table-column label="检查类型" prop="exam_type" align="center">
            <template #default="scope">
              <el-tag :type="getExamTypeTagType(scope.row.exam_type)">
                {{ scope.row.exam_type }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="检查日期" prop="exam_date" align="center" :formatter="formatDate" />
          <el-table-column label="报告文件" prop="report_path" align="center">
            <template #default="scope">
              <el-button 
                v-if="scope.row.report_path" 
                type="text" 
                style="color: #67c23a;" 
                @click="viewReport(scope.row)"
              >
                查看报告
              </el-button>
              <span v-else>--</span>
            </template>
          </el-table-column>
          <el-table-column label="报告编号" prop="report_no" align="center" :formatter="(row)=> row.report_no || '--'" />
          <el-table-column label="电子胶片" prop="image_path" align="center">
            <template #default="scope">
              <el-button 
                v-if="scope.row.image_path" 
                type="text" 
                style="color: #67c23a;" 
                @click="viewImage(scope.row)"
              >
                查看胶片
              </el-button>
              <span v-else>--</span>
            </template>
          </el-table-column>
          <el-table-column label="胶片检查号" prop="image_no" align="center" :formatter="(row)=> row.image_no || '--'" />
          <el-table-column label="打印状态" prop="is_printed" align="center">
            <template #default="scope">
              <el-tag :type="scope.row.is_printed === 1 ? 'success' : 'info'">
                {{ scope.row.is_printed === 1 ? '已打印' : '未打印' }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="创建时间" prop="create_time" align="center" :formatter="formatDateTime" />
          <el-table-column label="操作" align="center" width="300">
            <template #default="scope">
              <el-button type="text" style="color: #67c23a;" @click="openEditModal(scope.row)">编辑</el-button>
              <el-button 
                v-if="scope.row.is_printed !== 1" 
                type="text" 
                style="color: #409eff;" 
                @click="handlePrint(scope.row)"
              >
                打印
              </el-button>
              <el-button 
                v-if="scope.row.is_printed === 1" 
                type="text" 
                style="color: #e6a23c;" 
                @click="handleUnlockPrint(scope.row)"
              >
                解锁
              </el-button>
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
    <el-dialog v-model="showEditModal" width="600" :title="editForm.id ? '编辑检查记录' : '新增检查记录'">
      <el-form :model="editForm" label-width="120px">
        <el-form-item label="检查号" required>
          <el-input v-model="editForm.exam_no" placeholder="请输入检查号" />
        </el-form-item>
        <el-form-item label="患者" required>
          <el-select 
            v-model="editForm.patient_id" 
            placeholder="请选择患者" 
            filterable
            remote
            :remote-method="searchPatients"
            :loading="patientLoading"
            style="width: 100%"
          >
            <el-option
              v-for="patient in patientOptions"
              :key="patient.id"
              :label="`${patient.name} (${patient.medical_no || '无就诊号'})`"
              :value="patient.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="诊断医生" required>
          <el-select v-model="editForm.doctor_id" placeholder="请选择医生" filterable remote :remote-method="searchDoctors" :loading="doctorLoading" style="width: 100%">
            <el-option v-for="d in doctorOptions" :key="d.id" :label="d.name" :value="d.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="检查类型" required>
          <el-select v-model="editForm.exam_type" placeholder="请选择检查类型">
            <el-option label="CT" value="CT" />
            <el-option label="MRI" value="MRI" />
            <el-option label="超声" value="超声" />
            <el-option label="X光" value="X光" />
            <el-option label="心电图" value="心电图" />
            <el-option label="其他" value="其他" />
          </el-select>
        </el-form-item>
        <el-form-item label="检查日期" required>
          <el-date-picker
            v-model="editForm.exam_date"
            type="datetime"
            placeholder="选择检查日期时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="报告文件路径">
          <el-input v-model="editForm.report_path" placeholder="请输入报告文件路径" />
        </el-form-item>
        <el-form-item label="电子胶片路径">
          <el-input v-model="editForm.image_path" placeholder="请输入电子胶片路径" />
        </el-form-item>
        <el-form-item label="报告编号">
          <el-input v-model="editForm.report_no" placeholder="请输入报告编号" />
        </el-form-item>
        <el-form-item label="胶片检查号">
          <el-input v-model="editForm.image_no" placeholder="请输入胶片检查号" />
        </el-form-item>
        <!-- 隐藏字段，确保数据完整性 -->
        <el-form-item v-show="false">
          <el-input v-model="editForm.org_id" />
          <el-input v-model="editForm.status" />
          <el-input v-model="editForm.is_printed" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button class="cancel-btn" @click="closeEditModal">取消</el-button>
        <el-button type="primary" class="Btn-Save" @click="handleSave">保存</el-button>
      </template>
    </el-dialog>

    <!-- 报告查看弹窗 -->
    <el-dialog v-model="showReportModal" title="检查报告" width="80%" :close-on-click-modal="false">
      <div class="report-viewer">
        <iframe 
          v-if="currentReportPath" 
          :src="currentReportPath" 
          width="100%" 
          height="600px"
          frameborder="0"
        ></iframe>
        <div v-else class="no-content">暂无报告文件</div>
      </div>
      <template #footer>
        <el-button @click="showReportModal = false">关闭</el-button>
      </template>
    </el-dialog>

    <!-- 胶片查看弹窗 -->
    <el-dialog v-model="showImageModal" title="电子胶片" width="80%" :close-on-click-modal="false">
      <div class="image-viewer">
        <img 
          v-if="currentImagePath" 
          :src="currentImagePath" 
          alt="电子胶片"
          style="max-width: 100%; height: auto;"
        />
        <div v-else class="no-content">暂无胶片文件</div>
      </div>
      <template #footer>
        <el-button @click="showImageModal = false">关闭</el-button>
      </template>
    </el-dialog>

    <!-- 医生信息弹窗 -->
    <el-dialog v-model="showDoctorModal" title="医生信息" width="500px" :close-on-click-modal="false">
      <div v-if="currentDoctor" class="doctor-info">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="姓名">{{ currentDoctor.name || '--' }}</el-descriptions-item>
          <el-descriptions-item label="性别">{{ currentDoctor.gender === 1 ? '男' : (currentDoctor.gender === 2 ? '女' : '未知') }}</el-descriptions-item>
          <el-descriptions-item label="联系电话">{{ currentDoctor.phone || '--' }}</el-descriptions-item>
          <el-descriptions-item label="职称">{{ currentDoctor.title || '--' }}</el-descriptions-item>
          <el-descriptions-item label="简介">{{ currentDoctor.introduction || '--' }}</el-descriptions-item>
          <el-descriptions-item label="所属科室">{{ currentDoctor.holdepartment?.name || currentDoctor.department_name || '--' }}</el-descriptions-item>
        </el-descriptions>
      </div>
      <template #footer>
        <el-button type="primary" @click="showDoctorModal = false">关闭</el-button>
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
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import { ElSelect, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElTag, ElDialog, ElForm, ElFormItem, ElMessage, ElDatePicker } from 'element-plus';
import { addExaminationApi, deleteExaminationApi, editExaminationApi, getExaminationList, printExaminationApi, unlockPrintApi } from '../../../../api/examination';
import { getPatientList } from '../../../../api/patient';
import { getDoctorList } from '../../../../api/doctor';

const searchExamNo = ref('');
const searchPatientName = ref('');
const searchExamDate = ref('');
const examinationList = ref<any[]>([]);
const pageIndex = ref(1);
const pageSize = ref(10);
const showEditModal = ref(false);
const editForm = reactive<any>({});
const total = ref(0);
const selectedRows = ref<any[]>([]);

// 患者选择相关
const patientOptions = ref<any[]>([]);
const patientLoading = ref(false);
// 医生选择相关
const doctorOptions = ref<any[]>([]);
const doctorLoading = ref(false);

// 报告和胶片查看
const showReportModal = ref(false);
const showImageModal = ref(false);
const currentReportPath = ref('');
const currentImagePath = ref('');
// 医生详情
const showDoctorModal = ref(false);
const currentDoctor = ref<any>(null);
// 患者详情
const showPatientModal = ref(false);
const currentPatient = ref<any>(null);

// 格式化日期
const formatDate = (row: any, column: any, cellValue: any) => {
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

// 获取检查类型标签类型
const getExamTypeTagType = (examType: string) => {
  const typeMap: { [key: string]: 'primary' | 'success' | 'warning' | 'info' | 'danger' } = {
    'CT': 'primary',
    'MRI': 'success',
    '超声': 'warning',
    'X光': 'info',
    '心电图': 'danger',
    '其他': 'info'
  };
  return typeMap[examType] || 'info';
};

// 获取检查列表
async function fetchExaminationList(showTip: boolean = false) {
  await getExaminationList(
    searchExamNo.value, 
    searchPatientName.value, 
    searchExamDate.value, 
    pageIndex.value, 
    pageSize.value
  )
    .then((res: any) => {
      if (res && res.response) {
        examinationList.value = res.response;
        total.value = res.count || 0;
        if (showTip) {
          ElMessage.success(`查询成功，匹配到 ${total.value} 条记录`);
        }
      }
    })
    .catch((error) => {
      console.error('获取检查列表失败:', error);
      ElMessage.error('获取检查列表失败');
    });
}

// 主动搜索（重置到第1页并提示）
function onSearch() {
  pageIndex.value = 1;
  fetchExaminationList(true);
}

// 搜索患者
async function searchPatients(query: string) {
  if (!query) {
    patientOptions.value = [];
    return;
  }
  patientLoading.value = true;
  try {
    await getPatientList(query, '', 1, 20).then((res: any) => {
      if (res && res.response) {
        patientOptions.value = res.response;
      }
    });
  } catch (error) {
    console.error('搜索患者失败:', error);
  } finally {
    patientLoading.value = false;
  }
}

// 搜索医生
async function searchDoctors(query: string) {
  if (!query) {
    doctorOptions.value = [];
    return;
  }
  doctorLoading.value = true;
  try {
    const res: any = await getDoctorList(query, null, 1, 20);
    if (res && res.response) doctorOptions.value = res.response;
  } catch (error) {
    console.error('搜索医生失败:', error);
  } finally {
    doctorLoading.value = false;
  }
}

// 保存检查记录
async function handleSave() {
  try {
    // 验证必填字段
    if (!editForm.exam_no || editForm.exam_no.trim() === '') {
      ElMessage.error('检查号不能为空');
      return;
    }
    
    if (!editForm.patient_id || editForm.patient_id <= 0) {
      ElMessage.error('请选择患者');
      return;
    }
    
    if (!editForm.exam_type || editForm.exam_type.trim() === '') {
      ElMessage.error('请选择检查类型');
      return;
    }
    
    if (!editForm.doctor_id || editForm.doctor_id <= 0) {
      ElMessage.error('请选择诊断医生');
      return;
    }
    
    if (!editForm.exam_date || editForm.exam_date.trim() === '') {
      ElMessage.error('请选择检查日期');
      return;
    }

    // 找到选中的患者对象
    const selectedPatient = patientOptions.value.find(p => p.id === editForm.patient_id);
    if (!selectedPatient) {
      ElMessage.error('请选择患者');
      return;
    }

    // 准备发送给后端的数据，确保所有字段都存在且类型正确
    const formData = {
      id: editForm.id || 0,
      exam_no: editForm.exam_no.trim(),
      patient_id: Number(editForm.patient_id),
      patient: selectedPatient, // 添加完整的患者对象
      doctor_id: Number(editForm.doctor_id),
      org_id: Number(editForm.org_id) || 1,
      exam_type: editForm.exam_type.trim(),
      exam_date: new Date(editForm.exam_date).toISOString(), // 转换为ISO格式
      report_path: editForm.report_path || null,
      image_path: editForm.image_path || null,
      report_no: editForm.report_no || null,
      image_no: editForm.image_no || null,
      status: Number(editForm.status) || 1,
      is_printed: Number(editForm.is_printed) || 0
      // 注意：create_time 和 update_time 由后端设置，不需要前端发送
    };

    console.log('发送的数据:', JSON.stringify(formData, null, 2));

    if (editForm.id) {
      // 编辑
      await editExaminationApi(formData);
      ElMessage.success('编辑成功');
    } else {
      // 新增
      await addExaminationApi(formData);
      ElMessage.success('新增成功');
    }
    fetchExaminationList();
    closeEditModal();
  } catch (error: any) {
    console.error('保存失败:', error);
    console.error('错误详情:', error.response?.data);
    ElMessage.error('保存失败: ' + (error.response?.data?.message || error.message || '未知错误'));
  }
}

// 删除检查记录
async function handleDelete(examination: any) {
  try {
    await deleteExaminationApi([examination.id]);
    ElMessage.success('删除成功');
    fetchExaminationList();
  } catch (error) {
    console.error('删除失败:', error);
    ElMessage.error('删除失败');
  }
}

// 批量删除
async function handleBatchDelete() {
  try {
    const ids = selectedRows.value.map(item => item.id);
    await deleteExaminationApi(ids);
    ElMessage.success('批量删除成功');
    fetchExaminationList();
  } catch (error) {
    console.error('批量删除失败:', error);
    ElMessage.error('批量删除失败');
  }
}

// 工具：标准化URL与扩展名判断
function normalizeUrl(path: string) {
  if (!path) return '';
  const url = path.replace(/\\/g, '/');
  return encodeURI(url);
}
function getExt(path: string) {
  const m = path?.toLowerCase().match(/\.([a-z0-9]+)(?:\?|#|$)/);
  return m ? m[1] : '';
}
function isArchive(ext: string) {
  return ['zip', 'rar', '7z'].includes(ext);
}

// 查看报告：压缩包直接下载，否则弹窗预览（如 pdf/html 等）
function viewReport(row: any) {
  const url = normalizeUrl(row.report_path);
  const ext = getExt(url);
  if (isArchive(ext)) {
    const a = document.createElement('a');
    a.href = url;
    a.download = '';
    a.target = '_blank';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    return;
  }
  currentReportPath.value = url;
  showReportModal.value = true;
}

// 查看胶片：压缩包直接下载，否则弹窗预览图片
function viewImage(row: any) {
  const url = normalizeUrl(row.image_path);
  const ext = getExt(url);
  if (isArchive(ext)) {
    const a = document.createElement('a');
    a.href = url;
    a.download = '';
    a.target = '_blank';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    return;
  }
  currentImagePath.value = url;
  showImageModal.value = true;
}

// 打印检查报告
async function handlePrint(examination: any) {
  try {
    await printExaminationApi(examination.id);
    ElMessage.success('打印成功');
    fetchExaminationList();
  } catch (error) {
    console.error('打印失败:', error);
    ElMessage.error('打印失败');
  }
}

// 解锁打印状态
async function handleUnlockPrint(examination: any) {
  try {
    await unlockPrintApi(examination.id);
    ElMessage.success('解锁成功，可重新打印');
    fetchExaminationList();
  } catch (error) {
    console.error('解锁失败:', error);
    ElMessage.error('解锁失败');
  }
}

// 打开编辑弹窗
function openEditModal(examination?: any) {
  showEditModal.value = true;
  if (examination) {
    Object.keys(examination).forEach(key => {
      editForm[key] = examination[key];
    });
    // 设置患者选项
    if (examination.patient) {
      patientOptions.value = [examination.patient];
    }
    if (examination.doctor) {
      doctorOptions.value = [examination.doctor];
    }
  } else {
    // 重置表单，设置默认值
    editForm.id = '';
    editForm.exam_no = '';
    editForm.patient_id = null;
    editForm.org_id = 1; // 设置默认机构ID
    editForm.exam_type = '';
    editForm.exam_date = new Date().toISOString().slice(0, 19).replace('T', ' ');
    editForm.report_path = '';
    editForm.image_path = '';
    editForm.report_no = '';
    editForm.image_no = '';
    editForm.status = 1;
    editForm.is_printed = 0;
    patientOptions.value = [];
    editForm.doctor_id = null;
    doctorOptions.value = [];
  }
}

// 关闭编辑弹窗
function closeEditModal() {
  showEditModal.value = false;
  // 重置表单
  editForm.id = '';
  editForm.exam_no = '';
  editForm.patient_id = null;
  editForm.doctor_id = null;
  editForm.org_id = 1;
  editForm.exam_type = '';
  editForm.exam_date = '';
  editForm.report_path = '';
  editForm.image_path = '';
  editForm.report_no = '';
  editForm.image_no = '';
  editForm.status = 1;
  editForm.is_printed = 0;
  patientOptions.value = [];
  doctorOptions.value = [];
}

// 处理选择变化
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection;
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchExaminationList(false);
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchExaminationList(false);
}

onMounted(() => {
  fetchExaminationList(false);
});

// 显示医生信息
function showDoctor(doctor: any) {
  currentDoctor.value = doctor;
  showDoctorModal.value = true;
}

// 显示患者信息
function showPatient(patient: any) {
  currentPatient.value = patient;
  showPatientModal.value = true;
}
</script>

<style scoped>
.examination-management-container {
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

.examination-table-view {
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

.report-viewer, .image-viewer {
  text-align: center;
}

.no-content {
  padding: 40px;
  color: #999;
  font-size: 16px;
}
</style>
