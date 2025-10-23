using ModelClassLibrary.Model.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebIServices.IServices.FTPIServices
{
    /// <summary>
    /// FTP服务接口
    /// </summary>
    public interface IFTPService
    {
        /// <summary>
        /// 获取FTP配置
        /// </summary>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<FTPConfigDto> GetConfigAsync(long orgId);

        /// <summary>
        /// 测试FTP连接
        /// </summary>
        /// <param name="config">FTP配置</param>
        /// <returns></returns>
        Task<FTPConnectionTestDto> TestConnectionAsync(FTPConfigDto config);

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="request">上传请求</param>
        /// <returns></returns>
        Task<FTPBatchUploadResultDto> BatchUploadAsync(FTPUploadRequestDto request);

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="remotePath">远程路径</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<FTPUploadResultDto> UploadSingleFileAsync(string filePath, string remotePath, long orgId);

        /// <summary>
        /// 上传文件夹
        /// </summary>
        /// <param name="localFolderPath">本地文件夹路径</param>
        /// <param name="remoteFolderPath">远程文件夹路径</param>
        /// <param name="orgId">组织ID</param>
        /// <param name="recursive">是否递归上传子文件夹</param>
        /// <returns></returns>
        Task<FTPBatchUploadResultDto> UploadFolderAsync(string localFolderPath, string remoteFolderPath, long orgId, bool recursive = true);

        /// <summary>
        /// 检查远程文件是否存在
        /// </summary>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<bool> FileExistsAsync(string remotePath, long orgId);

        /// <summary>
        /// 删除远程文件
        /// </summary>
        /// <param name="remotePath">远程文件路径</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<bool> DeleteFileAsync(string remotePath, long orgId);

        /// <summary>
        /// 列出远程目录内容
        /// </summary>
        /// <param name="remotePath">远程目录路径</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<List<string>> ListDirectoryAsync(string remotePath, long orgId);

        /// <summary>
        /// 创建远程目录
        /// </summary>
        /// <param name="remotePath">远程目录路径</param>
        /// <param name="orgId">组织ID</param>
        /// <returns></returns>
        Task<bool> CreateDirectoryAsync(string remotePath, long orgId);
    }
}
