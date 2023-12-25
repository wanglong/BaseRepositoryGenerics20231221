using MediatR;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Queries
{
    public class ErrorMessageQuery : IRequest<ErrorMessageQueryResponse>
    {
        public ErrorMessageQueryRequest Request { get; set; } = new ErrorMessageQueryRequest();
    }
}
