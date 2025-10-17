using ModelClassLibrary.Model.Dto.TaskDto;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;
using WebTaskClass.Common;

namespace WebTaskClass.SampleJob
{
    /// <summary>
    /// OCR识别任务
    /// </summary>
    public class OCRIdentifyTask : IJob
    {
        private readonly ISqlHelper _dal;
        public OCRIdentifyTask(ISqlHelper dal)
        {
            _dal = dal;
        }
        /// <summary>
        /// OCR识别任务-识别影像/报告中的内容与数据库解析的内容进行对比，
        /// 更新识别状态（不一致触发弹窗预警）提示管理员进行处理人工核对
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var IsStartBaiduOCR = await _dal.Db.Queryable<HolOcrConfig>().FirstAsync();
            MedicalRecordDto res;
            if (IsStartBaiduOCR != null && IsStartBaiduOCR.is_enabled == 1 
                && !string.IsNullOrEmpty( IsStartBaiduOCR.api_key) 
                && !string.IsNullOrEmpty( IsStartBaiduOCR.secret_key)  )//启用百度OCR
            {
                var baiduOCR = new OcrCommon("HabweUEtyhlLjBqLCYMkyuEU","NriXrE1pr6wuxjQUT9XzmkxvuaAOvOHs");
                res =await baiduOCR.RecognizeMedicalDocument($@"C:\Users\luqiang\xwechat_files\wxid_i2yjz038qi6222_e10b\msg\attach\9e20f478899dc29eb19741386f9343c8\2025-10\Rec\eb2e694444ad1caf\F\0\temp.pdf");

            }
            else//启用本地OCR
            {
                var localhostOCR =new LocalTesseractOCR("");
                res =await localhostOCR.RecognizeMedicalDocument("");
            }

            // 与数据库中已有信息进行交叉校验
            var validationResult = await ValidateWithDatabase(res);
            res.ValidationStatus = validationResult.IsValid ? "验证通过" : $"验证失败: {validationResult.Message}";

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

    }
}
