using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Relational.Metadata;

namespace VitalChoice.Validation.Controllers
{
    public static class ContextExtensions
    {
        public static EntityTypeBuilder<TEntity> ToTable<TEntity>(this EntityTypeBuilder<TEntity> entityBuilder,string tableName) where TEntity : class
        {
            return entityBuilder.Annotation(RelationalAnnotationNames.Prefix + RelationalAnnotationNames.TableName, tableName);
        }
    }
}