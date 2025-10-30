using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using Quartz;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebTaskClass.SampleJob
{
    public class SyncHolPatientTask : IJob
    {
        private readonly ISqlHelper _dal;
        private readonly ILoggerHelper _logger;
        private readonly IAppSettinghelper _appconfig;

        public SyncHolPatientTask(ISqlHelper dal, ILoggerHelper logger, IAppSettinghelper appconfig)
        {
            _dal = dal;
            _logger = logger;
            _appconfig = appconfig;
        }

        /// <summary>
        /// 实时同步患者相关数据
        /// </summary>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var conDbList = await _dal.Db.Queryable<HolDbConfig>()
                                           .Where(a => a.status == 1)
                                           .ToListAsync();

                if (conDbList == null || !conDbList.Any())
                {
                    await _logger.LogInfo("没有需要同步的数据库配置");
                    return;
                }

                var semaphore = new SemaphoreSlim(3); // 限制最大并发数为3
                var tasks = new List<Task>();

                foreach (var dbConfig in conDbList)
                {
                    await semaphore.WaitAsync(); // 等待信号量，控制并发
                    tasks.Add(ProcessDbConfigAsync(dbConfig, semaphore)); // 传入信号量用于释放
                }

                // 等待所有任务完成
                await Task.WhenAll(tasks);
                await _logger.LogInfo("所有数据库同步任务已完成");
            }
            catch (Exception ex)
            {
                await _logger.LogError($"同步任务执行失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 异步执行单个数据库的同步操作（修正 async void 问题）
        /// </summary>
        private async Task ProcessDbConfigAsync(HolDbConfig holDbConfig, SemaphoreSlim semaphore)
        {
            try
            {
                string connectionString = BuildConnectionString(holDbConfig);
                if (string.IsNullOrEmpty(connectionString))
                {
                    await _logger.LogError($"数据库 {holDbConfig.config_name} 连接字符串生成失败");
                    return;
                }

                var db = _dal.CreateConnection(new ConnectionConfig
                {
                    ConnectionString = connectionString,
                    DbType = GetDbType(holDbConfig.database_type),
                });

                // 执行同步逻辑（示例：同步患者表）
                await SyncPatientDataAsync(db);
            }
            catch (Exception ex)
            {
                await _logger.LogError($"同步数据库 {holDbConfig.config_name} 失败：{ex.Message}");
            }
            finally
            {
                semaphore.Release(); // 释放信号量，允许其他任务执行
            }
        }

        /// <summary>
        /// 构建MySQL连接字符串
        /// </summary>
        private string BuildConnectionString(HolDbConfig config)
        {
            var type = (config.database_type ?? "MySQL").ToLower();
            var password = DecryptPasswordAsync(config.password_enc);
            return type switch
            {
                "mysql" => $"Server={config.server_ip};Port={config.server_port};Database={config.database_name};Uid={config.username};Pwd={password};",
                "sqlserver" => $"Server={config.server_ip},{config.server_port};Database={config.database_name};User Id={config.username};Password={password};",
                "oracle" => $"Data Source={config.server_ip}:{config.server_port}/{config.database_name};User Id={config.username};Password={password};",
                "postgresql" => $"Host={config.server_ip};Port={config.server_port};Database={config.database_name};Username={config.username};Password={password};",
                _ => $"Server={config.server_ip};Port={config.server_port};Database={config.database_name};Uid={config.username};Pwd={password};"
            };
        }

        /// <summary>
        /// 数据库类型映射（SqlSugar DbType）
        /// </summary>
        private DbType GetDbType(string dbType)
        {
            return dbType?.ToLower() switch
            {
                "mysql" => DbType.MySql,
                "sqlserver" => DbType.SqlServer,
                "sqlite" => DbType.Sqlite,
                "oracle" => DbType.Oracle,
                "postgresql" => DbType.PostgreSQL,
                "dm" => DbType.Dm,
                "kdbndp" => DbType.Kdbndp,
                "oscar" => DbType.Oscar,
                "mysqlconnector" => DbType.MySqlConnector,
                "access" => DbType.Access,
                "opengauss" => DbType.OpenGauss,
                "custom" => DbType.Custom,
                _ => throw new NotSupportedException($"不支持的数据库类型：{dbType}")
            };
        }

        /// <summary>
        /// 使用MySQL AES_DECRYPT解密密码
        /// </summary>
        private async Task<string> DecryptPasswordAsync(string encryptedPassword)
        {
            try
            {
                var AES_KEY = _appconfig.Get("AES_KEY:KEY");
                // 使用原生SQL执行AES_DECRYPT
                var result = await _dal.Db.Ado.SqlQueryAsync<string>($"SELECT AES_DECRYPT(UNHEX('{encryptedPassword}'), '{AES_KEY}')");
                return result.FirstOrDefault() ?? string.Empty;
            }
            catch (Exception)
            {
                // 如果AES_DECRYPT失败，尝试Base64解码
                try
                {
                    return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPassword));
                }
                catch
                {
                    return encryptedPassword;
                }
            }
        }

        /// <summary>
        /// 同步患者数据的具体逻辑
        /// </summary>
        private async Task SyncPatientDataAsync(ISqlSugarClient db)
        {
            var Time = DateTime.Now.AddHours(-1);
            var ThisSysPatient = await _dal.Db.Queryable<HolPatient>().Where(a => a.createtime > Time.AddHours(-5)).ToListAsync();
            var ThisSysHolExamination = await _dal.Db.Queryable<HolExamination>().Where(a => a.create_time > Time.AddHours(-3)).ToListAsync();
            var PatientInfo = await db.Queryable<HolMedicalRecord>("VIEW_FOR_PATIENTINFO")
                .Where(a => a.CreateTime >= Time).ToListAsync();
            var Data = PatientInfo.Select(a => new HolPatient
            {
                name = a.Name,
                gender = a.Sex,
                age = a.Age,
                id_card = a.IdNumber,
                contact = a.Phone,
                medical_no = a.OutPatientNo,
                createtime = a.CreateTime,
                updatetime = a.UpdateTime,
                status = 1,
                Account_no = a.AccessionNumber
            }).DistinctBy(a => a.medical_no).ToList();
            foreach (var item in Data)
            {
                var IsHas = ThisSysPatient.FirstOrDefault(a => a.medical_no == item.medical_no);
                
                if (IsHas != null)
                {
                    var IsExaminationHas = ThisSysHolExamination.Where(a => a.patient_id == IsHas.id).ToList();
                    item.id = IsHas.id;
                    await _dal.Db.Updateable(item).ExecuteCommandAsync();
                    var ItemList = PatientInfo.Where(a => a.OutPatientNo == item.medical_no)
                                .Select(a => new HolExamination
                                {
                                    id = IsExaminationHas.FirstOrDefault(b=>b.exam_no ==  a.AccessionNumber) == null ? 0:IsExaminationHas.FirstOrDefault(b=>b.exam_no ==  a.AccessionNumber).id,
                                    exam_no = a.AccessionNumber,
                                    patient_id = IsHas.id,
                                    exam_type = a.Modality,
                                    exam_date = a.StudyDate,
                                    report_path = a.PdfReportUrl,
                                    //image_path = a.Dicom_path,
                                    create_time = a.CreateTime,
                                    update_time = a.UpdateTime,
                                    image_no = a.StudyUid,
                                    report_status = a.ReportStatus,
                                    card_no = a.CardNo,
                                    in_patient_no = a.InPatientNo,
                                    tj_no = a.TjNo,
                                    id_number = a.IdNumber,
                                    medicare_id = a.MedicareId,
                                    patient_class = a.PatientClass,
                                    report_doctor = a.ReportDoctor,
                                    audit_doctor = a.AuditDoctor,
                                    reg_date = a.RegDate,
                                    report_date = a.ReportDate,
                                    audit_date = a.AuditDate,
                                    body_part = a.BodyPart,
                                    req_dept = a.ReqDept,
                                    req_physician = a.ReqPhysician,
                                    ward = a.Ward,
                                    bed_no = a.BedNo,
                                    exam_method = a.ExamMethod,
                                    description = a.Description,
                                    impression = a.Impression,
                                    recommendation = a.Recommendation,
                                    report_doctor_sign = a.ReportDoctorSign,
                                    audit_doctor_sign = a.AuditDoctorSign,
                                    studyuid = a.StudyUid,
                                    phone = a.Phone,
                                    need_efilm = (byte)a.NeedEfilm.ObjToInt(),
                                    filmtype = a.FilmType,
                                    isfees = (byte)(a.Isfee == true ? 1 : 0),
                                }).ToList();
                    await  _dal.Db.Insertable(ItemList.Where(a=>a.id == 0).ToList()).ExecuteCommandAsync();
                    await  _dal.Db.Updateable(ItemList.Where(a=>a.id != 0).ToList()).ExecuteCommandAsync();
                }
                else
                {
                    var InSertId = await _dal.Db.Insertable(item).ExecuteReturnBigIdentityAsync();
                    var ItemList = PatientInfo.Where(a => a.OutPatientNo == item.medical_no)
                        .Select(a => new HolExamination
                        {
                            exam_no = a.AccessionNumber,
                            patientexid = a.PatientId,
                            patient_id = InSertId,
                            exam_type = a.Modality,
                            exam_date = a.StudyDate,
                            report_path = a.PdfReportUrl,
                            //image_path = a.Dicom_path,
                            create_time = a.CreateTime,
                            update_time = a.UpdateTime,
                            image_no = a.StudyUid,
                            report_status = a.ReportStatus,
                            card_no = a.CardNo,
                            in_patient_no = a.InPatientNo,
                            tj_no = a.TjNo,
                            id_number = a.IdNumber,
                            medicare_id = a.MedicareId,
                            patient_class = a.PatientClass,
                            report_doctor = a.ReportDoctor,
                            audit_doctor = a.AuditDoctor,
                            reg_date = a.RegDate,
                            report_date = a.ReportDate,
                            audit_date = a.AuditDate,
                            body_part = a.BodyPart,
                            req_dept = a.ReqDept,
                            req_physician = a.ReqPhysician,
                            ward = a.Ward,
                            bed_no = a.BedNo,
                            exam_method = a.ExamMethod,
                            description = a.Description,
                            impression = a.Impression,
                            recommendation = a.Recommendation,
                            report_doctor_sign = a.ReportDoctorSign,
                            audit_doctor_sign = a.AuditDoctorSign,
                            studyuid = a.StudyUid,
                            phone = a.Phone,
                            need_efilm = (byte)a.NeedEfilm.ObjToInt(),
                            filmtype = a.FilmType,
                            isfees =(byte)(a.Isfee== true ? 1:0),
                        }).ToList();
                    await _dal.Db.Insertable(ItemList).ExecuteCommandAsync();
                }
            }


        }
    }
}