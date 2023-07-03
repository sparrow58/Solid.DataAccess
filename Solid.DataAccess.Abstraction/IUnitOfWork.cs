namespace Solid.DataAccess.Abstraction
{
    public interface IUnitOfWork
    {
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql, CancellationToken cancellationToken = default);
        void BeginTransaction();
        void RoleBack();
        IQueryable<TEntity> QueryNoTracking<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
