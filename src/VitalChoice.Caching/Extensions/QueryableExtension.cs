using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Query;

namespace VitalChoice.Caching.Extensions
{
    internal static class QueryableExtension
    {
        public static IQueryable<T> AsNonCached<T>(this IQueryable<T> source)
        {
            return QueryableHelpers.CreateQuery(source, s => s.AsNonCached());
        }
    }
}
