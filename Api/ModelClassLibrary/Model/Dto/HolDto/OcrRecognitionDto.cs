using System;
using System.Collections.Generic;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    /// <summary>
    /// OCR识别结果DTO
    /// </summary>
    public class OcrRecognitionDto
    {
        /// <summary>
        /// 识别是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 原始文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 识别的完整文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 平均置信度
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// 处理耗时（毫秒）
        /// </summary>
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 文本块列表
        /// </summary>
        public List<TextBlockDto> TextBlocks { get; set; }

        /// <summary>
        /// 文本块数量
        /// </summary>
        public int TextBlockCount { get; set; }
    }

    /// <summary>
    /// 文本块DTO
    /// </summary>
    public class TextBlockDto
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 置信度
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// 边界框
        /// </summary>
        public BoundingBoxDto BoundingBox { get; set; }

        /// <summary>
        /// 页码（PDF文件使用）
        /// </summary>
        public int? PageNumber { get; set; }
    }

    /// <summary>
    /// 边界框DTO
    /// </summary>
    public class BoundingBoxDto
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
    }
}

