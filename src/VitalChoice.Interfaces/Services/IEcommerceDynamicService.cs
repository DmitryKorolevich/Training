using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

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
