using ModelClassLibrary.Model.Dto.HolDto;
using System.Threading.Tasks;
using static WebProjectTest.Common.Message;
using WebIServices.IBase;

namespace WebIServices.IServices.OCRIServices
{
    /// <summary>
    /// OCR识别服务接口
    /// </summary>
    public interface IOCRRecognitionService : IBaseService
    {
        /// <summary>
        /// 识别图片或PDF文件
        /// 支持本地文件路径、URL链接、共享文件夹路径（UNC路径）
        /// </summary>
        /// <param name="filePath">文件路径（支持本地路径、URL、UNC路径如\\server\share\file.jpg）</param>
        /// <returns>识别结果</returns>
        Task<ApiResponse<OcrRecognitionDto>> RecognizeFileAsync(string filePath);

        /// <summary>
        /// 识别上传的文件
        /// </summary>
        /// <param name="fileBytes">文件字节数组</param>
        /// <param name="fileName">文件名</param>
        /// <returns>识别结果</returns>
        Task<ApiResponse<OcrRecognitionDto>> RecognizeUploadedFileAsync(byte[] fileBytes, string fileName);
    }
}

