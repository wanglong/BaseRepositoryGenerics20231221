using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebAPI.Net6.BaseRepositoryGenerics.Repositories
{
    /// <summary>
    /// base generics repository.
    /// </summary>
    /// <typeparam name="T"> type of model. </typeparam>
    public class BaseRepository<T>
        where T : class, new()
    {
        /// <summary>
        /// db context.
        /// </summary>
        private readonly MyContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}" /> class. base repository.
        /// </summary>
        /// <param name="dbContext"> my db context. </param>
        public BaseRepository(MyContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="whereLambda"> where Lambda. </param>
        /// <returns> bool. </returns>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> whereLambda)
        {
            if (await this.IsExistAsync(whereLambda))
            {
                var entity = await this.GetEntityAsync(whereLambda);
                DbSet<T> dbset = this._dbContext.Set<T>();
                dbset.Remove(entity);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes the range asynchronous.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <returns></returns>
        public async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> whereLambda)
        {
            if (await this.IsExistAsync(whereLambda))
            {
                var entitys = await this.SelectAsync(whereLambda);
                DbSet<T> dbset = this._dbContext.Set<T>();
                dbset.RemoveRange(entitys);
                return true;
            }

            return false;
        }

        /// <summary>
        /// get model.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> model. </returns>
        public async Task<T> GetEntityAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this._dbContext.Set<T>().FirstOrDefaultAsync(whereLambda);
        }

        /// <summary>
        /// insert.
        /// </summary>
        /// <param name="entity"> entity. </param>
        /// <returns> model T. </returns>
        public async ValueTask<EntityEntry<T>> InsertAsync(T entity)
        {
            return await this._dbContext.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Adds the range asynchronous.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// is exist.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> bool T/F. </returns>
        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this._dbContext.Set<T>().AnyAsync(whereLambda);
        }

        /// <summary>
        /// select list.
        /// </summary>
        /// <returns> list model. </returns>
        public async Task<List<T>> SelectAsync()
        {
            return await this._dbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// list model.
        /// </summary>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <returns> list. </returns>
        public async Task<List<T>> SelectAsync(Expression<Func<T, bool>> whereLambda)
        {
            return await this._dbContext.Set<T>().Where(whereLambda).ToListAsync();
        }

        /// <summary>
        /// list pager model.
        /// </summary>
        /// <typeparam name="S"> type of S. </typeparam>
        /// <param name="pageSize"> page size. </param>
        /// <param name="pageIndex"> page index. </param>
        /// <param name="whereLambda"> where lambda expression. </param>
        /// <param name="orderByLambda"> order lambda expression. </param>
        /// <param name="isAsc"> is asc T/F. </param>
        /// <returns> A <see cref="Task" /> representing the asynchronous operation. </returns>
        public async Task<(List<T> Models, int Result)> SelectAsync<S>(int pageSize, int pageIndex, Expression<Func<T, bool>> whereLambda, Expression<Func<T, S>> orderByLambda, bool isAsc)
        {
            var total = await this._dbContext.Set<T>().AsNoTracking().Where(whereLambda).CountAsync();
            if (isAsc)
            {
                var entities = await this._dbContext.Set<T>().AsNoTracking().Where(whereLambda)
                                      .OrderBy<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();
                return (entities, total);
            }
            else
            {
                var entities = await this._dbContext.Set<T>().AsNoTracking().Where(whereLambda)
                                      .OrderByDescending<T, S>(orderByLambda)
                                      .Skip(pageSize * (pageIndex - 1))
                                      .Take(pageSize).ToListAsync();
                return (entities, total);
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="whereLambda"> where Lambda. </param>
        /// <param name="entityfunc"> entityfunc. </param>
        /// <returns> int. </returns>
        public async Task<bool> UpdateAsync(Expression<Func<T, bool>> whereLambda, Func<T, T> entityfunc)
        {
            if (await this.IsExistAsync(whereLambda))
            {
                var mode = await this.GetEntityAsync(whereLambda);
                this.Update(entityfunc(mode));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="entity"> entity. </param>
        public EntityEntry<T> Update(T entity)
        {
            return this._dbContext.Set<T>().Update(entity);
        }

        /// <summary>
        /// insert.
        /// </summary>
        /// <param name="entity"> entity. </param>
        /// <returns> model T. </returns>
        public EntityEntry<T> Insert(T entity)
        {
            return this._dbContext.Set<T>().Add(entity);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="entitys"> The entitys. </param>
        public void AddRange(IEnumerable<T> entitys)
        {
            this._dbContext.Set<T>().AddRange(entitys);
        }
    }
}
