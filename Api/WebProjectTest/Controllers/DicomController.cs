using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WebServiceClass.Services.DICOMServices;

namespace WebProjectTest.Controllers
{
    /// <summary>
    /// DICOM服务控制器
    /// 提供DICOM SCP服务的监控和管理API
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DicomController : ControllerBase
    {
        private readonly ILogger<DicomController> _logger;

        public DicomController(ILogger<DicomController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取DICOM服务状态
        /// </summary>
        /// <returns>服务状态信息</returns>
        [HttpGet("status")]
        public IActionResult GetServiceStatus()
        {
            try
            {
                var statistics = DicomServiceStatus.Instance.GetStatistics();
                return Ok(new
                {
                    success = true,
                    data = statistics,
                    message = "获取服务状态成功"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取DICOM服务状态时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = "获取服务状态失败",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取活动连接列表
        /// </summary>
        /// <returns>活动连接信息</returns>
        [HttpGet("connections")]
        public IActionResult GetActiveConnections()
        {
            try
            {
                var connections = DicomServiceStatus.Instance.GetActiveConnections();
                return Ok(new
                {
                    success = true,
                    data = connections,
                    count = connections.Count,
                    message = "获取活动连接成功"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取活动连接时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = "获取活动连接失败",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取最近接收的文件列表
        /// </summary>
        /// <param name="count">返回文件数量，默认10个</param>
        /// <returns>最近接收的文件信息</returns>
        [HttpGet("recent-files")]
        public IActionResult GetRecentFiles([FromQuery] int count = 10)
        {
            try
            {
                var files = DicomServiceStatus.Instance.GetRecentFiles(count);
                return Ok(new
                {
                    success = true,
                    data = files,
                    count = files.Count,
                    message = "获取最近文件成功"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取最近文件时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = "获取最近文件失败",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 重置服务统计信息
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("reset-statistics")]
        public IActionResult ResetStatistics()
        {
            try
            {
                DicomServiceStatus.Instance.ResetStatistics();
                _logger.LogInformation("DICOM服务统计信息已重置");
                
                return Ok(new
                {
                    success = true,
                    message = "统计信息重置成功"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置统计信息时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = "重置统计信息失败",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取服务健康检查
        /// </summary>
        /// <returns>健康检查结果</returns>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            try
            {
                var statistics = DicomServiceStatus.Instance.GetStatistics();
                var isHealthy = statistics.IsServiceRunning;
                
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        isHealthy = isHealthy,
                        serviceRunning = statistics.IsServiceRunning,
                        uptime = statistics.ServiceUptime.ToString(@"dd\.hh\:mm\:ss"),
                        totalFiles = statistics.TotalFilesReceived,
                        activeConnections = statistics.ActiveConnections,
                        lastFileReceived = statistics.LastFileReceivedTime,
                        timestamp = DateTime.Now
                    },
                    message = isHealthy ? "服务运行正常" : "服务未运行"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "健康检查时发生错误");
                return StatusCode(500, new
                {
                    success = false,
                    message = "健康检查失败",
                    error = ex.Message
                });
            }
        }
    }
}
