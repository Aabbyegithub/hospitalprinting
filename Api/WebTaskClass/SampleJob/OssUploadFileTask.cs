using Aliyun.OSS;
using Microsoft.Identity.Client;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using Quartz;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using static WebProjectTest.Common.Message;

namespace WebTaskClass.SampleJob
{
    public class OssUploadFileTask : IJob
    {
        private readonly ISqlHelper _dal;
        private readonly IAppSettinghelper _appconfig;

        public OssUploadFileTask(ISqlHelper dal, IAppSettinghelper appconfig)
        {
            _dal = dal;
            _appconfig = appconfig;
        }
        /// <summary>
        /// 获取OSS配置
        /// </summary>
        public async Task<ApiResponse<HolOssConfigDto>> GetConfigAsync(long orgId)
        {
            try
            {
                var config = await _dal.Db.Queryable<HolOssConfig>()
                    .Where(x => x.org_id == orgId)
                    .FirstAsync();

                if (config == null)
                {
                    return Success(new HolOssConfigDto
                    {
                        org_id = orgId,
                        is_enabled = 0
                    }, "获取配置成功");
                }

                var dto = new HolOssConfigDto
                {
                    id = config.id,
                    org_id = config.org_id,
                    is_enabled = config.is_enabled,
                    endpoint = config.endpoint,
                    access_key_id = config.access_key_id,
                    access_key_secret = config.access_key_secret,
                    bucket_name = config.bucket_name,
                    region = config.region,
                    folder_prefix = config.folder_prefix,
                    create_time = config.create_time,
                    update_time = config.update_time
                };

                return Success(dto, "获取配置成功");
            }
            catch (Exception e)
            {
                return Error<HolOssConfigDto>($"获取配置失败：{e.Message}");
            }
        }
        /// <summary>
        /// 批量上传未上传的文件到OSS
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var Bathcount = _appconfig.Get("Bathcount:Bathcount");
                // 获取OSS配置
                var configResponse = await GetConfigAsync(1);
                if (!configResponse.success || configResponse.Response == null)
                    return ;

                var config = configResponse.Response;
                if (config.is_enabled != 1)
                    return ;

                var result = new BatchUploadResult
                {
                    TotalCount = 0,
                    SuccessCount = 0,
                    FailedCount = 0,
                    SuccessFiles = new List<string>(),
                    FailedFiles = new List<FailedFileInfo>()
                };

                // 获取未上传的检查记录
                var unuploadedExams = await _dal.Db.Queryable<HolExamination>()
                    .Includes(x => x.patient)
                    .Where(x => x.org_id == 1 && x.is_upload == 0 && x.status == 1)
                    .OrderBy(x => x.create_time)
                    .ToListAsync();
                
                result.TotalCount = unuploadedExams.Count;

                if (result.TotalCount == 0)
                {
                    return ;
                }

                // 创建OSS客户端
                var client = new OssClient(config.endpoint, config.access_key_id, config.access_key_secret);

                // 分批处理
                for (int i = 0; i < unuploadedExams.Count; i += Bathcount.ObjToInt())
                {
                    var batch = unuploadedExams.Skip(i).Take(Bathcount.ObjToInt()).ToList();

                    foreach (var exam in batch)
                    {
                        try
                        {
                            // 处理报告文件
                            if (!string.IsNullOrEmpty(exam.report_path))
                            {
                                var reportResult = await UploadSingleFileAsync(exam, exam.report_path, "report", client, config);
                                if (reportResult.Success)
                                {
                                    exam.report_path = reportResult.Url;
                                }
                                else
                                {
                                    result.FailedFiles.Add(new FailedFileInfo
                                    {
                                        ExamId = exam.id,
                                        FilePath = exam.report_path,
                                        FileType = "report",
                                        ErrorMessage = reportResult.ErrorMessage
                                    });
                                    result.FailedCount++;
                                    continue;
                                }
                            }

                            // 处理图像文件
                            if (!string.IsNullOrEmpty(exam.image_path))
                            {
                                var imageResult = await UploadSingleFileAsync(exam, exam.image_path, "image", client, config);
                                if (imageResult.Success)
                                {
                                    exam.image_path = imageResult.Url;
                                }
                                else
                                {
                                    result.FailedFiles.Add(new FailedFileInfo
                                    {
                                        ExamId = exam.id,
                                        FilePath = exam.image_path,
                                        FileType = "image",
                                        ErrorMessage = imageResult.ErrorMessage
                                    });
                                    result.FailedCount++;
                                    continue;
                                }
                            }

                            // 更新上传状态
                            exam.is_upload = 1;
                            exam.update_time = DateTime.Now;
                            await _dal.Db.Updateable(exam).ExecuteCommandAsync();

                            result.SuccessCount++;
                            result.SuccessFiles.Add($"检查记录 {exam.exam_no} 上传成功");
                        }
                        catch (Exception ex)
                        {
                            result.FailedCount++;
                            result.FailedFiles.Add(new FailedFileInfo
                            {
                                ExamId = exam.id,
                                FilePath = "未知",
                                FileType = "unknown",
                                ErrorMessage = ex.Message
                            });
                        }
                    }
                }

                return ;
            }
            catch (Exception e)
            {
                return;
            }
        }    
        /// <summary>
        /// 获取未上传文件统计
        /// </summary>
        public async Task<ApiResponse<UploadStatistics>> GetUploadStatisticsAsync(long orgId)
        {
            try
            {
                var totalCount = await _dal.Db.Queryable<HolExamination>()
                    .Where(x => x.org_id == orgId && x.status == 1)
                    .CountAsync();

                var uploadedCount = await _dal.Db.Queryable<HolExamination>()
                    .Where(x => x.org_id == orgId && x.is_upload == 1 && x.status == 1)
                    .CountAsync();

                var unuploadedCount = totalCount - uploadedCount;

                var statistics = new UploadStatistics
                {
                    TotalCount = totalCount,
                    UploadedCount = uploadedCount,
                    UnuploadedCount = unuploadedCount,
                    UploadPercentage = totalCount > 0 ? (double)uploadedCount / totalCount * 100 : 0
                };

                return Success(statistics, "获取统计信息成功");
            }
            catch (Exception e)
            {
                return Error<UploadStatistics>($"获取统计信息失败：{e.Message}");
            }
        }
        // <summary>
        /// 上传单个文件
        /// </summary>
        private async Task<UploadFileResult> UploadSingleFileAsync(HolExamination exam, string filePath, string fileType, OssClient client, HolOssConfigDto config)
        {
            try
            {
                // 转换文件路径
                string actualFilePath = ConvertUrlToLocalPath(filePath);

                if (string.IsNullOrEmpty(actualFilePath) || !File.Exists(actualFilePath))
                {
                    return new UploadFileResult
                    {
                        Success = false,
                        ErrorMessage = $"文件不存在: {actualFilePath}"
                    };
                }

                // 生成OSS对象键
                var objectKey = await GenerateObjectKeyWithPatientName(exam, fileType, config.folder_prefix);

                // 上传文件
                var result = await Task.Run(() =>
                {
                    return client.PutObject(config.bucket_name, objectKey, actualFilePath);
                });

                if (result != null)
                {
                    var url = $"https://{config.bucket_name}.{config.endpoint.Replace("https://", "").Replace("http://", "")}/{objectKey}";
                    return new UploadFileResult
                    {
                        Success = true,
                        Url = url
                    };
                }
                else
                {
                    return new UploadFileResult
                    {
                        Success = false,
                        ErrorMessage = "文件上传失败"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UploadFileResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        /// <summary>
        /// 从URL路径转换为本地文件路径
        /// </summary>
        private string ConvertUrlToLocalPath(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return string.Empty;

                // 如果已经是本地路径，直接返回
                if (File.Exists(filePath))
                    return filePath;

                // 如果是URL路径，转换为本地路径
                if (filePath.StartsWith("http://") || filePath.StartsWith("https://"))
                {
                    var uri = new Uri(filePath);
                    var pathSegments = uri.AbsolutePath.TrimStart('/').Split('/');

                    // 尝试Windows路径
                    var windowsPath = Path.Combine("D:/uploads/images", string.Join("/", pathSegments));
                    if (File.Exists(windowsPath))
                        return windowsPath;

                    // 尝试Linux路径
                    var linuxPath = Path.Combine("/var/uploads/images", string.Join("/", pathSegments));
                    if (File.Exists(linuxPath))
                        return linuxPath;
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        // <summary>
        /// 生成OSS对象键（使用患者姓名和日期）
        /// </summary>
        private async Task<string> GenerateObjectKeyWithPatientName(HolExamination exam, string fileType, string folderPrefix)
        {
            try
            {
                if (exam?.patient != null)
                {
                    // 使用患者姓名和日期生成文件名
                    var patientName = exam.patient.name ?? "Unknown";
                    var examDate = exam.exam_date.ToString("yyyyMMdd");
                    var fileExtension = Path.GetExtension(exam.report_path ?? exam.image_path ?? ".jpg");
                    var newFileName = $"{patientName}_{examDate}_{exam.id}_{fileType}{fileExtension}";

                    return $"{folderPrefix ?? "examinations"}/{exam.id}/{newFileName}";
                }
                else
                {
                    // 如果无法获取患者信息，使用原始文件名
                    var originalFileName = Path.GetFileName(exam.report_path ?? exam.image_path ?? "unknown.jpg");
                    return $"{folderPrefix ?? "examinations"}/{exam.id}/{fileType}_{originalFileName}";
                }
            }
            catch (Exception)
            {
                // 出错时使用默认文件名
                return $"{folderPrefix ?? "examinations"}/{exam.id}/{fileType}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
            }
        }


        /// <summary>
        /// 批量上传结果
        /// </summary>
        public class BatchUploadResult
        {
            /// <summary>
            /// 总数量
            /// </summary>
            public int TotalCount { get; set; }

            /// <summary>
            /// 成功数量
            /// </summary>
            public int SuccessCount { get; set; }

            /// <summary>
            /// 失败数量
            /// </summary>
            public int FailedCount { get; set; }

            /// <summary>
            /// 成功文件列表
            /// </summary>
            public List<string> SuccessFiles { get; set; } = new List<string>();

            /// <summary>
            /// 失败文件列表
            /// </summary>
            public List<FailedFileInfo> FailedFiles { get; set; } = new List<FailedFileInfo>();
        }

        /// <summary>
        /// 失败文件信息
        /// </summary>
        public class FailedFileInfo
        {
            /// <summary>
            /// 检查记录ID
            /// </summary>
            public long ExamId { get; set; }

            /// <summary>
            /// 文件路径
            /// </summary>
            public string FilePath { get; set; } = string.Empty;

            /// <summary>
            /// 文件类型
            /// </summary>
            public string FileType { get; set; } = string.Empty;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string ErrorMessage { get; set; } = string.Empty;
        }

        /// <summary>
        /// 上传统计信息
        /// </summary>
        public class UploadStatistics
        {
            /// <summary>
            /// 总数量
            /// </summary>
            public int TotalCount { get; set; }

            /// <summary>
            /// 已上传数量
            /// </summary>
            public int UploadedCount { get; set; }

            /// <summary>
            /// 未上传数量
            /// </summary>
            public int UnuploadedCount { get; set; }

            /// <summary>
            /// 上传百分比
            /// </summary>
            public double UploadPercentage { get; set; }
        }

        /// <summary>
        /// 单文件上传结果
        /// </summary>
        public class UploadFileResult
        {
            /// <summary>
            /// 是否成功
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// 文件URL
            /// </summary>
            public string Url { get; set; } = string.Empty;

            /// <summary>
            /// 错误信息
            /// </summary>
            public string ErrorMessage { get; set; } = string.Empty;
        }
    }
}
