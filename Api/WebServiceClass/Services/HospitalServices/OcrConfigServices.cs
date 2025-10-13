using ModelClassLibrary.Model.HolModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;
using WebIServices.IServices.HospitalIServices;

namespace WebServiceClass.Services.HospitalServices
{
    public class OcrConfigServices(ISqlHelper dal) : IBaseService, IOcrConfigServices
    {
        private readonly ISqlHelper _dal = dal;

        public async Task<ApiResponse<HolOcrConfig>> GetOcrConfigAsync(long OrgId)
        {
            try
            {
                var config = await _dal.Db.Queryable<HolOcrConfig>()
                    .Where(a => a.org_id == OrgId)
                    .FirstAsync();

                if (config == null)
                {
                    // 如果不存在配置，返回默认配置
                    config = new HolOcrConfig
                    {
                        id = 0,
                        is_enabled = 0,
                        org_id = OrgId,
                        create_time = DateTime.Now,
                        update_time = DateTime.Now
                    };
                }

                return Success(config);
            }
            catch (Exception e)
            {
                return Error<HolOcrConfig>($"获取配置失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> SaveOcrConfigAsync(HolOcrConfig ocrConfig, long OrgId, long UserId)
        {
            try
            {
                // 如果启用OCR，验证必填字段
                if (ocrConfig.is_enabled == 1)
                {
                    if (string.IsNullOrEmpty(ocrConfig.api_url)) return Error<string>("API URL不能为空");
                    if (string.IsNullOrEmpty(ocrConfig.app_id)) return Error<string>("应用ID不能为空");
                    if (string.IsNullOrEmpty(ocrConfig.api_key)) return Error<string>("API Key不能为空");
                    if (string.IsNullOrEmpty(ocrConfig.secret_key)) return Error<string>("Secret Key不能为空");
                }

                ocrConfig.org_id = OrgId;
                var now = DateTime.Now;

                // 检查是否已存在配置
                var existingConfig = await _dal.Db.Queryable<HolOcrConfig>()
                    .Where(a => a.org_id == OrgId)
                    .FirstAsync();

                if (existingConfig != null)
                {
                    // 更新现有配置
                    ocrConfig.id = existingConfig.id;
                    ocrConfig.create_time = existingConfig.create_time;
                    ocrConfig.update_time = now;

                    var rows = await _dal.Db.Updateable(ocrConfig).ExecuteCommandAsync();
                    return rows > 0 ? Success("保存成功") : Error<string>("保存失败");
                }
                else
                {
                    // 创建新配置
                    ocrConfig.create_time = now;
                    ocrConfig.update_time = now;

                    var id = await _dal.Db.Insertable(ocrConfig).ExecuteReturnBigIdentityAsync();
                    return id > 0 ? Success("保存成功") : Error<string>("保存失败");
                }
            }
            catch (Exception e)
            {
                return Error<string>($"保存失败：{e.Message}");
            }
        }

        public async Task<ApiResponse<string>> TestOcrConnectionAsync(HolOcrConfig ocrConfig)
        {
            try
            {
                if (ocrConfig.is_enabled != 1)
                {
                    return Error<string>("OCR功能未启用");
                }

                // 这里可以添加实际的百度云OCR API测试逻辑
                // 例如：调用百度云API获取访问令牌
                using var httpClient = new HttpClient();
                var requestData = new
                {
                    grant_type = "client_credentials",
                    client_id = ocrConfig.api_key,
                    client_secret = ocrConfig.secret_key
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://aip.baidubce.com/oauth/2.0/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    if (tokenResponse.TryGetProperty("access_token", out var accessToken))
                    {
                        return Success("连接测试成功");
                    }
                }

                return Error<string>("连接测试失败，请检查配置信息");
            }
            catch (Exception e)
            {
                return Error<string>($"连接测试失败：{e.Message}");
            }
        }
    }
}
