# FTP批量上传服务使用指南

## 📋 概述

FTP批量上传服务提供了完整的FTP文件上传功能，支持单个文件上传、批量文件上传、文件夹上传等操作。

## 🚀 功能特性

- ✅ **批量文件上传** - 支持同时上传多个文件
- ✅ **文件夹上传** - 支持递归上传整个文件夹
- ✅ **目录结构保持** - 可选择是否保持本地目录结构
- ✅ **并发控制** - 可配置同时上传的文件数量
- ✅ **断点续传** - 支持文件覆盖和跳过已存在文件
- ✅ **进度监控** - 实时监控上传进度和速度
- ✅ **错误处理** - 完善的错误处理和重试机制
- ✅ **连接测试** - 支持FTP连接测试
- ✅ **文件管理** - 支持文件存在检查、删除、目录列表等操作

## 🔧 配置说明

### 1. 数据库表结构

首先需要在数据库中创建FTP配置表：

```sql
CREATE TABLE HolFTPConfig (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    OrgId BIGINT NOT NULL,
    Host NVARCHAR(255) NOT NULL,
    Port INT NOT NULL DEFAULT 21,
    Username NVARCHAR(100) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    UsePassive BIT NOT NULL DEFAULT 1,
    UseSsl BIT NOT NULL DEFAULT 0,
    RemoteDirectory NVARCHAR(500) NOT NULL DEFAULT '/',
    Timeout INT NOT NULL DEFAULT 30,
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
    UpdateTime DATETIME NOT NULL DEFAULT GETDATE()
);
```

### 2. 配置文件设置

在 `appsettings.json` 中添加默认FTP配置：

```json
{
  "FTP": {
    "Host": "localhost",
    "Port": 21,
    "Username": "",
    "Password": "",
    "UsePassive": true,
    "UseSsl": false,
    "RemoteDirectory": "/",
    "Timeout": 30
  }
}
```

## 📚 API接口说明

### 1. 获取FTP配置

```http
GET /api/FTP/config/{orgId}
```

**响应示例：**
```json
{
  "success": true,
  "data": {
    "host": "ftp.example.com",
    "port": 21,
    "username": "user",
    "password": "***",
    "usePassive": true,
    "useSsl": false,
    "remoteDirectory": "/uploads",
    "timeout": 30,
    "isEnabled": true
  },
  "message": "获取FTP配置成功"
}
```

### 2. 保存FTP配置

```http
POST /api/FTP/config/{orgId}
Content-Type: application/json

{
  "host": "ftp.example.com",
  "port": 21,
  "username": "your_username",
  "password": "your_password",
  "usePassive": true,
  "useSsl": false,
  "remoteDirectory": "/uploads",
  "timeout": 30,
  "isEnabled": true
}
```

### 3. 测试FTP连接

```http
POST /api/FTP/test-connection
Content-Type: application/json

{
  "host": "ftp.example.com",
  "port": 21,
  "username": "your_username",
  "password": "your_password",
  "usePassive": true,
  "useSsl": false,
  "remoteDirectory": "/uploads",
  "timeout": 30,
  "isEnabled": true
}
```

### 4. 批量上传文件

```http
POST /api/FTP/batch-upload
Content-Type: application/json

{
  "filePaths": [
    "D:\\test\\file1.txt",
    "D:\\test\\file2.jpg",
    "D:\\test\\file3.pdf"
  ],
  "localRootDirectory": "D:\\test",
  "remoteRootDirectory": "/uploads",
  "keepDirectoryStructure": true,
  "overwriteExisting": true,
  "maxConcurrentUploads": 3,
  "orgId": 1
}
```

**响应示例：**
```json
{
  "success": true,
  "data": {
    "totalFiles": 3,
    "successCount": 3,
    "failureCount": 0,
    "totalDurationMs": 1500,
    "averageSpeedKbps": 1024.5,
    "allSuccess": true,
    "successRate": 100.0,
    "results": [
      {
        "success": true,
        "filePath": "D:\\test\\file1.txt",
        "remotePath": "/uploads/file1.txt",
        "fileSize": 1024,
        "uploadTime": "2024-01-01T10:00:00",
        "durationMs": 500,
        "uploadSpeedKbps": 2048.0,
        "errorMessage": null
      }
    ]
  },
  "message": "批量上传完成: 成功 3/3 个文件"
}
```

### 5. 上传单个文件

```http
POST /api/FTP/upload-single?filePath=D:\test\file1.txt&remotePath=/uploads/file1.txt&orgId=1
```

### 6. 上传文件夹

```http
POST /api/FTP/upload-folder?localFolderPath=D:\test&remoteFolderPath=/uploads&orgId=1&recursive=true
```

### 7. 检查远程文件是否存在

```http
GET /api/FTP/file-exists?remotePath=/uploads/file1.txt&orgId=1
```

### 8. 删除远程文件

```http
DELETE /api/FTP/file?remotePath=/uploads/file1.txt&orgId=1
```

### 9. 列出远程目录内容

```http
GET /api/FTP/list-directory?remotePath=/uploads&orgId=1
```

### 10. 创建远程目录

```http
POST /api/FTP/create-directory?remotePath=/uploads/newfolder&orgId=1
```

## 💡 使用示例

### C# 客户端示例

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class FTPClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public FTPClient(string baseUrl)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }

    // 批量上传文件
    public async Task<FTPBatchUploadResultDto> BatchUploadAsync(List<string> filePaths, long orgId)
    {
        var request = new FTPUploadRequestDto
        {
            FilePaths = filePaths,
            RemoteRootDirectory = "/uploads",
            KeepDirectoryStructure = true,
            OverwriteExisting = true,
            MaxConcurrentUploads = 3,
            OrgId = orgId
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/FTP/batch-upload", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<FTPBatchUploadResultDto>(responseContent);
    }

    // 测试FTP连接
    public async Task<bool> TestConnectionAsync(FTPConfigDto config)
    {
        var json = JsonConvert.SerializeObject(config);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/FTP/test-connection", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
        return result.success;
    }
}
```

### JavaScript 客户端示例

```javascript
class FTPClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }

    // 批量上传文件
    async batchUpload(filePaths, orgId) {
        const request = {
            filePaths: filePaths,
            remoteRootDirectory: "/uploads",
            keepDirectoryStructure: true,
            overwriteExisting: true,
            maxConcurrentUploads: 3,
            orgId: orgId
        };

        const response = await fetch(`${this.baseUrl}/api/FTP/batch-upload`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(request)
        });

        return await response.json();
    }

    // 测试FTP连接
    async testConnection(config) {
        const response = await fetch(`${this.baseUrl}/api/FTP/test-connection`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(config)
        });

        return await response.json();
    }
}

// 使用示例
const ftpClient = new FTPClient('https://localhost:7092');

// 批量上传文件
const filePaths = [
    'D:\\test\\file1.txt',
    'D:\\test\\file2.jpg',
    'D:\\test\\file3.pdf'
];

ftpClient.batchUpload(filePaths, 1)
    .then(result => {
        console.log('上传结果:', result);
    })
    .catch(error => {
        console.error('上传失败:', error);
    });
```

## ⚠️ 注意事项

1. **文件路径** - 确保本地文件路径存在且可访问
2. **权限设置** - 确保FTP服务器有足够的写入权限
3. **网络连接** - 确保网络连接稳定，避免大文件上传中断
4. **并发控制** - 根据服务器性能调整并发上传数量
5. **错误处理** - 实现适当的错误处理和重试机制
6. **安全性** - 使用安全的FTP连接（FTPS/SFTP）传输敏感文件

## 🔍 故障排除

### 常见问题

1. **连接超时**
   - 检查FTP服务器地址和端口
   - 增加超时时间设置
   - 检查防火墙设置

2. **认证失败**
   - 验证用户名和密码
   - 检查用户权限设置

3. **上传失败**
   - 检查远程目录权限
   - 验证文件路径是否正确
   - 检查磁盘空间是否充足

4. **性能问题**
   - 调整并发上传数量
   - 检查网络带宽
   - 优化文件大小

## 📞 技术支持

如有问题，请联系技术支持团队或查看项目文档。
