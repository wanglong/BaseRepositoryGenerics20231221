using MediatR;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Queries
{
    public class ErrorMessageQueryId : IRequest<ErrorMessageQueryIdResponse>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }
    }
}
