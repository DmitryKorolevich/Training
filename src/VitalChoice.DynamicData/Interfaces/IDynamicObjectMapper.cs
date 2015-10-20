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

    public class DynamicEntityPair<TDynamic, TEntity>
        where TEntity: Entity
        where TDynamic: MappedObject
    {
        public DynamicEntityPair(TDynamic dynamic, TEntity entity)
        {
            Dynamic = dynamic;
            Entity = entity;
        }

        public TEntity Entity { get; set; }
        public TDynamic Dynamic { get; set; }
    }

    public class GenericPair<T1, T2>
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

    public interface IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> : IDynamicToModelMapper, IDynamicToModelMapper<TDynamic>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject
    {
        IQueryOptionType<TOptionType> GetOptionTypeQuery();
        IEnumerable<TOptionType> FilterByType(IEnumerable<TOptionType> collection, int? objectType);
        Expression<Func<TOptionValue, int?>> ObjectIdSelector { get; }
        Task SyncCollectionsAsync(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);
        void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null);

        void UpdateEntity(TDynamic dynamic, TEntity entity);
        TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes = null);
        TDynamic FromEntity(TEntity entity, bool withDefaults = false);

        void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null);
        List<TEntity> ToEntityRange(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items);
        List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false);

        Task UpdateEntityAsync(TDynamic dynamic, TEntity entity);
        Task<TEntity> ToEntityAsync(TDynamic dynamic, ICollection<TOptionType> optionTypes = null);
        Task<TDynamic> FromEntityAsync(TEntity entity, bool withDefaults = false);

        Task UpdateEntityRangeAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        Task<List<TEntity>> ToEntityRangeAsync(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null);
        Task<ICollection<DynamicEntityPair<TDynamic, TEntity>>> ToEntityRangeAsync(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items);
        Task<List<TDynamic>> FromEntityRangeAsync(ICollection<TEntity> items, bool withDefaults = false);

        void RemoveSeсurityInformation(MappedObject dynamic);
    }
}