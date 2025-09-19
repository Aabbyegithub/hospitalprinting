using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model
{
    public class CommonEnmFixts
    {
        /// <summary>
        /// 操作属性
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// 编辑
            /// </summary>
            [Description("编辑")]
            Edit = 1,
            /// <summary>
            /// 删除
            /// </summary>
            [Description("删除")]
            Delete = 2,
            /// <summary>
            /// 新增
            /// </summary>
            [Description("新增")]
            Add = 3,
            /// <summary>
            /// 查询
            /// </summary>
            [Description("查询")]
            Search = 4,
            /// <summary>
            /// 提交
            /// </summary>
            [Description("提交")]
            Submit = 5,
            /// <summary>
            /// 导出
            /// </summary>
            [Description("导出")]
            Export = 6,
            /// <summary>
            /// 导入
            /// </summary>
            [Description("导入")]
            Import = 7,
            /// <summary>
            /// 上传
            /// </summary>
            [Description("上传")]
            Upload = 8,
            /// <summary>
            /// 下载
            /// </summary>
            [Description("下载")]
            Download = 9,
            /// <summary>
            /// 登陆
            /// </summary>
            [Description("登陆")]
            Landing = 10,
            /// <summary>
            /// 登出
            /// </summary>
            [Description("退出")]
            Exit = 11,
            /// <summary>
            /// 启动
            /// </summary>
            [Description("启动")]
            Open = 12,
            /// <summary>
            /// 关闭
            /// </summary>
            [Description("关闭")]
            Close = 13,
            /// <summary>
            /// 移除
            /// </summary>
            [Description("移除")]
            Remove = 14,
            /// <summary>
            /// 暂停
            /// </summary>
            [Description("暂停")]
            Pause = 15,
        }

        public enum PayChannel
        {
            AliPay_App,
            AliPay_Wap,
            AliPay_Page,
            AliPay_QRCode,
            AliPay_Barcode,
            WeChat_JSAPI,
            WeChat_App,
            WeChat_H5,
            WeChat_Native,
            WeChat_Barcode,
            Bank_NetBanking,
            Bank_QuickPay
        }
    }
}
