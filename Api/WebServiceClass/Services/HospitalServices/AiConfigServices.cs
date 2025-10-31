using ModelClassLibrary.Model.Dto.HolDto;
using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IServices.HospitalIServices;
using WebIServices.IBase;

namespace WebServiceClass.Services.HospitalServices
{
    /// <summary>
    /// AI配置服务实现
    /// </summary>
    public class AiConfigServices(ISqlHelper dal) : IBaseService, IAiConfigServices
    {
        private readonly ISqlHelper _dal = dal;

        /// <summary>
        /// 获取AI配置
        /// </summary>
        public async Task<ApiResponse<HolAiConfigDto>> GetConfigAsync(long orgId)
        {
            try
            {
                var config = await _dal.Db.Queryable<HolAiConfig>()
                    .Where(x => x.org_id == orgId)
                    .FirstAsync();

                if (config == null)
                {
                    // 返回默认空配置
                    return Success(new HolAiConfigDto
                    {
                        id = 0,
                        org_id = orgId,
                        is_enabled = 0,
                        api_url = string.Empty,
                        api_key = string.Empty,
                        remark = string.Empty
                    });
                }

                var dto = new HolAiConfigDto
                {
                    id = config.id,
                    org_id = config.org_id,
                    is_enabled = config.is_enabled,
                    api_url = config.api_url,
                    api_key = DecryptApiKey(config.api_key), // 解密
                    remark = config.remark
                };

                return Success(dto);
            }
            catch (Exception e)
            {
                return Error<HolAiConfigDto>($"获取配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 保存AI配置
        /// </summary>
        public async Task<ApiResponse<string>> SaveConfigAsync(HolAiConfigDto dto)
        {
            try
            {
                // 验证必填字段
                if (dto.is_enabled == 1)
                {
                    if (string.IsNullOrWhiteSpace(dto.api_url))
                    {
                        return Error<string>("AI接口URL不能为空");
                    }
                    if (string.IsNullOrWhiteSpace(dto.api_key))
                    {
                        return Error<string>("API密钥不能为空");
                    }
                }

                var existingConfig = await _dal.Db.Queryable<HolAiConfig>()
                    .Where(x => x.org_id == dto.org_id)
                    .FirstAsync();

                if (existingConfig == null)
                {
                    // 新增
                    var newConfig = new HolAiConfig
                    {
                        org_id = dto.org_id,
                        is_enabled = dto.is_enabled,
                        api_url = dto.api_url ?? string.Empty,
                        api_key = EncryptApiKey(dto.api_key ?? string.Empty), // 加密
                        remark = dto.remark,
                        create_time = DateTime.Now,
                        update_time = DateTime.Now
                    };

                    await _dal.Db.Insertable(newConfig).ExecuteCommandAsync();
                    return Success("保存配置成功");
                }
                else
                {
                    // 更新
                    existingConfig.is_enabled = dto.is_enabled;
                    existingConfig.api_url = dto.api_url ?? string.Empty;
                    existingConfig.api_key = EncryptApiKey(dto.api_key ?? string.Empty); // 加密
                    existingConfig.remark = dto.remark;
                    existingConfig.update_time = DateTime.Now;

                    await _dal.Db.Updateable(existingConfig).ExecuteCommandAsync();
                    return Success("更新配置成功");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"保存配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 测试AI连接
        /// </summary>
        public async Task<ApiResponse<string>> TestConnectionAsync(HolAiConfigDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.api_url))
                {
                    return Error<string>("AI接口URL不能为空");
                }
                if (string.IsNullOrWhiteSpace(dto.api_key))
                {
                    return Error<string>("API密钥不能为空");
                }

                // 使用 HttpClient 发送测试请求
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

                    // 添加Authorization头
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {dto.api_key}");

                    // 发送一个简单的测试请求
                    var response = await client.GetAsync(dto.api_url);

                    if (response.IsSuccessStatusCode)
                    {
                        return Success("连接测试成功");
                    }
                    else
                    {
                        return Error<string>($"连接测试失败：{response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception e)
            {
                return Error<string>($"连接测试失败：{e.Message}");
            }
        }

        /// <summary>
        /// 加密API密钥
        /// </summary>
        private string EncryptApiKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return string.Empty;

            // 使用简单的Base64编码（生产环境建议使用更安全的加密方式）
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        /// 解密API密钥
        /// </summary>
        private string DecryptApiKey(string encryptedKey)
        {
            if (string.IsNullOrWhiteSpace(encryptedKey))
                return string.Empty;

            try
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedKey));
            }
            catch
            {
                return encryptedKey; // 如果解密失败，返回原值
            }
        }

        // Helper methods
        //private ApiResponse<T> Success<T>(T data)
        //{
        //    return new ApiResponse<T> { success = true, Response = data };
        //}

        //private ApiResponse<T> Error<T>(string message)
        //{
        //    return new ApiResponse<T> { success = false, Message = message };
        //}
    }
}
