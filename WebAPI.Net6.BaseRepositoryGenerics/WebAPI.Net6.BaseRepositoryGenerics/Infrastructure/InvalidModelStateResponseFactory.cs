using Microsoft.AspNetCore.Mvc;
using WebAPI.Net6.BaseRepositoryGenerics.Extensions;

namespace WebAPI.Net6.BaseRepositoryGenerics.Infrastructure
{
    public static class InvalidModelStateResponseFactory
    {
        public static IActionResult ProduceErrorResponse(ActionContext context)
        {
            var errors = context.ModelState.GetErrorMessages();
            var response = new ErrorResource(messages: errors);
            return new BadRequestObjectResult(response);
        }
    }
}
