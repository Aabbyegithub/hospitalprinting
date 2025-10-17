using FellowOakDicom;
using FellowOakDicom.Imaging;
using iText.IO.Source;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.Services.DICOMServices
{
    public  class DICOMHelper
    {
        /// <summary>
        /// 解析 DICOM 文件并获取指定标签的内容
        /// </summary>
        /// <param name="filePath">DICOM 文件路径</param>
        /// <param name="tags">要获取的 DICOM 标签列表</param>
        /// <returns>包含标签名和对应值的字典</returns>
        public Dictionary<string, string> ParseDicomFile(string filePath, List<DicomTag> tags)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("DICOM 文件不存在", filePath);
                }

                DicomFile dicomFile = DicomFile.Open(filePath);
                DicomDataset dataset = dicomFile.Dataset;

                foreach (var tag in tags)
                {
                    string tagName = tag.ToString();
                    if (dataset.Contains(tag))
                    {
                        result[tagName] = dataset.GetString(tag);
                    }
                    else
                    {
                        result[tagName] = "标签不存在";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"解析 DICOM 文件时发生错误：{ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// 将 DICOM 图像转换为字节数组（用于显示或保存为其他格式）
        /// </summary>
        /// <param name="filePath">DICOM 文件路径</param>
        /// <returns>图像的字节数组，若失败则为 null</returns>
        public byte[] ConvertDicomImageToBytes(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("DICOM 文件不存在", filePath);
                }

                DicomFile dicomFile = DicomFile.Open(filePath);
                DicomImage dicomImage = new DicomImage(dicomFile.Dataset);
                var bitmap =dicomImage.RenderImage().As<Bitmap>();

                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"转换 DICOM 图像时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取 DICOM 文件中的所有标签信息
        /// </summary>
        /// <param name="filePath">DICOM 文件路径</param>
        /// <returns>包含所有标签名和对应值的字典</returns>
        public Dictionary<string, string> GetAllTags(string filePath)
        {
            Dictionary<string, string> allTags = new Dictionary<string, string>();
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("DICOM 文件不存在", filePath);
                }

                DicomFile dicomFile = DicomFile.Open(filePath);
                DicomDataset dataset = dicomFile.Dataset;

                foreach (DicomItem item in dataset)
                {
                    string tagName = item.Tag.ToString();
                    string value = string.Empty;

                    if (item is DicomElement element)
                    {
                        if (element.Buffer is ByteBuffer byteBuffer)
                        {
                            // 简单处理字节数据，实际场景可能需要更复杂的解析
                            byte[] dataArray = byteBuffer.ToByteArray();
                            value = string.Join(", ", dataArray);
                        }
                        else
                        {
                            value = element.Get<string>();
                        }
                    }

                    allTags[tagName] = value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取所有标签时发生错误：{ex.Message}");
            }
            return allTags;
        }
    }
}
