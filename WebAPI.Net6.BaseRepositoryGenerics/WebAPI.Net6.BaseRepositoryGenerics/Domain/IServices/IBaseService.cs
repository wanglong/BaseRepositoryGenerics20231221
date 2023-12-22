using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;

namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices
{
    /// <summary>
    /// interface base service.
    /// </summary>
    /// <typeparam name="T"> T. </typeparam>
    public interface IBaseService<T>
        where T : class, new()
    {
        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <returns> </returns>
        IUnitOfWork GetUnitOfWork();

        /// <summary>
        /// Inserts the asynchronous model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<(EntityEntry<T> Model, int Result)> InsertAsync(T entity);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        int AddRange(IEnumerable<T> entitys);

        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        /// <returns> </returns>
        Task<int> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update model.
        /// </summary>
        /// <param name="entity"> entity. </param>
        /// <returns> int. </returns>
        int Update(T entity);

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="whereLambda"> whereLambda. </param>
        /// <param name="entityfunc"> entity. </param>
        /// <returns> int. </returns>
        Task<int> UpdateAsync(Expression<Func<T, bool>> whereLambda, Func<T, T> entityfunc);

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="whereLambda"> whereLambda. </param>
        /// <returns> int. </returns>
        Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// Deletes the range asynchronous.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <returns></returns>
        Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// is exist model.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> true/false. </returns>
        Task<bool> IsExistAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// get model.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> model. </returns>
        Task<T> GetEntityAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// select model list.
        /// </summary>
        /// <returns> list. </returns>
        Task<List<T>> SelectAsync();

        /// <summary>
        /// select model list.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> list. </returns>
        Task<List<T>> SelectAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// select model page.
        /// </summary>
        /// <typeparam name="S"> typeof model. </typeparam>
        /// <param name="pageSize"> page size. </param>
        /// <param name="pageIndex"> page index. </param>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <param name="orderByLambda"> order by lambda expression. </param>
        /// <param name="isAsc"> is asc or desc. </param>
        /// <returns> list. </returns>
        Task<(List<T> Models, int Result)> SelectAsync<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc);
    }
}
