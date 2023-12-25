using MediatR;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Commands
{
    public class DeleteErrorMessageCommand : IRequest<ErrorMessageDeleteResponse>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int? Id { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; } = null!;
    }
}
