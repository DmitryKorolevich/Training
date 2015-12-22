using VitalChoice.Caching.Data;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IEntityInfoStorage
    {
        EntityPrimaryKey GetPrimaryKey<T>(T entity);
    }
}