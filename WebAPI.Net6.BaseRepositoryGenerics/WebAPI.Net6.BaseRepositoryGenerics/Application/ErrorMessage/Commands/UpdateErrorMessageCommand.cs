using MediatR;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Commands
{
    public partial class UpdateErrorMessageCommand : IRequest<ErrorMessageUpdateResponse>
    {
        public ErrorMessageUpdateRequest Request { get; set; } = new ErrorMessageUpdateRequest();
    }
}
