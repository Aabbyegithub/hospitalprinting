using FellowOakDicom.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebServiceClass.Services.DICOMServices
{
    /// <summary>
    /// DICOM服务托管服务
    /// </summary>
    public class DicomHostedService : IHostedService
    {
        private readonly ILogger<DicomHostedService> _logger;
        private readonly IDicomServerFactory _dicomServerFactory;
        private readonly DicomServiceConfig _config;
        private IDicomServer? _server;

        public DicomHostedService(
            ILogger<DicomHostedService> logger, 
            IDicomServerFactory dicomServerFactory,
            IOptions<DicomServiceConfig> config)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dicomServerFactory = dicomServerFactory ?? throw new ArgumentNullException(nameof(dicomServerFactory));
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("DICOM服务已禁用");
                return;
            }

            try
            {
                _logger.LogInformation($"正在启动DICOM SCP服务...");
                _logger.LogInformation($"端口: {_config.Port}, AE Title: {_config.AETitle}");
                _logger.LogInformation($"保存目录: {_config.SaveDirectory}");

                // 确保保存目录存在
                if (_config.AutoCreateDirectory && !Directory.Exists(_config.SaveDirectory))
                {
                    Directory.CreateDirectory(_config.SaveDirectory);
                    _logger.LogInformation($"创建DICOM文件保存目录: {_config.SaveDirectory}");
                }

                // 创建DICOM服务器
                _server = _dicomServerFactory.Create<HospitalDicomScp>(_config.Port);
                
                _logger.LogInformation($"✅ DICOM SCP服务已启动 - 端口: {_config.Port}, AE: {_config.AETitle}");
                _logger.LogInformation($"📁 文件保存路径: {_config.SaveDirectory}");
                _logger.LogInformation($"🔗 最大连接数: {_config.MaxConnections}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动DICOM服务失败");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_server != null)
                {
                    _logger.LogInformation("正在停止DICOM SCP服务...");
                    _server.Stop();
                    _server.Dispose();
                    _server = null;
                    _logger.LogInformation("✅ DICOM SCP服务已停止");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止DICOM服务时发生错误");
            }

            return Task.CompletedTask;
        }
    }
}
