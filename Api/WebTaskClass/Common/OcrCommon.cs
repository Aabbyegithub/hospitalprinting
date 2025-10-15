using Baidu.Aip.Ocr;
using ModelClassLibrary.Model.Dto.TaskDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebTaskClass.Common
{
/// <summary>
    /// OCR识别服务，处理电子胶片和报告的信息提取与校验
    /// </summary>
    public class OcrCommon
    {
        private readonly Ocr _baiduOcrClient;
        private readonly ISqlHelper _dal;
        private readonly string _appId;
        private readonly string _apiKey;
        private readonly string _secretKey;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OcrService(ISqlHelper dal)
        {
            _dal = dal;
            
            // 从配置文件获取百度OCR API信息
            _appId = configuration["BaiduOcr:AppId"];
            _apiKey = configuration["BaiduOcr:ApiKey"];
            _secretKey = configuration["BaiduOcr:SecretKey"];
            
            // 初始化百度OCR客户端
            _baiduOcrClient = new Ocr( _apiKey, _secretKey);
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
            // 读取PDF文件
            byte[] pdfBytes = await File.ReadAllBytesAsync(filePath);
            
            // 调用百度API识别PDF
            var response = await Task.Run(() => 
                _baiduOcrClient.AccuratePdf(pdfBytes, true));
            
            // 解析识别结果
            return ParseOcrResult(response);
        }

        /// <summary>
        /// 识别图片文件(电子胶片)
        /// </summary>
        private async Task<MedicalRecordDto> RecognizeImage(string filePath)
        {
            // 读取图片文件
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            
            // 调用百度API识别图片
            var response = await Task.Run(() => 
                _baiduOcrClient.GeneralEnhanced(imageBytes));
            
            // 解析识别结果
            return ParseOcrResult(response);
        }

        /// <summary>
        /// 解析OCR识别结果
        /// </summary>
        private MedicalRecordDto ParseOcrResult(dynamic response)
        {
            var result = new MedicalRecordDto();
            var fullText = new List<string>();

            // 提取所有识别文本
            foreach (var item in response.words_result)
            {
                fullText.Add(item.words.ToString());
            }
            result.FullOcrText = string.Join("\n", fullText);

            // 解析关键信息
            var textContent = result.FullOcrText;
            
            // 患者姓名
            result.PatientName = ExtractValue(textContent, @"患者姓名[:：\s]*([^\n]+)");
            result.PatientName ??= ExtractValue(textContent, @"姓名[:：\s]*([^\n]+)");
            
            // 性别
            result.Gender = ExtractValue(textContent, @"性别[:：\s]*([男女])");
            
            // 年龄
            result.Age = ExtractValue(textContent, @"年龄[:：\s]*(\d+岁?)");
            
            // 胶片检查号
            result.FilmCheckNumber = ExtractValue(textContent, @"胶片检查号[:：\s]*([A-Z0-9]+)");
            result.FilmCheckNumber ??= ExtractValue(textContent, @"检查编号[:：\s]*([A-Z0-9]+)");
            
            // 报告编号
            result.ReportNumber = ExtractValue(textContent, @"报告编号[:：\s]*([A-Z0-9]+)");
            
            // 检查类型
            result.ExamType = ExtractValue(textContent, @"检查类型[:：\s]*([^\n]+)");
            result.ExamType ??= ExtractValue(textContent, @"检查项目[:：\s]*([^\n]+)");
            
            // 检查日期
            if (DateTime.TryParse(ExtractValue(textContent, @"检查日期[:：\s]*(\d{4}[年/-]\d{1,2}[月/-]\d{1,2}[日]?)"), out DateTime examDate))
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
            var match = System.Text.RegularExpressions.Regex.Match(text, pattern);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        /// <summary>
        /// 与数据库中已有信息交叉校验
        /// </summary>
        private async Task<(bool IsValid, string Message)> ValidateWithDatabase(MedicalRecordDto record)
        {
            // 如果有检查号，通过检查号查询数据库中的信息
            if (!string.IsNullOrEmpty(record.FilmCheckNumber))
            {
                var dbRecord = await _dal.QueryFirstOrDefaultAsync<MedicalRecordDto>(
                    "SELECT * FROM MedicalRecords WHERE FilmCheckNumber = @CheckNumber",
                    new { CheckNumber = record.FilmCheckNumber });

                if (dbRecord != null)
                {
                    // 检查关键信息是否匹配
                    var mismatches = new List<string>();
                    
                    if (!string.Equals(record.PatientName, dbRecord.PatientName, StringComparison.OrdinalIgnoreCase))
                    {
                        mismatches.Add($"患者姓名不匹配 (OCR: {record.PatientName}, 数据库: {dbRecord.PatientName})");
                    }
                    
                    if (!string.Equals(record.Gender, dbRecord.Gender, StringComparison.OrdinalIgnoreCase))
                    {
                        mismatches.Add($"性别不匹配 (OCR: {record.Gender}, 数据库: {dbRecord.Gender})");
                    }

                    if (mismatches.Count > 0)
                    {
                        return (false, string.Join("; ", mismatches));
                    }
                    
                    // 验证通过，补充数据库中可能缺失的信息
                    if (string.IsNullOrEmpty(dbRecord.ReportNumber) && !string.IsNullOrEmpty(record.ReportNumber))
                    {
                        dbRecord.ReportNumber = record.ReportNumber;
                        await _dal.UpdateAsync(dbRecord);
                    }
                    
                    return (true, "验证通过");
                }
            }
            
            // 如果没有找到匹配的记录，视为新记录
            return (true, "新记录，无匹配历史数据");
        }

        /// <summary>
        /// 将识别结果保存到数据库
        /// </summary>
        public async Task SaveToDatabase(MedicalRecordDto record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            // 检查是否已存在相同记录
            var existingRecord = await _dal.QueryFirstOrDefaultAsync<MedicalRecordDto>(
                "SELECT * FROM MedicalRecords WHERE FilmCheckNumber = @CheckNumber",
                new { CheckNumber = record.FilmCheckNumber });

            if (existingRecord != null)
            {
                // 更新现有记录
                existingRecord.PatientName = record.PatientName ?? existingRecord.PatientName;
                existingRecord.Gender = record.Gender ?? existingRecord.Gender;
                existingRecord.Age = record.Age ?? existingRecord.Age;
                existingRecord.ReportNumber = record.ReportNumber ?? existingRecord.ReportNumber;
                existingRecord.ExamType = record.ExamType ?? existingRecord.ExamType;
                existingRecord.ExamDate = record.ExamDate ?? existingRecord.ExamDate;
                existingRecord.FullOcrText = record.FullOcrText;
                existingRecord.ValidationStatus = record.ValidationStatus;
                existingRecord.UpdateTime = DateTime.Now;

                await _dal.UpdateAsync(existingRecord);
            }
            else
            {
                // 插入新记录
                record.CreateTime = DateTime.Now;
                record.UpdateTime = DateTime.Now;
                await _dal.InsertAsync(record);
            }
        }
    }
}
