using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Extensions
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            StreamReader reqSr = StreamReader.Null;
            StreamReader resSr = StreamReader.Null;
            try
            {
                Endpoint? ep = httpContext.GetEndpoint();
                if (ep != null)
                {
                    //排除心跳，不然每次都会写入日志
                    if (ep!.DisplayName!.Contains("Controller") && !ep.DisplayName.Contains("HealthCheckController"))
                    {
                        string requestPath = string.Empty;
                        string requestType = string.Empty;
                        string requestData = string.Empty;
                        string responseData = string.Empty;
                        string CreatedBy = string.Empty;

                        string authHeader = httpContext.Request.Headers["Authorization"];//Header中的token
                        if (!string.IsNullOrEmpty(authHeader))
                        {
                            authHeader = authHeader.Replace("Bearer", "").Trim();
                            CreatedBy = "system";
                        }

                        var request = httpContext.Request;
                        requestPath = request.Path;
                        requestType = request.Method;
                        if (request.Method.ToLower().Equals("post") || request.Method.ToLower().Equals("put"))
                        {
                            //启用读取request
                            request.EnableBuffering();

                            //设置当前流中的位置为起点
                            request.Body.Seek(0, SeekOrigin.Begin);

                            reqSr = new StreamReader(request.Body);

                            //把请求body流转换成字符串
                            requestData = await reqSr.ReadToEndAsync();

                            request.Body.Seek(0, SeekOrigin.Begin);
                        }
                        else if (request.Method.ToLower().Equals("get") || request.Method.ToLower().Equals("delete"))
                        {
                            requestData = request.QueryString.Value ?? string.Empty;
                        }

                        var originalBodyStream = httpContext.Response.Body;
                        using (var responseBody = new MemoryStream())
                        {
                            httpContext.Response.Body = responseBody;
                            await _next(httpContext);

                            //设置当前流中的位置为起点
                            responseBody.Seek(0, SeekOrigin.Begin);

                            resSr = new StreamReader(responseBody);

                            var responesInfo = await resSr.ReadToEndAsync();

                            //设置当前流中的位置为起点
                            responseBody.Seek(0, SeekOrigin.Begin);

                            // 编码转换，解决中文乱码
                            responesInfo = Regex.Unescape(responesInfo);

                            //返回参数
                            responseData = responesInfo;
                            //返回代码

                            await responseBody.CopyToAsync(originalBodyStream);
                            httpContext.Response.Body = originalBodyStream;

                            //添加日志
                            _logger.LogError(authHeader, requestPath, requestType, requestData, responseData, CreatedBy);

                        }
                    }
                    else
                    {
                        await _next.Invoke(httpContext);
                    }
                }
                else
                {
                    await _next.Invoke(httpContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(StringHelper.BuildExceptionMessage(ex)); // 日志记录
            }
            finally
            {
                reqSr.Dispose();
                resSr.Dispose();
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
