//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using PaddleOCRSharp;

//namespace WinSelfMachine.Common
//{
//    /// <summary>
//    /// 使用 PaddleOCRSharp 与 PaddleOCRSharp.PDF 对 PDF 进行 OCR 的帮助类。
//    /// </summary>
//    public class PdfOcrHelper : IDisposable
//    {
//        private readonly PaddleOCREngine _engine;
//        private bool _disposed;

//        /// <summary>
//        /// 初始化 OCR 引擎。
//        /// </summary>
//        /// <param name="modelRoot">模型根目录（为空则使用默认内置模型路径）。</param>
//        /// <param name="parameter">OCR 参数（为空则使用默认参数）。</param>
//        public PdfOcrHelper(string modelRoot = null, OCRParameter parameter = null)
//        {
//            //// 准备模型配置
//            //OCRModelConfig modelConfig = string.IsNullOrWhiteSpace(modelRoot)
//            //    ? PaddleOCREngine.GetDefaultConfig(PaddleOCREngine.GetRootDirectory())
//            //    : PaddleOCREngine.GetDefaultConfig(modelRoot);

//            //// 参数
//            //if (parameter == null)
//            //{
//            //    parameter = new OCRParameter
//            //    {
//            //        use_gpu = false,
//            //        max_side_len = 1536,
//            //        det_db_box_thresh = 0.5f,
//            //        det_db_thresh = 0.3f,
//            //        det_db_unclip_ratio = 2.0f,
//            //        rec_img_h = 48,
//            //        rec_img_w = 320,
//            //        cpu_math_library_num_threads = Environment.ProcessorCount > 0 ? Environment.ProcessorCount : 4,
//            //        enable_mkldnn = true,
//            //        use_angle_cls = true,
//            //        rec_batch_num = 6
//            //    };
//            //}

//            _engine = new PaddleOCREngine();
//        }

//        /// <summary>
//        /// 对 PDF 文件进行 OCR 识别，返回拼接后的全文本。
//        /// </summary>
//        public string RecognizePdfToText(string pdfPath, float dpi = 220f, Action<int> onProgress = null)
//        {
//            EnsureNotDisposed();
//            var pdfResult = PDFExtensions.DetectTextPDF(_engine, pdfPath, dpi, onProgress);
//            if (pdfResult == null || pdfResult.Pages == null || pdfResult.Pages.Count == 0)
//            {
//                return string.Empty;
//            }
//            var sb = new StringBuilder();
//            for (int i = 0; i < pdfResult.Pages.Count; i++)
//            {
//                var page = pdfResult.Pages[i];
//                if (page?.TextBlocks == null) continue;
//                foreach (var block in page.TextBlocks)
//                {
//                    if (!string.IsNullOrWhiteSpace(block.Text))
//                    {
//                        sb.AppendLine(block.Text.Trim());
//                    }
//                }
//                if (i != pdfResult.Pages.Count - 1)
//                {
//                    sb.AppendLine();
//                }
//            }
//            return sb.ToString();
//        }

//        /// <summary>
//        /// 对 PDF 文件进行 OCR 识别，返回结构化结果。
//        /// </summary>
//        public PdfOcrResult RecognizePdf(string pdfPath, float dpi = 220f, Action<int> onProgress = null)
//        {
//            EnsureNotDisposed();
//            var pdfResult = PDFExtensions.DetectTextPDF(_engine, pdfPath, dpi, onProgress);
//            return ConvertToDto(pdfResult);
//        }

//        /// <summary>
//        /// 对 PDF 二进制进行 OCR 识别，返回结构化结果。
//        /// </summary>
//        public PdfOcrResult RecognizePdf(byte[] pdfBytes, float dpi = 220f, Action<int> onProgress = null)
//        {
//            EnsureNotDisposed();
//            var pdfResult = PDFExtensions.DetectTextPDF(_engine, pdfBytes, dpi, onProgress);
//            return ConvertToDto(pdfResult);
//        }

//        private static PdfOcrResult ConvertToDto(PDFOCRResult pdfResult)
//        {
//            var dto = new PdfOcrResult
//            {
//                Pages = new List<PdfOcrPageResult>(),
//                FullText = string.Empty
//            };

//            if (pdfResult == null || pdfResult.Pages == null) return dto;

//            var full = new StringBuilder();
//            for (int i = 0; i < pdfResult.Pages.Count; i++)
//            {
//                var page = pdfResult.Pages[i];
//                var pageDto = new PdfOcrPageResult
//                {
//                    PageIndex = i,
//                    Text = string.Empty,
//                    Blocks = new List<PdfOcrTextBlock>()
//                };

//                if (page?.TextBlocks != null)
//                {
//                    var pageSb = new StringBuilder();
//                    foreach (var block in page.TextBlocks)
//                    {
//                        var blockText = block?.Text?.Trim();
//                        if (string.IsNullOrWhiteSpace(blockText)) continue;
//                        pageSb.AppendLine(blockText);
//                        pageDto.Blocks.Add(new PdfOcrTextBlock
//                        {
//                            Text = blockText,
//                            Score = block.Score,
//                            BoxPoints = block.BoxPoints?.Select(p => new PdfOcrPoint { X = p.X, Y = p.Y }).ToList()
//                        });
//                    }
//                    pageDto.Text = pageSb.ToString();
//                }

//                dto.Pages.Add(pageDto);
//                if (!string.IsNullOrWhiteSpace(pageDto.Text))
//                {
//                    full.AppendLine(pageDto.Text);
//                }
//            }

//            dto.FullText = full.ToString();
//            return dto;
//        }

//        private void EnsureNotDisposed()
//        {
//            if (_disposed) throw new ObjectDisposedException(nameof(PdfOcrHelper));
//        }

//        public void Dispose()
//        {
//            if (_disposed) return;
//            _engine?.Dispose();
//            _disposed = true;
//        }
//    }

//    /// <summary>
//    /// 结构化的 PDF OCR 返回结果。
//    /// </summary>
//    public class PdfOcrResult
//    {
//        public List<PdfOcrPageResult> Pages { get; set; }
//        public string FullText { get; set; }
//    }

//    public class PdfOcrPageResult
//    {
//        public int PageIndex { get; set; }
//        public string Text { get; set; }
//        public List<PdfOcrTextBlock> Blocks { get; set; }
//    }

//    public class PdfOcrTextBlock
//    {
//        public string Text { get; set; }
//        public float Score { get; set; }
//        public List<PdfOcrPoint> BoxPoints { get; set; }
//    }

//    public class PdfOcrPoint
//    {
//        public int X { get; set; }
//        public int Y { get; set; }
//    }
//}


