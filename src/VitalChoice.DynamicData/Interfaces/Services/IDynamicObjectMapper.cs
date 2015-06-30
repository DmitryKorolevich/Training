using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Interfaces.Services
{
    public interface IDynamicToModelMapper
    {
        object ToModel(dynamic dynamic, Type modelType);
        object FromModel(Type modelType, dynamic model);
    }

    public interface IDynamicObjectMapper<TDynamic, TEntity, TOptionValue, TOptionType> : IDynamicToModelMapper
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject<TEntity, TOptionType, TOptionValue>
    {
        TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new();

        TDynamic FromModel<TModel>(TModel model);

        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes);
        TDynamic FromEntity(TEntity entity, bool withDefaults);
    }
}