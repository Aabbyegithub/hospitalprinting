using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSelfMachine.Common
{
    public static class SqlSugarClientFactory
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConSqlClient"].ToString();

        public static ISqlSugarClient CreateClient()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            return db;
        }


        public static ISqlSugarClient sqlSugarClient()
        {
            using (var _db = CreateClient())
            {
                return _db;
            }
        }
    }
}
