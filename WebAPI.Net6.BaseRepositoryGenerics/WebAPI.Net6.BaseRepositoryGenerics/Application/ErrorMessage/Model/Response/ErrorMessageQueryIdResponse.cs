using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Common;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response
{
    public partial class ErrorMessageQueryIdResponse : BaseResponse
    {
        public TErrorMessage? ErrorMessage { get; set; }
    }
}
