﻿using MediatR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Dynamic;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Commands
{
    public class DeleteErrorMessageCommandHandler : IRequestHandler<DeleteErrorMessageCommand, ErrorMessageDeleteResponse>
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
        /// Initializes a new instance of the <see cref="DeleteErrorMessageCommandHandler"/> class.
        /// </summary>
        /// <param name="errorMessageService">The error message service.</param>
        /// <param name="logger">The logger.</param>
        public DeleteErrorMessageCommandHandler(IErrorMessageService errorMessageService
            , ILogger<DeleteErrorMessageCommandHandler> logger)
        {
            _errorMessageService = errorMessageService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<ErrorMessageDeleteResponse> Handle(DeleteErrorMessageCommand request, CancellationToken cancellationToken)
        {
            var response = new ErrorMessageDeleteResponse();
            string msg = string.Empty;
            Stopwatch watchQueryId = new Stopwatch();
            Exception exceptionQueryId = new Exception();
            watchQueryId.Start();
            try
            {
                // TODO: DELETE OPERATION.
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
                sysLog.handler = "DeleteErrorMessageCommandHandler";
                LoggerHelper._logger.Information("{@log}", JsonConvert.SerializeObject(sysLog));
            }

            return response;
        }
    }
}
