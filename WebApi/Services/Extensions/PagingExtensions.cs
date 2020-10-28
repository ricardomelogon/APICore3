using System.Collections.Generic;
using System.Linq;

namespace WebApi.Services.Extensions
{
    public static class PagingExtensions
    {
        //used by LINQ to SQL
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int pageSize)
        {
            page--;
            return source.Skip((page) * pageSize).Take(pageSize);
        }

        //used by LINQ
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            page--;
            return source.Skip((page) * pageSize).Take(pageSize);
        }
    }
}