namespace WebProjectTest.Common
{
    /// <summary>
    /// 封装接口返回样式
    /// </summary>
    public static class Message
    {
        public static ApiResponse<T> Success<T>(T response,string message = "Success", bool State = true )
        {
            return new ApiResponse<T>(200,State, message, response);
        }

        public static ApiResponse<T> Error<T>(string message,bool State = false,  int Start = 500)
        {
            return new ApiResponse<T>(Start,State, message, default(T));
        }

        public static ApiResponse<T> Fail<T>( string message, bool State = false, int Start = 422)
        {
            return new ApiResponse<T>(Start,State, message, default(T));
        }

        public class ApiResponse<T>
        {
            public int Start { get; set; } // 状态码
            public bool success { get; set; } // 状态
            public string Message { get; set; } // 消息
            public T Response { get; set; } // 数据

            public ApiResponse(int start,bool state, string message, T response)
            {
                Start = start;
                success = state;
                Message = message;
                Response = response;
            }
        }

        public static ApiPageResponse<T> PageSuccess<T>( T response, int count, string message = "Success", bool State = true)
        {
            return new ApiPageResponse<T>(200, State, message, response,count);
        }

        public static ApiPageResponse<T> PageError<T>(string message, bool State = false, int Start = 500)
        {
            return new ApiPageResponse<T>(Start, State, message, default(T),null);
        }

        public static ApiPageResponse<T> PageFail<T>(string message, bool State = false, int Start = 422)
        {
            return new ApiPageResponse<T>(Start, State, message, default(T),null);
        }

        public class ApiPageResponse<T>
        {
            public int Start { get; set; } // 状态码
            public bool success { get; set; } // 状态
            public string Message { get; set; } // 消息
            public T Response { get; set; } // 数据
            public int? Count {  get; set; } //总数

            public ApiPageResponse(int start, bool state, string message, T response, int? count)
            {
                Start = start;
                success = state;
                Message = message;
                Response = response;
                Count = count;
            }
        }

    }
}
