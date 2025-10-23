using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelClassLibrary.Model.Dto;
using WebIServices.IServices.FTPIServices;
using WebIServices.IBase;

namespace WebServiceClass.Services.FTPServices
{
    /// <summary>
    /// FTP服务实现
    /// </summary>
    public class FTPService : IFTPService, IBaseService
    {
        private readonly ILogger<FTPService> _logger;
        private readonly ISqlHelper _dal;
        private readonly IConfiguration _configuration;

        public FTPService(ILogger<FTPService> logger, ISqlHelper dal, IConfiguration configuration)
        {
            _logger = logger;
            _dal = dal;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取FTP配置
        /// </summary>
        public async Task<FTPConfigDto> GetConfigAsync(long orgId)
        {
            try
            {
                //// 从数据库获取FTP配置
                //var sql = @"
                //    SELECT Host, Port, Username, Password, UsePassive, UseSsl, 
                //           RemoteDirectory, Timeout, IsEnabled
                //    FROM HolFTPConfig 
                //    WHERE OrgId = @OrgId AND IsEnabled = 1";

                //var config = await _dal.QueryFirstOrDefaultAsync<FTPConfigDto>(sql, new { OrgId = orgId });

                //if (config == null)
                //{
                //    // 返回默认配置
                //    return new FTPConfigDto
                //    {
                //        Host = _configuration["FTP:Host"] ?? "localhost",
                //        Port = _configuration.GetValue<int>("FTP:Port", 21),
                //        Username = _configuration["FTP:Username"] ?? "",
                //        Password = _configuration["FTP:Password"] ?? "",
                //        UsePassive = _configuration.GetValue<bool>("FTP:UsePassive", true),
                //        UseSsl = _configuration.GetValue<bool>("FTP:UseSsl", false),
                //        RemoteDirectory = _configuration["FTP:RemoteDirectory"] ?? "/",
                //        Timeout = _configuration.GetValue<int>("FTP:Timeout", 30),
                //        IsEnabled = true
                //    };
                //}

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取FTP配置失败: OrgId={orgId}");
                throw;
            }
        }

        /// <summary>
        /// 测试FTP连接
        /// </summary>
        public async Task<FTPConnectionTestDto> TestConnectionAsync(FTPConfigDto config)
        {
            var result = new FTPConnectionTestDto();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (var client = CreateFtpClient(config))
                {
                    // 测试连接
                    await Task.Run(() =>
                    {
                        client.Connect();
                        result.ServerInfo = client.SystemType;
                    });

                    stopwatch.Stop();
                    result.IsConnected = true;
                    result.ConnectionTimeMs = stopwatch.ElapsedMilliseconds;
                    result.TestTime = DateTime.Now;

                    _logger.LogInformation($"FTP连接测试成功: {config.Host}:{config.Port}, 耗时: {result.ConnectionTimeMs}ms");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.IsConnected = false;
                result.ConnectionTimeMs = stopwatch.ElapsedMilliseconds;
                result.ErrorMessage = ex.Message;
                result.TestTime = DateTime.Now;

                _logger.LogError(ex, $"FTP连接测试失败: {config.Host}:{config.Port}");
            }

            return result;
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        public async Task<FTPBatchUploadResultDto> BatchUploadAsync(FTPUploadRequestDto request)
        {
            var result = new FTPBatchUploadResultDto
            {
                TotalFiles = request.FilePaths.Count
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // 获取FTP配置
                var config = await GetConfigAsync(request.OrgId);
                if (!config.IsEnabled)
                {
                    throw new InvalidOperationException("FTP服务未启用");
                }

                // 验证文件存在
                var validFiles = request.FilePaths.Where(File.Exists).ToList();
                if (validFiles.Count == 0)
                {
                    throw new FileNotFoundException("没有找到有效的文件");
                }

                _logger.LogInformation($"开始批量上传 {validFiles.Count} 个文件到FTP服务器");

                // 使用信号量控制并发数量
                using var semaphore = new SemaphoreSlim(request.MaxConcurrentUploads, request.MaxConcurrentUploads);
                var uploadTasks = validFiles.Select(async filePath =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await UploadSingleFileInternalAsync(filePath, request, config);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var uploadResults = await Task.WhenAll(uploadTasks);

                stopwatch.Stop();
                result.TotalDurationMs = stopwatch.ElapsedMilliseconds;
                result.Results = uploadResults.ToList();
                result.SuccessCount = uploadResults.Count(r => r.Success);
                result.FailureCount = uploadResults.Count(r => !r.Success);

                if (result.SuccessCount > 0)
                {
                    var totalSize = uploadResults.Where(r => r.Success).Sum(r => r.FileSize);
                    result.AverageSpeedKbps = totalSize / (result.TotalDurationMs / 1000.0) / 1024.0;
                }

                _logger.LogInformation($"批量上传完成: 成功 {result.SuccessCount}/{result.TotalFiles}, 耗时: {result.TotalDurationMs}ms");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.TotalDurationMs = stopwatch.ElapsedMilliseconds;
                _logger.LogError(ex, "批量上传失败");
                throw;
            }

            return result;
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        public async Task<FTPUploadResultDto> UploadSingleFileAsync(string filePath, string remotePath, long orgId)
        {
            var config = await GetConfigAsync(orgId);
            var request = new FTPUploadRequestDto
            {
                FilePaths = new List<string> { filePath },
                RemoteRootDirectory = remotePath,
                OrgId = orgId
            };

            var results = await BatchUploadAsync(request);
            return results.Results.FirstOrDefault() ?? new FTPUploadResultDto
            {
                Success = false,
                FilePath = filePath,
                ErrorMessage = "上传失败"
            };
        }

        /// <summary>
        /// 上传文件夹
        /// </summary>
        public async Task<FTPBatchUploadResultDto> UploadFolderAsync(string localFolderPath, string remoteFolderPath, long orgId, bool recursive = true)
        {
            if (!Directory.Exists(localFolderPath))
            {
                throw new DirectoryNotFoundException($"本地文件夹不存在: {localFolderPath}");
            }

            var files = GetFilesRecursively(localFolderPath, recursive);
            var request = new FTPUploadRequestDto
            {
                FilePaths = files,
                LocalRootDirectory = localFolderPath,
                RemoteRootDirectory = remoteFolderPath,
                KeepDirectoryStructure = true,
                OrgId = orgId
            };

            return await BatchUploadAsync(request);
        }

        /// <summary>
        /// 检查远程文件是否存在
        /// </summary>
        public async Task<bool> FileExistsAsync(string remotePath, long orgId)
        {
            try
            {
                var config = await GetConfigAsync(orgId);
                using var client = CreateFtpClient(config);
                await Task.Run(() => client.Connect());
                return await Task.Run(() => client.FileExists(remotePath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"检查远程文件是否存在失败: {remotePath}");
                return false;
            }
        }

        /// <summary>
        /// 删除远程文件
        /// </summary>
        public async Task<bool> DeleteFileAsync(string remotePath, long orgId)
        {
            try
            {
                var config = await GetConfigAsync(orgId);
                using var client = CreateFtpClient(config);
                await Task.Run(() => client.Connect());
                await Task.Run(() => client.DeleteFile(remotePath));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除远程文件失败: {remotePath}");
                return false;
            }
        }

        /// <summary>
        /// 列出远程目录内容
        /// </summary>
        public async Task<List<string>> ListDirectoryAsync(string remotePath, long orgId)
        {
            try
            {
                var config = await GetConfigAsync(orgId);
                using var client = CreateFtpClient(config);
                await Task.Run(() => client.Connect());
                var items = await Task.Run(() => client.GetListing(remotePath));
                return items.Select(item => item.Name).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"列出远程目录失败: {remotePath}");
                return new List<string>();
            }
        }

        /// <summary>
        /// 创建远程目录
        /// </summary>
        public async Task<bool> CreateDirectoryAsync(string remotePath, long orgId)
        {
            try
            {
                var config = await GetConfigAsync(orgId);
                using var client = CreateFtpClient(config);
                await Task.Run(() => client.Connect());
                await Task.Run(() => client.CreateDirectory(remotePath));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建远程目录失败: {remotePath}");
                return false;
            }
        }

        #region 私有方法

        /// <summary>
        /// 创建FTP客户端
        /// </summary>
        private FluentFTP.FtpClient CreateFtpClient(FTPConfigDto config)
        {
            var client = new FluentFTP.FtpClient(config.Host, config.Username, config.Password, config.Port);
            client.Config.EncryptionMode = config.UseSsl ? FluentFTP.FtpEncryptionMode.Explicit : FluentFTP.FtpEncryptionMode.None;
            client.Config.DataConnectionType = config.UsePassive ? FluentFTP.FtpDataConnectionType.PASV : FluentFTP.FtpDataConnectionType.PORT;
            client.Config.ConnectTimeout = config.Timeout * 1000;
            client.Config.ReadTimeout = config.Timeout * 1000;
            client.Config.DataConnectionConnectTimeout = config.Timeout * 1000;
            client.Config.DataConnectionReadTimeout = config.Timeout * 1000;
            return client;
        }

        /// <summary>
        /// 内部上传单个文件方法
        /// </summary>
        private async Task<FTPUploadResultDto> UploadSingleFileInternalAsync(string filePath, FTPUploadRequestDto request, FTPConfigDto config)
        {
            var result = new FTPUploadResultDto
            {
                FilePath = filePath,
                UploadTime = DateTime.Now
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var fileInfo = new FileInfo(filePath);
                result.FileSize = fileInfo.Length;

                // 计算远程路径
                var remotePath = CalculateRemotePath(filePath, request);

                using var client = CreateFtpClient(config);
                await Task.Run(() => client.Connect());

                // 确保远程目录存在
                var remoteDir = Path.GetDirectoryName(remotePath).Replace("\\", "/");
                if (!string.IsNullOrEmpty(remoteDir) && remoteDir != "/")
                {
                    await Task.Run(() => client.CreateDirectory(remoteDir));
                }

                // 检查文件是否已存在
                if (!request.OverwriteExisting && await Task.Run(() => client.FileExists(remotePath)))
                {
                    result.Success = false;
                    result.ErrorMessage = "文件已存在且不允许覆盖";
                    return result;
                }

                // 上传文件
                var uploadResult = await Task.Run(() => client.UploadFile(filePath, remotePath, 
                    request.OverwriteExisting ? FluentFTP.FtpRemoteExists.Overwrite : FluentFTP.FtpRemoteExists.Skip));

                stopwatch.Stop();
                result.DurationMs = stopwatch.ElapsedMilliseconds;
                result.RemotePath = remotePath;

                if (uploadResult == FluentFTP.FtpStatus.Success)
                {
                    result.Success = true;
                    result.UploadSpeedKbps = result.FileSize / (result.DurationMs / 1000.0) / 1024.0;
                    _logger.LogInformation($"文件上传成功: {filePath} -> {remotePath}, 耗时: {result.DurationMs}ms");
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"上传失败: {uploadResult}";
                    _logger.LogWarning($"文件上传失败: {filePath}, 状态: {uploadResult}");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.DurationMs = stopwatch.ElapsedMilliseconds;
                _logger.LogError(ex, $"上传文件异常: {filePath}");
            }

            return result;
        }

        /// <summary>
        /// 计算远程路径
        /// </summary>
        private string CalculateRemotePath(string filePath, FTPUploadRequestDto request)
        {
            var fileName = Path.GetFileName(filePath);
            var remotePath = request.RemoteRootDirectory?.TrimEnd('/') + "/" + fileName;

            if (request.KeepDirectoryStructure && !string.IsNullOrEmpty(request.LocalRootDirectory))
            {
                var relativePath = Path.GetRelativePath(request.LocalRootDirectory, filePath);
                var relativeDir = Path.GetDirectoryName(relativePath);
                if (!string.IsNullOrEmpty(relativeDir))
                {
                    remotePath = request.RemoteRootDirectory?.TrimEnd('/') + "/" + relativeDir.Replace("\\", "/") + "/" + fileName;
                }
            }

            return remotePath.Replace("\\", "/");
        }

        /// <summary>
        /// 递归获取文件夹中的所有文件
        /// </summary>
        private List<string> GetFilesRecursively(string folderPath, bool recursive)
        {
            var files = new List<string>();
            
            try
            {
                if (recursive)
                {
                    files.AddRange(Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories));
                }
                else
                {
                    files.AddRange(Directory.GetFiles(folderPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取文件夹文件失败: {folderPath}");
            }

            return files;
        }

        #endregion
    }
}
