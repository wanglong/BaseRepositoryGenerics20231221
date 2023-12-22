using System.Linq.Expressions;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories.ErrorMessage
{
    public interface IErrorMessageRepository : IBaseRepository<TErrorMessage>
    {
        /// <summary>
        /// Fetches the entitys asynchronous.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <returns></returns>
        Task<TErrorMessage> FetchEntitysAsync(Expression<Func<TErrorMessage, bool>> whereLambda);
    }
}
