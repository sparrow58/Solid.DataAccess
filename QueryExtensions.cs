using Microsoft.EntityFrameworkCore;

namespace Solid.DataAccess.EfCore
{
    public static class QueryExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (source is IAsyncEnumerable<TSource>)
                return source.ToListAsync();
            else
                return Task.FromResult(source.ToList());
        }

        public static Task<TSource> FirstOrDefaultAsyncSafe<TSource>(this IQueryable<TSource> source)
        {
            return Task.FromResult(source.FirstOrDefault());
            //return source.SingleOrDefaultAsync();
            //if (source == null)
            //    throw new ArgumentNullException(nameof(source));
            //if (source is IAsyncEnumerable<TSource>)
            //else
            //    return Task.FromResult(source.FirstOrDefault());
        }
    }
}
