using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Helpers
{
    public class IncludableQueryLite<TEntity, TPreviousProperty> : QueryLite<TEntity>, IIncludableQueryLite<TEntity, TPreviousProperty>
        where TEntity : Entity
    {
        internal IIncludableQueryFluent<TEntity, TPreviousProperty> IncludableQuery { get; }

        public IncludableQueryLite(IIncludableQueryFluent<TEntity, TPreviousProperty> queryFluent) : base(queryFluent)
        {
            IncludableQuery = queryFluent;
        }
    }
}