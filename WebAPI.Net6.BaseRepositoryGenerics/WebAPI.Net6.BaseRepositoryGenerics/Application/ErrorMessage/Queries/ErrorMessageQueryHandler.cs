using MediatR;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Queries
{
    public class ErrorMessageQueryHandler : IRequestHandler<ErrorMessageQuery, ErrorMessageQueryResponse>
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
        /// Initializes a new instance of the <see cref="ErrorMessageQueryHandler"/> class.
        /// </summary>
        /// <param name="errorMessageService">The error message service.</param>
        /// <param name="logger">The logger.</param>
        public ErrorMessageQueryHandler(IErrorMessageService errorMessageService
            , ILogger<ErrorMessageQueryHandler> logger)
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
        public async Task<ErrorMessageQueryResponse> Handle(ErrorMessageQuery request, CancellationToken cancellationToken)
        {
            var response = new ErrorMessageQueryResponse();
            string msg = string.Empty;
            Stopwatch watchQueryId = new Stopwatch();
            Exception exceptionQueryId = new Exception();
            watchQueryId.Start();
            try
            {
                // TODO: PAGER SEARCH.
                // 设置 whereLambda 表达式.
                Expression<Func<TErrorMessage, bool>> whereLambda = (x => x.Keywords == request.Request.Keywords);

                // 设置 orderByLambda 表达式.
                Expression<Func<TErrorMessage, DateTime>> orderByLambda = x => x.UpdateTime;

                // 或者设置为 false，根据需要.
                bool isAsc = false;
                var result = await _errorMessageService.SelectAsync<DateTime>(request.Request.PageSize, request.Request.PageIndex, whereLambda, orderByLambda, isAsc);
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
                sysLog.handler = "ErrorMessageQueryHandler";
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
