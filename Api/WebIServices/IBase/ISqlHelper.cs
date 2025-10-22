using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IBase
{
    public interface ISqlHelper
    {
        /// <summary>
        /// 没有指定数据库时，默认取第一个
        /// </summary>
        /// <returns></returns>
        ISqlSugarClient Db { get; }

        /// <summary>
        /// 根据configid获取指定数据库
        /// </summary>
        /// <param name="configid"></param>
        /// <returns></returns>
        ISqlSugarClient DbContext(string configid);

        ISqlSugarClient CreateConnection(ConnectionConfig config);
    }
}
