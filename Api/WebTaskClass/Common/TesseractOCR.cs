using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;
using WebIServices.IBase;
using ModelClassLibrary.Model.Dto.TaskDto;
using Pdfium.NET;

namespace WebTaskClass.Common
{
    /// <summary>
    /// OCR识别服务，处理电子胶片和报告的信息提取与校验
    /// </summary>
    public class TesseractOCR
    {
        private readonly ISqlHelper _dal;
        private readonly string _tesseractDataPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TesseractOCR(ISqlHelper dal, string tesseractDataPath)
        {
            _dal = dal;
            _tesseractDataPath = tesseractDataPath;
        }

        /// <summary>
        /// 识别电子胶片/报告并提取关键信息
        /// </summary>
        public async Task<MedicalRecordDto> RecognizeMedicalDocument(string filePath)
        {
            try
            {
                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("文件不存在", filePath);
                }

                // 根据文件扩展名判断处理方式
                var fileExtension = Path.GetExtension(filePath).ToLower();
                MedicalRecordDto result;

                if (fileExtension == ".pdf")
                {
                    // 处理PDF文件
                    result = await RecognizePdf(filePath);
                }
                else if (new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(fileExtension))
                {
                    // 处理图片文件(电子胶片)
                    result = await RecognizeImage(filePath);
                }
                else
                {
                    throw new NotSupportedException($"不支持的文件格式: {fileExtension}");
                }

                // 设置文件路径
                result.SourceFilePath = filePath;
                result.RecognizeTime = DateTime.Now;

                // 与数据库中已有信息进行交叉校验
                var validationResult = await ValidateWithDatabase(result);
                result.ValidationStatus = validationResult.IsValid ? "验证通过" : $"验证失败: {validationResult.Message}";

                return result;
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Console.WriteLine($"OCR识别失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 识别PDF文件
        /// </summary>
        private async Task<MedicalRecordDto> RecognizePdf(string filePath)
        {
            try
            {
                // PDF识别可以转为图片，然后使用Tesseract处理
                // 使用PdfToImage的第三方库如Pdfium来实现
                // 例如，Pdfium库可以将PDF转换为图片，可以使用 Tesseract 来识别这些图片
                // 这里先略过转换细节，假设PDF转换为图片后调用 RecognizeImage 方法

                byte[] pdfBytes = await File.ReadAllBytesAsync(filePath);
                var images = ConvertPdfToImages(pdfBytes); // 假设你有一个方法将PDF转换为图片
                var result = new MedicalRecordDto();

                // 使用Tesseract识别图片
                foreach (var image in images)
                {
                    var pageResult = await RecognizeImage(image);
                    result.FullOcrText += pageResult.FullOcrText;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF识别失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 识别图片文件(电子胶片)
        /// </summary>
        private async Task<MedicalRecordDto> RecognizeImage(string filePath)
        {
            try
            {
                // 使用Tesseract进行图片识别
                using (var engine = new TesseractEngine(_tesseractDataPath, "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(filePath))
                    {
                        var result = engine.Process(img);
                        var ocrText = result.GetText();

                        return ParseOcrResult(ocrText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图片识别失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 解析OCR识别结果
        /// </summary>
        private MedicalRecordDto ParseOcrResult(string ocrText)
        {
            var result = new MedicalRecordDto();

            // 解析关键信息
            result.FullOcrText = ocrText;

            // 提取患者信息
            result.PatientName = ExtractValue(ocrText, @"患者姓名[:：\s]*([^\n]+)");
            result.PatientName ??= ExtractValue(ocrText, @"姓名[:：\s]*([^\n]+)");

            result.Gender = ExtractValue(ocrText, @"性别[:：\s]*([男女])");

            result.Age = ExtractValue(ocrText, @"年龄[:：\s]*(\d+岁?)");

            result.FilmCheckNumber = ExtractValue(ocrText, @"胶片检查号[:：\s]*([A-Z0-9]+)");
            result.FilmCheckNumber ??= ExtractValue(ocrText, @"检查编号[:：\s]*([A-Z0-9]+)");

            result.ReportNumber = ExtractValue(ocrText, @"报告编号[:：\s]*([A-Z0-9]+)");

            result.ExamType = ExtractValue(ocrText, @"检查类型[:：\s]*([^\n]+)");
            result.ExamType ??= ExtractValue(ocrText, @"检查项目[:：\s]*([^\n]+)");

            if (DateTime.TryParse(ExtractValue(ocrText, @"检查日期[:：\s]*(\d{4}[年/-]\d{1,2}[月/-]\d{1,2}[日]?)"), out DateTime examDate))
            {
                result.ExamDate = examDate;
            }

            return result;
        }

        /// <summary>
        /// 从文本中提取指定信息
        /// </summary>
        private string ExtractValue(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        /// <summary>
        /// 与数据库中已有信息交叉校验
        /// </summary>
        private async Task<(bool IsValid, string Message)> ValidateWithDatabase(MedicalRecordDto record)
        {
            try
            {
                //if (!string.IsNullOrEmpty(record.FilmCheckNumber))
                //{
                //    var dbRecord = await _dal.QueryFirstOrDefaultAsync<MedicalRecordDto>(
                //        "SELECT * FROM MedicalRecords WHERE FilmCheckNumber = @CheckNumber",
                //        new { CheckNumber = record.FilmCheckNumber });

                //    if (dbRecord != null)
                //    {
                //        var mismatches = new List<string>();

                //        if (!string.Equals(record.PatientName, dbRecord.PatientName, StringComparison.OrdinalIgnoreCase))
                //        {
                //            mismatches.Add($"患者姓名不匹配 (OCR: {record.PatientName}, 数据库: {dbRecord.PatientName})");
                //        }

                //        if (!string.Equals(record.Gender, dbRecord.Gender, StringComparison.OrdinalIgnoreCase))
                //        {
                //            mismatches.Add($"性别不匹配 (OCR: {record.Gender}, 数据库: {dbRecord.Gender})");
                //        }

                //        if (mismatches.Count > 0)
                //        {
                //            return (false, string.Join("; ", mismatches));
                //        }

                //        if (string.IsNullOrEmpty(dbRecord.ReportNumber) && !string.IsNullOrEmpty(record.ReportNumber))
                //        {
                //            dbRecord.ReportNumber = record.ReportNumber;
                //            await _dal.UpdateAsync(dbRecord);
                //        }

                //        return (true, "验证通过");
                //    }
                //}

                return (true, "新记录，无匹配历史数据");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"数据库校验失败: {ex.Message}");
                return (false, "数据库校验失败");
            }
        }

        /// <summary>
        /// 将识别结果保存到数据库
        /// </summary>
        public async Task SaveToDatabase(MedicalRecordDto record)
        {
            try
            {
                if (record == null)
                    throw new ArgumentNullException(nameof(record));

                //var existingRecord = await _dal.QueryFirstOrDefaultAsync<MedicalRecordDto>(
                //    "SELECT * FROM MedicalRecords WHERE FilmCheckNumber = @CheckNumber",
                //    new { CheckNumber = record.FilmCheckNumber });

                //if (existingRecord != null)
                //{
                //    existingRecord.PatientName = record.PatientName ?? existingRecord.PatientName;
                //    existingRecord.Gender = record.Gender ?? existingRecord.Gender;
                //    existingRecord.Age = record.Age ?? existingRecord.Age;
                //    existingRecord.ReportNumber = record.ReportNumber ?? existingRecord.ReportNumber;
                //    existingRecord.ExamType = record.ExamType ?? existingRecord.ExamType;
                //    existingRecord.ExamDate = record.ExamDate ?? existingRecord.ExamDate;
                //    existingRecord.FullOcrText = record.FullOcrText;
                //    existingRecord.ValidationStatus = record.ValidationStatus;
                //    existingRecord.UpdateTime = DateTime.Now;

                //    await _dal.UpdateAsync(existingRecord);
                //}
                //else
                //{
                //    record.CreateTime = DateTime.Now;
                //    record.UpdateTime = DateTime.Now;
                //    await _dal.InsertAsync(record);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存到数据库失败: {ex.Message}");
                throw;
            }
        }

        // 将PDF转换为图片
        public List<string> ConvertPdfToImages(byte[] pdfBytes)
        {
            List<string> imagePaths = new List<string>();

            // 使用 Pdfium.Net 进行加载
            using (MemoryStream stream = new MemoryStream(pdfBytes))
            {
                var pdfDocument = new PdfDocument(stream);

                // 遍历PDF的每一页
                for (int pageIndex = 0; pageIndex < pdfDocument.PageCount; pageIndex++)
                {
                    // 渲染每页为图像
                    //var pageImage = pdfDocument.Render(pageIndex, 300, 300);

                    //// 保存图片
                    //string tempImagePath = Path.Combine(Path.GetTempPath(), $"page_{pageIndex + 1}.png");
                    //pageImage.Save(tempImagePath);

                    //imagePaths.Add(tempImagePath);
                }
            }

            return imagePaths;
        }
    }
}
