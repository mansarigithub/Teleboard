using System;
using System.Collections.Generic;
using Teleboard.Common.Data;
using System.Linq;

namespace Teleboard.Common.ExtensionMethod
{
    public static class EnumerableExtention
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static DataSourceResult ToDataSourceResult<T>(this IEnumerable<T> collection, int totalRecords)
        {
            return new DataSourceResult() {
                total = totalRecords,
                records = collection,
            };
        }
    }
}
