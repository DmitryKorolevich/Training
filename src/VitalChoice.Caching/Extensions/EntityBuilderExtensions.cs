using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityBuilderExtensions
    {
        public const string NonCachedAnnotationName = "NonCachedAnnotation";
        public const string UniqueIndexAnnotationName = "CacheUniqueIndexAnnotation";
        public const string ConditionallyUniqueIndexAnnotationName = "CacheIndexUniquenessCondition";
        public const string FullCollectionAnnotationName = "CacheFullCollectionCondition";

        public static IndexBuilder CacheIndexWhen<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, bool>> condition,
            Expression<Func<T, object>> index)
            where T : class
        {
            return typeBuilder.HasIndex(index).HasAnnotation(ConditionallyUniqueIndexAnnotationName, condition);
        }

        public static IndexBuilder CacheIndexWhen<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, bool>> condition,
            params string[] properties)
            where T : class
        {
            return typeBuilder.HasIndex(properties).HasAnnotation(ConditionallyUniqueIndexAnnotationName, condition);
        }

        public static EntityTypeBuilder<T> CacheListWhen<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, bool>> condition)
            where T : class
        {
            return typeBuilder.HasAnnotation(FullCollectionAnnotationName, condition);
        }

        public static EntityTypeBuilder<T> NonCached<T>(this EntityTypeBuilder<T> typeBuilder)
            where T : class
        {
            return typeBuilder.HasAnnotation(NonCachedAnnotationName, typeof(T));
        }

        public static IndexBuilder CacheUniqueIndex<T>(this EntityTypeBuilder<T> typeBuilder, Expression<Func<T, object>> index)
            where T : class
        {
            return typeBuilder.HasIndex(index).IsUnique(true).HasAnnotation(UniqueIndexAnnotationName, typeof(T));
        }
    }
}