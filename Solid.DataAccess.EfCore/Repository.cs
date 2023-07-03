using Microsoft.EntityFrameworkCore;
using Solid.DataAccess.Abstraction;
using System.Linq.Expressions;

namespace Solid.DataAccess.EfCore
{
    public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        internal DbSet<TEntity> dbSet;

        public Repository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsyncSafe();
            }
            else
            {
                return query.ToListAsyncSafe();
            }
        }
        public Task<List<TResult>> GetAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query.Where(predicate);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToListAsyncSafe();
            }
            else
            {
                return query.Select(selector).ToListAsyncSafe();
            }
        }

        public Task<List<TResult>> GetAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).ToListAsyncSafe();
            }
            else
            {
                return query.Select(selector).ToListAsyncSafe();
            }
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet.Where(predicate);

            foreach (var includeProperty in includeProperties.Split
                  (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefaultAsyncSafe();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(dbSet.Any(predicate));
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return dbSet.MaxAsync(selector);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return dbSet.Where(predicate).MaxAsync(selector);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).CountAsync();
        }

        public Task<long> CountLongAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).LongCountAsync();
        }

        public Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet.Where(predicate);

            foreach (var includeProperty in includeProperties.Split
                  (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.Select(selector).FirstOrDefaultAsyncSafe();
        }

        public TEntity GetById(object id)
        {
            return dbSet.Find(id);
        }
        
        public TEntity GetById(params object[] ids)
        {
            return dbSet.Find(ids);
        }

        public ValueTask<TEntity> GetByIdAsync(object id)
        {
            return dbSet.FindAsync(id);
        } 

        public ValueTask<TEntity> GetByIdAsync(params object[] ids)
        {
            return dbSet.FindAsync(ids);
        }

        public void Add(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public void UpdateIfNoTracking(TEntity entityToUpdate)
        {
            var keys = GetPrimaryKeys(context, entityToUpdate);

            bool tracked = context.Entry(entityToUpdate).State != EntityState.Detached;

            if (tracked)
                return;

            if (keys != null)
            {
                
                var oldValues = dbSet.Find(keys);

                context.Entry(oldValues).CurrentValues.SetValues(entityToUpdate);
            }
            else
            {
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }

        private static object[] GetPrimaryKeys<T>(DbContext context, T value)
        {
            var keyNames = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                   .Select(x => x.Name).ToArray();
            var result = new object[keyNames.Length];
            for (int i = 0; i < keyNames.Length; i++)
            {
                result[i] = typeof(T).GetProperty(keyNames[i])?.GetValue(value);
            }
            return result;
        }
    }
}
