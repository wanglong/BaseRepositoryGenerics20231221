using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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


    }
}
