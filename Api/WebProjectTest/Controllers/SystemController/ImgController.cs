using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using WebProjectTest.Common;
using static WebProjectTest.Common.Message;

namespace WebProjectTest.Controllers.SystemController
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImgController : ControllerBase
    {
        private static readonly string _windowsBasePath = AppSettings.GetConfig("UpFile:Windows");
        private static readonly string _linuxBasePath = AppSettings.GetConfig("UpFile:Linux");
        private static readonly string _Url = AppSettings.GetConfig("UpFile:FileUrl");
        ///<summary>
        ///图片上传
        ///</summary>
        [HttpPost]
        public async Task<ApiResponse<string>> UpImg(IFormFile file)
        {
            string basePath = OperatingSystem.IsWindows() ? _windowsBasePath : _linuxBasePath;
            try
            {
                // 1. 验证文件是否存在
                if (file == null || file.Length == 0)
                {
                   return Fail<string>("请选择要上传的图片");
                }

                // 2. 验证文件类型
                var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                if (!allowedContentTypes.Contains(file.ContentType))
                {
                    return Fail<string>("仅支持JPG、PNG、GIF、BMP格式的图片");
                }

                // 3. 验证文件大小（限制10MB）
                if (file.Length > 10 * 1024 * 1024)
                {
                    return Fail<string>("图片大小不能超过10MB");
                }

                // 5. 创建日期子目录（按日期分类存储）
                string dateDir = DateTime.Now.ToString("yyyyMMdd");
                string savePath = Path.Combine(basePath, dateDir);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                // 6. 生成唯一文件名（避免重复）
                string fileExtension = Path.GetExtension(file.FileName);
                string fileName = $"{Guid.NewGuid()}{fileExtension}";
                string fullPath = Path.Combine(savePath, fileName);

                // 7. 保存文件
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string relativePath = Path.GetRelativePath(basePath, fullPath)
                          .Replace(Path.DirectorySeparatorChar, '/');

                // 7. 拼接正确的URL（确保URL前缀以"/"结尾，避免拼接错误）
                string fullUrl = $"{_Url.TrimEnd('/')}/{relativePath}";
                return Success(fullUrl);

            }
            catch (Exception e)
            {

                return Error<string>($"上传失败{e.Message}");
            }
        }
    }
}
