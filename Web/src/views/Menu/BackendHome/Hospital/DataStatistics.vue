<template>
  <div class="data-statistics-container">
    <div class="page-header">
      <h2>数据统计</h2>
      <div class="filter-bar">
        <el-date-picker
          v-model="dateRange"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          format="YYYY-MM-DD"
          value-format="YYYY-MM-DD"
          @change="onDateRangeChange"
        />
        <el-button type="primary" @click="refreshData">刷新数据</el-button>
      </div>
    </div>

    <!-- 概览卡片 -->
    <div class="overview-cards">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-card class="stat-card">
            <div class="card-content">
              <div class="card-icon patient">
                <i class="el-icon-user"></i>
              </div>
              <div class="card-info">
                <div class="card-title">总患者数</div>
                <div class="card-value">{{ statistics.totalPatients }}</div>
                <div class="card-trend">
                  <span class="trend-text">较昨日</span>
                  <span :class="statistics.patientTrend >= 0 ? 'trend-up' : 'trend-down'">
                    {{ statistics.patientTrend >= 0 ? '+' : '' }}{{ statistics.patientTrend }}
                  </span>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card">
            <div class="card-content">
              <div class="card-icon examination">
                <i class="el-icon-document"></i>
              </div>
              <div class="card-info">
                <div class="card-title">总检查数</div>
                <div class="card-value">{{ statistics.totalExaminations }}</div>
                <div class="card-trend">
                  <span class="trend-text">较昨日</span>
                  <span :class="statistics.examinationTrend >= 0 ? 'trend-up' : 'trend-down'">
                    {{ statistics.examinationTrend >= 0 ? '+' : '' }}{{ statistics.examinationTrend }}
                  </span>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card">
            <div class="card-content">
              <div class="card-icon doctor">
                <i class="el-icon-user-solid"></i>
              </div>
              <div class="card-info">
                <div class="card-title">总医生数</div>
                <div class="card-value">{{ statistics.totalDoctors }}</div>
                <div class="card-trend">
                  <span class="trend-text">较昨日</span>
                  <span :class="statistics.doctorTrend >= 0 ? 'trend-up' : 'trend-down'">
                    {{ statistics.doctorTrend >= 0 ? '+' : '' }}{{ statistics.doctorTrend }}
                  </span>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stat-card">
            <div class="card-content">
              <div class="card-icon print">
                <i class="el-icon-printer"></i>
              </div>
              <div class="card-info">
                <div class="card-title">总打印数</div>
                <div class="card-value">{{ statistics.totalPrints }}</div>
                <div class="card-trend">
                  <span class="trend-text">较昨日</span>
                  <span :class="statistics.printTrend >= 0 ? 'trend-up' : 'trend-down'">
                    {{ statistics.printTrend >= 0 ? '+' : '' }}{{ statistics.printTrend }}
                  </span>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 图表区域 -->
    <div class="charts-section">
      <el-row :gutter="20">
        <!-- 患者性别分布 -->
        <el-col :span="12">
          <el-card class="chart-card">
            <template #header>
              <span>患者性别分布</span>
            </template>
            <div ref="genderChart" class="chart-container"></div>
          </el-card>
        </el-col>
        <!-- 检查类型分布 -->
        <el-col :span="12">
          <el-card class="chart-card">
            <template #header>
              <span>检查类型分布</span>
            </template>
            <div ref="examTypeChart" class="chart-container"></div>
          </el-card>
        </el-col>
      </el-row>

      <el-row :gutter="20" style="margin-top: 20px;">
        <!-- 科室患者分布 -->
        <el-col :span="12">
          <el-card class="chart-card">
            <template #header>
              <span>科室患者分布</span>
            </template>
            <div ref="departmentChart" class="chart-container"></div>
          </el-card>
        </el-col>
        <!-- 检查趋势 -->
        <el-col :span="12">
          <el-card class="chart-card">
            <template #header>
              <span>检查趋势</span>
            </template>
            <div ref="trendChart" class="chart-container"></div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 详细统计表格 -->
    <div class="detail-section">
      <el-card>
        <template #header>
          <span>详细统计</span>
        </template>
        <el-tabs v-model="activeTab" @tab-change="onTabChange">
          <el-tab-pane label="患者统计" name="patient">
            <el-table :data="patientStats" stripe border>
              <el-table-column prop="department" label="科室" align="center" />
              <el-table-column prop="totalPatients" label="总患者数" align="center" />
              <el-table-column prop="malePatients" label="男性患者" align="center" />
              <el-table-column prop="femalePatients" label="女性患者" align="center" />
              <el-table-column prop="todayPatients" label="今日新增" align="center" />
            </el-table>
          </el-tab-pane>
          <el-tab-pane label="检查统计" name="examination">
            <el-table :data="examinationStats" stripe border>
              <el-table-column prop="examType" label="检查类型" align="center" />
              <el-table-column prop="totalCount" label="总检查数" align="center" />
              <el-table-column prop="printedCount" label="已打印" align="center" />
              <el-table-column prop="unprintedCount" label="未打印" align="center" />
              <el-table-column prop="todayCount" label="今日检查" align="center" />
            </el-table>
          </el-tab-pane>
          <el-tab-pane label="医生统计" name="doctor">
            <el-table :data="doctorStats" stripe border>
              <el-table-column prop="department" label="科室" align="center" />
              <el-table-column prop="totalDoctors" label="总医生数" align="center" />
              <el-table-column prop="attendingDoctors" label="主治医师" align="center" />
              <el-table-column prop="residentDoctors" label="住院医师" align="center" />
              <el-table-column prop="chiefDoctors" label="主任医师" align="center" />
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </el-card>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import * as echarts from 'echarts'
import { getPatientList } from '../../../../api/patient'
import { getExaminationList } from '../../../../api/examination'
import { getDoctorList } from '../../../../api/doctor'
import { getDepartmentList } from '../../../../api/department'
import { getPrintRecordList } from '../../../../api/printRecord'

// 响应式数据
const loading = ref(false)
const dateRange = ref<[string, string]>(['', ''])
const activeTab = ref('patient')

// 统计数据
const statistics = reactive({
  totalPatients: 0,
  totalExaminations: 0,
  totalDoctors: 0,
  totalPrints: 0,
  patientTrend: 0,
  examinationTrend: 0,
  doctorTrend: 0,
  printTrend: 0
})

// 详细统计数据
const patientStats = ref<any[]>([])
const examinationStats = ref<any[]>([])
const doctorStats = ref<any[]>([])

// 图表引用
const genderChart = ref()
const examTypeChart = ref()
const departmentChart = ref()
const trendChart = ref()

// 初始化日期范围（最近30天）
onMounted(() => {
  const endDate = new Date()
  const startDate = new Date()
  startDate.setDate(startDate.getDate() - 30)
  
  dateRange.value = [
    startDate.toISOString().split('T')[0],
    endDate.toISOString().split('T')[0]
  ]
  
  fetchAllData()
})

// 获取所有统计数据
async function fetchAllData() {
  loading.value = true
  try {
    await Promise.all([
      fetchPatientStatistics(),
      fetchExaminationStatistics(),
      fetchDoctorStatistics(),
      fetchPrintStatistics()
    ])
    
    // 等待DOM更新后初始化图表
    await nextTick()
    initCharts()
  } catch (error) {
    console.error('获取统计数据失败:', error)
    ElMessage.error('获取统计数据失败')
  } finally {
    loading.value = false
  }
}

// 获取患者统计
async function fetchPatientStatistics() {
  try {
    const res: any = await getPatientList('', '', 1, 10000)
    if (res && res.response) {
      const patients = res.response
      statistics.totalPatients = patients.length
      
      // 计算性别分布
      const genderData = patients.reduce((acc: any, patient: any) => {
        acc[patient.gender] = (acc[patient.gender] || 0) + 1
        return acc
      }, {})
      
      // 计算科室分布
      const departmentData = patients.reduce((acc: any, patient: any) => {
        const dept = patient.department || '未知科室'
        acc[dept] = (acc[dept] || 0) + 1
        return acc
      }, {})
      
      // 生成患者统计表格数据
      patientStats.value = Object.entries(departmentData).map(([department, count]) => ({
        department,
        totalPatients: count,
        malePatients: patients.filter((p: any) => p.department === department && p.gender === 1).length,
        femalePatients: patients.filter((p: any) => p.department === department && p.gender === 2).length,
        todayPatients: patients.filter((p: any) => p.department === department && isToday(p.createtime)).length
      }))
    }
  } catch (error) {
    console.error('获取患者统计失败:', error)
  }
}

// 获取检查统计
async function fetchExaminationStatistics() {
  try {
    const res: any = await getExaminationList('', '', '', 1, 10000)
    if (res && res.response) {
      const examinations = res.response
      statistics.totalExaminations = examinations.length
      
      // 计算检查类型分布
      const examTypeData = examinations.reduce((acc: any, exam: any) => {
        acc[exam.exam_type] = (acc[exam.exam_type] || 0) + 1
        return acc
      }, {})
      
      // 生成检查统计表格数据
      examinationStats.value = Object.entries(examTypeData).map(([examType, count]) => ({
        examType,
        totalCount: count,
        printedCount: examinations.filter((e: any) => e.exam_type === examType && e.is_printed === 1).length,
        unprintedCount: examinations.filter((e: any) => e.exam_type === examType && e.is_printed === 0).length,
        todayCount: examinations.filter((e: any) => e.exam_type === examType && isToday(e.exam_date)).length
      }))
    }
  } catch (error) {
    console.error('获取检查统计失败:', error)
  }
}

// 获取医生统计
async function fetchDoctorStatistics() {
  try {
    const res: any = await getDoctorList('', null, 1, 10000)
    if (res && res.response) {
      const doctors = res.response
      statistics.totalDoctors = doctors.length
      
      // 计算科室分布
      const departmentData = doctors.reduce((acc: any, doctor: any) => {
        const dept = doctor.department_name || '未知科室'
        acc[dept] = (acc[dept] || 0) + 1
        return acc
      }, {})
      
      // 生成医生统计表格数据
      doctorStats.value = Object.entries(departmentData).map(([department, count]) => ({
        department,
        totalDoctors: count,
        attendingDoctors: doctors.filter((d: any) => d.department_name === department && d.title?.includes('主治')).length,
        residentDoctors: doctors.filter((d: any) => d.department_name === department && d.title?.includes('住院')).length,
        chiefDoctors: doctors.filter((d: any) => d.department_name === department && d.title?.includes('主任')).length
      }))
    }
  } catch (error) {
    console.error('获取医生统计失败:', error)
  }
}

// 获取打印统计
async function fetchPrintStatistics() {
  try {
    const res: any = await getPrintRecordList(null, 1, 10000)
    if (res && res.response) {
      statistics.totalPrints = res.response.length
    }
  } catch (error) {
    console.error('获取打印统计失败:', error)
  }
}

// 初始化图表
function initCharts() {
  initGenderChart()
  initExamTypeChart()
  initDepartmentChart()
  initTrendChart()
}

// 患者性别分布图表
function initGenderChart() {
  if (!genderChart.value) return
  const chart = echarts.init(genderChart.value)
  const maleCount = patientStats.value.reduce((sum, item) => sum + item.malePatients, 0)
  const femaleCount = patientStats.value.reduce((sum, item) => sum + item.femalePatients, 0)
  chart.setOption({
    tooltip: { trigger: 'item' },
    legend: { bottom: 0 },
    series: [
      {
        name: '患者性别',
        type: 'pie',
        radius: ['50%', '70%'],
        avoidLabelOverlap: false,
        label: { show: false },
        emphasis: { label: { show: true, fontSize: 14, fontWeight: 'bold' } },
        data: [
          { value: maleCount, name: '男性' },
          { value: femaleCount, name: '女性' }
        ]
      }
    ]
  })
}

// 检查类型分布图表
function initExamTypeChart() {
  if (!examTypeChart.value) return
  const chart = echarts.init(examTypeChart.value)
  chart.setOption({
    tooltip: { trigger: 'item' },
    legend: { bottom: 0 },
    series: [
      {
        name: '检查类型',
        type: 'pie',
        radius: ['50%', '70%'],
        data: examinationStats.value.map(item => ({ value: item.totalCount, name: item.examType }))
      }
    ]
  })
}

// 科室患者分布图表
function initDepartmentChart() {
  if (!departmentChart.value) return
  const chart = echarts.init(departmentChart.value)
  chart.setOption({
    tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' } },
    grid: { left: '3%', right: '4%', bottom: '8%', containLabel: true },
    xAxis: { type: 'category', data: patientStats.value.map(item => item.department), axisLabel: { rotate: 45 } },
    yAxis: { type: 'value' },
    series: [
      { name: '患者数', type: 'bar', data: patientStats.value.map(item => item.totalPatients), itemStyle: { color: '#409EFF' } }
    ]
  })
}

// 检查趋势图表
function initTrendChart() {
  if (!trendChart.value) return
  const chart = echarts.init(trendChart.value)
  chart.setOption({
    tooltip: { trigger: 'axis' },
    grid: { left: '3%', right: '4%', bottom: '8%', containLabel: true },
    xAxis: { type: 'category', data: generateDateRange() },
    yAxis: { type: 'value' },
    series: [
      { name: '检查数', type: 'line', smooth: true, data: generateTrendData(), lineStyle: { color: '#67C23A' }, areaStyle: { color: 'rgba(103, 194, 58, 0.1)' } }
    ]
  })
}

// 生成日期范围
function generateDateRange() {
  const dates = []
  const start = new Date(dateRange.value[0])
  const end = new Date(dateRange.value[1])
  
  for (let d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
    dates.push(d.toISOString().split('T')[0])
  }
  return dates
}

// 生成趋势数据（模拟数据）
function generateTrendData() {
  const dates = generateDateRange()
  return dates.map(() => Math.floor(Math.random() * 50) + 10)
}

// 判断是否为今天
function isToday(dateStr: string) {
  if (!dateStr) return false
  const date = new Date(dateStr)
  const today = new Date()
  return date.toDateString() === today.toDateString()
}

// 日期范围变化
function onDateRangeChange() {
  fetchAllData()
}

// 刷新数据
function refreshData() {
  fetchAllData()
  ElMessage.success('数据已刷新')
}

// 标签页切换
function onTabChange() {
  // 可以在这里添加标签页切换时的逻辑
}
</script>

<style scoped>
.data-statistics-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  color: #303133;
}

.filter-bar {
  display: flex;
  gap: 10px;
  align-items: center;
}

.overview-cards {
  margin-bottom: 30px;
}

.stat-card {
  height: 120px;
}

.card-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.card-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  font-size: 24px;
  color: white;
}

.card-icon.patient {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.card-icon.examination {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.card-icon.doctor {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.card-icon.print {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
}

.card-info {
  flex: 1;
}

.card-title {
  font-size: 14px;
  color: #909399;
  margin-bottom: 5px;
}

.card-value {
  font-size: 28px;
  font-weight: bold;
  color: #303133;
  margin-bottom: 5px;
}

.card-trend {
  font-size: 12px;
}

.trend-text {
  color: #909399;
  margin-right: 5px;
}

.trend-up {
  color: #67C23A;
}

.trend-down {
  color: #F56C6C;
}

.charts-section {
  margin-bottom: 30px;
}

.chart-card {
  height: 400px;
}

.chart-container {
  height: 300px;
  width: 100%;
}

.detail-section {
  margin-bottom: 20px;
}
</style>
