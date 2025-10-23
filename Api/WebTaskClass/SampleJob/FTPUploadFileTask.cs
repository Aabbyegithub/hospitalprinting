using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebIServices.IServices.FTPIServices;
using ModelClassLibrary.Model.Dto;

namespace WebTaskClass.SampleJob
{
    public class FTPUploadFileTask : IJob
    {
        private readonly ISqlHelper _dal;
        private readonly ILoggerHelper _logger;
        private readonly IFTPService _FTPServices;
        private readonly IAppSettinghelper _appSetting;
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
                var foldersToUpload = GetFoldersToUpload();
                
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
        private List<FolderUploadInfo> GetFoldersToUpload()
        {
            var folders = new List<FolderUploadInfo>();
            
            try
            {
                // 获取所有子目录
                var directories = Directory.GetDirectories(_UpLoadFilePath, "*", SearchOption.TopDirectoryOnly);
                
                foreach (var directory in directories)
                {
                    var dirInfo = new DirectoryInfo(directory);
                    
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
                        LastModified = dirInfo.LastWriteTime
                    });
                }

                // 按修改时间排序，优先上传较新的文件夹
                folders = folders.OrderByDescending(f => f.LastModified).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取文件夹列表失败--{ex.Message}");
            }

            return folders;
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
                    // 上传成功，移动到备份目录
                    var folderName = Path.GetFileName(folderPath);
                    var backupFolderPath = Path.Combine(backupPath, $"{folderName}_{DateTime.Now:yyyyMMdd_HHmmss}");
                    
                    Directory.Move(folderPath, backupFolderPath);
                    await _logger.LogInfo($"文件夹已移动到备份目录: {backupFolderPath}", "FTPUploadTask");
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
