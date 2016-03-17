using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IExtendedDynamicServiceAsync<T, TEntity, TOptionType, TOptionValue> : IDynamicServiceAsync<T, TEntity>,
        IExtendedDynamicReadServiceAsync<T, TEntity>
        where T : MappedObject, new()
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
    {
    }

    public interface IDynamicServiceAsync<T, TEntity> : IDynamicReadServiceAsync<T, TEntity>, IObjectServiceAsync<T>
        where T: MappedObject, new()
        where TEntity: DynamicDataEntity
    {
    }
}