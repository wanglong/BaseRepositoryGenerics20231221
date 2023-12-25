using FluentValidation;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Response;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Validators
{
    public class ErrorMessageAddRequestValidator : AbstractValidator<ErrorMessageCreateRequest>
    {
        public ErrorMessageAddRequestValidator()
        {
            this.RuleFor(x => x.Keywords).NotNull()
                .WithMessage("Keywords必须有值.");
        }
    }
}
