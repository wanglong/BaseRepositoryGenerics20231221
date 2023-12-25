using FluentValidation;
using WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Model.Request;

namespace WebAPI.Net6.BaseRepositoryGenerics.Application.ErrorMessage.Validators
{
    public class ErrorMessageUpdateRequestValidator : AbstractValidator<ErrorMessageUpdateRequest>
    {
        public ErrorMessageUpdateRequestValidator()
        {
            this.RuleFor(x => x.Keywords).NotNull()
                .WithMessage("Keywords必须有值.");
        }
    }
}
