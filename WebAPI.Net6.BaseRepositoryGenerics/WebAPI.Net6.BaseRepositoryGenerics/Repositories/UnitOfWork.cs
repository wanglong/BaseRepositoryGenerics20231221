using WebAPI.Net6.BaseRepositoryGenerics.Domain.IRepositories;

namespace WebAPI.Net6.BaseRepositoryGenerics.Repositories
{
    /// <summary>
    /// Unit Of Work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// CYeetmBusi Context.
        /// </summary>
        private readonly MyContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class. Unit Of Work.
        /// </summary>
        /// <param name="context"> DbContext. </param>
        public UnitOfWork(MyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get DbContext.
        /// </summary>
        /// <returns> MyContext. </returns>
        public MyContext GetDbContext()
        {
            return this._context;
        }

        /// <summary>
        /// Save Changes.
        /// </summary>
        /// <returns> int. </returns>
        public int SaveChanges()
        {
            var result = _context.SaveChanges();
            return result;
        }

        /// <summary>
        /// Save Changes Async.
        /// </summary>
        /// <returns> int. </returns>
        public async Task<int> SaveChangesAsync()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
