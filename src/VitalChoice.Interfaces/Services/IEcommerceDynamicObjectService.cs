using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services
{
    public interface IEcommerceDynamicObjectService<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicObjectServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
        where TDynamic : MappedObject
    {
    }
}
