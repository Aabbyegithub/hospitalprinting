# DICOM接收服务使用说明

## 概述

本系统实现了DICOM SCP（Service Class Provider）服务，可以接收其他系统通过IP AE（Application Entity）转发的DICOM文件。

## 功能特性

- **DICOM C-STORE接收**: 支持接收DICOM C-STORE请求
- **文件管理**: 自动保存接收到的DICOM文件
- **标签解析**: 解析DICOM文件中的关键标签信息
- **RESTful API**: 提供完整的REST API接口
- **自动启动**: 应用启动时自动启动DICOM服务

## 配置说明

在 `appsettings.json` 中配置DICOM服务：

```json
{
  "SCPService": {
    "Enabled": true,                    // 是否启用服务
    "AETitle": "HOSPITAL_SCP_AE",      // AE标题
    "Port": 104,                       // 监听端口
    "IPAddress": "0.0.0.0",           // 监听IP地址
    "OutputDirectory": "ReceivedDICOMs", // 文件保存目录
    "MaxFileSize": 104857600,         // 最大文件大小（字节）
    "AllowedCallingAEs": [],          // 允许的调用方AE列表
    "LogLevel": "Information",         // 日志级别
    "MonitorInterval": 30              // 文件监控间隔（秒）
  }
}
```

## API接口

### 1. 启动DICOM服务
```
POST /api/DICOM/start
```

### 2. 停止DICOM服务
```
POST /api/DICOM/stop
```

### 3. 获取服务状态
```
GET /api/DICOM/status
```

### 4. 获取接收到的文件列表
```
GET /api/DICOM/files?pageIndex=1&pageSize=10
```

### 5. 获取特定文件信息
```
GET /api/DICOM/files/{fileId}
```

### 6. 下载DICOM文件
```
GET /api/DICOM/files/{fileId}/download
```

### 7. 解析DICOM文件标签
```
GET /api/DICOM/files/{fileId}/tags
```

### 8. 删除DICOM文件
```
DELETE /api/DICOM/files/{fileId}
```

### 9. 获取服务配置
```
GET /api/DICOM/config
```

## 使用步骤

### 1. 启动应用
启动Web应用后，DICOM SCP服务会自动启动（如果配置中启用了服务）。

### 2. 配置其他系统
在其他DICOM系统中配置AE，将目标设置为：
- **AE Title**: HOSPITAL_SCP_AE
- **IP Address**: 你的服务器IP
- **Port**: 104

### 3. 发送DICOM文件
其他系统可以通过C-STORE操作将DICOM文件发送到你的服务。

### 4. 查看接收结果
通过API接口查看接收到的文件列表和详细信息。

## 文件存储

接收到的DICOM文件会保存在以下目录结构中：
```
ReceivedDICOMs/
├── 2024-01-01/
│   ├── 1.2.3.4.5.6.7.8.9.10.dcm
│   └── 1.2.3.4.5.6.7.8.9.11.dcm
└── 2024-01-02/
    └── 1.2.3.4.5.6.7.8.9.12.dcm
```

## 支持的DICOM标签

系统会自动解析以下关键标签：
- Patient ID
- Patient Name
- Study Instance UID
- Series Instance UID
- SOP Instance UID
- Modality
- Study Date
- Study Time
- Study Description
- Series Description

## 错误处理

- 服务启动失败时会记录错误日志
- 文件接收失败时会返回相应的DICOM状态码
- 所有操作都有完整的异常处理和日志记录

## 注意事项

1. 确保端口104没有被其他服务占用
2. 防火墙需要开放相应端口
3. 确保有足够的磁盘空间存储DICOM文件
4. 建议定期清理旧文件以节省存储空间

## 测试

可以使用提供的 `DICOMTest.http` 文件进行API测试，或者使用DICOM测试工具（如DCMTK）发送测试文件。
