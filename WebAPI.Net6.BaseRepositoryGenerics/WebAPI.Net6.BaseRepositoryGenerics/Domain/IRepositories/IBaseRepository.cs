using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories
{
    /// <summary>
    /// interface base repository.
    /// </summary>
    /// <typeparam name="T"> typeof model. </typeparam>
    public interface IBaseRepository<T>
        where T : class, new()
    {
        /// <summary>
        /// insert model.
        /// </summary>
        /// <param name="entity"> model. </param>
        /// <returns> model. </returns>
        ValueTask<EntityEntry<T>> InsertAsync(T entity);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        void AddRange(IEnumerable<T> entitys);

        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        /// <returns> </returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// update model.
        /// </summary>
        /// <param name="entity"> model. </param>
        EntityEntry<T> Update(T entity);

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="whereLambda"> where Lambda. </param>
        /// <param name="entityfunc"> entityfunc. </param>
        /// <returns> true/false. </returns>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> whereLambda, Func<T, T> entityfunc);

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="whereLambda"> where Lambda </param>
        /// <returns> true/false. </returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> whereLambda);

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
        /// select list model.
        /// </summary>
        /// <returns> list model. </returns>
        Task<List<T>> SelectAsync();

        /// <summary>
        /// select model list.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> list model. </returns>
        Task<List<T>> SelectAsync(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// select list model.
        /// </summary>
        /// <typeparam name="S"> typeof model. </typeparam>
        /// <param name="pageSize"> page size. </param>
        /// <param name="pageIndex"> page index. </param>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <param name="orderByLambda"> order lambda expression. </param>
        /// <param name="isAsc"> is asc desc. </param>
        /// <returns> list model. </returns>
        Task<(List<T> Models, int Result)> SelectAsync<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        EntityEntry<T> Insert(T entity);
    }
}
