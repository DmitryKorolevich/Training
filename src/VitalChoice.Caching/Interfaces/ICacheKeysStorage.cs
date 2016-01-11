using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheKeysStorage<in T>
    {
        EntityKey GetPrimaryKeyValue(T entity);
        EntityIndex GetIndexValue(T entity);
        EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo);
    }
}