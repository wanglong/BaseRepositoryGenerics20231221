using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using WebAPI.Net6.BaseRepositoryGenerics.Repositories;

namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories
{
    /// <summary>
    /// interface Unit Of Work.
    /// </summary>
    public interface IUnitOfWork : IDisposable

    {
        /// <summary>
        /// Get DbContext.
        /// </summary>
        /// <returns> CYeetmBusiContext. </returns>
        MyContext GetDbContext();

        /// <summary>
        /// Save Changes.
        /// </summary>
        /// <returns> int. </returns>
        int SaveChanges();

        /// <summary>
        /// Save Changes Async.
        /// </summary>
        /// <returns> int. </returns>
        Task<int> SaveChangesAsync();
    }
}
