# Tesseract OCR 安装说明

本OCR工具使用 Tesseract OCR 引擎进行文字识别。

## 安装步骤

### 方法一：使用 NuGet 包（推荐）

在项目目录执行以下命令安装 Tesseract：

```powershell
# 安装Tesseract包
nuget install Tesseract -Version 5.2.0

# 安装中文和英文语言包
nuget install Tesseract.Data.chi_sim -Version 5.2.0
nuget install Tesseract.Data.English -Version 5.2.0
```

### 方法二：手动下载安装

1. 下载 Tesseract-OCR 安装程序：
   - https://github.com/UB-Mannheim/tesseract/wiki
   - 选择 Windows 版本下载

2. 运行安装程序，安装到默认目录：`C:\Program Files\Tesseract-OCR`

3. 下载语言数据文件：
   - 访问：https://github.com/tesseract-ocr/tessdata
   - 下载中文（chi_sim.traineddata）和英文（eng.traineddata）数据文件
   - 将这些文件复制到 `C:\Program Files\Tesseract-OCR\tessdata`

### 方法三：在程序目录放置 tessdata

将 tessdata 文件夹复制到程序运行目录（bin\Debug 或 bin\Release）下。

## 验证安装

安装完成后，程序会自动查找 tessdata 文件夹。如果找不到，会显示错误信息和查找路径。

## 语言支持

默认配置支持：
- 中文简体：chi_sim
- 英文：eng

如需支持其他语言，请下载相应的 .traineddata 文件到 tessdata 文件夹。

## 常见问题

### Q: 提示找不到 tessdata 文件夹
A: 确保 tessdata 文件夹存在，并且包含所需的 .traineddata 文件。

### Q: 报错 "Failed to initialise tesseract engine"
A: **这是最常见的问题**，说明缺少 Tesseract 本机库（DLL 文件）。

**解决方案**：
1. **最简单的方法**：安装完整的 Tesseract-OCR
   - 下载：https://github.com/UB-Mannheim/tesseract/wiki
   - 安装到默认位置：`C:\Program Files\Tesseract-OCR`
   - 下载 tessdata 并放入：`C:\Program Files\Tesseract-OCR\tessdata`
   - 重启程序

2. **或使用 NuGet 重新安装**：
   ```powershell
   cd WindowsOCR
   nuget install Tesseract -Version 5.2.0
   ```

3. **查看 INSTALL_NATIVE_LIBS.md 获取详细说明**

### Q: 识别准确率低
A: 尝试以下改进方法：
1. 确保图片清晰度足够
2. 使用高质量扫描图片
3. 调整图片尺寸和对比度
4. 选择更准确的区域进行识别

### Q: 程序启动报错
A: 检查是否安装了：
1. .NET Framework 4.6.1 或更高版本
2. Visual C++ Redistributable（通常 Tesseract 安装程序会自动安装）

## 使用说明

1. **打开图片**：文件 → 打开图片
2. **选择区域**：在图片上按住鼠标左键拖拽
3. **识别文字**：操作 → 识别选中区域（或识别全图）
4. **复制结果**：点击右侧“复制”按钮

## 技术支持

如有问题，请检查：
- Tesseract 版本兼容性
- 语言数据文件完整性
- 图片质量和格式

