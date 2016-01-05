using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata.Builders;

namespace VitalChoice.Caching.Extensions
{
    public static class IndexBuilderExtension
    {
        public const string UniqueIndexAnnotationName = "IndexUniquenessCondition";

        public static IndexBuilder CacheIndexWhen<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, bool>> condition,
            Expression<Func<T, object>> index)
            where T : class
        {
            return typeBuilder.HasIndex(index).HasAnnotation(UniqueIndexAnnotationName, condition);
        }

        public static IndexBuilder CacheIndexWhen<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, bool>> condition,
            params string[] properties)
            where T : class
        {
            return typeBuilder.HasIndex(properties).HasAnnotation(UniqueIndexAnnotationName, condition);
        }
    }
}