using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public interface IIncludableQueryLite<TEntity, TPreviousProperty>: IQueryLite<TEntity>
        where TEntity : Entity
    {
    }
}