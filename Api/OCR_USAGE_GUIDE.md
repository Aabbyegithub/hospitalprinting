# 本地OCR识别服务使用说明

## 📋 功能概述

基于Tesseract引擎的本地OCR文字识别服务，支持多种图片格式和语言识别。

## 🚀 主要功能

- ✅ **单张图片识别** - 识别指定路径的图片文件
- ✅ **上传图片识别** - 通过API上传图片并识别
- ✅ **批量识别** - 批量处理文件夹中的图片
- ✅ **多语言支持** - 支持中文、英文及混合语言
- ✅ **置信度评估** - 提供识别置信度评分
- ✅ **文字块定位** - 提供文字位置和边界信息

## 📦 依赖安装

### 1. 安装Tesseract引擎

#### Windows
```bash
# 下载Tesseract安装包
# https://github.com/UB-Mannheim/tesseract/wiki

# 安装后添加到环境变量
# 默认安装路径: C:\Program Files\Tesseract-OCR
```

#### Linux (Ubuntu/Debian)
```bash
sudo apt-get update
sudo apt-get install tesseract-ocr
sudo apt-get install tesseract-ocr-chi-sim  # 中文简体
sudo apt-get install tesseract-ocr-chi-tra  # 中文繁体
```

#### macOS
```bash
brew install tesseract
brew install tesseract-lang
```

### 2. 安装NuGet包
```bash
dotnet add package Tesseract
dotnet add package System.Drawing.Common
```

## 🔧 配置说明

### appsettings.json配置
```json
{
  "OCR": {
    "TessDataPath": "tessdata",
    "DefaultLanguage": "chi_sim+eng",
    "MaxFileSize": 10485760,
    "SupportedFormats": [".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif"],
    "ConfidenceThreshold": 60.0
  }
}
```

### 语言代码说明
- `chi_sim` - 简体中文
- `chi_tra` - 繁体中文  
- `eng` - 英文
- `chi_sim+eng` - 中英文混合
- `chi_tra+eng` - 繁英文混合

## 📡 API接口

### 1. 识别图片文件
```http
POST /api/OCR/recognize?filePath=/path/to/image.jpg&language=chi_sim+eng
```

### 2. 上传图片识别
```http
POST /api/OCR/upload?language=chi_sim+eng
Content-Type: multipart/form-data

file: [图片文件]
```

### 3. 批量识别
```http
POST /api/OCR/batch?folderPath=/path/to/folder&language=chi_sim+eng
```

### 4. 获取支持格式
```http
GET /api/OCR/formats
```

### 5. 获取支持语言
```http
GET /api/OCR/languages
```

## 💻 使用示例

### C# 客户端示例
```csharp
// 注入OCR服务
private readonly IOCRService _ocrService;

// 识别图片文件
var result = await _ocrService.RecognizeTextAsync("image.jpg", "chi_sim+eng");
if (result.Success)
{
    Console.WriteLine($"识别结果: {result.Text}");
    Console.WriteLine($"置信度: {result.Confidence:F2}%");
}

// 识别字节数组
byte[] imageBytes = File.ReadAllBytes("image.jpg");
var result2 = await _ocrService.RecognizeTextAsync(imageBytes, "chi_sim+eng");

// 批量识别
var results = await _ocrService.RecognizeFolderAsync("/path/to/folder", "chi_sim+eng");
```

### JavaScript/前端示例
```javascript
// 上传图片识别
const formData = new FormData();
formData.append('file', fileInput.files[0]);

fetch('/api/OCR/upload?language=chi_sim+eng', {
    method: 'POST',
    body: formData
})
.then(response => response.json())
.then(data => {
    if (data.success) {
        console.log('识别结果:', data.data.text);
        console.log('置信度:', data.data.confidence);
    }
});

// 识别本地文件
fetch('/api/OCR/recognize?filePath=/path/to/image.jpg&language=chi_sim+eng', {
    method: 'POST'
})
.then(response => response.json())
.then(data => {
    console.log('识别结果:', data.data);
});
```

## 📊 返回结果格式

```json
{
  "success": true,
  "data": {
    "filePath": "image.jpg",
    "text": "识别的文字内容",
    "confidence": 85.5,
    "processingTimeMs": 1250,
    "success": true,
    "errorMessage": "",
    "textBlocks": [
      {
        "text": "文字块",
        "confidence": 90.0,
        "boundingBox": {
          "x": 100,
          "y": 200,
          "width": 300,
          "height": 50
        },
        "pageNumber": 1
      }
    ]
  }
}
```

## 🎯 最佳实践

### 1. 图片预处理
- 确保图片清晰度足够
- 避免倾斜和变形
- 适当调整对比度和亮度
- 去除噪点和背景干扰

### 2. 语言选择
- 纯中文文档使用 `chi_sim`
- 纯英文文档使用 `eng`
- 中英混合文档使用 `chi_sim+eng`
- 繁体中文使用 `chi_tra`

### 3. 性能优化
- 批量处理时使用异步方法
- 大文件建议先压缩再识别
- 设置合理的置信度阈值
- 缓存识别结果避免重复处理

## 🔍 故障排除

### 常见问题

#### 1. Tesseract未安装
```
错误: Unable to load library 'libtesseract'
解决: 安装Tesseract引擎并配置环境变量
```

#### 2. 语言包缺失
```
错误: Error opening data file tessdata/chi_sim.traineddata
解决: 下载对应语言包到tessdata目录
```

#### 3. 图片格式不支持
```
错误: 不支持的图片格式
解决: 转换为支持的格式 (jpg, png, bmp, tiff)
```

#### 4. 识别准确率低
```
解决: 
- 提高图片质量
- 选择合适的语言包
- 调整图片预处理参数
- 使用更清晰的字体
```

## 📈 性能指标

- **识别速度**: 约1-3秒/张 (取决于图片大小和复杂度)
- **准确率**: 85-95% (取决于图片质量和文字类型)
- **支持格式**: JPG, PNG, BMP, TIFF
- **最大文件**: 10MB
- **并发处理**: 支持多线程并发识别

## 🔄 更新日志

- v1.0.0 - 基础OCR识别功能
- v1.1.0 - 添加批量识别和上传功能
- v1.2.0 - 支持文字块定位和置信度评估
- v1.3.0 - 优化性能和错误处理
