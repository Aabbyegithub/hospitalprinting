using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.FTPIServices;
using ModelClassLibrary.Model.Dto;
using SqlSugar.Extensions;

namespace WebTaskClass.SampleJob
{
    public class FTPUploadFileTask : IJob
    {
        private readonly ISqlHelper _dal;
        private readonly ILoggerHelper _logger;
        private readonly IFTPService _FTPServices;
        private readonly IAppSettinghelper _appSetting;
        private int _UpLoadTimeSpacingFile;
        private int _UpLoadFile;
        /// <summary>
        /// 上传本地文件目录
        /// </summary>
        private readonly string _UpLoadFilePath =  Path.Combine(AppContext.BaseDirectory, "ReceivedDicom");

        public FTPUploadFileTask(ISqlHelper dal, ILoggerHelper logger, IFTPService FTPServices, IAppSettinghelper appSetting)
        {
            _dal = dal;
            _logger = logger;
            _FTPServices = FTPServices;
            _appSetting = appSetting;
            _UpLoadFile = _appSetting.Get("FTP:UpLoadFile").ObjToInt();
            _UpLoadTimeSpacingFile = _appSetting.Get("FTP:UpLoadTimeSpacingFile").ObjToInt();
        }

        /// <summary>
        /// 定时将范围内的文件上传FTP服务器-采用批量上传文件夹的方式（按日期）
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _logger.LogInfo("开始执行FTP批量上传任务", "FTPUploadTask");

                // 检查本地目录是否存在
                if (!Directory.Exists(_UpLoadFilePath))
                {
                    await _logger.LogWarning($"本地目录不存在: {_UpLoadFilePath}");
                    return;
                }
                
                // 获取FTP配置
                var ftpConfig = await _FTPServices.GetConfigAsync();
                if (ftpConfig == null || !ftpConfig.IsEnabled)
                {
                    await _logger.LogWarning("FTP服务未配置或未启用");
                    return;
                }

                // 获取需要上传的文件夹列表（按日期分组）
                var foldersToUpload =await GetFoldersToUpload();
                
                if (foldersToUpload.Count == 0)
                {
                    await _logger.LogInfo("没有找到需要上传的文件夹", "FTPUploadTask");
                    return;
                }

                await _logger.LogInfo($"找到 {foldersToUpload.Count} 个文件夹需要上传", "FTPUploadTask");

                // 批量上传文件夹
                var totalSuccess = 0;
                var totalFailure = 0;

                foreach (var folderInfo in foldersToUpload)
                {
                    try
                    {
                        await _logger.LogInfo($"开始上传文件夹: {folderInfo.LocalPath}", "FTPUploadTask");
                        
                        // 使用文件夹上传功能
                        var result = await _FTPServices.UploadFolderAsync(
                            folderInfo.LocalPath, 
                            folderInfo.RemotePath, 
                            true // 递归上传子文件夹
                        );

                        if (result.AllSuccess)
                        {
                            totalSuccess++;
                            await _logger.LogInfo($"文件夹上传成功: {folderInfo.LocalPath}, 文件数: {result.TotalFiles}", "FTPUploadTask");
                            
                            // 上传成功后，可以选择删除本地文件或移动到备份目录
                            await HandleUploadedFolder(folderInfo.LocalPath, true);
                        }
                        else
                        {
                            totalFailure++;
                            await _logger.LogWarning($"文件夹上传失败: {folderInfo.LocalPath}, 成功: {result.SuccessCount}/{result.TotalFiles}");
                            
                            // 上传失败，保留本地文件
                            await HandleUploadedFolder(folderInfo.LocalPath, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        totalFailure++;
                        await _logger.LogError($"上传文件夹异常: {folderInfo.LocalPath}-{ex.Message}");
                    }
                }

                await _logger.LogInfo($"FTP批量上传任务完成: 成功 {totalSuccess} 个文件夹, 失败 {totalFailure} 个文件夹", "FTPUploadTask");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"获取文件夹列表失败--{ex.Message}");
            }
        }

        /// <summary>
        /// 获取需要上传的文件夹列表
        /// </summary>
        private async Task<List<FolderUploadInfo>> GetFoldersToUpload()
        {
            var folders = new List<FolderUploadInfo>();
            
            try
            {
                // 计算日期范围
                var startDate = DateTime.Now.AddDays(-_UpLoadTimeSpacingFile-_UpLoadFile);
                var endDate = DateTime.Now.AddDays(-_UpLoadFile);
                
                await _logger.LogInfo($"开始扫描文件夹，日期范围: {startDate:yyyy-MM-dd} 到 {endDate:yyyy-MM-dd}");
                
                // 获取所有子目录
                var directories = Directory.GetDirectories(_UpLoadFilePath, "*", SearchOption.TopDirectoryOnly);
                
                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);
                    
                    // 从文件夹名称解析日期
                    var folderDate =await ParseDateFromFolderName(dirInfo.Name);
                    if (folderDate == null)
                    {
                        continue; // 跳过无法解析日期的文件夹
                    }
                    
                    // 检查目录是否在指定日期范围内
                    if (folderDate.Value < startDate || folderDate.Value > endDate)
                    {
                        continue; // 跳过不在日期范围内的目录
                    }
                    
                    // 检查目录是否包含文件
                    var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        continue; // 跳过空目录
                    }

                    // 按日期创建远程路径
                    var remotePath = $"/uploads/{dirInfo.Name}";
                    
                    folders.Add(new FolderUploadInfo
                    {
                        LocalPath = directory,
                        RemotePath = remotePath,
                        FolderName = dirInfo.Name,
                        FileCount = files.Length,
                        LastModified = folderDate.Value
                    });
                }

                // 按修改时间排序，优先上传较新的文件夹
                folders = folders.OrderByDescending(f => f.LastModified).ToList();
                
                // 根据配置限制上传数量
                if (_UpLoadFile > 0 && folders.Count > _UpLoadFile)
                {
                    await _logger.LogInfo($"找到 {folders.Count} 个文件夹，根据配置限制为 {_UpLoadFile} 个");
                    folders = folders.Take(_UpLoadFile).ToList();
                }
                
                await _logger.LogInfo($"最终确定上传 {folders.Count} 个文件夹");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"获取文件夹列表失败--{ex.Message}");
            }

            return folders;
        }

        /// <summary>
        /// 从文件夹名称解析日期
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <returns>解析出的日期，如果解析失败返回null</returns>
        private async Task<DateTime?> ParseDateFromFolderName(string folderName)
        {
            try
            {
                // 常见的日期格式
                var dateFormats = new[]
                {
                    "yyyyMMdd",           // 20240101
                    "yyyy-MM-dd",         // 2024-01-01
                    "yyyy_MM_dd",         // 2024_01_01
                    "yyyy.MM.dd",         // 2024.01.01
                    "yyyy/MM/dd",         // 2024/01/01
                    "MMddyyyy",           // 01012024
                    "ddMMyyyy",           // 01012024
                    "yyyyMMdd_HHmmss",    // 20240101_143022
                    "yyyy-MM-dd_HH-mm-ss" // 2024-01-01_14-30-22
                };

                // 尝试直接解析整个文件夹名称
                if (DateTime.TryParseExact(folderName, dateFormats, null, System.Globalization.DateTimeStyles.None, out var directDate))
                {
                    return directDate;
                }

                // 尝试从文件夹名称中提取日期部分
                // 支持格式：20240101_Patient001, 2024-01-01_Study001 等
                var patterns = new[]
                {
                    @"^(\d{8})",                    // 8位数字开头
                    @"^(\d{4}-\d{2}-\d{2})",        // yyyy-MM-dd格式
                    @"^(\d{4}_\d{2}_\d{2})",        // yyyy_MM_dd格式
                    @"^(\d{4}\.\d{2}\.\d{2})",      // yyyy.MM.dd格式
                    @"^(\d{4}/\d{2}/\d{2})",        // yyyy/MM/dd格式
                    @"^(\d{8}_\d{6})",              // yyyyMMdd_HHmmss格式
                    @"^(\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2})" // yyyy-MM-dd_HH-mm-ss格式
                };

                foreach (var pattern in patterns)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(folderName, pattern);
                    if (match.Success)
                    {
                        var dateString = match.Groups[1].Value;
                        
                        // 标准化日期格式
                        if (dateString.Length == 8 && dateString.All(char.IsDigit))
                        {
                            // yyyyMMdd 格式
                            dateString = $"{dateString.Substring(0, 4)}-{dateString.Substring(4, 2)}-{dateString.Substring(6, 2)}";
                        }
                        else if (dateString.Contains("_") && !dateString.Contains("-"))
                        {
                            // yyyy_MM_dd 格式转换为 yyyy-MM-dd
                            dateString = dateString.Replace("_", "-");
                        }
                        else if (dateString.Contains("."))
                        {
                            // yyyy.MM.dd 格式转换为 yyyy-MM-dd
                            dateString = dateString.Replace(".", "-");
                        }
                        else if (dateString.Contains("/"))
                        {
                            // yyyy/MM/dd 格式转换为 yyyy-MM-dd
                            dateString = dateString.Replace("/", "-");
                        }

                        if (DateTime.TryParse(dateString, out var parsedDate))
                        {
                            return parsedDate;
                        }
                    }
                }

                await _logger.LogWarning($"无法从文件夹名称解析日期: {folderName}");
                return null;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"解析文件夹日期异常: {folderName} - {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="sourceFolderPath">源文件夹路径</param>
        /// <param name="zipFilePath">压缩文件路径</param>
        private async Task CompressFolderAsync(string sourceFolderPath, string zipFilePath)
        {
            try
            {
                await _logger.LogInfo($"开始压缩文件夹: {sourceFolderPath} -> {zipFilePath}");
                
                // 确保目标目录存在
                var targetDir = Path.GetDirectoryName(zipFilePath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                
                // 删除已存在的压缩文件
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
                
                // 创建压缩文件
                ZipFile.CreateFromDirectory(sourceFolderPath, zipFilePath, CompressionLevel.Optimal, false);
                
                // 获取压缩文件大小
                var zipFileInfo = new FileInfo(zipFilePath);
                var originalSize = GetDirectorySize(sourceFolderPath);
                var compressedSize = zipFileInfo.Length;
                var compressionRatio = originalSize > 0 ? (double)compressedSize / originalSize * 100 : 0;
                
                await _logger.LogInfo($"压缩完成: 原大小 {FormatFileSize(originalSize)}, 压缩后 {FormatFileSize(compressedSize)}, 压缩率 {compressionRatio:F1}%");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"压缩文件夹失败: {sourceFolderPath} -> {zipFilePath} - {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns>文件夹大小（字节）</returns>
        private long GetDirectorySize(string directoryPath)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                return files.Sum(file => new FileInfo(file).Length);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的文件大小字符串</returns>
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 处理已上传的文件夹
        /// </summary>
        private async Task HandleUploadedFolder(string folderPath, bool uploadSuccess)
        {
            try
            {
                var backupPath = Path.Combine(AppContext.BaseDirectory, "BackupDicom");
                if (!Directory.Exists(backupPath))
                {
                    Directory.CreateDirectory(backupPath);
                }

                if (uploadSuccess)
                {
                    // 上传成功，移动到备份目录并压缩
                    var folderName = Path.GetFileName(folderPath);
                    var backupFolderPath = Path.Combine(backupPath, $"{folderName}_{DateTime.Now:yyyyMMdd_HHmmss}");
                    var zipFilePath = $"{backupFolderPath}.zip";
                    
                    // 先移动到备份目录
                    Directory.Move(folderPath, backupFolderPath);
                    await _logger.LogInfo($"文件夹已移动到备份目录: {backupFolderPath}", "FTPUploadTask");
                    
                    // 压缩文件夹
                    await CompressFolderAsync(backupFolderPath, zipFilePath);
                    await _logger.LogInfo($"文件夹已压缩: {zipFilePath}", "FTPUploadTask");
                    
                    // 删除原文件夹，只保留压缩文件
                    Directory.Delete(backupFolderPath, true);
                    await _logger.LogInfo($"原文件夹已删除，只保留压缩文件: {zipFilePath}", "FTPUploadTask");
                }
                else
                {
                    // 上传失败，保留原文件夹
                    await _logger.LogInfo($"文件夹上传失败，保留原文件夹: {folderPath}", "FTPUploadTask");
                }
            }
            catch (Exception ex)
            {
                await _logger.LogError($"处理已上传文件夹失败: {folderPath}-{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 文件夹上传信息
    /// </summary>
    public class FolderUploadInfo
    {
        /// <summary>
        /// 本地文件夹路径
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// 远程文件夹路径
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModified { get; set; }
    }

}
