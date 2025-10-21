using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WebServiceClass.Services.DICOMServices
{
    /// <summary>
    /// DICOM服务状态管理
    /// </summary>
    public class DicomServiceStatus
    {
        private static readonly Lazy<DicomServiceStatus> _instance = new Lazy<DicomServiceStatus>(() => new DicomServiceStatus());
        public static DicomServiceStatus Instance => _instance.Value;

        private readonly ConcurrentDictionary<string, DicomConnectionInfo> _activeConnections;
        private readonly ConcurrentQueue<DicomFileInfo> _receivedFiles;
        private readonly object _lockObject = new object();

        public bool IsServiceRunning { get; set; }
        public DateTime ServiceStartTime { get; set; }
        public long TotalFilesReceived { get; private set; }
        public long TotalBytesReceived { get; private set; }
        public DateTime LastFileReceivedTime { get; private set; }

        private DicomServiceStatus()
        {
            _activeConnections = new ConcurrentDictionary<string, DicomConnectionInfo>();
            _receivedFiles = new ConcurrentQueue<DicomFileInfo>();
        }

        /// <summary>
        /// 添加活动连接
        /// </summary>
        public void AddConnection(string connectionId, string callingAE, string calledAE)
        {
            var connectionInfo = new DicomConnectionInfo
            {
                ConnectionId = connectionId,
                CallingAE = callingAE,
                CalledAE = calledAE,
                ConnectedTime = DateTime.Now,
                IsActive = true
            };

            _activeConnections.TryAdd(connectionId, connectionInfo);
        }

        /// <summary>
        /// 移除活动连接
        /// </summary>
        public void RemoveConnection(string connectionId)
        {
            if (_activeConnections.TryGetValue(connectionId, out var connectionInfo))
            {
                connectionInfo.IsActive = false;
                connectionInfo.DisconnectedTime = DateTime.Now;
                _activeConnections.TryRemove(connectionId, out _);
            }
        }

        /// <summary>
        /// 记录接收的文件
        /// </summary>
        public void RecordFileReceived(string filePath, long fileSize, string sopInstanceUID)
        {
            lock (_lockObject)
            {
                TotalFilesReceived++;
                TotalBytesReceived += fileSize;
                LastFileReceivedTime = DateTime.Now;

                var fileInfo = new DicomFileInfo
                {
                    FilePath = filePath,
                    FileSize = fileSize,
                    SOPInstanceUID = sopInstanceUID,
                    ReceivedTime = DateTime.Now
                };

                _receivedFiles.Enqueue(fileInfo);

                // 保持队列大小在合理范围内（最多保留1000个文件记录）
                while (_receivedFiles.Count > 1000)
                {
                    _receivedFiles.TryDequeue(out _);
                }
            }
        }

        /// <summary>
        /// 获取活动连接列表
        /// </summary>
        public List<DicomConnectionInfo> GetActiveConnections()
        {
            return _activeConnections.Values.Where(c => c.IsActive).ToList();
        }

        /// <summary>
        /// 获取最近接收的文件列表
        /// </summary>
        public List<DicomFileInfo> GetRecentFiles(int count = 10)
        {
            return _receivedFiles.TakeLast(count).ToList();
        }

        /// <summary>
        /// 获取服务统计信息
        /// </summary>
        public DicomServiceStatistics GetStatistics()
        {
            lock (_lockObject)
            {
                return new DicomServiceStatistics
                {
                    IsServiceRunning = IsServiceRunning,
                    ServiceStartTime = ServiceStartTime,
                    ServiceUptime = IsServiceRunning ? DateTime.Now - ServiceStartTime : TimeSpan.Zero,
                    TotalFilesReceived = TotalFilesReceived,
                    TotalBytesReceived = TotalBytesReceived,
                    ActiveConnections = _activeConnections.Count(c => c.Value.IsActive),
                    LastFileReceivedTime = LastFileReceivedTime,
                    AverageFileSize = TotalFilesReceived > 0 ? TotalBytesReceived / TotalFilesReceived : 0
                };
            }
        }

        /// <summary>
        /// 重置统计信息
        /// </summary>
        public void ResetStatistics()
        {
            lock (_lockObject)
            {
                TotalFilesReceived = 0;
                TotalBytesReceived = 0;
                LastFileReceivedTime = DateTime.MinValue;
                
                while (_receivedFiles.TryDequeue(out _)) { }
            }
        }
    }

    /// <summary>
    /// DICOM连接信息
    /// </summary>
    public class DicomConnectionInfo
    {
        public string ConnectionId { get; set; }
        public string CallingAE { get; set; }
        public string CalledAE { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime? DisconnectedTime { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan Duration => (DisconnectedTime ?? DateTime.Now) - ConnectedTime;
    }

    /// <summary>
    /// DICOM文件信息
    /// </summary>
    public class DicomFileInfo
    {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string SOPInstanceUID { get; set; }
        public DateTime ReceivedTime { get; set; }
    }

    /// <summary>
    /// DICOM服务统计信息
    /// </summary>
    public class DicomServiceStatistics
    {
        public bool IsServiceRunning { get; set; }
        public DateTime ServiceStartTime { get; set; }
        public TimeSpan ServiceUptime { get; set; }
        public long TotalFilesReceived { get; set; }
        public long TotalBytesReceived { get; set; }
        public int ActiveConnections { get; set; }
        public DateTime LastFileReceivedTime { get; set; }
        public long AverageFileSize { get; set; }
    }
}
