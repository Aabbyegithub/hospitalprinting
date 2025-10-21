using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using static WebProjectTest.Common.Message;
using System.Text;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;
using Aliyun.OSS;

namespace WebServiceClass.Services.HospitalServices
{
    /// <summary>
    /// 阿里云OSS配置服务
    /// </summary>
    public class OssConfigServices(ISqlHelper dal) : IBaseService, IOssConfigServices
    {
        private readonly ISqlHelper _dal = dal;

        /// <summary>
        /// 获取OSS配置
        /// </summary>
        public async Task<ApiResponse<HolOssConfigDto>> GetConfigAsync(long orgId)
        {
            try
            {
                var config = await _dal.Db.Queryable<HolOssConfig>()
                    .Where(x => x.org_id == orgId)
                    .FirstAsync();

                if (config == null)
                {
                    return Success(new HolOssConfigDto
                    {
                        org_id = orgId,
                        is_enabled = 0
                    }, "获取配置成功");
                }

                var dto = new HolOssConfigDto
                {
                    id = config.id,
                    org_id = config.org_id,
                    is_enabled = config.is_enabled,
                    endpoint = config.endpoint,
                    access_key_id = config.access_key_id,
                    access_key_secret = config.access_key_secret,
                    bucket_name = config.bucket_name,
                    region = config.region,
                    folder_prefix = config.folder_prefix,
                    create_time = config.create_time,
                    update_time = config.update_time
                };

                return Success(dto, "获取配置成功");
            }
            catch (Exception e)
            {
                return Error<HolOssConfigDto>($"获取配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 保存OSS配置
        /// </summary>
        public async Task<ApiResponse<string>> SaveConfigAsync(HolOssConfigDto config, long orgId, long userId)
        {
            try
            {
                if (config.is_enabled == 1)
                {
                    if (string.IsNullOrEmpty(config.endpoint))
                        return Error<string>("OSS访问域名不能为空");
                    if (string.IsNullOrEmpty(config.access_key_id))
                        return Error<string>("AccessKey ID不能为空");
                    if (string.IsNullOrEmpty(config.access_key_secret))
                        return Error<string>("AccessKey Secret不能为空");
                    if (string.IsNullOrEmpty(config.bucket_name))
                        return Error<string>("存储桶名称不能为空");
                    if (string.IsNullOrEmpty(config.region))
                        return Error<string>("地域不能为空");
                }

                var existingConfig = await _dal.Db.Queryable<HolOssConfig>()
                    .Where(x => x.org_id == orgId)
                    .FirstAsync();

                if (existingConfig == null)
                {
                    // 新增配置
                    var newConfig = new HolOssConfig
                    {
                        org_id = orgId,
                        is_enabled = config.is_enabled,
                        endpoint = config.endpoint,
                        access_key_id = config.access_key_id,
                        access_key_secret = config.access_key_secret,
                        bucket_name = config.bucket_name,
                        region = config.region,
                        folder_prefix = config.folder_prefix,
                        create_time = DateTime.Now,
                        update_time = DateTime.Now
                    };

                    await _dal.Db.Insertable(newConfig).ExecuteCommandAsync();
                }
                else
                {
                    // 更新配置
                    existingConfig.is_enabled = config.is_enabled;
                    existingConfig.endpoint = config.endpoint;
                    existingConfig.access_key_id = config.access_key_id;
                    existingConfig.access_key_secret = config.access_key_secret;
                    existingConfig.bucket_name = config.bucket_name;
                    existingConfig.region = config.region;
                    existingConfig.folder_prefix = config.folder_prefix;
                    existingConfig.update_time = DateTime.Now;

                    await _dal.Db.Updateable(existingConfig).ExecuteCommandAsync();
                }

                return Success("保存配置成功");
            }
            catch (Exception e)
            {
                return Error<string>($"保存配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 测试OSS连接
        /// </summary>
        public async Task<ApiResponse<string>> TestConnectionAsync(HolOssConfigDto config)
        {
            try
            {
                if (config.is_enabled != 1)
                    return Error<string>("请先启用OSS配置");

                if (string.IsNullOrEmpty(config.endpoint) ||
                    string.IsNullOrEmpty(config.access_key_id) ||
                    string.IsNullOrEmpty(config.access_key_secret) ||
                    string.IsNullOrEmpty(config.bucket_name))
                {
                    return Error<string>("OSS配置信息不完整");
                }

                var client = new OssClient(config.endpoint, config.access_key_id, config.access_key_secret);

                // 测试连接
                var bucketExists = await Task.Run(() => client.DoesBucketExist(config.bucket_name));

                if (bucketExists)
                {
                    return Success("OSS连接测试成功");
                }
                else
                {
                    return Error<string>("存储桶不存在或无法访问");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"OSS连接测试失败：{e.Message}");
            }
        }

        /// <summary>
        /// 上传文件到OSS
        /// </summary>
        public async Task<ApiResponse<string>> UploadFileAsync(long examId, string filePath, string fileName, long orgId)
        {
            try
            {
                // 获取OSS配置
                var configResponse = await GetConfigAsync(orgId);
                if (!configResponse.success || configResponse.Response == null)
                    return Error<string>("获取OSS配置失败");

                var config = configResponse.Response;
                if (config.is_enabled != 1)
                    return Error<string>("OSS功能未启用");

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    return Error<string>("文件不存在");

                // 创建OSS客户端
                var client = new OssClient(config.endpoint, config.access_key_id, config.access_key_secret);

                // 生成OSS对象键
                var objectKey = $"{config.folder_prefix ?? "examinations"}/{examId}/{fileName}";

                // 上传文件
                var result = await Task.Run(() =>
                {
                    return client.PutObject(config.bucket_name, objectKey, filePath);
                });

                if (result != null)
                {
                    // 生成访问URL
                    var url = $"https://{config.bucket_name}.{config.endpoint.Replace("https://", "").Replace("http://", "")}/{objectKey}";
                    return Success(url, "文件上传成功");
                }
                else
                {
                    return Error<string>("文件上传失败");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"文件上传失败：{e.Message}");
            }
        }
    }
}
