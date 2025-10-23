using System.ComponentModel.DataAnnotations;

namespace ModelClassLibrary.Model
{
    /// <summary>
    /// DICOM服务配置
    /// </summary>
    public class DicomServiceConfig
    {
        /// <summary>
        /// DICOM服务端口
        /// </summary>
        [Required]
        public int Port { get; set; } = 104;

        /// <summary>
        /// 应用程序实体标题(AE Title)
        /// </summary>
        [Required]
        public string AETitle { get; set; } = "HOSPITAL_SCP";

        /// <summary>
        /// 接收文件的保存目录
        /// </summary>
        [Required]
        public string SaveDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "ReceivedDicom");

        /// <summary>
        /// 是否启用DICOM服务
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// 连接超时时间(秒)
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// 是否自动创建保存目录
        /// </summary>
        public bool AutoCreateDirectory { get; set; } = true;
    }
}
