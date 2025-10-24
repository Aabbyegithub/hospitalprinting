using ModelClassLibrary.Model.HolModel;
using SqlSugar;
using static WebProjectTest.Common.Message;
using System.Data;
using MySqlConnector;
using WebIServices.IBase;

namespace WebServiceClass.Services.HospitalServices
{
    /// <summary>
    /// 数据库配置服务
    /// </summary>
    public class DbConfigServices : IBaseService, IDbConfigServices
    {
        private readonly ISqlHelper _dal;
        private readonly IAppSettinghelper _appconfig;
        public DbConfigServices(ISqlHelper dal, IAppSettinghelper appconfig) 
        {
            _dal = dal;
            _appconfig = appconfig;
        }
        
         string AES_KEY="" ;

        /// <summary>
        /// 获取数据库配置列表
        /// </summary>
        public async Task<ApiResponse<List<HolDbConfigDto>>> GetConfigListAsync(string configName, long orgId, int pageIndex, int pageSize)
        {
            try
            {
                var query = _dal.Db.Queryable<HolDbConfig>()
                    .Where(x => x.org_id == orgId && x.status == 1);

                if (!string.IsNullOrEmpty(configName))
                {
                    query = query.Where(x => x.config_name.Contains(configName));
                }

                var configs = await query
                    .OrderBy(x => x.create_time, OrderByType.Desc)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = configs.Select(config => new HolDbConfigDto
                {
                    id = config.id,
                    config_name = config.config_name,
                    server_ip = config.server_ip,
                    server_port = config.server_port,
                    database_name = config.database_name,
                    username = config.username,
                    database_type = config.database_type,
                    password = "******", // 密码不返回明文
                    org_id = config.org_id,
                    status = config.status,
                    create_time = config.create_time,
                    update_time = config.update_time
                }).ToList();

                return Success(result, "获取配置列表成功");
            }
            catch (Exception e)
            {
                return Error<List<HolDbConfigDto>>($"获取配置列表失败：{e.Message}");
            }
        }

        /// <summary>
        /// 获取数据库配置详情
        /// </summary>
        public async Task<ApiResponse<HolDbConfigDto>> GetConfigByIdAsync(long id)
        {
            try
            {
                var config = await _dal.Db.Queryable<HolDbConfig>()
                    .Where(x => x.id == id && x.status == 1)
                    .FirstAsync();

                if (config == null)
                {
                    return Error<HolDbConfigDto>("配置不存在");
                }

                var result = new HolDbConfigDto
                {
                    id = config.id,
                    config_name = config.config_name,
                    server_ip = config.server_ip,
                    server_port = config.server_port,
                    database_name = config.database_name,
                    username = config.username,
                    database_type = config.database_type,
                    password = "******", // 密码不返回明文
                    org_id = config.org_id,
                    status = config.status,
                    create_time = config.create_time,
                    update_time = config.update_time
                };

                return Success(result, "获取配置详情成功");
            }
            catch (Exception e)
            {
                return Error<HolDbConfigDto>($"获取配置详情失败：{e.Message}");
            }
        }

        /// <summary>
        /// 新增数据库配置
        /// </summary>
        public async Task<ApiResponse<string>> AddConfigAsync(HolDbConfigDto configDto)
        {
            try
            {
                // 检查配置名称是否重复
                var existingConfig = await _dal.Db.Queryable<HolDbConfig>()
                    .Where(x => x.config_name == configDto.config_name && x.org_id == configDto.org_id && x.status == 1)
                    .FirstAsync();

                if (existingConfig != null)
                {
                    return Error<string>("配置名称已存在");
                }

                // 使用MySQL AES_ENCRYPT加密密码
                var encryptedPassword = await EncryptPasswordAsync(configDto.password);

                var config = new HolDbConfig
                {
                    config_name = configDto.config_name,
                    server_ip = configDto.server_ip,
                    server_port = configDto.server_port,
                    database_name = configDto.database_name,
                    username = configDto.username,
                    database_type = configDto.database_type,
                    password_enc = encryptedPassword,
                    org_id = configDto.org_id,
                    status = 1,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now
                };

                await _dal.Db.Insertable(config).ExecuteCommandAsync();

                return Success("新增配置成功");
            }
            catch (Exception e)
            {
                return Error<string>($"新增配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 更新数据库配置
        /// </summary>
        public async Task<ApiResponse<string>> UpdateConfigAsync(HolDbConfigDto configDto)
        {
            try
            {
                var existingConfig = await _dal.Db.Queryable<HolDbConfig>()
                    .Where(x => x.id == configDto.id && x.status == 1)
                    .FirstAsync();

                if (existingConfig == null)
                {
                    return Error<string>("配置不存在");
                }

                // 检查配置名称是否重复（排除自己）
                var duplicateConfig = await _dal.Db.Queryable<HolDbConfig>()
                    .Where(x => x.config_name == configDto.config_name && x.org_id == configDto.org_id && x.id != configDto.id && x.status == 1)
                    .FirstAsync();

                if (duplicateConfig != null)
                {
                    return Error<string>("配置名称已存在");
                }

                existingConfig.config_name = configDto.config_name;
                existingConfig.server_ip = configDto.server_ip;
                existingConfig.server_port = configDto.server_port;
                existingConfig.database_name = configDto.database_name;
                existingConfig.username = configDto.username;
                existingConfig.database_type = configDto.database_type;

                // 如果密码不是******，则更新密码
                if (configDto.password != "******" && !string.IsNullOrEmpty(configDto.password))
                {
                    existingConfig.password_enc = await EncryptPasswordAsync(configDto.password);
                }

                // 显式指定更新列，确保 database_type 被持久化
                await _dal.Db.Updateable<HolDbConfig>()
                    .SetColumns(x => new HolDbConfig
                    {
                        config_name = existingConfig.config_name,
                        server_ip = existingConfig.server_ip,
                        server_port = existingConfig.server_port,
                        database_name = existingConfig.database_name,
                        username = existingConfig.username,
                        database_type = existingConfig.database_type,
                        password_enc = existingConfig.password_enc,
                        update_time = existingConfig.update_time
                    })
                    .Where(x => x.id == existingConfig.id)
                    .ExecuteCommandAsync();

                return Success("更新配置成功");
            }
            catch (Exception e)
            {
                return Error<string>($"更新配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 删除数据库配置
        /// </summary>
        public async Task<ApiResponse<string>> DeleteConfigAsync(List<long> ids)
        {
            try
            {
                await _dal.Db.Updateable<HolDbConfig>()
                    .SetColumns(x => new HolDbConfig { status = 0, update_time = DateTime.Now })
                    .Where(x => ids.Contains(x.id))
                    .ExecuteCommandAsync();

                return Success("删除配置成功");
            }
            catch (Exception e)
            {
                return Error<string>($"删除配置失败：{e.Message}");
            }
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        public async Task<ApiResponse<string>> TestConnectionAsync(HolDbConfigDto config)
        {
            try
            {
                var connectionString =await BuildConnectionString(config);

                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                return Success("数据库连接测试成功");
            }
            catch (Exception e)
            {
                return Error<string>($"数据库连接测试失败：{e.Message}");
            }
        }

        #region 私有方法

        /// <summary>
        /// 使用MySQL AES_ENCRYPT加密密码
        /// </summary>
        private async Task<string> EncryptPasswordAsync(string password)
        {
            try
            {

                AES_KEY = _appconfig.Get("AES_KEY:KEY");
                // 使用原生SQL执行AES_ENCRYPT
                var result = await _dal.Db.Ado.SqlQueryAsync<string>($"SELECT HEX(AES_ENCRYPT('{password}', '{AES_KEY}'))");
                return result.FirstOrDefault() ?? string.Empty;
            }
            catch (Exception)
            {
                // 如果AES_ENCRYPT失败，使用Base64作为备用方案
                return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// 使用MySQL AES_DECRYPT解密密码
        /// </summary>
        private async Task<string> DecryptPasswordAsync(string encryptedPassword)
        {
            try
            {
                AES_KEY = _appconfig.Get("AES_KEY:KEY");
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
        /// 构建MySQL连接字符串
        /// </summary>
        private async Task<string> BuildConnectionString(HolDbConfigDto config)
        {
            var type = (config.database_type ?? "MySQL").ToLower();
            var password =await DecryptPasswordAsync(config.password);
            return type switch
            {
                "mysql" => $"Server={config.server_ip};Port={config.server_port};Database={config.database_name};Uid={config.username};Pwd={password};",
                "sqlserver" => $"Server={config.server_ip},{config.server_port};Database={config.database_name};User Id={config.username};Password={password};",
                "oracle" => $"Data Source={config.server_ip}:{config.server_port}/{config.database_name};User Id={config.username};Password={password};",
                "postgresql" => $"Host={config.server_ip};Port={config.server_port};Database={config.database_name};Username={config.username};Password={password};",
                _ => $"Server={config.server_ip};Port={config.server_port};Database={config.database_name};Uid={config.username};Pwd={password};"
            };
        }

        #endregion
    }
}
