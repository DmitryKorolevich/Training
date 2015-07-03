using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicToModelMapper
    {
        object ToModel(dynamic dynamic, Type modelType);
        MappedObject FromModel(Type modelType, dynamic model);
    }

    public interface IDynamicToModelMapper<TDynamic>
        where TDynamic : MappedObject
    {
        TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new();

        TDynamic FromModel<TModel>(TModel model);
    }

    public struct Pair<T1, T2>
    {
        public Pair(T1 first, T2 second)
        {
            FirstValue = first;
            SecondValue = second;
        }

        public T1 FirstValue { get; set; }
        public T2 SecondValue { get; set; }
    }

    public interface IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> : IDynamicToModelMapper, IDynamicToModelMapper<TDynamic>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject
    {
        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes);
        TDynamic FromEntity(TEntity entity, bool withDefaults = false);
        void UpdateEntityRange(ICollection<Pair<TDynamic, TEntity>> items);
        List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes);
        List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false);
    }
}