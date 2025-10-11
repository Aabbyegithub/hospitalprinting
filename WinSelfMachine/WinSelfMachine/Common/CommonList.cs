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

    }
}
