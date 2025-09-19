using ModelClassLibrary.Model;
using MyNamespace;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using WebIServices.IBase;
using WebServiceClass.Helper;

namespace WebProjectTest.Common
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(IServiceCollection services)
        {
            List<ConnectionConfig> connectionConfigs = AppSettings.App<ConnectionConfig>(new string[] { "ConnectionConfigs" });
            //sqlsugar注册
            AddSqlSugar(services,connectionConfigs);
        }

        public static void AddSqlSugar(IServiceCollection services, List<ConnectionConfig> connectionConfigs)
        {
            foreach (var config in connectionConfigs)
            {
                // 对于每个配置，创建并注册一个ISqlSugarClient实例
                services.AddScoped<ISqlSugarClient>(provider => 
                {
                    var db = new SqlSugarClient(config);
                    SetLog(db, config.ConfigId.ToString());
                    return db;
                }); 
                //或者，如果需要针对每个租户有不同的上下文，可以考虑使用 Scoped 或 Transient 生命周期
            }
            services.AddSingleton(connectionConfigs); 
            services.AddScoped<ISqlHelper,SqlHelper>();
        }

        //日志
        private static void SetLog(SqlSugarClient db, string configid)
        {
            //支持MySql = 0,SqlServer = 1,Sqlite = 2,Oracle = 3,PostgreSQL = 4,Dm = 5,Kdbndp = 6,Oscar = 7,MySqlConnector = 8,Access = 9,OpenGauss = 10,Custom = 900
            db.Aop.OnLogExecuting = (sql, para) =>
            {
                DbType dbtype = configid switch
                {
                    "MySql" => DbType.MySql,
                    "SqlServer" => DbType.SqlServer,
                    "Sqlite" => DbType.Sqlite,
                    "Oracle" => DbType.Oracle,
                    "PostgreSQL" => DbType.PostgreSQL,
                    "Dm" => DbType.Dm,
                    "Kdbndp" => DbType.Kdbndp,
                    "Oscar" => DbType.Oscar,
                    "MySqlConnector" => DbType.MySqlConnector,
                    "Access" => DbType.Access,
                    "OpenGauss" => DbType.OpenGauss,
                    "Custom" => DbType.Custom,
                    _=>throw new NotImplementedException(),
                };
                //var param = para.Select(it => it.Value).ToArray();
                string sqlQuery = UtilMethods.GetSqlString(dbtype, sql, para);
                Console.WriteLine(sqlQuery);
                Console.WriteLine();
            };
        }

        /// <summary>
        /// 添加全局过滤器
        /// </summary>
        /// <param name="provider"></param>
        private static void SetQueryFilter(SqlSugarProvider provider)
        {
            //添加全局过滤器
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Sqlsugar.Business.dll");
            if (files.Length > 0)
            {
                Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity)).ToArray();
                foreach (var entityType in types)
                {
                    var lambda = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(
                        new[] { Expression.Parameter(entityType, "it") },
                        typeof(bool), $"{nameof(BaseEntity.IsDeleted)} ==  @0",
                        false);
                    provider.QueryFilter.Add(new TableFilterItem<object>(entityType, lambda, true)); //将Lambda传入过滤器
                }
            }
        }
    }

}
