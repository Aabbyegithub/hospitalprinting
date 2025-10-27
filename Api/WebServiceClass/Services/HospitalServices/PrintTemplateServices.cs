using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using ModelClassLibrary.Model.HolModel;
using Newtonsoft.Json;
using MyNamespace;

namespace WebServiceClass.Services.HospitalServices
{
    public class PrintTemplateServices(ISqlHelper dal) : IBaseService, IPrintTemplateServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<string>> AddPrintTemplateAsync(HolPrintTemplate template, long OrgId, long UserId)
        {
            try
            {
                // 验证必填字段
                if (string.IsNullOrEmpty(template.name))
                    return Error<string>("模板名称不能为空");

                if (string.IsNullOrEmpty(template.template_type))
                    return Error<string>("模板类型不能为空");

                if (string.IsNullOrEmpty(template.barcode_data_source))
                    return Error<string>("条形码数据来源不能为空");

                template.org_id = OrgId;
                template.create_by = UserId;
                template.create_time = DateTime.Now;
                template.update_time = DateTime.Now;

                var id = await _dal.Db.Insertable(template).ExecuteReturnBigIdentityAsync();
                return id > 0 ? Success("添加成功") : Error<string>("保存失败");
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<List<HolPrintTemplate>> GetPrintTemplatePageAsync(string? name, string? templateType, int page, int size, RefAsync<int> count, long OrgId)
        {
            return await _dal.Db.Queryable<HolPrintTemplate>()
                .Where(a => a.org_id == OrgId && a.status == 1)
                .WhereIF(!string.IsNullOrEmpty(name), a => a.name.Contains(name))
                .WhereIF(!string.IsNullOrEmpty(templateType), a => a.template_type == templateType)
                .OrderBy(a => a.is_default, OrderByType.Desc)
                .OrderBy(a => a.create_time, OrderByType.Desc)
                .ToPageListAsync(page, size, count);
        }

        public async Task<ApiResponse<string>> UpdatePrintTemplateAsync(HolPrintTemplate template, long UserId)
        {
            try
            {
                template.update_by = UserId;
                template.update_time = DateTime.Now;
                var rows = await _dal.Db.Updateable(template).ExecuteCommandAsync();
                return rows > 0 ? Success("更新成功") : Error<string>("更新失败");
            }
            catch (Exception e)
            {
                return Error<string>($"更新失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeletePrintTemplateAsync(List<long> ids, long UserId, long OrgId)
        {
            try
            {
                // 逻辑删除
                await _dal.Db.Updateable<HolPrintTemplate>()
                    .SetColumns(a => new HolPrintTemplate { status = 0, update_time = DateTime.Now, update_by = UserId })
                    .Where(a => ids.Contains(a.id) && a.org_id == OrgId)
                    .ExecuteCommandAsync();

                return Success("删除成功");
            }
            catch (Exception e)
            {
                return Error<string>($"删除失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<HolPrintTemplate>> GetTemplateDetailAsync(long templateId)
        {
            try
            {
                var template = await _dal.Db.Queryable<HolPrintTemplate>()
                    .FirstAsync(a => a.id == templateId && a.status == 1);

                return template != null ? Success(template) : Error<HolPrintTemplate>("模板不存在");
            }
            catch (Exception e)
            {
                return Error<HolPrintTemplate>($"获取失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> SetDefaultTemplateAsync(long templateId, long OrgId, long UserId)
        {
            try
            {
                _dal.Db.Ado.BeginTran();
                try
                {
                    // 先取消所有默认模板
                    await _dal.Db.Updateable<HolPrintTemplate>()
                        .SetColumns(a => new HolPrintTemplate { is_default = 0, update_time = DateTime.Now, update_by = UserId })
                        .Where(a => a.org_id == OrgId && a.template_type == ( _dal.Db.Queryable<HolPrintTemplate>().Where(x => x.id == templateId).Select(x => x.template_type).First()))
                        .ExecuteCommandAsync();

                    // 设置新的默认模板
                    await _dal.Db.Updateable<HolPrintTemplate>()
                        .SetColumns(a => new HolPrintTemplate { is_default = 1, update_time = DateTime.Now, update_by = UserId })
                        .Where(a => a.id == templateId && a.org_id == OrgId)
                        .ExecuteCommandAsync();

                    _dal.Db.Ado.CommitTran();
                    return Success("设置默认模板成功");
                }
                catch (Exception e)
                {
                    _dal.Db.Ado.RollbackTran();
                    return Error<string>($"设置失败：{e.Message}");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"设置失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> PreviewTemplateAsync(long templateId, long examId)
        {
            try
            {
                // 获取模板
                var template = await _dal.Db.Queryable<HolPrintTemplate>()
                    .FirstAsync(a => a.id == templateId && a.status == 1);

                if (template == null)
                    return Error<string>("模板不存在");

                // 获取检查数据
                var exam = await _dal.Db.Queryable<HolExamination>()
                    .Includes(a => a.patient)
                    .Includes(a => a.doctor)
                    .FirstAsync(a => a.id == examId && a.status == 1);

                // 如果没有找到检查记录，创建示例数据用于预览
                if (exam == null)
                {
                    exam = new HolExamination
                    {
                        id = examId,
                        exam_no = "E202501001",
                        exam_date = DateTime.Now,
                        exam_type = "CT",
                        report_no = "R202501001",
                        image_no = "I202501001",
                        org_id = 1,
                        patient = new HolPatient
                        {
                            name = "张三",
                            gender = "男",
                            age = 35,
                            medical_no = "M202501001",
                            id_card = "110101199001011234",
                            contact = "13800138000"
                        },
                        doctor = new HolDoctor
                        {
                            name = "李医生"
                        }
                    };
                }

                // 生成预览HTML
                var html = GeneratePrintHtml(template, exam);
                return Success(html);
            }
            catch (Exception e)
            {
                return Error<string>($"预览失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> PrintTemplateAsync(long templateId, long examId, long UserId, long OrgId)
        {
            try
            {
                // 获取模板
                var template = await _dal.Db.Queryable<HolPrintTemplate>()
                    .FirstAsync(a => a.id == templateId && a.status == 1);

                if (template == null)
                    return Error<string>("模板不存在");

                // 获取检查数据
                var exam = await _dal.Db.Queryable<HolExamination>()
                    .Includes(a => a.patient)
                    .Includes(a => a.doctor)
                    .FirstAsync(a => a.id == examId && a.status == 1);

                if (exam == null)
                    return Error<string>("检查记录不存在");

                // 生成打印HTML
                var html = GeneratePrintHtml(template, exam);

                // 记录打印日志
                await _dal.Db.Insertable(new HolPrintRecord
                {
                    exam_id = examId,
                    template_id = templateId,
                    print_type = template.template_type,
                    print_data = JsonConvert.SerializeObject(new { template = template, exam = exam }),
                    print_status = 1,
                    print_time = DateTime.Now,
                    print_by = UserId,
                    org_id = OrgId,
                    create_time = DateTime.Now
                }).ExecuteCommandAsync();

                return Success(html);
            }
            catch (Exception e)
            {
                return Error<string>($"打印失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<List<object>>> GetAvailableFieldsAsync(string templateType)
        {
            try
            {
                var fields = new List<object>();

                if (templateType == "report")
                {
                    // 胶片模板可用字段
                    fields.AddRange(new[]
                    {
                        new { field = "exam_no", label = "检查号", type = "string" },
                        new { field = "patient_name", label = "患者姓名", type = "string" },
                        new { field = "exam_date", label = "检查日期", type = "datetime" },
                        new { field = "exam_type", label = "检查类型", type = "string" },
                        new { field = "doctor_name", label = "诊断医生", type = "string" },
                        new { field = "report_no", label = "报告编号", type = "string" },
                        new { field = "image_no", label = "胶片检查号", type = "string" },
                        new { field = "org_name", label = "机构名称", type = "string" }
                    });
                }
                else if (templateType == "patient")
                {
                    // 患者模板可用字段
                    fields.AddRange(new[]
                    {
                        new { field = "patient_name", label = "患者姓名", type = "string" },
                        new { field = "gender", label = "性别", type = "string" },
                        new { field = "age", label = "年龄", type = "number" },
                        new { field = "medical_no", label = "就诊号", type = "string" },
                        new { field = "id_card", label = "身份证号", type = "string" },
                        new { field = "contact", label = "联系方式", type = "string" },
                        new { field = "create_time", label = "创建时间", type = "datetime" }
                    });
                }

                return Success(fields);
            }
            catch (Exception e)
            {
                return Error<List<object>>($"获取字段失败：{e.Message}");
            }
        }

        /// <summary>
        /// 生成打印HTML
        /// </summary>
        private string GeneratePrintHtml(HolPrintTemplate template, HolExamination exam)
        {
            var displayFields = JsonConvert.DeserializeObject<List<dynamic>>(template.display_fields) ?? new List<dynamic>();
            var barcodeData = GetFieldValue(exam, template.barcode_data_source);
            var orgName = GetOrgName(exam.org_id);

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>打印模板</title>
    <style>
        body {{ 
            font-family: 'Microsoft YaHei', Arial, sans-serif; 
            margin: 0; 
            padding: 20px; 
            background: white;
            font-size: 14px;
            line-height: 1.4;
        }}
        .print-container {{
            max-width: 800px;
            margin: 0 auto;
            background: white;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            padding: 40px;
            min-height: 600px;
            position: relative;
        }}
        .header {{
            text-align: center;
            margin-bottom: 40px;
            border-bottom: 3px solid #1e40af;
            padding-bottom: 25px;
        }}
        .hospital-name {{
            font-size: 28px;
            font-weight: bold;
            color: #1e40af;
            margin-bottom: 8px;
            letter-spacing: 2px;
        }}
        .report-title {{
            font-size: 20px;
            font-weight: bold;
            color: #374151;
            margin-bottom: 15px;
        }}
        .barcode-section {{
            position: absolute;
            top: 30px;
            right: 30px;
            width: 100px;
            height: 60px;
            background: white;
            border: 1px solid #d1d5db;
            border-radius: 6px;
            padding: 5px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }}
        .barcode {{
            width: 100%;
            height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        .barcode svg {{
            max-width: 100%;
            max-height: 100%;
        }}
        .barcode canvas {{
            max-width: 100%;
            max-height: 100%;
        }}
        .barcode-text {{
            display: none;
        }}
        .info-section {{
            margin-bottom: 40px;
            margin-right: 120px;
        }}
        .info-grid {{
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
            margin-top: 25px;
        }}
        .info-item {{
            display: flex;
            align-items: center;
            padding: 12px 0;
            border-bottom: 1px solid #e5e7eb;
            transition: background-color 0.2s;
        }}
        .info-item:hover {{
            background-color: #f9fafb;
        }}
        .info-label {{
            font-weight: 600;
            color: #374151;
            margin-right: 20px;
            min-width: 120px;
            flex-shrink: 0;
            font-size: 15px;
        }}
        .info-value {{
            flex: 1;
            color: #6b7280;
            font-size: 15px;
            font-weight: 500;
        }}
        .footer {{
            margin-top: 50px;
            text-align: center;
            font-size: 13px;
            color: #9ca3af;
            border-top: 2px solid #e5e7eb;
            padding-top: 25px;
        }}
        @media print {{
            body {{ margin: 0; padding: 0; }}
            .print-container {{ box-shadow: none; }}
        }}
    </style>
</head>
<body>
    <div class='print-container'>
        <!-- 头部 -->
        <div class='header'>
            <div class='hospital-name'>{orgName}</div>
            <div class='report-title'>{GetTemplateTitle(template.template_type)}</div>
        </div>
        
        <!-- 右上角条形码 -->
        <div class='barcode-section'>
            <div class='barcode'>
                <div id='barcode'></div>
            </div>
        </div>
        
        <!-- 信息区域 -->
        <div class='info-section'>
            <div class='info-grid'>";

            foreach (dynamic field in displayFields)
            {
                if (field.show == true)
                {
                    var value = GetFieldValue(exam, field.field.ToString());
                    html += $@"
                <div class='info-item'>
                    <div class='info-label'>{field.label}:</div>
                    <div class='info-value'>{value}</div>
                </div>";
                }
            }

            html += $@"
            </div>
        </div>
        
        <!-- 底部 -->
        <div class='footer'>
            <div>打印时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</div>
        </div>
    </div>
    
    <script src='https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js'></script>
    <script>
        JsBarcode('#barcode', '{barcodeData}', {{
            format: '{template.barcode_type}',
            width: 1,
            height: 50,
            displayValue: false,
            background: 'transparent',
            lineColor: '#000000',
            margin: 1,
            fontSize: 8
        }});
    </script>
</body>
</html>";

            return html;
        }

        /// <summary>
        /// 获取机构名称
        /// </summary>
        private string GetOrgName(long orgId)
        {
            try
            {
                // 查询机构表获取机构名称
                var org = _dal.Db.Queryable<sys_orgid>()
                    .First(a => a.orgid_id == orgId);
                return org?.orgid_name ?? "医院名称";
            }
            catch
            {
                return "医院名称";
            }
        }

        /// <summary>
        /// 获取模板标题
        /// </summary>
        private string GetTemplateTitle(string templateType)
        {
            return templateType switch
            {
                "report" => "胶片打印标签",
                "patient" => "患者信息标签",
                _ => "打印标签"
            };
        }

        /// <summary>
        /// 获取字段值
        /// </summary>
        private string GetFieldValue(HolExamination exam, string fieldName)
        {
            return fieldName switch
            {
                "exam_no" => exam.exam_no ?? "",
                "patient_name" => exam.patient?.name ?? "",
                "exam_date" => exam.exam_date.ToString("yyyy-MM-dd"),
                "exam_type" => exam.exam_type ?? "",
                "doctor_name" => exam.doctor?.name ?? "",
                "report_no" => exam.report_no ?? "",
                "image_no" => exam.image_no ?? "",
                "gender" => exam.patient?.gender ?? "",
                "age" => exam.patient?.age?.ToString() ?? "",
                "medical_no" => exam.patient?.medical_no ?? "",
                "id_card" => exam.patient?.id_card ?? "",
                "contact" => exam.patient?.contact ?? "",
                "create_time" => exam.create_time.ToString("yyyy-MM-dd HH:mm:ss"),
                _ => ""
            };
        }
    }
}
