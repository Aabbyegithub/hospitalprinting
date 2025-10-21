using System.Drawing;
using WebIServices.IBase;

namespace WebIServices.IServices.OCRIServices
{
    /// <summary>
    /// OCR识别服务接口
    /// </summary>
    public interface IOCRService
    {
        /// <summary>
        /// 识别图片文件中的文字
        /// </summary>
        /// <param name="imagePath">图片文件路径</param>
        /// <param name="language">识别语言 (chi_sim=简体中文, eng=英文)</param>
        /// <returns>识别结果</returns>
        Task<OCRResult> RecognizeTextAsync(string imagePath, string language = "chi_sim+eng");

        /// <summary>
        /// 识别字节数组中的图片
        /// </summary>
        /// <param name="imageBytes">图片字节数组</param>
        /// <param name="language">识别语言</param>
        /// <returns>识别结果</returns>
        Task<OCRResult> RecognizeTextAsync(byte[] imageBytes, string language = "chi_sim+eng");

        /// <summary>
        /// 批量识别文件夹中的图片
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="language">识别语言</param>
        /// <returns>识别结果列表</returns>
        Task<List<OCRResult>> RecognizeFolderAsync(string folderPath, string language = "chi_sim+eng");
    }

    /// <summary>
    /// OCR识别结果
    /// </summary>
    public class OCRResult
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 识别的文字内容
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 识别置信度 (0-100)
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 识别耗时 (毫秒)
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// 是否识别成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 识别的文字块列表
        /// </summary>
        public List<OCRTextBlock> TextBlocks { get; set; } = new List<OCRTextBlock>();
    }

    /// <summary>
    /// OCR文字块
    /// </summary>
    public class OCRTextBlock
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 置信度
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 边界框
        /// </summary>
        public Rectangle BoundingBox { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; }
    }

}
