<template>
  <div class="examination-management-container">
    <div class="page-header">
      <div class="operation-bar">
        <div class="filter-group">
          <el-input v-model="searchExamNo" placeholder="搜索检查号" class="search-input" @keyup.enter="fetchExaminationList" />
          <el-input v-model="searchPatientName" placeholder="搜索患者姓名" class="search-input" @keyup.enter="fetchExaminationList" />
          <el-date-picker
            v-model="searchExamDate"
            type="date"
            placeholder="选择检查日期"
            class="search-input"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            @change="fetchExaminationList"
          />
          <el-button class="search-btn" @click="fetchExaminationList">搜索</el-button>
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
          <el-table-column label="患者姓名" prop="patient.name" align="center" />
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
          layout="prev, pager, next, ->, sizes, jumper"
          :total="total"
          :page-size="pageSize"
          :current-page="pageIndex"
          :prev-text="'<'"
          :next-text="'>'"
          :page-sizes="[10, 20, 30, 40, 50]"
          :display-page-count="5"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
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
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue';
import { ElSelect, ElInput, ElButton, ElTable, ElTableColumn, ElPagination, ElTag, ElDialog, ElForm, ElFormItem, ElMessage, ElDatePicker } from 'element-plus';
import { addExaminationApi, deleteExaminationApi, editExaminationApi, getExaminationList, printExaminationApi, unlockPrintApi } from '../../../../api/examination';
import { getPatientList } from '../../../../api/patient';

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

// 报告和胶片查看
const showReportModal = ref(false);
const showImageModal = ref(false);
const currentReportPath = ref('');
const currentImagePath = ref('');

// 格式化日期
const formatDate = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  return new Date(cellValue).toLocaleDateString('zh-CN');
};

// 格式化日期时间
const formatDateTime = (row: any, column: any, cellValue: any) => {
  if (!cellValue) return '--';
  return new Date(cellValue).toLocaleString('zh-CN');
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
async function fetchExaminationList() {
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
      }
    })
    .catch((error) => {
      console.error('获取检查列表失败:', error);
      ElMessage.error('获取检查列表失败');
    });
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
      org_id: Number(editForm.org_id) || 1,
      exam_type: editForm.exam_type.trim(),
      exam_date: new Date(editForm.exam_date).toISOString(), // 转换为ISO格式
      report_path: editForm.report_path || null,
      image_path: editForm.image_path || null,
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

// 查看报告
function viewReport(row: any) {
  currentReportPath.value = row.report_path;
  showReportModal.value = true;
}

// 查看胶片
function viewImage(row: any) {
  currentImagePath.value = row.image_path;
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
    editForm.status = 1;
    editForm.is_printed = 0;
    patientOptions.value = [];
  }
}

// 关闭编辑弹窗
function closeEditModal() {
  showEditModal.value = false;
  // 重置表单
  editForm.id = '';
  editForm.exam_no = '';
  editForm.patient_id = null;
  editForm.org_id = 1;
  editForm.exam_type = '';
  editForm.exam_date = '';
  editForm.report_path = '';
  editForm.image_path = '';
  editForm.status = 1;
  editForm.is_printed = 0;
  patientOptions.value = [];
}

// 处理选择变化
function handleSelectionChange(selection: any[]) {
  selectedRows.value = selection;
}

// 分页处理
function handleSizeChange(val: number) {
  pageSize.value = val;
  fetchExaminationList();
}

function handlePageChange(val: number) {
  pageIndex.value = val;
  fetchExaminationList();
}

onMounted(() => {
  fetchExaminationList();
});
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
