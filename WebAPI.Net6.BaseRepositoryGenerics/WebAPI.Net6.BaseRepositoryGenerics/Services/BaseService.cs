using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;

namespace WebAPI.Net6.BaseRepositoryGenerics.Services
{
    /// <summary>
    /// base service.
    /// </summary>
    /// <typeparam name="T"> genery type. </typeparam>
    public class BaseService<T>
        where T : class, new()
    {
        /// <summary>
        /// unit of work.
        /// </summary>
        protected IUnitOfWork unitOfWork;

        /// <summary>
        /// current repository.
        /// </summary>
        protected IBaseRepository<T> currentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}" /> class. base service constructor.
        /// </summary>
        /// <param name="unitOfWork"> unit of work. </param>
        /// <param name="currentRepository"> current repository. </param>
        public BaseService(IUnitOfWork unitOfWork, IBaseRepository<T> currentRepository)
        {
            this.unitOfWork = unitOfWork;
            this.currentRepository = currentRepository;
        }

        /// <summary>
        /// Gets the unit of work.
        /// </summary>
        /// <returns> </returns>
        public IUnitOfWork GetUnitOfWork()
        {
            return this.unitOfWork;
        }

        /// <summary>
        /// insert model.
        /// </summary>
        /// <param name="entity"> entity. </param>
        /// <returns> int. </returns>
        public async Task<(EntityEntry<T> Model, int Result)> InsertAsync(T entity)
        {
            var insertedEntity = await this.currentRepository.InsertAsync(entity);
            var result = await this.unitOfWork.SaveChangesAsync();
            return (insertedEntity, result);

        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        public int AddRange(IEnumerable<T> entitys)
        {
            this.currentRepository.AddRange(entitys);
            return this.unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        /// <returns> </returns>
        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            await this.currentRepository.AddRangeAsync(entities);
            return await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// update model.
        /// </summary>
        /// <param name="entity"> entity. </param>
        /// <returns> int </returns>
        public int Update(T entity)
        {
            this.currentRepository.Update(entity);
            return this.unitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="whereLambda"> whereLambda. </param>
        /// <param name="entityfunc"> entity func. </param>
        /// <returns> int. </returns>
        public async Task<int> UpdateAsync(Expression<Func<T, bool>> whereLambda, Func<T, T> entityfunc)
        {
            await this.currentRepository.UpdateAsync(whereLambda, entityfunc);
            return await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="whereLambda"> whereLambda. </param>
        /// <returns> int. </returns>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            await this.currentRepository.DeleteAsync(whereLambda);
            return await this.unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the range asynchronous.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <returns></returns>
        public async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this.currentRepository.DeleteRangeAsync(whereLambda);
        }

        /// <summary>
        /// is exist.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> T/F bool. </returns>
        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this.currentRepository.IsExistAsync(whereLambda);
        }

        /// <summary>
        /// get entity.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> genery type model. </returns>
        public async Task<T> GetEntityAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this.currentRepository.GetEntityAsync(whereLambda);
        }

        /// <summary>
        /// list model.
        /// </summary>
        /// <returns> list. </returns>
        public async Task<List<T>> SelectAsync()
        {
            return await this.currentRepository.SelectAsync();
        }

        /// <summary>
        /// list model.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> list. </returns>
        public async Task<List<T>> SelectAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this.currentRepository.SelectAsync(whereLambda);
        }

        /// <summary>
        /// Select.
        /// </summary>
        /// <typeparam name="S"> type s. </typeparam>
        /// <param name="pageSize"> pageSize. </param>
        /// <param name="pageIndex"> pageIndex. </param>
        /// <param name="whereLambda"> whereLambda. </param>
        /// <param name="orderByLambda"> orderByLambda. </param>
        /// <param name="isAsc"> isAsc. </param>
        /// <returns> A <see cref="Task" /> representing the asynchronous operation. </returns>
        public async Task<(List<T> Models, int Result)> SelectAsync<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            return await this.currentRepository.SelectAsync(pageSize, pageIndex, whereLambda, orderByLambda, isAsc);
        }
    }
}
