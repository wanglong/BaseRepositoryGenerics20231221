using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories.ErrorMessage;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Repositories.ErrorMessage
{
    public class ErrorMessageRepository : BaseRepository<TErrorMessage>, IErrorMessageRepository
    {
        /// <summary>
        /// The service scope factory.
        /// </summary>
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageRepository" /> class. 构造函数.
        /// </summary>
        /// <param name="myDbContext"> ErrorMessageRepository. </param>
        public ErrorMessageRepository(MyContext myDbContext)
            : base(myDbContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterFlightCarrierLogRepository"/> class.
        /// </summary>
        /// <param name="myDbContext">The cyeetm busi database context.</param>
        /// <param name="serviceScopeFactory">The service scope factory.</param>
        public ErrorMessageRepository(MyContext myDbContext, IServiceScopeFactory serviceScopeFactory)
            : base(myDbContext)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Fetches the entitys asynchronous.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <returns></returns>
        public async Task<TErrorMessage> FetchEntitysAsync(Expression<Func<TErrorMessage, bool>> whereLambda)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var cyeetmBusiContext = scope.ServiceProvider.GetService<MyContext>();
                return await cyeetmBusiContext!.Set<TErrorMessage>().FirstOrDefaultAsync(whereLambda);
            }
        }
    }
}
