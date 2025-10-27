using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.IO.Reader;
using SixLabors.ImageSharp.Formats.Bmp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace WinFormsOCR
{
    /// <summary>
    /// DICOM文件转换辅助类
    /// </summary>
    public static class DicomConverter
    {
        /// <summary>
        /// 将DICOM文件转换为Bitmap
        /// </summary>
        /// <param name="dicomFilePath">DICOM文件路径</param>
        /// <returns>转换后的Bitmap对象</returns>
        public static Bitmap? ConvertDicomToBitmap(string dicomFilePath)
        {
            try
            {
                if (!File.Exists(dicomFilePath))
                    throw new FileNotFoundException($"DICOM 文件不存在: {dicomFilePath}");

                var fileInfo = new FileInfo(dicomFilePath);
                if (fileInfo.Length < 128)
                    throw new Exception("文件太小，不是有效的 DICOM 文件");

                DicomFile? dicomFile = null;
                DicomDataset? dataset = null;

                var fileName = Path.GetFileNameWithoutExtension(dicomFilePath);
                bool isUidFormat = IsDicomUidFormat(fileName);

                // ====== 尝试多种方式打开 DICOM 文件 ======
                try
                {
                    if (isUidFormat)
                    {
                        dicomFile = DicomFile.Open(dicomFilePath, FileReadOption.ReadLargeOnDemand);
                        dataset = dicomFile.Dataset;
                    }
                    else
                    {
                        dicomFile = DicomFile.Open(dicomFilePath);
                        dataset = dicomFile.Dataset;
                    }
                }
                catch (DicomReaderException ex1)
                {
                    try
                    {
                        dicomFile = DicomFile.Open(dicomFilePath, FileReadOption.ReadLargeOnDemand);
                        dataset = dicomFile.Dataset;
                    }
                    catch (DicomReaderException ex2)
                    {
                        try
                        {
                            dicomFile = DicomFile.Open(dicomFilePath, FileReadOption.ReadLargeOnDemand);
                            dataset = dicomFile.Dataset;
                        }
                        catch (Exception ex3)
                        {
                            throw new Exception($"无法读取 DICOM 文件。尝试了多种方法都失败。\n" +
                                $"文件名: {fileName}\n" +
                                $"是否为 UID 格式: {isUidFormat}\n" +
                                $"方法1错误: {ex1.Message}\n" +
                                $"方法2错误: {ex2.Message}\n" +
                                $"方法3错误: {ex3.Message}", ex3);
                        }
                    }
                }

                if (dataset == null)
                    throw new Exception("DICOM 文件数据集为空");

                if (!dataset.Contains(DicomTag.PixelData))
                    throw new Exception("DICOM 文件不包含图像数据");

                // ====== 读取图像并转换 ======
                var dicomImage = new DicomImage(dicomFilePath);

                var width = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
                var height = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
                if (width <= 0 || height <= 0)
                    throw new Exception($"无效的图像尺寸: {width}x{height}");

                Bitmap? bitmap = null;

                try
                {
                    // fo-dicom 5.x 使用 ImageSharp 渲染
                    var sharpImage = dicomImage.RenderImage().AsSharpImage();

                    // 将 ImageSharp 图像写入内存流，再转成 Bitmap
                    using (var ms = new MemoryStream())
                    {
                        sharpImage.Save(ms, new BmpEncoder()); // 使用 BMP 格式避免压缩丢失
                        ms.Seek(0, SeekOrigin.Begin);
                        bitmap = new Bitmap(ms);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"DICOM 图像渲染失败: {ex.Message}", ex);
                }

                if (bitmap == null)
                    throw new Exception("无法将 DICOM 图像转换为 Bitmap");

                if (bitmap.Width <= 0 || bitmap.Height <= 0)
                {
                    bitmap.Dispose();
                    throw new Exception($"生成的 Bitmap 尺寸无效: {bitmap.Width}x{bitmap.Height}");
                }

                // ====== 灰度转 RGB（OCR 兼容） ======
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    try
                    {
                        var rgbBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                        using (var g = Graphics.FromImage(rgbBitmap))
                        {
                            g.DrawImage(bitmap, 0, 0);
                        }
                        bitmap.Dispose();
                        return rgbBitmap;
                    }
                    catch (Exception ex)
                    {
                        bitmap.Dispose();
                        throw new Exception($"灰度图像转换为 RGB 失败: {ex.Message}", ex);
                    }
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception($"DICOM 文件转换失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 将DICOM文件转换为PNG格式并保存
        /// </summary>
        /// <param name="dicomFilePath">DICOM文件路径</param>
        /// <param name="outputPath">输出PNG文件路径</param>
        /// <returns>是否转换成功</returns>
        public static bool ConvertDicomToPng(string dicomFilePath, string outputPath)
        {
            try
            {
                var bitmap = ConvertDicomToBitmap(dicomFilePath);
                if (bitmap != null)
                {
                    bitmap.Save(outputPath, ImageFormat.Png);
                    bitmap.Dispose();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"DICOM转PNG失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取DICOM文件的基本信息
        /// </summary>
        /// <param name="dicomFilePath">DICOM文件路径</param>
        /// <returns>DICOM信息字典</returns>
        public static Dictionary<string, string> GetDicomInfo(string dicomFilePath)
        {
            var info = new Dictionary<string, string>();
            
            try
            {
                if (!File.Exists(dicomFilePath))
                {
                    throw new FileNotFoundException($"DICOM文件不存在: {dicomFilePath}");
                }

                DicomFile? dicomFile = null;
                DicomDataset? dataset = null;

                // 检查文件名是否为UID格式
                var fileName = Path.GetFileNameWithoutExtension(dicomFilePath);
                bool isUidFormat = IsDicomUidFormat(fileName);

                // 尝试多种方式打开DICOM文件
                try
                {
                    if (isUidFormat)
                    {
                        // 对于UID格式的文件，直接使用大文件模式读取
                        dicomFile = DicomFile.Open(dicomFilePath, FileReadOption.ReadLargeOnDemand);
                        dataset = dicomFile.Dataset;
                    }
                    else
                    {
                        dicomFile = DicomFile.Open(dicomFilePath);
                        dataset = dicomFile.Dataset;
                    }
                }
                catch (DicomReaderException)
                {
                    try
                    {
                        dicomFile = DicomFile.Open(dicomFilePath, FileReadOption.ReadLargeOnDemand);
                        dataset = dicomFile.Dataset;
                    }
                    catch
                    {
                        // 如果所有方法都失败，抛出异常
                        throw new Exception($"无法读取DICOM文件，请检查文件格式是否正确。文件名: {fileName}, UID格式: {isUidFormat}");
                    }
                }

                if (dataset == null)
                {
                    throw new Exception("无法读取DICOM文件数据集");
                }

                // 安全获取DICOM信息，避免异常
                try
                {
                    // 获取患者信息
                    info["患者姓名"] = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "未知");
                    info["患者ID"] = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "未知");
                    info["患者性别"] = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, "未知");
                    info["患者年龄"] = dataset.GetSingleValueOrDefault(DicomTag.PatientAge, "未知");
                    info["患者生日"] = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, "未知");

                    // 获取检查信息
                    info["检查日期"] = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, "未知");
                    info["检查时间"] = dataset.GetSingleValueOrDefault(DicomTag.StudyTime, "未知");
                    info["检查描述"] = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "未知");
                    info["检查UID"] = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "未知");

                    // 获取设备信息
                    info["设备制造商"] = dataset.GetSingleValueOrDefault(DicomTag.Manufacturer, "未知");
                    info["设备型号"] = dataset.GetSingleValueOrDefault(DicomTag.ManufacturerModelName, "未知");
                    info["设备序列号"] = dataset.GetSingleValueOrDefault(DicomTag.DeviceSerialNumber, "未知");
                    info["检查类型"] = dataset.GetSingleValueOrDefault(DicomTag.Modality, "未知");

                    // 获取图像信息
                    var columns = dataset.GetSingleValueOrDefault(DicomTag.Columns, 0);
                    var rows = dataset.GetSingleValueOrDefault(DicomTag.Rows, 0);
                    var bitsAllocated = dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, 0);
                    
                    info["图像宽度"] = columns.ToString();
                    info["图像高度"] = rows.ToString();
                    info["位深度"] = bitsAllocated.ToString();
                    
                    // 安全获取像素间距
                    try
                    {
                        var pixelSpacing = dataset.GetSingleValueOrDefault(DicomTag.PixelSpacing, "");
                        info["像素间距"] = pixelSpacing.ToString();
                    }
                    catch
                    {
                        info["像素间距"] = "未知";
                    }

                    // 获取医院信息
                    info["医院名称"] = dataset.GetSingleValueOrDefault(DicomTag.InstitutionName, "未知");
                    info["科室名称"] = dataset.GetSingleValueOrDefault(DicomTag.InstitutionalDepartmentName, "未知");
                    info["操作员"] = dataset.GetSingleValueOrDefault(DicomTag.OperatorsName, "未知");
                    info["检查医生"] = dataset.GetSingleValueOrDefault(DicomTag.PerformingPhysicianName, "未知");
                }
                catch (Exception ex)
                {
                    info["错误信息"] = $"读取DICOM信息时出错: {ex.Message}";
                }

                return info;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取DICOM信息失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证文件是否为有效的DICOM文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为有效的DICOM文件</returns>
        public static bool IsValidDicomFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                // 检查文件大小
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < 128) // DICOM文件至少需要128字节的头部
                    return false;

                // 检查文件名是否为UID格式（如：1.2.826.0.1.3680043.6.625.28974.20250802183439.328.26511.2025.08.02.18.34.43.2076）
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (IsDicomUidFormat(fileName))
                {
                    // 对于UID格式的文件名，尝试直接读取
                    try
                    {
                        var dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand);
                        return dicomFile != null && dicomFile.Dataset != null;
                    }
                    catch
                    {
                        return false;
                    }
                }

                // 尝试多种方式打开DICOM文件
                try
                {
                    // 方法1：标准DICOM文件
                    var dicomFile = DicomFile.Open(filePath);
                    return dicomFile != null && dicomFile.Dataset != null;
                }
                catch (DicomReaderException)
                {
                    // 方法2：尝试作为无头DICOM文件打开
                    try
                    {
                        var dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand);
                        return dicomFile != null && dicomFile.Dataset != null;
                    }
                    catch
                    {
                        // 方法3：检查文件头是否为DICOM格式
                        return CheckDicomFileHeader(filePath);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文件名是否为DICOM UID格式
        /// </summary>
        /// <param name="fileName">文件名（不含扩展名）</param>
        /// <returns>是否为UID格式</returns>
        private static bool IsDicomUidFormat(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            // DICOM UID格式：数字和点组成，通常很长
            // 例如：1.2.826.0.1.3680043.6.625.28974.20250802183439.328.26511.2025.08.02.18.34.43.2076
            var uidPattern = @"^\d+(\.\d+)+$";
            return System.Text.RegularExpressions.Regex.IsMatch(fileName, uidPattern) && fileName.Length > 20;
        }

        /// <summary>
        /// 检查文件头是否为DICOM格式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>是否为DICOM格式</returns>
        private static bool CheckDicomFileHeader(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var buffer = new byte[132]; // 读取前132字节
                fs.Read(buffer, 0, buffer.Length);

                // 检查DICOM文件头
                // DICOM文件的前128字节是前导码，接下来的4字节是"DICM"标识
                for (int i = 128; i < 132; i++)
                {
                    if (buffer[i] != "DICM"[i - 128])
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取DICOM文件的详细诊断信息
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>诊断信息字符串</returns>
        public static string GetDicomDiagnosticInfo(string filePath)
        {
            var info = new StringBuilder();
            info.AppendLine("=== DICOM文件诊断信息 ===");
            
            try
            {
                if (!File.Exists(filePath))
                {
                    info.AppendLine("❌ 文件不存在");
                    return info.ToString();
                }

                var fileInfo = new FileInfo(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var isUidFormat = IsDicomUidFormat(fileName);
                
                info.AppendLine($"📁 文件路径: {filePath}");
                info.AppendLine($"📏 文件大小: {fileInfo.Length:N0} 字节");
                info.AppendLine($"📅 创建时间: {fileInfo.CreationTime}");
                info.AppendLine($"📅 修改时间: {fileInfo.LastWriteTime}");
                info.AppendLine($"🏷️ 文件名: {fileName}");
                info.AppendLine($"🔢 UID格式: {(isUidFormat ? "是" : "否")}");

                // 检查文件头
                info.AppendLine("\n🔍 文件头检查:");
                if (fileInfo.Length < 128)
                {
                    info.AppendLine("❌ 文件太小，不是有效的DICOM文件");
                    return info.ToString();
                }

                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var buffer = new byte[132];
                fs.Read(buffer, 0, buffer.Length);

                // 检查DICM标识
                bool hasDicomHeader = true;
                for (int i = 128; i < 132; i++)
                {
                    if (buffer[i] != "DICM"[i - 128])
                    {
                        hasDicomHeader = false;
                        break;
                    }
                }

                if (hasDicomHeader)
                {
                    info.AppendLine("✅ 发现DICOM文件头标识 (DICM)");
                }
                else
                {
                    info.AppendLine("⚠️ 未发现DICOM文件头标识，可能是无头DICOM文件");
                }

                // 尝试读取DICOM文件
                info.AppendLine("\n🔧 DICOM读取测试:");
                try
                {
                    DicomFile? dicomFile = null;
                    
                    if (isUidFormat)
                    {
                        // 对于UID格式的文件，直接使用大文件模式读取
                        dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand);
                        info.AppendLine("✅ UID格式文件读取成功（大文件模式）");
                    }
                    else
                    {
                        // 标准DICOM文件读取
                        dicomFile = DicomFile.Open(filePath);
                        info.AppendLine("✅ 标准DICOM读取成功");
                    }
                    
                    var dataset = dicomFile.Dataset;
                    if (dataset != null)
                    {
                        info.AppendLine("✅ DICOM数据集读取成功");
                        
                        // 检查关键标签
                        info.AppendLine("\n📋 关键标签检查:");
                        info.AppendLine($"   Modality: {dataset.GetSingleValueOrDefault(DicomTag.Modality, "未知")}");
                        info.AppendLine($"   Columns: {dataset.GetSingleValueOrDefault(DicomTag.Columns, 0)}");
                        info.AppendLine($"   Rows: {dataset.GetSingleValueOrDefault(DicomTag.Rows, 0)}");
                        info.AppendLine($"   Bits Allocated: {dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, 0)}");
                        info.AppendLine($"   Has Pixel Data: {(dataset.Contains(DicomTag.PixelData) ? "是" : "否")}");
                        
                        // 如果是UID格式，显示更多信息
                        if (isUidFormat)
                        {
                            info.AppendLine($"   Study Instance UID: {dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "未知")}");
                            info.AppendLine($"   Series Instance UID: {dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, "未知")}");
                            info.AppendLine($"   SOP Instance UID: {dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, "未知")}");
                        }
                    }
                    else
                    {
                        info.AppendLine("❌ DICOM数据集为空");
                    }
                }
                catch (DicomReaderException ex)
                {
                    info.AppendLine($"❌ 标准DICOM读取失败: {ex.Message}");
                    
                    // 尝试其他方法
                    try
                    {
                        var dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand);
                        info.AppendLine("✅ 大文件模式读取成功");
                    }
                    catch (Exception ex2)
                    {
                        info.AppendLine($"❌ 大文件模式读取失败: {ex2.Message}");
                        
                        try
                        {
                            // 尝试最后一种方法
                            var dicomFile = DicomFile.Open(filePath, FileReadOption.ReadLargeOnDemand);
                            info.AppendLine("✅ 大文件模式读取成功");
                        }
                        catch (Exception ex3)
                        {
                            info.AppendLine($"❌ 所有读取方法都失败: {ex3.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                info.AppendLine($"❌ 诊断过程中发生错误: {ex.Message}");
            }

            return info.ToString();
        }
    }
}
