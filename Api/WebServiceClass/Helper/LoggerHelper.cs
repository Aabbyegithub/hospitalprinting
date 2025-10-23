using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebServiceClass.Helper
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LoggerHelper : ILoggerHelper, IDisposable,IBaseService
    {
        /*请求样例
          using (var logger2 = new Logger(moduleName: "module2", logFileName: "custom_log"))
        {
            logger2.LogInfo("This is an info message for module2.");
            logger2.LogWarning("This is a warning message for module2.");
            logger2.LogError("This is an error message for module2.");
        }

        using (var customLogger = new Logger("custom_logs", "custom_module", "custom_log", ".txt"))
        {
            customLogger.LogInfo("This is an info message for custom_module.");
            customLogger.LogWarning("This is a warning message for custom_module.");
            customLogger.LogError("This is an error message for custom_module.");
        }
         */
        private readonly string _baseLogDirectory;
        private readonly string _moduleName;
        private readonly string _logFileName;
        private readonly string _fileExtension;
        private StreamWriter _infoLogWriter;
        private StreamWriter _warningLogWriter;
        private StreamWriter _errorLogWriter;
        private string _currentLogDirectory;

        public LoggerHelper(string baseLogDirectory = "LogInfos", string moduleName = "default", string logFileName = "defaultlog", string fileExtension = ".log")
        {
            _baseLogDirectory = baseLogDirectory;
            _moduleName = moduleName;
            _logFileName = logFileName;
            _fileExtension = fileExtension;
            UpdateLogFilePaths();
        }

        private void UpdateLogFilePaths()
        {
            string currentDateString = DateTime.Now.ToString("yyyy-MM-dd");
            _currentLogDirectory = Path.Combine(_baseLogDirectory, currentDateString, _moduleName);
            Directory.CreateDirectory(_currentLogDirectory); // 确保模块目录存在

            
           
           
        }

        private StreamWriter CreateLogWriter(string logType, string logFileName)
        {
            string logTypeDirectory = Path.Combine(_currentLogDirectory, logType);
            Directory.CreateDirectory(logTypeDirectory); // 确保日志类型目录存在

            string logFilePath = Path.Combine(logTypeDirectory, $"{logFileName}_{logType}{_fileExtension}");
            return new StreamWriter(logFilePath, append: true)
            {
                AutoFlush = true
            };
        }

        public async Task LogInfo(string message)
        {
            using (_infoLogWriter = CreateLogWriter("INFO", _logFileName))
            {
                 Log("INFO", message, _infoLogWriter);
            }           
        }

        public async Task LogWarning(string message)
        {
            using ( _warningLogWriter = CreateLogWriter("WARNING", _logFileName))
            {
                 Log("WARNING", message, _warningLogWriter);
            };
           
        }

        public async Task LogError(string message)
        {
            using ( _errorLogWriter = CreateLogWriter("ERROR", _logFileName))
            {
                  await Log("ERROR", message, _errorLogWriter);
            };
          
        }

        private async Task Log(string logLevel, string message, StreamWriter logWriter)
        {
            // 每次记录日志时，检查日志文件夹是否为当天的日志文件夹
            string currentDateString = DateTime.Now.ToString("yyyy-MM-dd");
            string newLogDirectory = Path.Combine(_baseLogDirectory, currentDateString, _moduleName);

            if (_currentLogDirectory != newLogDirectory)
            {
                DisposeLogWriters();
                UpdateLogFilePaths();
            }

            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {message}";
            Console.WriteLine(logMessage);
            logWriter.WriteLine(logMessage);
        }

        private void DisposeLogWriters()
        {
            _infoLogWriter?.Dispose();
            _warningLogWriter?.Dispose();
            _errorLogWriter?.Dispose();
        }

        public void Dispose()
        {
            DisposeLogWriters();
        }
    }
}
