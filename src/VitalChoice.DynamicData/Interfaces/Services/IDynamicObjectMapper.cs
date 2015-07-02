using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Interfaces.Services
{
    public interface IDynamicToModelMapper
    {
        object ToModel(dynamic dynamic, Type modelType);
        MappedObject FromModel(Type modelType, dynamic model);
    }

    public interface IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> : IDynamicToModelMapper
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject
    {
        TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new();

        TDynamic FromModel<TModel>(TModel model);

        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes);
        TDynamic FromEntity(TEntity entity, bool withDefaults);
    }
}