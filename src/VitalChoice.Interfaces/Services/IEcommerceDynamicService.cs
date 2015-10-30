using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services
{
    public interface IEcommerceDynamicService<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
        where TDynamic : MappedObject
    {
    }
}
