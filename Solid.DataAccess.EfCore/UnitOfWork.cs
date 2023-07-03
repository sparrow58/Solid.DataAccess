using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Solid.DataAccess.Abstraction;

namespace Solid.DataAccess.EfCore
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext dbContext;
        private readonly Dictionary<Type, object> repositories = new();
        private IDbContextTransaction dbContextTransaction;

        public UnitOfWork(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
            => dbContext.Set<TEntity>();

        public IQueryable<TEntity> QueryNoTracking<TEntity>() where TEntity : class
            => dbContext.Set<TEntity>().AsNoTracking();

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (repositories.ContainsKey(typeof(TEntity)))
                return repositories[typeof(TEntity)] as IRepository<TEntity>;

            var repo = new Repository<TEntity>(dbContext);
            repositories.Add(typeof(TEntity), repo);

            return repo;
        }

        public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
            => dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

        public Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql, CancellationToken cancellationToken = default)
            => dbContext.Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
        public void BeginTransaction()
            => dbContextTransaction ??= dbContext.Database.BeginTransaction();
        public void RoleBack() => dbContextTransaction?.Rollback();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            var result = await SaveChangesAsync(cancellationToken);

            dbContextTransaction?.Commit();
            dbContext.Dispose();

            return result;
        }
    }
}
