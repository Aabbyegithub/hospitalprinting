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
        private readonly ILoggerHelper _logger;

        public DicomFileParserService(ISqlHelper dal, ILoggerHelper logger)
        {
            _dal = dal;
            _logger = logger;
        }

        /// <summary>
        /// 解析DICOM文件并保存到数据库
        /// </summary>
        /// <param name="filePath">DICOM文件路径</param>
        /// <returns>解析结果</returns>
        public async Task<HolDicomParsedData> ParsePatientInfoAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    await _logger.LogWarning($"DICOM文件不存在: {filePath}");
                    return null;
                }

                var dicomFile = await DicomFile.OpenAsync(filePath);
                var dataset = dicomFile.Dataset;
                var fileInfo = new FileInfo(filePath);

                var parsedData = new HolDicomParsedData
                {
                    file_path = filePath,
                    file_name = fileInfo.Name,
                    parse_time = DateTime.Now,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    is_deleted = false
                };

                // 提取患者信息
                parsedData.patient_id = dataset.GetSingleValueOrDefault(DicomTag.PatientID, "");
                parsedData.patient_name = dataset.GetSingleValueOrDefault(DicomTag.PatientName, "");
                parsedData.patient_birth_date = dataset.GetSingleValueOrDefault(DicomTag.PatientBirthDate, "");
                parsedData.patient_sex = dataset.GetSingleValueOrDefault(DicomTag.PatientSex, "");
                parsedData.patient_age = dataset.GetSingleValueOrDefault(DicomTag.PatientAge, "");

                // 提取检查信息
                parsedData.study_instance_uid = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, "");
                parsedData.study_date = dataset.GetSingleValueOrDefault(DicomTag.StudyDate, "");
                parsedData.study_time = dataset.GetSingleValueOrDefault(DicomTag.StudyTime, "");
                parsedData.study_description = dataset.GetSingleValueOrDefault(DicomTag.StudyDescription, "");
                parsedData.study_id = dataset.GetSingleValueOrDefault(DicomTag.StudyID, "");
                parsedData.accession_number = dataset.GetSingleValueOrDefault(DicomTag.AccessionNumber, "");


                // 提取图像信息
                parsedData.sop_instance_uid = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, "");
                parsedData.instance_number = dataset.GetSingleValueOrDefault(DicomTag.InstanceNumber, "");
                parsedData.image_type = dataset.GetSingleValueOrDefault(DicomTag.ImageType, "");

                // 提取设备信息
                parsedData.institutional_department_name = dataset.GetSingleValueOrDefault(DicomTag.InstitutionalDepartmentName, "");
                parsedData.modality = dataset.GetSingleValueOrDefault(DicomTag.Modality, "");

                // 提取检查参数
                parsedData.study_comments = dataset.GetSingleValueOrDefault(DicomTag.StudyCommentsRETIRED, "");
                parsedData.study_status_id = dataset.GetSingleValueOrDefault(DicomTag.Status, "");
                parsedData.study_priority_id = dataset.GetSingleValueOrDefault(DicomTag.StudyPriorityIDRETIRED, "");
                parsedData.referring_physician_name = dataset.GetSingleValueOrDefault(DicomTag.ReferringPhysicianName, "");
                parsedData.performing_physician_name = dataset.GetSingleValueOrDefault(DicomTag.PerformingPhysicianName, "");
                parsedData.operator_name = dataset.GetSingleValueOrDefault(DicomTag.OperatorsName, "");

                // 保存到数据库
                var DicomId = await _dal.Db.Insertable(parsedData).ExecuteReturnBigIdentityAsync();

                var partient = await _dal.Db.Queryable<HolExamination>().FirstAsync(a => a.patientid == parsedData.patient_id && a.studyuid == parsedData.study_instance_uid);

                if (partient != null )
                {
                    partient.image_path = parsedData.file_path;
                    partient.ocr_identify_type = 1;
                    await _dal.Db.Updateable(partient).ExecuteCommandAsync();
                    await _dal.Db.Updateable<HolDicomParsedData>().SetColumns(a=>a.is_verify == true).Where(a=>a.id == DicomId).ExecuteCommandAsync();
                }

                await _logger.LogInfo($"DICOM数据解析并保存成功: {filePath}","DICOM文件解析");
                return parsedData;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"解析并保存DICOM数据失败: {filePath}--{ex.Message}");
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
                   await _logger.LogWarning($"DICOM文件不存在: {filePath}");
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
                       await _logger.LogError( $"无法读取DICOM标签 {tag.Key}-{ex.Message}");
                    }
                }

                // 添加文件信息
                var fileInfo = new FileInfo(filePath);
                metadata["FileName"] = fileInfo.Name;
                metadata["FileSize"] = fileInfo.Length;
                metadata["FileCreationTime"] = fileInfo.CreationTime;
                metadata["FileLastWriteTime"] = fileInfo.LastWriteTime;

                 await _logger.LogInfo($"提取DICOM元数据成功: {filePath}, 共{metadata.Count}个字段","DICOM文件解析");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"提取DICOM元数据失败: {filePath}-{ex.Message}");
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
                
                await _logger.LogInfo($"DICOM文件验证结果: {filePath}, 有效: {isValid}","DICOM文件解析");
                return isValid;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"DICOM文件验证失败: {filePath}-{ex.Message}");
                return false;
            }
        }
    }
}
