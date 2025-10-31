<template>
  <div class="ai-config-management">
    <!-- 未启用状态 - 显示空白页面和启动按钮 -->
    <div v-if="!configForm.is_enabled" class="disabled-state">
      <div class="empty-content">
        <div class="empty-icon">
          <el-icon size="80" color="#c0c4cc">
            <MagicStick />
          </el-icon>
        </div>
        <div class="empty-text">AI功能未启用</div>
        <div class="empty-description">点击下方按钮启用AI链接配置</div>
        <el-button type="primary" size="large" @click="enableAi">启用AI功能</el-button>
      </div>
    </div>

    <!-- 已启用状态 - 显示配置表单 -->
    <div v-else class="enabled-state">
      <div class="config-header">
        <div class="header-left">
          <h3>AI链接配置</h3>
          <el-tag type="success">已启用</el-tag>
        </div>
        <div class="header-right">
          <el-button @click="disableAi">停用AI</el-button>
        </div>
      </div>

      <el-form :model="configForm" :rules="configRules" ref="configFormRef" label-width="120px" class="config-form">
        <el-form-item label="API URL" prop="api_url">
          <el-input v-model="configForm.api_url" placeholder="请输入AI接口URL" />
        </el-form-item>
        <el-form-item label="API Key" prop="api_key">
          <el-input v-model="configForm.api_key" type="password" placeholder="请输入API Key" show-password />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="configForm.remark" type="textarea" :rows="3" placeholder="请输入备注信息" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="saveConfig">保存配置</el-button>
          <el-button @click="resetConfig">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { MagicStick } from '@element-plus/icons-vue'
import { getAiConfigApi, saveAiConfigApi, testAiConnectionApi } from '../../../../api/aiConfig'

// 配置表单
const configFormRef = ref()
const configForm = reactive({
  id: 0,
  is_enabled: 0,
  api_url: '',
  api_key: '',
  remark: ''
})

// 表单验证规则
const configRules = {
  api_url: [{ required: true, message: '请输入API URL', trigger: 'blur' }],
  api_key: [{ required: true, message: '请输入API Key', trigger: 'blur' }]
}

// 获取配置
async function fetchConfig() {
  try {
    const res:any = await getAiConfigApi()
    if (res && res.response) {
      Object.assign(configForm, res.response)
    } else {
      ElMessage.error(res.message || '获取配置失败')
    }
  } catch (error) {
    ElMessage.error('获取配置失败')
  }
}

// 启用AI
async function enableAi() {
  configForm.is_enabled = 1
  
  // 直接保存状态
  try {
    const res: any = await saveAiConfigApi(configForm)
    if (res && res.response) {
      ElMessage.success('AI功能已启用')
      await fetchConfig()
    } else {
      ElMessage.error(res.message || '启用失败')
    }
  } catch (error) {
    ElMessage.error('启用失败')
  }
}

// 停用AI
async function disableAi() {
  try {
    await ElMessageBox.confirm('确定要停用AI功能吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    configForm.is_enabled = 0
    await saveConfig()
    ElMessage.success('AI功能已停用')
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('停用失败')
    }
  }
}

// 保存配置
async function saveConfig() {
  if (configForm.is_enabled === 1) {
    if (!configFormRef.value) return
    
    await configFormRef.value.validate()
  }
  
  try {
    const res:any = await saveAiConfigApi(configForm)
    if (res && res.response) {
      ElMessage.success('保存成功')
      await fetchConfig()
    } else {
      ElMessage.error(res.message || '保存失败')
    }
  } catch (error) {
    ElMessage.error('保存失败')
  }
}

// 重置配置
function resetConfig() {
  configForm.api_url = ''
  configForm.api_key = ''
  configForm.remark = ''
}

// 初始化
onMounted(() => {
  fetchConfig()
})
</script>

<style scoped>
.ai-config-management {
  padding: 20px;
  min-height: 60vh;
}

.disabled-state {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 60vh;
}

.empty-content {
  text-align: center;
}

.empty-icon {
  margin-bottom: 20px;
}

.empty-text {
  font-size: 18px;
  color: #606266;
  margin-bottom: 10px;
}

.empty-description {
  font-size: 14px;
  color: #909399;
  margin-bottom: 30px;
}

.enabled-state {
  background: #fff;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.config-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 1px solid #ebeef5;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 15px;
}

.header-left h3 {
  margin: 0;
  color: #303133;
}

.header-right {
  display: flex;
  gap: 10px;
}

.config-form {
  max-width: 600px;
}
</style>

