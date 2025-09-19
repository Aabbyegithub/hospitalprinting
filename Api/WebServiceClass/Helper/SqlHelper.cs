using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
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

    }
}
