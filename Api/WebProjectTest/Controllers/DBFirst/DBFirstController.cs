using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebIServices.IBase;

namespace WebProjectTest.Controllers.DBFirst
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DBFirstController : ControllerBase
    {
        private readonly IEntityGenerator _entityGenerator;
        public DBFirstController(IEntityGenerator entityGenerator)
        {
            _entityGenerator = entityGenerator;
        }
        /// <summary>
        /// 批量生成实体类文件
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GenerateEntitiesList([FromBody] List<string> tableNames)
        {
            try
            {
                // 指定输出路径
                var outputPath = $@"D:\DbFirst文件";//Path.Combine(Directory.GetCurrentDirectory(), "DbFirst文件");

                // 确保输出路径存在
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                // 生成实体类文件
                _entityGenerator.GenerateEntityFiles(tableNames, outputPath);

                return Ok($"实体类文件生成成功，路径{outputPath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"生成失败: {ex.Message}");
            }
        }


        /// <summary>
        /// 单个生成实体类文件
        /// </summary>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GenerateEntities(string tableName)
        {
            try
            {
                // 指定输出路径
                var outputPath = $@"D:\DbFirst文件";

                // 确保输出路径存在
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                // 生成实体类文件
                _entityGenerator.GenerateEntityFiles(tableName, outputPath);

                return Ok($"实体类文件生成成功，路径{outputPath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"生成失败: {ex.Message}");
            }
        }
    }
}
