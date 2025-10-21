<template>
  <div class="aliyun-oss-config-container">
    <div class="page-header">
      <h2>阿里云OSS配置</h2>
    </div>

    <!-- 未启用状态 -->
    <div v-if="!configForm.is_enabled" class="disabled-state">
      <div class="empty-content">
        <div class="empty-icon">
          <i class="el-icon-cloud-upload"></i>
        </div>
        <div class="empty-text">OSS功能未启用</div>
        <div class="empty-description">启用后可以将检查数据上传到阿里云OSS</div>
        <el-button type="primary" @click="enableOss">启用OSS功能</el-button>
      </div>
    </div>

    <!-- 已启用状态 -->
    <div v-else class="enabled-state">
      <el-card class="config-card">
        <template #header>
          <div class="card-header">
            <span>OSS配置信息</span>
            <el-button type="danger" @click="disableOss">禁用OSS功能</el-button>
          </div>
        </template>

        <el-form :model="configForm" label-width="140px" class="config-form">
          <el-form-item label="OSS访问域名" required>
            <el-input v-model="configForm.endpoint" placeholder="请输入OSS访问域名，如：oss-cn-hangzhou.aliyuncs.com" />
          </el-form-item>

          <el-form-item label="AccessKey ID" required>
            <el-input v-model="configForm.access_key_id" placeholder="请输入AccessKey ID" show-password />
          </el-form-item>

          <el-form-item label="AccessKey Secret" required>
            <el-input v-model="configForm.access_key_secret" placeholder="请输入AccessKey Secret" show-password />
          </el-form-item>

          <el-form-item label="存储桶名称" required>
            <el-input v-model="configForm.bucket_name" placeholder="请输入存储桶名称" />
          </el-form-item>

          <el-form-item label="地域" required>
            <el-select v-model="configForm.region" placeholder="请选择地域" style="width: 100%">
              <el-option label="华东1（杭州）" value="cn-hangzhou" />
              <el-option label="华东2（上海）" value="cn-shanghai" />
              <el-option label="华北1（青岛）" value="cn-qingdao" />
              <el-option label="华北2（北京）" value="cn-beijing" />
              <el-option label="华北3（张家口）" value="cn-zhangjiakou" />
              <el-option label="华北5（呼和浩特）" value="cn-hohhot" />
              <el-option label="华北6（乌兰察布）" value="cn-wulanchabu" />
              <el-option label="华南1（深圳）" value="cn-shenzhen" />
              <el-option label="华南2（河源）" value="cn-heyuan" />
              <el-option label="华南3（广州）" value="cn-guangzhou" />
              <el-option label="西南1（成都）" value="cn-chengdu" />
              <el-option label="西南2（重庆）" value="cn-chongqing" />
              <el-option label="中国香港" value="cn-hongkong" />
              <el-option label="美国西部1（硅谷）" value="us-west-1" />
              <el-option label="美国东部1（弗吉尼亚）" value="us-east-1" />
              <el-option label="亚太东南1（新加坡）" value="ap-southeast-1" />
              <el-option label="亚太东南2（悉尼）" value="ap-southeast-2" />
              <el-option label="亚太东南3（吉隆坡）" value="ap-southeast-3" />
              <el-option label="亚太东南5（雅加达）" value="ap-southeast-5" />
              <el-option label="亚太东南6（马尼拉）" value="ap-southeast-6" />
              <el-option label="亚太东南7（曼谷）" value="ap-southeast-7" />
              <el-option label="亚太南部1（孟买）" value="ap-south-1" />
              <el-option label="亚太东北1（日本）" value="ap-northeast-1" />
              <el-option label="亚太东北2（首尔）" value="ap-northeast-2" />
              <el-option label="欧洲中部1（法兰克福）" value="eu-central-1" />
              <el-option label="欧洲西部1（英国）" value="eu-west-1" />
              <el-option label="中东东部1（迪拜）" value="me-east-1" />
            </el-select>
          </el-form-item>

          <el-form-item label="文件夹前缀">
            <el-input v-model="configForm.folder_prefix" placeholder="请输入文件夹前缀，如：hospital/examinations" />
            <div class="form-tip">可选，用于在OSS中组织文件结构</div>
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="saveConfig">保存配置</el-button>
            <el-button @click="testConnection">测试连接</el-button>
            <el-button @click="resetConfig">重置</el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getOssConfigApi, saveOssConfigApi, testOssConnectionApi } from '../../../../api/OssConfig'

// 配置表单
const configForm = reactive({
  id: 0,
  org_id: 1,
  is_enabled: 0,
  endpoint: '',
  access_key_id: '',
  access_key_secret: '',
  bucket_name: '',
  region: '',
  folder_prefix: ''
})

// 获取配置
async function fetchConfig() {
  try {
    const res: any = await getOssConfigApi()
    if (res && res.response) {
      Object.assign(configForm, res.response)
    }
  } catch (error) {
    console.error('获取配置失败:', error)
    ElMessage.error('获取配置失败')
  }
}

// 启用OSS功能
async function enableOss() {
  configForm.is_enabled = 1
  // 设置默认配置
  configForm.endpoint = 'oss-cn-hangzhou.aliyuncs.com'
  configForm.region = 'cn-hangzhou'
  configForm.folder_prefix = 'hospital/examinations'

  // 直接保存状态
  try {
    const res: any = await saveOssConfigApi(configForm)
    if (res && res.response) {
      ElMessage.success('OSS功能已启用')
      await fetchConfig()
    } else {
      ElMessage.error(res.message || '启用失败')
    }
  } catch (error) {
    ElMessage.error('启用失败')
  }
}

// 禁用OSS功能
async function disableOss() {
  try {
    await ElMessageBox.confirm('确定要禁用OSS功能吗？', '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })

    configForm.is_enabled = 0
    await saveConfig()
  } catch (error) {
    // 用户取消
  }
}

// 保存配置
async function saveConfig() {
  try {
    if (configForm.is_enabled === 1) {
      if (!configForm.endpoint?.trim()) {
        ElMessage.error('请输入OSS访问域名')
        return
      }
      if (!configForm.access_key_id?.trim()) {
        ElMessage.error('请输入AccessKey ID')
        return
      }
      if (!configForm.access_key_secret?.trim()) {
        ElMessage.error('请输入AccessKey Secret')
        return
      }
      if (!configForm.bucket_name?.trim()) {
        ElMessage.error('请输入存储桶名称')
        return
      }
      if (!configForm.region?.trim()) {
        ElMessage.error('请选择地域')
        return
      }
    }

    const res: any = await saveOssConfigApi(configForm)
    if (res && res.response) {
      ElMessage.success('配置保存成功')
      await fetchConfig()
    } else {
      ElMessage.error(res.message || '保存失败')
    }
  } catch (error) {
    console.error('保存配置失败:', error)
    ElMessage.error('保存配置失败')
  }
}

// 测试连接
async function testConnection() {
  try {
    if (configForm.is_enabled !== 1) {
      ElMessage.warning('请先启用OSS功能')
      return
    }

    if (!configForm.endpoint?.trim() || 
        !configForm.access_key_id?.trim() || 
        !configForm.access_key_secret?.trim() || 
        !configForm.bucket_name?.trim()) {
      ElMessage.error('请填写完整的OSS配置信息')
      return
    }

    const res: any = await testOssConnectionApi(configForm)
    if (res && res.response) {
      ElMessage.success('OSS连接测试成功')
    } else {
      ElMessage.error(res.message || '连接测试失败')
    }
  } catch (error) {
    console.error('测试连接失败:', error)
    ElMessage.error('连接测试失败')
  }
}

// 重置配置
function resetConfig() {
  configForm.endpoint = ''
  configForm.access_key_id = ''
  configForm.access_key_secret = ''
  configForm.bucket_name = ''
  configForm.region = ''
  configForm.folder_prefix = ''
}

onMounted(() => {
  fetchConfig()
})
</script>

<style scoped>
.aliyun-oss-config-container {
  padding: 20px;
}

.page-header {
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  color: #303133;
}

.disabled-state {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.empty-content {
  text-align: center;
}

.empty-icon {
  font-size: 64px;
  color: #c0c4cc;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 18px;
  color: #606266;
  margin-bottom: 8px;
}

.empty-description {
  font-size: 14px;
  color: #909399;
  margin-bottom: 24px;
}

.enabled-state {
  max-width: 800px;
  margin: 0 auto;
}

.config-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.config-form {
  padding: 20px 0;
}

.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}
</style>
