namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    /// <summary>
    /// ApiResponseGeneric.
    /// </summary>
    /// <typeparam name="T"> T. </typeparam>
    public class ApiResponseGeneric<T>
    {
        /// <summary>
        /// Gets or sets 返回请求代码.
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// Gets or sets 返回的消息.
        /// </summary>
        public string message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets 返回的数据.
        /// </summary>
        public T data { get; set; } = default;

        public static ApiResponseGeneric<T> OK()
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 200,
                message = "Success",
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> OK(T data)
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 200,
                message = "Success",
                data = data,
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> OK(string message)
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 200,
                message = message,
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> OK(string message, T data)
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 200,
                message = message,
                data = data,
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> Fail()
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 500,
                message = "Fail",
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> Fail(string message)
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 500,
                message = message,
            };
            return apiResponse;
        }

        public static ApiResponseGeneric<T> Fail(string message, T data)
        {
            ApiResponseGeneric<T> apiResponse = new ApiResponseGeneric<T>()
            {
                code = 500,
                message = message,
                data = data,
            };
            return apiResponse;
        }
    }
}
