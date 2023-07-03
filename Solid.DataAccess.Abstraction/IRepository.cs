using System.Linq.Expressions;

namespace Solid.DataAccess.Abstraction
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        TEntity GetById(object id);
        void Add(TEntity entity);
        void UpdateIfNoTracking(TEntity entityToUpdate);
        void AddRange(IEnumerable<TEntity> entities);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");
        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, string includeProperties = "");
        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        ValueTask<TEntity> GetByIdAsync(object id);
        Task<List<TResult>> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Task<List<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector);
        Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<long> CountLongAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity GetById(params object[] ids);
        ValueTask<TEntity> GetByIdAsync(params object[] ids);
    }
}
