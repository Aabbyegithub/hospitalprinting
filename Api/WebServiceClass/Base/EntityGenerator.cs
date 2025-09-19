using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebIServices.IBase;

namespace WebServiceClass.Base
{
    public class EntityGenerator : IEntityGenerator
    {
        private readonly ISqlHelper _dal;
        public EntityGenerator(ISqlHelper dal)
        {
            _dal = dal;
        }

        /// <summary>
        /// 批量生成表实体
        /// </summary>
        /// <param name="tableNames"></param>
        /// <param name="outputPath"></param>
        public void GenerateEntityFiles(List<string> tableNames,string outputPath)
        {
            //var outputPath = $@"D:\项目\测试test\WebApi";
            foreach (var tableName in tableNames)
            {
                // 使用 DBFirst 功能生成实体类
                _dal.Db.DbFirst.Where(tableName)
                    .IsCreateAttribute()//增加字段属性
                    .StringNullable()//如果string 类型字段可为空配置null
                    .CreateClassFile(outputPath, "MyNamespace");
            }
        }   

        /// <summary>
        /// 生成单个表实体
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="outputPath"></param>
        public void GenerateEntityFiles(string tableName, string outputPath)
        {
   
                // 使用 DBFirst 功能生成实体类
           _dal.Db.DbFirst.Where(tableName)
                .IsCreateAttribute()//增加字段属性
                .StringNullable()//如果string 类型字段可为空配置null
                .CreateClassFile(outputPath, "MyNamespace");
            //var db = _dal.CurrentConnectionConfig.ConfigId
            
        }
    }
}
