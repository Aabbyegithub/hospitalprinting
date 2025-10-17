using Baidu.Aip.Ocr;
using ModelClassLibrary.Model.Dto.TaskDto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public OcrCommon(string apiKey, string secretKey)
        {
            // 初始化百度OCR客户端
            _baiduOcrClient = new Ocr(apiKey, secretKey);
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
                // 读取PDF文件
                byte[] pdfBytes = await File.ReadAllBytesAsync(filePath);

                // 调用百度API识别PDF
                var response = await Task.Run(() =>
                    _baiduOcrClient.AccuratePdf(pdfBytes, new Dictionary<string, object> { { "is_table", true } }));

                // 解析识别结果
                return ParseOcrResult(response);
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
                // 读取图片文件
                byte[] imageBytes = await File.ReadAllBytesAsync(filePath);

                // 调用百度API识别图片
                var response = await Task.Run(() =>
                    _baiduOcrClient.General(imageBytes));

                // 解析识别结果
                return ParseOcrResult(response);
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
        private MedicalRecordDto ParseOcrResult(dynamic response)
        {
            var result = new MedicalRecordDto();
            var fullText = new List<string>();

            // 提取所有识别文本
            foreach (var item in response["words_result"])
            {
                fullText.Add(item["words"].ToString());
            }
            result.FullOcrText = string.Join("\n", fullText);

            // 解析关键信息
            var textContent = result.FullOcrText;

            // 提取患者信息
            result.PatientName = ExtractValue(textContent, @"患者姓名[:：\s]*([^\n]+)");
            result.PatientName ??= ExtractValue(textContent, @"姓名[:：\s]*([^\n]+)");

            result.Gender = ExtractValue(textContent, @"性别[:：\s]*([男女])");

            result.Age = ExtractValue(textContent, @"年龄[:：\s]*(\d+岁?)");

            result.FilmCheckNumber = ExtractValue(textContent, @"检查号[:：\s]*([A-Z0-9]+)");
            result.FilmCheckNumber ??= ExtractValue(textContent, @"检查编号[:：\s]*([A-Z0-9]+)");

            result.OutpatientNumber = ExtractValue(textContent, @"门诊号[:：\s]*([A-Z0-9]+)");
            result.HospitalAdmissionNumber = ExtractValue(textContent, @"住院号[:：\s]*([A-Z0-9]+)");
            result.BedNumber = ExtractValue(textContent, @"床号[:：\s]*([A-Z0-9]+)");
            result.ReferringDoctor = ExtractValue(textContent, @"送检医生[:：\s]*([^\n]+)");
            result.DepartmentInspection = ExtractValue(textContent, @"送检科室[:：\s]*([^\n]+)");
            result.ClinicalDiagnosis = ExtractValue(textContent, @"临床诊断[:：\s]*([^\n]+)");
            // 影像所见（匹配到“诊断结论”前的所有内容）
            result.ImagingFindings = ExtractValue(textContent, @"影像所见[:：\s]*([\s\S]+?)(?=诊断结论|$)");

            // 诊断结论（匹配到“报告日期”前的所有内容）
            result.DiagnosisConclusion = ExtractValue(textContent, @"诊断结论[:：\s]*([\s\S]+?)(?=报告日期|$)");
            result.ReportDoctor = ExtractValue(textContent, @"报告医生[:：\s]*([^\n]+)");
            result.ReviewingDoctor = ExtractValue(textContent, @"审核医生[:：\s]*([^\n]+)");

            result.ExamType = ExtractValue(textContent, @"检查类型[:：\s]*([^\n]+)");
            result.ExamType ??= ExtractValue(textContent, @"检查部位[:：\s]*([^\n]+)");

            if (DateTime.TryParse(ExtractValue(textContent, @"报告日期[:：\s]*(\d{4}[年/-]\d{1,2}[月/-]\d{1,2}[日]?)"), out DateTime examDate))
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
    }
}
