using System;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace VitalChoice.Infrastructure.Context
{
    public static class ContextExtensions
    {
        public static ModelBuilder.EntityBuilder<TEntity> ToTable<TEntity>(this ModelBuilder.EntityBuilder<TEntity> entityBuilder,string tableName) where TEntity : class
        {
            return entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.TableName, tableName);
        }
    }
}