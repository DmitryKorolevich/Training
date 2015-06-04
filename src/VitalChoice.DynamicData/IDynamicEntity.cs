using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData
{
    public interface IDynamicEntity<TEntity, TOptionValue, TOptionType>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        IDynamicEntity<TEntity, TOptionValue, TOptionType> FromEntity(TEntity entity);
        IDynamicEntity<TEntity, TOptionValue, TOptionType> FromEntityWithDefaults(TEntity entity);
        TEntity ToEntity();
    }
}