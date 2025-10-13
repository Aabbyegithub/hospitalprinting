<template>
  <div class="ocr-config-management">
    <!-- 未启用状态 - 显示空白页面和启动按钮 -->
    <div v-if="!configForm.is_enabled" class="disabled-state">
      <div class="empty-content">
        <div class="empty-icon">
          <el-icon size="80" color="#c0c4cc">
            <Document />
          </el-icon>
        </div>
        <div class="empty-text">OCR识别功能未启用</div>
        <div class="empty-description">点击下方按钮启用百度云OCR识别功能</div>
        <el-button type="primary" size="large" @click="enableOcr">启用OCR识别</el-button>
      </div>
    </div>

    <!-- 已启用状态 - 显示配置表单 -->
    <div v-else class="enabled-state">
      <div class="config-header">
        <div class="header-left">
          <h3>百度云OCR配置</h3>
          <el-tag type="success">已启用</el-tag>
        </div>
        <div class="header-right">
          <el-button @click="disableOcr">停用OCR</el-button>
          <!-- <el-button type="primary" @click="testConnection">测试连接</el-button> -->
        </div>
      </div>

      <el-form :model="configForm" :rules="configRules" ref="configFormRef" label-width="120px" class="config-form">
        <el-form-item label="API URL" prop="api_url">
          <el-input v-model="configForm.api_url" placeholder="请输入百度云OCR接口URL" />
        </el-form-item>
        <el-form-item label="应用ID" prop="app_id">
          <el-input v-model="configForm.app_id" placeholder="请输入百度云应用ID" />
        </el-form-item>
        <el-form-item label="API Key" prop="api_key">
          <el-input v-model="configForm.api_key" type="password" placeholder="请输入百度云API Key" show-password />
        </el-form-item>
        <el-form-item label="Secret Key" prop="secret_key">
          <el-input v-model="configForm.secret_key" type="password" placeholder="请输入百度云Secret Key" show-password />
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
import { Document } from '@element-plus/icons-vue'
import { getOcrConfigApi, saveOcrConfigApi, testOcrConnectionApi } from '../../../../api/ocrConfig'

// 配置表单
const configFormRef = ref()
const configForm = reactive({
  id: 0,
  is_enabled: 0,
  api_url: '',
  app_id: '',
  api_key: '',
  secret_key: '',
  remark: ''
})

// 表单验证规则
const configRules = {
  api_url: [{ required: true, message: '请输入API URL', trigger: 'blur' }],
  app_id: [{ required: true, message: '请输入应用ID', trigger: 'blur' }],
  api_key: [{ required: true, message: '请输入API Key', trigger: 'blur' }],
  secret_key: [{ required: true, message: '请输入Secret Key', trigger: 'blur' }]
}

// 获取配置
async function fetchConfig() {
  try {
    const res:any = await getOcrConfigApi()
    if (res && res.response) {
      Object.assign(configForm, res.response)
    } else {
      ElMessage.error(res.message || '获取配置失败')
    }
  } catch (error) {
    ElMessage.error('获取配置失败')
  }
}

// 启用OCR
async function enableOcr() {
  configForm.is_enabled = 1
  // 设置默认API URL
  configForm.api_url = 'https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic'
  
  // 直接保存状态
  try {
    const res: any = await saveOcrConfigApi(configForm)
    if (res && res.response) {
      ElMessage.success('OCR功能已启用')
      await fetchConfig()
    } else {
      ElMessage.error(res.message || '启用失败')
    }
  } catch (error) {
    ElMessage.error('启用失败')
  }
}

// 停用OCR
async function disableOcr() {
  try {
    await ElMessageBox.confirm('确定要停用OCR识别功能吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    configForm.is_enabled = 0
    await saveConfig()
    ElMessage.success('OCR功能已停用')
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
    const res:any = await saveOcrConfigApi(configForm)
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

// 测试连接
async function testConnection() {
  if (!configFormRef.value) return
  
  await configFormRef.value.validate()
  
  try {
    const response = await testOcrConnectionApi(configForm)
    if (response.data.success) {
      ElMessage.success('连接测试成功')
    } else {
      ElMessage.error(response.data.message || '连接测试失败')
    }
  } catch (error) {
    ElMessage.error('连接测试失败')
  }
}

// 重置配置
function resetConfig() {
  configForm.api_url = ''
  configForm.app_id = ''
  configForm.api_key = ''
  configForm.secret_key = ''
  configForm.remark = ''
}

// 初始化
onMounted(() => {
  fetchConfig()
})
</script>

<style scoped>
.ocr-config-management {
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

.w-160{ width:160px; }
.btn{ background:#22a2b6; color:#fff; border:none; padding:4px 12px; border-radius:4px; }
.btn.ghost{ background:#f5f7fa; color:#606266; border:1px solid #dcdfe6; }
</style>
