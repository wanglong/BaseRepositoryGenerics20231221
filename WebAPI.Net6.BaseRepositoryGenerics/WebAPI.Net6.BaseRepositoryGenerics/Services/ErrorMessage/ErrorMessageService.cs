using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Services.ErrorMessage
{
    public class ErrorMessageService : BaseService<TErrorMessage>, IErrorMessageService
    {
        public ErrorMessageService(IUnitOfWork unitOfWork, IErrorMessageRepository currentRepository)
            : base(unitOfWork, currentRepository)
        {
        }
    }
}
