using FellowOakDicom;
using FellowOakDicom.Imaging.Codec;
using Microsoft.Extensions.Logging;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using SqlSugar.Extensions;
using System.Text.Json;
using WebIServices.IBase;
using WebIServices.IServices.DICOMIServices;

namespace WebServiceClass.Services.DICOMServices
{
    public class DicomFileParserService : IDicomFileParserService,IBaseService
    {
        private readonly ISqlHelper _dal;
        private readonly ILogger<DicomFileParserService> _logger;

        public DicomFileParserService(ISqlHelper dal, ILogger<DicomFileParserService> logger)
        {
            _dal = dal;
            _logger = logger;
        }

        public async Task<HolPatient?> ParsePatientInfoAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"DICOM文件不存在: {filePath}");
                    return null;
                }

                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;

                var patient = new HolPatient
                {
                    //patient_id
                    medical_no = dataset.GetSingleValueOrDefault(DicomTag.PatientID, ""),
                    name = dataset.GetSingleValueOrDefault(DicomTag.PatientName, ""),
                    //patient_birth_date = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, ""),
                    gender = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, ""),
                    age = dataset.GetSingleValueOrDefault(DicomTag.PatientAge, "").ObjToInt(),
                    createtime = DateTime.Now,
                    updatetime = DateTime.Now
                };

                _logger.LogInformation($"解析患者信息成功: {patient.name} ({patient.medical_no})");
                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"解析患者信息失败: {filePath}");
                return null;
            }
        }

        public async Task<HolExamination?> ParseExaminationInfoAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"DICOM文件不存在: {filePath}");
                    return null;
                }

                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;

                var examination = new HolExamination
                {
                    //study_instance_uid = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, ""),
                    //series_instance_uid = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, ""),
                    //sop_instance_uid = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, ""),
                    //study_date = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, ""),
                    //study_time = dataset.GetSingleValueOrDefault(DicomTag.StudyTime, ""),
                    //modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, ""),
                    //study_description = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, ""),
                    //series_description = dataset.GetSingleValueOrDefault(DicomTag.SeriesDescription, ""),
                    //patient_id = dataset.GetSingleValueOrDefault(DicomTag.PatientID, ""),
                    //institution_name = dataset.GetSingleValueOrDefault(DicomTag.InstitutionName, ""),
                    //department_name = dataset.GetSingleValueOrDefault(DicomTag.InstitutionalDepartmentName, ""),
                    //physician_name = dataset.GetSingleValueOrDefault(DicomTag.ReferringPhysicianName, ""),
                    //create_time = DateTime.Now,
                    //update_time = DateTime.Now
                };

                //_logger.LogInformation($"解析检查信息成功: {examination.study_description} ({examination.modality})");
                return examination;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"解析检查信息失败: {filePath}");
                return null;
            }
        }

        public async Task<Dictionary<string, object>> GetDicomMetadataAsync(string filePath)
        {
            var metadata = new Dictionary<string, object>();

            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"DICOM文件不存在: {filePath}");
                    return metadata;
                }

                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;

                // 提取常用的DICOM标签
                var tags = new Dictionary<string, DicomTag>
                {
                    ["PatientID"] = DicomTag.PatientID,
                    ["PatientName"] = DicomTag.PatientName,
                    ["PatientBirthDate"] = DicomTag.PatientBirthDate,
                    ["PatientSex"] = DicomTag.PatientSex,
                    ["StudyInstanceUID"] = DicomTag.StudyInstanceUID,
                    ["SeriesInstanceUID"] = DicomTag.SeriesInstanceUID,
                    ["SOPInstanceUID"] = DicomTag.SOPInstanceUID,
                    ["StudyDate"] = DicomTag.StudyDate,
                    ["StudyTime"] = DicomTag.StudyTime,
                    ["Modality"] = DicomTag.Modality,
                    ["StudyDescription"] = DicomTag.StudyDescription,
                    ["SeriesDescription"] = DicomTag.SeriesDescription,
                    ["InstitutionName"] = DicomTag.InstitutionName,
                    ["Manufacturer"] = DicomTag.Manufacturer,
                    ["ManufacturerModelName"] = DicomTag.ManufacturerModelName,
                    ["SoftwareVersions"] = DicomTag.SoftwareVersions
                };

                foreach (var tag in tags)
                {
                    try
                    {
                        var value = dataset.GetSingleValueOrDefault(tag.Value, "");
                        if (!string.IsNullOrEmpty(value?.ToString()))
                        {
                            metadata[tag.Key] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, $"无法读取DICOM标签 {tag.Key}");
                    }
                }

                // 添加文件信息
                var fileInfo = new FileInfo(filePath);
                metadata["FileName"] = fileInfo.Name;
                metadata["FileSize"] = fileInfo.Length;
                metadata["FileCreationTime"] = fileInfo.CreationTime;
                metadata["FileLastWriteTime"] = fileInfo.LastWriteTime;

                _logger.LogInformation($"提取DICOM元数据成功: {filePath}, 共{metadata.Count}个字段");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"提取DICOM元数据失败: {filePath}");
            }

            return metadata;
        }

        public async Task<bool> ValidateDicomFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                // 尝试打开DICOM文件
                var dicomFile = await DicomFile.OpenAsync(filePath);
                
                // 检查是否包含必要的标签
                var dataset = dicomFile.Dataset;
                var hasPatientID = dataset.Contains(DicomTag.PatientID);
                var hasStudyInstanceUID = dataset.Contains(DicomTag.StudyInstanceUID);
                var hasSOPInstanceUID = dataset.Contains(DicomTag.SOPInstanceUID);

                var isValid = hasPatientID && hasStudyInstanceUID && hasSOPInstanceUID;
                
                _logger.LogInformation($"DICOM文件验证结果: {filePath}, 有效: {isValid}");
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DICOM文件验证失败: {filePath}");
                return false;
            }
        }
    }
}
