using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    /// <summary>
    /// 百度云OCR配置DTO
    /// </summary>
    public class HolOcrConfigDto
    {
        public long id { get; set; }
        public int is_enabled { get; set; }
        public string? api_url { get; set; }
        public string? app_id { get; set; }
        public string? api_key { get; set; }
        public string? secret_key { get; set; }
        public string? access_token { get; set; }
        public DateTime? token_expires_at { get; set; }
        public string? remark { get; set; }
    }
}
