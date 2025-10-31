using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.PayDto
{
    /// <summary>
    /// 微信支付配置映射类
    /// </summary>
    public class WeChatPayConfig
    {
        public string AppId { get; set; }
        public string MchId { get; set; }
        public string Key { get; set; }
        public string NotifyUrl { get; set; }
        public string CertPath { get; set; }
    }
}
