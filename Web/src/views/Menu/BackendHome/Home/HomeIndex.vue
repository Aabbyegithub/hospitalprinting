<template>
  <div class="home-index-container">
    <!-- 欢迎横幅 -->
    <div class="welcome-banner">
      <div class="welcome-content">
        <div class="welcome-text">
          <h1 class="greeting">{{ greeting }}</h1>
          <p class="welcome-desc">{{ userName }}，欢迎使用医院打印管理系统</p>
          <p class="current-time">{{ currentTime }}</p>
        </div>
        <div class="welcome-decoration">
          <el-icon :size="120" color="#1D99AD">
            <HomeFilled />
          </el-icon>
        </div>
      </div>
    </div>

    <!-- 统计卡片 -->
    <div class="statistics-section">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="6" :lg="6">
          <el-card class="stat-card" shadow="hover">
            <div class="stat-content">
              <div class="stat-icon patient-icon">
                <el-icon :size="32"><UserFilled /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">患者总数</div>
                <div class="stat-value">{{ statistics.totalPatients }}</div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6" :lg="6">
          <el-card class="stat-card" shadow="hover">
            <div class="stat-content">
              <div class="stat-icon examination-icon">
                <el-icon :size="32"><Document /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">检查总数</div>
                <div class="stat-value">{{ statistics.totalExaminations }}</div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6" :lg="6">
          <el-card class="stat-card" shadow="hover">
            <div class="stat-content">
              <div class="stat-icon print-icon">
                <el-icon :size="32"><Printer /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">打印总数</div>
                <div class="stat-value">{{ statistics.totalPrints }}</div>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :md="6" :lg="6">
          <el-card class="stat-card" shadow="hover">
            <div class="stat-content">
              <div class="stat-icon doctor-icon">
                <el-icon :size="32"><Avatar /></el-icon>
              </div>
              <div class="stat-info">
                <div class="stat-label">医生总数</div>
                <div class="stat-value">{{ statistics.totalDoctors }}</div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 快速导航 -->
    <div class="quick-access-section">
      <h2 class="section-title">快速导航</h2>
      <el-row :gutter="20">
        <el-col 
          v-for="item in quickAccessItems" 
          :key="item.name"
          :xs="24" :sm="12" :md="8" :lg="6"
        >
          <el-card 
            class="quick-access-card" 
            shadow="hover"
            @click="handleQuickAccess(item.route)"
          >
            <div class="quick-access-content">
              <div class="quick-access-icon" :style="{ backgroundColor: item.color }">
                <el-icon :size="28"><component :is="item.icon" /></el-icon>
              </div>
              <div class="quick-access-text">
                <div class="quick-access-title">{{ item.title }}</div>
                <div class="quick-access-desc">{{ item.desc }}</div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- 今日概览 -->
    <div class="today-overview-section">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :lg="8">
          <el-card class="overview-card" shadow="hover">
            <template #header>
              <div class="card-header">
                <el-icon><Calendar /></el-icon>
                <span>今日数据</span>
              </div>
            </template>
            <div class="today-stats">
              <div class="today-stat-item">
                <span class="today-stat-label">新增患者</span>
                <span class="today-stat-value">{{ todayStats.newPatients }}</span>
              </div>
              <div class="today-stat-item">
                <span class="today-stat-label">新增检查</span>
                <span class="today-stat-value">{{ todayStats.newExaminations }}</span>
              </div>
              <div class="today-stat-item">
                <span class="today-stat-label">完成打印</span>
                <span class="today-stat-value">{{ todayStats.completedPrints }}</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="12" :lg="8">
          <el-card class="overview-card" shadow="hover">
            <template #header>
              <div class="card-header">
                <el-icon><Warning /></el-icon>
                <span>待处理事项</span>
              </div>
            </template>
            <div class="pending-items">
              <div 
                v-for="item in pendingItems" 
                :key="item.label"
                class="pending-item"
              >
                <span class="pending-label">{{ item.label }}</span>
                <el-badge :value="item.count" :hidden="item.count === 0" class="pending-badge">
                  <span class="pending-count">{{ item.count }}</span>
                </el-badge>
              </div>
              <div v-if="pendingItems.every(item => item.count === 0)" class="no-pending">
                <el-icon><CircleCheck /></el-icon>
                <span>暂无待处理事项</span>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :xs="24" :sm="24" :lg="8">
          <el-card class="overview-card" shadow="hover">
            <template #header>
              <div class="card-header">
                <el-icon><InfoFilled /></el-icon>
                <span>系统信息</span>
              </div>
            </template>
            <div class="system-info">
              <div class="info-item">
                <span class="info-label">系统状态：</span>
                <el-tag type="success" size="small">运行正常</el-tag>
              </div>
              <div class="info-item">
                <span class="info-label">登录时间：</span>
                <span class="info-value">{{ loginTime }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">当前角色：</span>
                <span class="info-value">{{ userRole }}</span>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  HomeFilled, 
  UserFilled, 
  Document, 
  Printer, 
  Avatar,
  Calendar,
  Warning,
  InfoFilled,
  CircleCheck,
  User,
  Tickets,
  Setting
} from '@element-plus/icons-vue'
import { getPatientList } from '../../../../api/patient'
import { getExaminationList } from '../../../../api/examination'
import { getDoctorList } from '../../../../api/doctor'
import { getPrintRecordList } from '../../../../api/printRecord'
import { ElMessage } from 'element-plus'

const router = useRouter()

// 用户信息
const userInfoStr = localStorage.getItem('UserInfo')
const userName = userInfoStr ? JSON.parse(userInfoStr).userName : '用户'
const userRole = userInfoStr ? JSON.parse(userInfoStr).role || '普通用户' : '普通用户'
const loginTime = new Date().toLocaleString('zh-CN', { 
  year: 'numeric', 
  month: '2-digit', 
  day: '2-digit', 
  hour: '2-digit', 
  minute: '2-digit' 
})

// 问候语
const greeting = ref('')
const currentTime = ref('')

function updateGreeting() {
  const hour = new Date().getHours()
  if (hour < 6) {
    greeting.value = '夜深了'
  } else if (hour < 9) {
    greeting.value = '早上好'
  } else if (hour < 12) {
    greeting.value = '上午好'
  } else if (hour < 14) {
    greeting.value = '中午好'
  } else if (hour < 18) {
    greeting.value = '下午好'
  } else if (hour < 22) {
    greeting.value = '晚上好'
  } else {
    greeting.value = '夜深了'
  }
  
  currentTime.value = new Date().toLocaleString('zh-CN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    weekday: 'long',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 统计数据
const statistics = reactive({
  totalPatients: 0,
  totalExaminations: 0,
  totalPrints: 0,
  totalDoctors: 0
})

// 今日统计
const todayStats = reactive({
  newPatients: 0,
  newExaminations: 0,
  completedPrints: 0
})

// 待处理事项
const pendingItems = reactive([
  { label: '待打印检查', count: 0 },
  { label: '待处理申请', count: 0 },
  { label: '系统通知', count: 0 }
])

// 快速导航项
const quickAccessItems = [
  {
    name: 'PatientManagement',
    title: '患者管理',
    desc: '管理患者信息',
    icon: User,
    route: '/Layout/Backendhome/PatientManagement',
    color: '#409EFF'
  },
  {
    name: 'ExaminationManagement',
    title: '检查管理',
    desc: '管理检查记录',
    icon: Document,
    route: '/Layout/Backendhome/ExaminationManagement',
    color: '#67C23A'
  },
  {
    name: 'PrintRecordManagement',
    title: '打印记录',
    desc: '查看打印历史',
    icon: Printer,
    route: '/Layout/Backendhome/PrintRecordManagement',
    color: '#E6A23C'
  },
  {
    name: 'DoctorManagement',
    title: '医生管理',
    desc: '管理医生信息',
    icon: Avatar,
    route: '/Layout/Backendhome/DoctorManagement',
    color: '#F56C6C'
  },
  {
    name: 'DataStatistics',
    title: '数据统计',
    desc: '查看数据报表',
    icon: Tickets,
    route: '/Layout/Backendhome/DataStatistics',
    color: '#909399'
  },
  {
    name: 'SystemSettings',
    title: '系统设置',
    desc: '配置系统参数',
    icon: Setting,
    route: '/Layout/Backendhome',
    color: '#1D99AD'
  }
]

// 获取统计数据
async function fetchStatistics() {
  try {
    // 获取患者总数
    const patientRes: any = await getPatientList('', '', 1, 1)
    if (patientRes) {
      statistics.totalPatients = patientRes.count || (Array.isArray(patientRes.response) ? patientRes.response.length : 0)
    }

    // 获取检查总数
    const examRes: any = await getExaminationList('', '', '', 1, 1)
    if (examRes) {
      statistics.totalExaminations = examRes.count || (Array.isArray(examRes.response) ? examRes.response.length : 0)
    }

    // 获取打印总数
    const printRes: any = await getPrintRecordList(null, 1, 1)
    if (printRes) {
      statistics.totalPrints = printRes.count || (Array.isArray(printRes.response) ? printRes.response.length : 0)
    }

    // 获取医生总数
    const doctorRes: any = await getDoctorList('', null, 1, 1)
    if (doctorRes) {
      statistics.totalDoctors = doctorRes.count || (Array.isArray(doctorRes.response) ? doctorRes.response.length : 0)
    }

    // 获取今日数据
    await fetchTodayStats()

    // 获取待处理事项
    await fetchPendingItems()
  } catch (error) {
    console.error('获取统计数据失败:', error)
    ElMessage.warning('数据加载中...')
  }
}

// 获取今日统计
async function fetchTodayStats() {
  try {
    const today = new Date().toISOString().split('T')[0]
    
    // 今日新增患者
    const todayPatientsRes: any = await getPatientList('', '', 1, 1000)
    if (todayPatientsRes?.response && Array.isArray(todayPatientsRes.response)) {
      todayStats.newPatients = todayPatientsRes.response.filter((p: any) => {
        if (!p.createTime) return false
        const createDate = new Date(p.createTime).toISOString().split('T')[0]
        return createDate === today
      }).length
    }

    // 今日新增检查（通过日期筛选）
    const todayExamRes: any = await getExaminationList('', '', today, 1, 1000)
    if (todayExamRes?.response && Array.isArray(todayExamRes.response)) {
      todayStats.newExaminations = todayExamRes.response.length
    }

    // 今日完成打印
    const todayPrintRes: any = await getPrintRecordList(null, 1, 1000)
    if (todayPrintRes?.response && Array.isArray(todayPrintRes.response)) {
      todayStats.completedPrints = todayPrintRes.response.filter((p: any) => {
        if (!p.printTime) return false
        const printDate = new Date(p.printTime).toISOString().split('T')[0]
        return printDate === today
      }).length
    }
  } catch (error) {
    console.error('获取今日统计失败:', error)
  }
}

// 获取待处理事项
async function fetchPendingItems() {
  try {
    // 待打印检查
    const unprintedRes: any = await getExaminationList('', '', '', 1, 1000, 'false')
    if (unprintedRes?.response && Array.isArray(unprintedRes.response)) {
      pendingItems[0].count = unprintedRes.response.length
    } else if (unprintedRes?.count) {
      pendingItems[0].count = unprintedRes.count
    }
  } catch (error) {
    console.error('获取待处理事项失败:', error)
  }
}

// 快速导航处理
function handleQuickAccess(route: string) {
  router.push(route)
}

// 定时更新时间和问候语
let timeInterval: any = null

onMounted(() => {
  updateGreeting()
  fetchStatistics()
  
  // 每分钟更新一次时间
  timeInterval = setInterval(() => {
    updateGreeting()
  }, 60000)
})

onUnmounted(() => {
  if (timeInterval) {
    clearInterval(timeInterval)
  }
})
</script>

<style scoped>
.home-index-container {
  padding: 20px;
  min-height: calc(100vh - 60px);
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

/* 欢迎横幅 */
.welcome-banner {
  background: linear-gradient(135deg, #1D99AD 0%, #2E86AB 100%);
  border-radius: 12px;
  padding: 30px 40px;
  margin-bottom: 24px;
  box-shadow: 0 4px 12px rgba(29, 153, 173, 0.3);
}

.welcome-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: white;
}

.welcome-text {
  flex: 1;
}

.greeting {
  font-size: 36px;
  font-weight: 600;
  margin: 0 0 12px 0;
  color: white;
}

.welcome-desc {
  font-size: 18px;
  margin: 0 0 8px 0;
  opacity: 0.95;
}

.current-time {
  font-size: 14px;
  margin: 0;
  opacity: 0.85;
}

.welcome-decoration {
  opacity: 0.2;
}

/* 统计卡片区域 */
.statistics-section {
  margin-bottom: 24px;
}

.stat-card {
  border-radius: 12px;
  transition: all 0.3s ease;
  cursor: pointer;
  border: none;
  height: 120px;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.stat-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.stat-icon {
  width: 64px;
  height: 64px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 16px;
  color: white;
}

.patient-icon {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.examination-icon {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.print-icon {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.doctor-icon {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
}

.stat-info {
  flex: 1;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-bottom: 8px;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #303133;
}

/* 快速导航区域 */
.quick-access-section {
  margin-bottom: 24px;
}

.section-title {
  font-size: 20px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 16px 0;
  padding-left: 4px;
}

.quick-access-card {
  border-radius: 12px;
  transition: all 0.3s ease;
  cursor: pointer;
  border: none;
  margin-bottom: 20px;
  height: 100px;
}

.quick-access-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.quick-access-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.quick-access-icon {
  width: 56px;
  height: 56px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 16px;
  color: white;
}

.quick-access-text {
  flex: 1;
}

.quick-access-title {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 4px;
}

.quick-access-desc {
  font-size: 13px;
  color: #909399;
}

/* 今日概览区域 */
.today-overview-section {
  margin-bottom: 24px;
}

.overview-card {
  border-radius: 12px;
  transition: all 0.3s ease;
  border: none;
  height: 100%;
  min-height: 200px;
}

.overview-card:hover {
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.12);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 16px;
  color: #303133;
}

.today-stats {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.today-stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: #f5f7fa;
  border-radius: 8px;
}

.today-stat-label {
  font-size: 14px;
  color: #606266;
}

.today-stat-value {
  font-size: 20px;
  font-weight: 700;
  color: #1D99AD;
}

.pending-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.pending-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: #f5f7fa;
  border-radius: 8px;
}

.pending-label {
  font-size: 14px;
  color: #606266;
}

.pending-badge {
  margin-left: auto;
}

.pending-count {
  font-size: 16px;
  font-weight: 600;
  color: #F56C6C;
}

.no-pending {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 30px;
  color: #909399;
  gap: 8px;
}

.no-pending .el-icon {
  font-size: 48px;
  color: #67C23A;
}

.system-info {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.info-item {
  display: flex;
  align-items: center;
  padding: 12px;
  background: #f5f7fa;
  border-radius: 8px;
}

.info-label {
  font-size: 14px;
  color: #606266;
  margin-right: 8px;
}

.info-value {
  font-size: 14px;
  color: #303133;
  font-weight: 500;
}

/* 响应式设计 */
@media (max-width: 768px) {
  .welcome-content {
    flex-direction: column;
    text-align: center;
  }

  .welcome-decoration {
    margin-top: 20px;
  }

  .greeting {
    font-size: 28px;
  }

  .stat-card,
  .quick-access-card {
    margin-bottom: 16px;
  }
}
</style>
