# DICOM SCP 服务使用说明

## 概述

基于FellowOakDicom库实现的DICOM SCP (Service Class Provider) 服务，用于接收医疗设备发送的DICOM文件并进行解析处理。

## 功能特性

- ✅ 接收DICOM文件 (C-STORE)
- ✅ 连接测试 (C-ECHO)
- ✅ 自动文件解析和元数据提取
- ✅ 按日期和患者ID组织文件存储
- ✅ 异步文件处理
- ✅ 完整的日志记录
- ✅ RESTful API管理接口

## 配置说明

在 `appsettings.json` 中配置DICOM服务：

```json
{
  "DicomService": {
    "Enabled": true,                    // 是否启用服务
    "Port": 104,                       // 监听端口
    "AETitle": "HOSPITAL_SCP",         // AE标题
    "SaveDirectory": "D:/ReceivedDicom", // 文件保存目录
    "MaxConnections": 10,              // 最大连接数
    "ConnectionTimeoutSeconds": 30,     // 连接超时时间
    "AutoCreateDirectory": true        // 自动创建目录
  }
}
```

## 医疗设备配置

### CT/MRI设备配置示例

在医疗设备的DICOM设置中配置：

- **目标AE Title**: `HOSPITAL_SCP`
- **目标IP地址**: 服务器IP地址
- **目标端口**: `104`
- **传输语法**: `Implicit VR Little Endian`

### 常见设备配置

1. **GE设备**:
   - Network Settings → DICOM → Add Destination
   - AE Title: `HOSPITAL_SCP`
   - IP: 服务器IP
   - Port: 104

2. **西门子设备**:
   - Service → DICOM → Export
   - Destination AE: `HOSPITAL_SCP`
   - Destination IP: 服务器IP
   - Destination Port: 104

3. **飞利浦设备**:
   - Configuration → DICOM → Network
   - Remote AE: `HOSPITAL_SCP`
   - Remote IP: 服务器IP
   - Remote Port: 104

## API接口

### 1. 获取服务配置
```
GET /api/Dicom/config
```

### 2. 更新服务配置
```
POST /api/Dicom/config
Content-Type: application/json

{
  "Enabled": true,
  "Port": 104,
  "AETitle": "HOSPITAL_SCP",
  "SaveDirectory": "D:/ReceivedDicom"
}
```

### 3. 获取接收的文件列表
```
GET /api/Dicom/files?date=20231201&patientId=12345
```

参数：
- `date`: 日期筛选 (格式: yyyyMMdd)
- `patientId`: 患者ID筛选

### 4. 解析DICOM文件
```
POST /api/Dicom/parse
Content-Type: application/json

{
  "FilePath": "D:/ReceivedDicom/20231201/12345/image.dcm"
}
```

### 5. 测试服务连接
```
POST /api/Dicom/test
```

## 文件存储结构

接收的DICOM文件按以下结构存储：

```
ReceivedDicom/
├── 20231201/           # 检查日期
│   ├── 12345/         # 患者ID
│   │   ├── 1.2.3.4.5_CT_143022.dcm
│   │   └── 1.2.3.4.6_CT_143025.dcm
│   └── 67890/
│       └── 1.2.3.4.7_MR_150030.dcm
└── 20231202/
    └── 11111/
        └── 1.2.3.4.8_CT_090015.dcm
```

## 解析的数据字段

### 患者信息 (HolPatient)
- `patient_id`: 患者ID
- `patient_name`: 患者姓名
- `patient_birth_date`: 出生日期
- `patient_sex`: 性别
- `patient_age`: 年龄

### 检查信息 (HolExamination)
- `study_instance_uid`: 检查实例UID
- `series_instance_uid`: 序列实例UID
- `sop_instance_uid`: SOP实例UID
- `study_date`: 检查日期
- `study_time`: 检查时间
- `modality`: 设备类型 (CT, MR, CR等)
- `study_description`: 检查描述
- `series_description`: 序列描述
- `institution_name`: 机构名称
- `department_name`: 科室名称
- `physician_name`: 医生姓名

## 日志记录

服务会记录以下信息：
- DICOM连接建立和释放
- 文件接收和保存
- 文件解析结果
- 错误和异常信息

日志级别可通过配置文件调整。

## 故障排除

### 1. 服务无法启动
- 检查端口是否被占用
- 确认配置文件格式正确
- 检查保存目录权限

### 2. 设备无法连接
- 确认AE Title匹配
- 检查网络连接
- 验证端口配置

### 3. 文件解析失败
- 检查DICOM文件格式
- 确认文件完整性
- 查看错误日志

## 性能优化

- 调整 `MaxConnections` 参数
- 优化文件保存路径 (SSD存储)
- 配置合适的日志级别
- 定期清理旧文件

## 安全考虑

- 限制网络访问 (防火墙)
- 定期备份重要文件
- 监控异常连接
- 设置文件访问权限
