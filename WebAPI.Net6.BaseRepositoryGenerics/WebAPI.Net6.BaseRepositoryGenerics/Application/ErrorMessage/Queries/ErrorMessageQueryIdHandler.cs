using MediatR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Dynamic;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Queries
{
    public class ErrorMessageQueryIdHandler : IRequestHandler<ErrorMessageQueryId, ErrorMessageQueryIdResponse>
    {
        /// <summary>
        /// The error message service
        /// </summary>
        private readonly IErrorMessageService _errorMessageService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The lock.
        /// </summary>
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageQueryIdHandler"/> class.
        /// </summary>
        /// <param name="errorMessageService">The error message service.</param>
        /// <param name="logger">The logger.</param>
        public ErrorMessageQueryIdHandler(IErrorMessageService errorMessageService
            , ILogger<ErrorMessageQueryIdHandler> logger)
        {
            _errorMessageService = errorMessageService;
            _logger = logger;
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request"> The request </param>
        /// <param name="cancellationToken"> Cancellation token </param>
        /// <returns> Response from the request </returns>
        public async Task<ErrorMessageQueryIdResponse> Handle(ErrorMessageQueryId request, CancellationToken cancellationToken)
        {
            var response = new ErrorMessageQueryIdResponse();
            string msg = string.Empty;
            Stopwatch watchQueryId = new Stopwatch();
            Exception exceptionQueryId = new Exception();
            watchQueryId.Start();
            try
            {
                var result = await ExecuteWithAsyncLock(async () =>
                {
                    return await _errorMessageService.GetEntityAsync(c => c.Id == request.Id);
                });
                if (result != null && !cancellationToken.IsCancellationRequested)
                {
                    response.ErrorMessage = result;
                }
                else
                {
                    msg += "id error!";
                }
            }
            catch (OperationCanceledException ex)
            {
                // 在操作被取消时捕获OperationCanceledException异常
                msg += "Request canceled" + StringHelper.BuildExceptionMessage(ex);
            }
            catch (Exception ex)
            {
                exceptionQueryId = ex;
                msg += StringHelper.BuildExceptionMessage(ex);
                LoggerHelper._logger.Error("{@log}", msg);
            }
            finally
            {
                watchQueryId.Stop();
                var speed = (watchQueryId.ElapsedMilliseconds).ToString();
                dynamic sysLog = new ExpandoObject();
                sysLog.traceid = response.TraceId;
                sysLog.ex = JsonConvert.SerializeObject(exceptionQueryId);
                sysLog.request = JsonConvert.SerializeObject(request);
                sysLog.response = JsonConvert.SerializeObject(response);
                sysLog.speed = speed;
                sysLog.msg = msg;
                sysLog.handler = "ErrorMessageQueryIdHandler";
                LoggerHelper._logger.Information("{@log}", JsonConvert.SerializeObject(sysLog));
            }

            return response;
        }

        /// <summary>
        /// Executes the with asynchronous lock.
        /// </summary>
        /// <param name="action">The action.</param>
        private async Task ExecuteWithAsyncLock(Func<Task> action)
        {
            await _lock.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                _lock.Release();
            }
        }

        // 异步锁执行方法.        
        /// <summary>
        /// Executes the with asynchronous lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        private async Task<T> ExecuteWithAsyncLock<T>(Func<Task<T>> func)
        {
            await _lock.WaitAsync();
            try
            {
                return await func.Invoke();
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
