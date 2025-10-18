using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Response
    {
        public class ApiResponse<T>
        {
            public int Start { get; set; } // 状态码
            public bool success { get; set; } // 状态
            public string Message { get; set; } // 消息
            public T Response { get; set; } // 数据

            public ApiResponse(int start, bool state, string message, T response)
            {
                Start = start;
                success = state;
                Message = message;
                Response = response;
            }
        }
    }
}
