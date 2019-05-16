using System;
using System.Linq;
using Teleboard.Common.Data;

namespace Teleboard.Common.ExtensionMethod
{
    public static class IQueryableExtension
    {
        public static DataSourceResult ToDataSourceResult<TSource, TResult>(this IQueryable<TSource> queryable, DataSourceRequest request, Func<TSource, TResult> selector)
        {
            var total = queryable.Count();
            var data = queryable
                .ToList()
                .Select(selector);

            return new DataSourceResult()
            {
                total = total,
                records = data
            };
        }


        public static DataSourceResult ToDataSourceResult<TSource>(this IQueryable<TSource> queryable, DataSourceRequest request)
        {
            var total = queryable.Count();
            if (request.limit.HasValue && request.page.HasValue)
            {
                queryable = queryable.Skip(request.limit.Value * (request.page.Value - 1)).Take(request.limit.Value);
            }
            var data = queryable.ToList();

            return new DataSourceResult()
            {
                total = total,
                records = data
            };
        }

        public static IQueryable<TSource> ApplyRequest<TSource>(this IQueryable<TSource> queryable, DataSourceRequest request)
        {
            if (request.limit.HasValue && request.page.HasValue)
            {
                queryable = queryable.Skip(request.limit.Value * (request.page.Value - 1)).Take(request.limit.Value);
            }
            return queryable;
        }

    }
}
