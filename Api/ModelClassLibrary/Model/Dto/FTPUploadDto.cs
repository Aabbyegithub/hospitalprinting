using System;
using System.Collections.Generic;

namespace ModelClassLibrary.Model.Dto
{
    /// <summary>
    /// 重复文件处理策略
    /// </summary>
    public enum DuplicateFileStrategy
    {
        /// <summary>
        /// 覆盖已存在的文件
        /// </summary>
        Overwrite = 0,

        /// <summary>
        /// 跳过已存在的文件
        /// </summary>
        Skip = 1,

        /// <summary>
        /// 重命名文件（添加时间戳）
        /// </summary>
        Rename = 2,

        /// <summary>
        /// 重命名文件（添加序号）
        /// </summary>
        RenameWithNumber = 3
    }
    /// <summary>
    /// FTP上传配置DTO
    /// </summary>
    public class FTPConfigDto
    {
        /// <summary>
        /// FTP服务器地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// FTP端口
        /// </summary>
        public int Port { get; set; } = 21;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否使用被动模式
        /// </summary>
        public bool UsePassive { get; set; } = true;

        /// <summary>
        /// 是否使用SSL/TLS
        /// </summary>
        public bool UseSsl { get; set; } = false;

        /// <summary>
        /// 远程目录
        /// </summary>
        public string RemoteDirectory { get; set; } = "/";

        /// <summary>
        /// 连接超时时间（秒）
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// FTP上传请求DTO
    /// </summary>
    public class FTPUploadRequestDto
    {
        /// <summary>
        /// 要上传的文件路径列表
        /// </summary>
        public List<string> FilePaths { get; set; } = new List<string>();

        /// <summary>
        /// 本地根目录（可选，用于保持目录结构）
        /// </summary>
        public string LocalRootDirectory { get; set; }

        /// <summary>
        /// 远程根目录
        /// </summary>
        public string RemoteRootDirectory { get; set; } = "/";

        /// <summary>
        /// 是否保持目录结构
        /// </summary>
        public bool KeepDirectoryStructure { get; set; } = true;

        /// <summary>
        /// 是否覆盖已存在的文件
        /// </summary>
        public bool OverwriteExisting { get; set; } = true;

        /// <summary>
        /// 重复文件处理策略
        /// </summary>
        public DuplicateFileStrategy DuplicateStrategy { get; set; } = DuplicateFileStrategy.Overwrite;

        /// <summary>
        /// 并发上传数量
        /// </summary>
        public int MaxConcurrentUploads { get; set; } = 3;

        /// <summary>
        /// 组织ID
        /// </summary>
        public long OrgId { get; set; }
    }

    /// <summary>
    /// FTP上传结果DTO
    /// </summary>
    public class FTPUploadResultDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 远程路径
        /// </summary>
        public string RemotePath { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public long DurationMs { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 上传速度（KB/s）
        /// </summary>
        public double UploadSpeedKbps { get; set; }
    }

    /// <summary>
    /// FTP批量上传结果DTO
    /// </summary>
    public class FTPBatchUploadResultDto
    {
        /// <summary>
        /// 总文件数
        /// </summary>
        public int TotalFiles { get; set; }

        /// <summary>
        /// 成功上传数
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失败数
        /// </summary>
        public int FailureCount { get; set; }

        /// <summary>
        /// 总耗时（毫秒）
        /// </summary>
        public long TotalDurationMs { get; set; }

        /// <summary>
        /// 平均上传速度（KB/s）
        /// </summary>
        public double AverageSpeedKbps { get; set; }

        /// <summary>
        /// 上传结果列表
        /// </summary>
        public List<FTPUploadResultDto> Results { get; set; } = new List<FTPUploadResultDto>();

        /// <summary>
        /// 是否全部成功
        /// </summary>
        public bool AllSuccess => FailureCount == 0;

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate => TotalFiles > 0 ? (double)SuccessCount / TotalFiles * 100 : 0;
    }

    /// <summary>
    /// FTP连接测试结果DTO
    /// </summary>
    public class FTPConnectionTestDto
    {
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// 连接时间（毫秒）
        /// </summary>
        public long ConnectionTimeMs { get; set; }

        /// <summary>
        /// 服务器信息
        /// </summary>
        public string ServerInfo { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime TestTime { get; set; } = DateTime.Now;
    }
}
