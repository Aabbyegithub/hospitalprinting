using MyNamespace;
using SqlSugar;
using StackExchange.Redis;


namespace WebServiceClass.Helper
{
    /// <summary>
    /// 操作日志记录
    /// </summary>
    public static class Operationlog
    {
        /// <summary>
        /// 系统操作日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="operationlog"></param>
        /// <returns></returns>
        public static ISugarQueryable<T> OperationLog<T>(this ISugarQueryable<T> query, lq_operationlog operationlog) where T : class
        {
            try
            {
                query.Context.Insertable(operationlog).ExecuteCommand();
                // 返回原始对象，继续支持链式调用
                return query;
            }
            catch (Exception)
            {
                throw new Exception("日志记录失败！");
            }

        }
        public static IInsertable<T> OperationLog<T>(this IInsertable<T> query, lq_operationlog operationlog) where T : class,new()
        {
            try
            {
                SqlSugarProvider db = null;
                if (query is InsertableProvider<T> provider)
                {
                    db = provider.Context;
                }

                db.Insertable(operationlog).ExecuteCommand();
                // 返回原始对象，继续支持链式调用
                return query;
            }
            catch (Exception)
            {

                throw new Exception("日志记录失败！");
            }

        }

        public static IUpdateable<T> OperationLog<T>(this IUpdateable<T> query, lq_operationlog operationlog) where T : class, new()
        {
            try
            {
                SqlSugarProvider db = null;
                if (query is UpdateableProvider<T> provider)
                {
                    db = provider.Context;
                }

                db.Insertable(operationlog).ExecuteCommand();
                // 返回原始对象，继续支持链式调用
                return query;
            }
            catch (Exception)
            {

                throw new Exception("日志记录失败！");
            }

        }

        public static IDeleteable<T> OperationLog<T>(this IDeleteable<T> query, lq_operationlog operationlog) where T : class, new()
        {
            try
            {
                ISqlSugarClient db = null;
                if (query is DeleteableProvider<T> provider)
                {
                    db = provider.Context;
                }

                db.Insertable(operationlog).ExecuteCommand();
                // 返回原始对象，继续支持链式调用
                return query;
            }
            catch (Exception)
            {

                throw new Exception("日志记录失败！");
            }

        }
    }
}
