using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VitalChoice.Caching.Extensions
{
    internal static class QueryableExtension
    {
        private static readonly MethodInfo AsNonCachedMethod = typeof(QueryableExtension).GetTypeInfo().GetDeclaredMethod("AsNonCached");

        public static IQueryable<T> AsNonCached<T>(this IQueryable<T> source)
            => source.Provider.CreateQuery<T>(Expression.Call(null, AsNonCachedMethod.MakeGenericMethod(typeof(T)), source.Expression));
    }
}
