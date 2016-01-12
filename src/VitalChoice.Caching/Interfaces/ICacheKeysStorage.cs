using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheKeysStorage
    {
        EntityKey GetPrimaryKeyValue(object entity);
        EntityIndex GetIndexValue(object entity);
        EntityIndex GetConditionalIndexValue(object entity, EntityConditionalIndexInfo conditionalInfo);
    }

    public interface ICacheKeysStorage<in T> : ICacheKeysStorage
    {
        EntityKey GetPrimaryKeyValue(T entity);
        EntityIndex GetIndexValue(T entity);
        EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo);
    }
}