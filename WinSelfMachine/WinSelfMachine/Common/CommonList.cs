using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSelfMachine.Common
{
    public static class CommonList
    {
        /// <summary>
        /// 定义包含胶片尺寸的List
        /// </summary>        
        public static List<string> PrintFilmSizes()
        {
            return new List<string>
            {
                "14IN×17IN",
                "14IN×14IN",
                "11IN×14IN",
                "10IN×12IN",
                "8IN×10IN",
                "A3",
                "A4"
            };
        }


        public static Dictionary<string, string> PrintDepFilmSizes()
        {
            return new Dictionary<string, string>
            {
                // 主流影像模态
                { "CT", "14IN×17IN,11IN×14IN" },          // CT 优先大尺寸，可选中等尺寸
                { "MRI", "14IN×17IN,11IN×14IN" },         // MRI 同 CT，支持多序列排版
                { "超声", "8IN×10IN,A4" },                 // 超声常用小尺寸，A4 便于归档
                { "DX", "14IN×17IN,14IN×14IN" },          // X线摄影（如胸部用14×17，四肢用14×14）
                { "DR", "14IN×17IN,11IN×14IN" },          // 数字化X线，同 DX 适配大尺寸
                { "心电图", "A4,8IN×10IN" },               // 心电图标准报告用 A4
                { "PET-CT", "14IN×17IN" },                // 核医学影像需大尺寸展示融合图像
                { "PECT", "14IN×17IN" },                  // 正电子显像同 PET-CT
                { "NM", "11IN×14IN,8IN×10IN" },           // 核医学常规影像，可选中等尺寸
                { "钼靶（MG）", "11IN×14IN,8IN×10IN" },    // 乳腺钼靶，中等尺寸为主
                { "内镜（如胃镜/肠镜）", "A4,8IN×10IN" },   // 内镜图像多为局部，小尺寸即可
                { "造影（Angio）", "14IN×17IN" },          // 血管造影需大尺寸展示完整血管走形
                { "CR", "14IN×17IN,11IN×14IN" },          // 计算机X线摄影，同 DR 适配
        
                // 通用纸张尺寸
                { "A3", "A3" },
                { "A4", "A4" },
                { "10IN×12IN", "10IN×12IN" },             // 备用中等尺寸
                { "11IN×14IN", "11IN×14IN" }              // 备用中等尺寸
            };
        }

    }
}
