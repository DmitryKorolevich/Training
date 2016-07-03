﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.DynamicData.Interfaces
{
    // ReSharper disable once UnusedTypeParameter
    public interface IDynamicMapper<TDynamic, TEntity> : IObjectMapper<TDynamic> 
        where TDynamic : MappedObject, new()
        where TEntity: DynamicDataEntity
    {
        Task<TDynamic> FromModelAsync<TModel>(TModel model, int idObjectType);
        Task UpdateObjectAsync<TModel>(TModel model, TDynamic obj, int idObjectType, bool loadDefaults = true)
            where TModel : class, new();

        Task<TDynamic> CreatePrototypeAsync(int idObjectType);
        Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new();
        TDynamic CreatePrototype(int idObjectType);
        TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new();
    }

    public class DynamicEntityPair<TDynamic, TEntity>
        where TEntity : DynamicDataEntity 
        where TDynamic : MappedObject
    {
        public DynamicEntityPair(TDynamic dynamic, TEntity entity)
        {
            Dynamic = dynamic;
            Entity = entity;
        }

        public TEntity Entity { get; set; }
        //public TEntity InitialEntity { get; set; }
        public TDynamic Dynamic { get; set; }
    }

    public class GenericObjectPair<T1, T2>
    {
        public GenericObjectPair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
    }

    public struct GenericPair<T1, T2>
    {
        public GenericPair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public T1 Value1 { get; set; }
        public T2 Value2 { get; set; }
    }

    public struct DynamicModelPair<TDynamic, TModel>
        where TDynamic : MappedObject
    {
        public DynamicModelPair(TDynamic dynamic, TModel model)
        {
            Dynamic = dynamic;
            Model = model;
        }

        public TModel Model { get; set; }
        public TDynamic Dynamic { get; set; }
    }

    public interface IOptionTypeQueryProvider
    {
        
    }

    public interface IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue> : IOptionTypeQueryProvider
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        Expression<Func<TOptionValue, int>> ObjectIdSelector { get; }
        ICollection<TOptionType> OptionTypes { get; }
        ICollection<TOptionType> FilterByType(int? objectType);
        IEnumerable<TOptionType> OptionsForType(int? objectType);
    }

    public interface IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> : IDynamicMapper<TDynamic, TEntity>, IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        Task SyncCollectionsAsync(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);
        void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);

        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes = null);
        TDynamic FromEntity(TEntity entity, bool withDefaults = false);

        void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null);
        ICollection<DynamicEntityPair<TDynamic, TEntity>> ToEntityRange(ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items);
        List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false);

        Task UpdateEntityAsync(TDynamic dynamic, TEntity entity);
        Task<TEntity> ToEntityAsync(TDynamic dynamic, ICollection<TOptionType> optionTypes = null);
        Task<TDynamic> FromEntityAsync(TEntity entity, bool withDefaults = false);

        Task UpdateEntityRangeAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        Task<List<TEntity>> ToEntityRangeAsync(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null);
        Task<ICollection<DynamicEntityPair<TDynamic, TEntity>>> ToEntityRangeAsync(ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items);
        Task<List<TDynamic>> FromEntityRangeAsync(ICollection<TEntity> items, bool withDefaults = false);
    }
}