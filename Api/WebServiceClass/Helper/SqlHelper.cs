using Aliyun.OSS;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Data;
using WebIServices.IBase;

namespace WebServiceClass.Helper
{
    public class SqlHelper : ISqlHelper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<ConnectionConfig> _connectionConfigs;

        public SqlHelper(IServiceProvider serviceProvider, List<ConnectionConfig> connectionConfigs)
        {
            _serviceProvider = serviceProvider;
            _connectionConfigs = connectionConfigs;
        }

        /// <summary>
        /// 根据configid获取指定数据库
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ISqlSugarClient DbContext(string tenantId)
        {
            var config = _connectionConfigs.FirstOrDefault(c => c.ConfigId.ToString()== tenantId);
            if (config == null)
            {
                throw new Exception($"没有配置数据库 '{tenantId}'.");
            }

            // 使用服务提供者获取已注册的 ISqlSugarClient 实例
            var client = _serviceProvider.GetServices<ISqlSugarClient>().FirstOrDefault(c => c.CurrentConnectionConfig.ConnectionString == config.ConnectionString);

            if (client == null)
            {
                throw new Exception($"没有已注册的 ISqlSugarClient 实例'{tenantId}'.");
            }

            return client;
        }

        /// <summary>
        /// 没有指定数据库，默认操作第一个数据库
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //public ISqlSugarClient Db()
        //{
            
        //    var config = _connectionConfigs.FirstOrDefault();
        //    if (config == null)
        //    {
        //        throw new Exception($"没有配置数据库");
        //    }

        //    // 使用服务提供者获取已注册的 ISqlSugarClient 实例
        //    var client = _serviceProvider.GetServices<ISqlSugarClient>().FirstOrDefault(c => c.CurrentConnectionConfig.ConnectionString == config.ConnectionString);

        //    if (client == null)
        //    {
        //        throw new Exception($"没有已注册的 ISqlSugarClient 实例");
        //    }

        //    return client;
        //}

        public ISqlSugarClient Db
        {
            get
            {
                var config = _connectionConfigs.FirstOrDefault();
                if (config == null)
                {
                    throw new Exception($"没有配置数据库");
                }

                // 使用服务提供者获取已注册的 ISqlSugarClient 实例
                var client = _serviceProvider.GetServices<ISqlSugarClient>().FirstOrDefault(c => c.CurrentConnectionConfig.ConnectionString == config.ConnectionString);

                if (client == null)
                {
                    throw new Exception($"没有已注册的 ISqlSugarClient 实例");
                }

                return client;
            }
        }


        /// <summary>
        /// 创建数据库连接
        /// </summary>
        public ISqlSugarClient CreateConnection(ConnectionConfig config)
        {
            var connectionConfig = new ConnectionConfig
            {
                ConnectionString = config.ConnectionString,
                DbType = config.DbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            };

            var client = new SqlSugarClient(connectionConfig);

            // 配置日志
            client.Aop.OnLogExecuting = (sql, pars) =>
            {
                string sqlQuery = UtilMethods.GetSqlString(config.DbType, sql, pars);
                Console.WriteLine(sqlQuery);
                Console.WriteLine();
            };
            return client;
        }

    }
}
