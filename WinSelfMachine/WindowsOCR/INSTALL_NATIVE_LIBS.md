# Tesseract 本机库安装说明

如果遇到 "Failed to initialise tesseract engine" 错误，说明需要安装 Tesseract 本机库。

## 解决方案

### 方法一：使用 NuGet 包（推荐，最简单）

Tesseract.NET 包应该自动包含本机库。如果缺少，请执行以下步骤：

1. **清理并重新安装 NuGet 包**：
   ```powershell
   cd "D:\ProJect\LqProjrctTest\HospitalPrintingSystem\WinSelfMachine\WindowsOCR"
   nuget install Tesseract -Version 5.2.0 -OutputDirectory packages
   ```

2. **在 csproj 中添加引用**：
   可能需要手动添加本机库的引用。

### 方法二：手动下载本机库（如果需要）

1. **下载 Tesseract.NET 相关文件**：
   - 从 https://github.com/UglyToad/PdfPig/releases 下载
   - 或者从 https://github.com/charlesw/tesseract/releases 下载

2. **将 DLL 文件复制到程序目录**：
   - 解压下载的包
   - 找到 `x86` 文件夹
   - 将以下文件复制到你的程序目录（`bin\Debug`）：
     ```
     tesseract53.dll
     leptonica-1.82.0.dll
     以及其他相关 DLL
     ```

### 方法三：使用官方安装程序（最佳方案）

1. **下载并安装 Tesseract-OCR**：
   - 访问：https://github.com/UB-Mannheim/tesseract/wiki
   - 下载 Windows 安装程序
   - 运行安装程序，安装到默认位置：`C:\Program Files\Tesseract-OCR`

2. **验证安装**：
   安装完成后，程序会自动检测和使用。

3. **检查 tessdata**：
   - 确保 `C:\Program Files\Tesseract-OCR\tessdata` 存在
   - 如果不存在，从 https://github.com/tesseract-ocr/tessdata 下载
   - 将 `chi_sim.traineddata` 和 `eng.traineddata` 放入 tessdata 文件夹

## 快速诊断

运行程序并查看调试信息：
```
调试信息：查找路径 - ...
调试信息：tessdata目录 - ...
本机库检查：
  找到 tesseract53.dll: ...  (如果有这一行，说明本机库存在)
```

如果本机库检查后没有找到 DLL，请按上述方法安装。

## 常见问题

### Q: 为什么需要本机库？
A: Tesseract.NET 是一个 .NET 包装器，但它仍然需要 Tesseract 的原生 C++ 库（本机库）才能运行。

### Q: 如何验证本机库已安装？
A: 在 bin\Debug 或 bin\Release 目录下查找以下文件：
- tesseract53.dll
- leptonica-1.82.0.dll

### Q: 使用完整安装程序 vs NuGet 包？
A: 
- **完整安装程序**：更可靠，包含所有必需文件
- **NuGet 包**：更方便，但可能在某些情况下缺少本机库

## 推荐的最终方案

**最简单的方法**：
1. 下载并安装 Tesseract-OCR 官方安装程序
2. 下载 tessdata 文件并放入 `C:\Program Files\Tesseract-OCR\tessdata`
3. 将本地的 tessdata 复制到程序目录：`D:\...\WindowsOCR\bin\Debug\tessdata`

这样就不需要手动管理本机库了。

