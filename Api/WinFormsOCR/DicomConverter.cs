using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using System.Drawing;
using System.Drawing.Imaging;

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
                {
                    throw new FileNotFoundException($"DICOM文件不存在: {dicomFilePath}");
                }

                // 打开DICOM文件
                var dicomFile = DicomFile.Open(dicomFilePath);
                var dataset = dicomFile.Dataset;

                // 创建DICOM图像对象
                var dicomImage = new DicomImage(dataset);

                // 渲染图像为Bitmap - 使用AsBitmap()方法
                var bitmap = dicomImage.RenderImage().As<Bitmap>();

                // 如果图像是单色（灰度），转换为RGB格式以便OCR处理
                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    var rgbBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                    using (var g = Graphics.FromImage(rgbBitmap))
                    {
                        g.DrawImage(bitmap, 0, 0);
                    }
                    bitmap.Dispose();
                    return rgbBitmap;
                }

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception($"DICOM文件转换失败: {ex.Message}", ex);
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

                var dicomFile = DicomFile.Open(dicomFilePath);
                var dataset = dicomFile.Dataset;

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
                info["图像宽度"] = dataset.GetSingleValueOrDefault(DicomTag.Columns, "未知").ToString();
                info["图像高度"] = dataset.GetSingleValueOrDefault(DicomTag.Rows, "未知").ToString();
                info["像素间距"] = dataset.GetSingleValueOrDefault(DicomTag.PixelSpacing, "未知").ToString();
                info["位深度"] = dataset.GetSingleValueOrDefault(DicomTag.BitsAllocated, "未知").ToString();

                // 获取医院信息
                info["医院名称"] = dataset.GetSingleValueOrDefault(DicomTag.InstitutionName, "未知");
                info["科室名称"] = dataset.GetSingleValueOrDefault(DicomTag.InstitutionalDepartmentName, "未知");
                info["操作员"] = dataset.GetSingleValueOrDefault(DicomTag.OperatorsName, "未知");
                info["检查医生"] = dataset.GetSingleValueOrDefault(DicomTag.PerformingPhysicianName, "未知");

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

                var dicomFile = DicomFile.Open(filePath);
                return dicomFile != null && dicomFile.Dataset != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
