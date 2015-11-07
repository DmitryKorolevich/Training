using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicMapper<TDynamic> : IObjectMapper<TDynamic> 
        where TDynamic : MappedObject
    {
    }

    public class DynamicEntityPair<TDynamic, TEntity>
        where TEntity : Entity
        where TDynamic : MappedObject
    {
        public DynamicEntityPair(TDynamic dynamic, TEntity entity)
        {
            Dynamic = dynamic;
            Entity = entity;
        }

        public TEntity Entity { get; set; }
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

    public interface IOptionTypeQueryProvider<TEntity, TOptionType> : IOptionTypeQueryProvider
        where TOptionType : OptionType
    {
        IQueryOptionType<TOptionType> GetOptionTypeQuery();
        ICollection<TOptionType> OptionTypes { get; }
        IEnumerable<TOptionType> FilterByType(IEnumerable<TOptionType> collection, int? objectType);
    }

    public interface IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> : IObjectMapper, IDynamicMapper<TDynamic>, IOptionTypeQueryProvider<TEntity, TOptionType>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject
    {
        Expression<Func<TOptionValue, int?>> ObjectIdSelector { get; }
        Task SyncCollectionsAsync(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);
        void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);

        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes = null);
        TDynamic FromEntity(TEntity entity, bool withDefaults = false);

        void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null);
        List<TEntity> ToEntityRange(ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items);
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