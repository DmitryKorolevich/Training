using System;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Builders;

namespace VitalChoice.Infrastructure.Context
{
    public static class ContextExtensions
    {
        public static BasicModelBuilder.EntityTypeBuilder<TEntity> ToTable<TEntity>(this BasicModelBuilder.EntityTypeBuilder<TEntity> entityBuilder,string tableName) where TEntity : class
        {
            return entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.TableName, tableName);
        }
    }
}