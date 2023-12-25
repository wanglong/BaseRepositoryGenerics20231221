using MediatR;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Commands
{
    public class CreateErrorMessageCommand : IRequest<ErrorMessageCreateResponse>
    {
        public ErrorMessageCreateRequest Request { get; set; } = new ErrorMessageCreateRequest();
    }
}
