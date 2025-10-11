using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;
using iText.Layout.Element;
using ModelClassLibrary.Model.Dto.TaskDto;

namespace WebTaskClass.Common
{

    /// <summary>
    /// 支持多路径的PDF报告监听与解析器
    /// </summary>
    public class PdfReportMonitor : IDisposable
    {
        // 存储所有监听路径及对应的FileSystemWatcher
        private readonly Dictionary<string, FileSystemWatcher> _watchers = new();
        private bool _disposed;

        /// <summary>
        /// 报告解析完成事件
        /// </summary>
        public event Action<ReportInfo>? OnReportParsed;

        /// <summary>
        /// 添加监听路径
        /// </summary>
        /// <param name="paths">需要监听的文件夹路径集合</param>
        public void AddMonitorPaths(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                AddMonitorPath(path);
            }
        }

        /// <summary>
        /// 添加单个监听路径
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <exception cref="DirectoryNotFoundException">路径不存在时抛出</exception>
        public void AddMonitorPath(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"监听路径不存在：{path}");

            if (_watchers.ContainsKey(path))
                return; // 避免重复添加

            // 创建针对该路径的监听器
            var watcher = new FileSystemWatcher(path)
            {
                Filter = "*.pdf",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                EnableRaisingEvents = false
            };

            // 绑定文件创建事件
            watcher.Created += async (s, e) => await OnFileCreatedAsync(e.FullPath);
            _watchers[path] = watcher;
        }

        /// <summary>
        /// 开始所有路径的监听
        /// </summary>
        public void StartMonitoring()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = true;
            }
            Console.WriteLine($"已开始监听 {_watchers.Count} 个路径（仅PDF文件）");
        }

        /// <summary>
        /// 停止所有路径的监听
        /// </summary>
        public void StopMonitoring()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
            }
            Console.WriteLine("已停止所有路径监听");
        }

        /// <summary>
        /// 新增文件处理逻辑（复用原有解析逻辑）
        /// </summary>
        private async Task OnFileCreatedAsync(string filePath)
        {
            try
            {
                // 等待文件写入完成
                await WaitForFileReadyAsync(filePath, TimeSpan.FromSeconds(30));

                // 解析PDF
                var reportInfo = await ParsePdfAsync(filePath);
                if (reportInfo != null)
                {
                    OnReportParsed?.Invoke(reportInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理文件异常：{filePath}，错误：{ex.Message}");
            }
        }

        // 以下方法与原PdfReportMonitor保持一致（复用解析逻辑）
        private async Task WaitForFileReadyAsync(string filePath, TimeSpan timeout)
        {
            var startTime = DateTime.Now;
            while (DateTime.Now - startTime < timeout)
            {
                try
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (stream.Length > 0)
                            return;
                    }
                }
                catch (IOException)
                {
                    await Task.Delay(100);
                }
                catch (UnauthorizedAccessException)
                {
                    throw;
                }
            }
            throw new TimeoutException($"等待文件就绪超时：{filePath}");
        }

        private async Task<ReportInfo?> ParsePdfAsync(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            using var pdfDocument = new PdfDocument(new PdfReader(fileStream));

            var pdfText = new System.Text.StringBuilder();
            int pageCount = pdfDocument.GetNumberOfPages();

            for (int i = 1; i <= pageCount; i++)
            {
                var page = pdfDocument.GetPage(i);
                var text = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page);
                pdfText.AppendLine(text);
            }

            var fullText = pdfText.ToString();
            if (string.IsNullOrWhiteSpace(fullText))
                return null;

            // 提取报告信息（根据实际格式调整正则）
            return new ReportInfo
            {
                FilePath = filePath,
                CheckNumber = ExtractValue(fullText, @"检查号：([A-Z0-9]+)"),
                PatientName = ExtractValue(fullText, @"患者姓名：([^，\n]+)"),
                PatientId = ExtractValue(fullText, @"住院号：([0-9]+)|门诊号：([0-9]+)"),
                ExamType = ExtractValue(fullText, @"检查类型：([^，\n]+)"),
                ReportTime = DateTime.TryParse(ExtractValue(fullText, @"报告时间：(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})"), out var time) ? time : null
            };
        }

        private string ExtractValue(string sourceText, string regexPattern)
        {
            var match = System.Text.RegularExpressions.Regex.Match(sourceText, regexPattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

            for (int i = 1; i < match.Groups.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(match.Groups[i].Value))
                    return match.Groups[i].Value.Trim();
            }
            return string.Empty;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var watcher in _watchers.Values)
                {
                    watcher.Dispose();
                }
                _watchers.Clear();
            }

            _disposed = true;
        }

        ~PdfReportMonitor() => Dispose(false);
    }

}
