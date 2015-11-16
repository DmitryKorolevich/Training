using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Helpers
{
    public interface IIncludableQueryFluent<TEntity, TPreviousProperty>: IQueryFluent<TEntity> where TEntity : Entity
    {
    }
}