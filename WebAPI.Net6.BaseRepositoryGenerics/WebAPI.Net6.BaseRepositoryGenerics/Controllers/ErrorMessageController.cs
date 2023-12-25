using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Commands;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Queries;
using WebAPI.Net6.BaseRepositoryGenerics.Infrastructure;

namespace WebAPI.Net6.BaseRepositoryGenerics.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    [EnableCors("any")] // 设置跨域处理的 代理
    public class ErrorMessageController : ControllerBase
    {
        /// <summary>
        /// The mediator.
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageController" /> class.
        /// </summary>
        /// <param name="mediator"> mediator. </param>
        public ErrorMessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // restful api v1/Error/1
        [HttpGet("Error/{id}")]
        public async Task<ApiResponseGeneric<ErrorMessageQueryIdResponse>> Get([FromRoute] int id)
        {
            var result = await this._mediator.Send(new ErrorMessageQueryId
            {
                Id = id,
            });

            return ApiResponseGeneric<ErrorMessageQueryIdResponse>.OK(result);
        }

        // restful api v1/Error/
        [HttpGet("Error")]
        public async Task<ApiResponseGeneric<ErrorMessageQueryResponse>> Search([FromQuery] ErrorMessageQueryRequest request)
        {
            var result = await this._mediator.Send(new ErrorMessageQuery
            {
                Request = request,
            });

            return ApiResponseGeneric<ErrorMessageQueryResponse>.OK(result);
        }

        // restful api v1/Error/
        [HttpPost("Error")]
        public async Task<ApiResponseGeneric<ErrorMessageCreateResponse>> Add([FromBody] ErrorMessageCreateRequest request)
        {
            var result = await this._mediator.Send(new CreateErrorMessageCommand
            {
                Request = request,
            });

            if (result != null)
            {
                if (result.Success.HasValue && result.Success.Value)
                {
                    return ApiResponseGeneric<ErrorMessageCreateResponse>.OK(result);
                }
                else
                {
                    return ApiResponseGeneric<ErrorMessageCreateResponse>.Fail(result.Msg, result);
                }
            }
            else
            {
                return ApiResponseGeneric<ErrorMessageCreateResponse>.Fail();
            }
        }

        // restful api v1/Error/
        [HttpPut("Error")]
        public async Task<ApiResponseGeneric<ErrorMessageUpdateResponse>> Update([FromBody] ErrorMessageUpdateRequest request)
        {
            var result = await this._mediator.Send(new UpdateErrorMessageCommand
            {
                Request = request
            });

            if (result != null)
            {
                if (result.Success.HasValue && result.Success.Value)
                {
                    return ApiResponseGeneric<ErrorMessageUpdateResponse>.OK(result);
                }
                else
                {
                    return ApiResponseGeneric<ErrorMessageUpdateResponse>.Fail(result.Msg, result);
                }
            }
            else
            {
                return ApiResponseGeneric<ErrorMessageUpdateResponse>.Fail();
            }
        }

        // restful api v1/Error/1/cwtqa
        [HttpDelete("Error/{id}/{name}")]
        public async Task<ApiResponseGeneric<ErrorMessageDeleteResponse>> Delete([FromRoute] int id, [FromRoute] string name)
        {
            var result = await this._mediator.Send(new DeleteErrorMessageCommand
            {
                Id = id,
                Operator = name
            });

            if (result != null)
            {
                if (result.Success.HasValue && result.Success.Value)
                {
                    return ApiResponseGeneric<ErrorMessageDeleteResponse>.OK(result);
                }
                else
                {
                    return ApiResponseGeneric<ErrorMessageDeleteResponse>.Fail(result.TraceId, result);
                }
            }
            else
            {
                return ApiResponseGeneric<ErrorMessageDeleteResponse>.Fail();
            }
        }
    }
}
