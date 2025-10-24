using MyNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IServices.DICOMIServices
{
    /// <summary>
    /// DICOM文件解析服务
    /// </summary>
    public interface IDicomFileParserService
    {
        /// <summary>
        /// 解析DICOM文件并提取患者信息
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <returns>患者信息</returns>
        Task<HolDicomParsedData?> ParsePatientInfoAsync(string filePath);

        /// <summary>
        /// 获取DICOM文件的元数据
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <returns>元数据字典</returns>
        Task<Dictionary<string, object>> GetDicomMetadataAsync(string filePath);

        /// <summary>
        /// 验证DICOM文件格式
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <returns>是否有效</returns>
        Task<bool> ValidateDicomFileAsync(string filePath);
    }
}
