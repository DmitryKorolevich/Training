using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public abstract class DynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IIndex<TypePair, IModelToDynamicConverter> _converters;
        private readonly IReadRepositoryAsync<TOptionType> _optionTypeRepositoryAsync;
        private readonly ModelTypeConverter _typeConverter;

        protected abstract Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items, bool withDefaults = false);
        protected abstract Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        public abstract Expression<Func<TOptionValue, int?>> ObjectIdSelector { get; }
        private Action<TOptionValue, int> _valueSetter;

        protected DynamicObjectMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<TypePair, IModelToDynamicConverter> converters,
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync)
        {
            _converters = converters;
            _optionTypeRepositoryAsync = optionTypeRepositoryAsync;
            _typeConverter = new ModelTypeConverter(mappers);
        }

        public Action<TOptionValue, int> GetValueObjectIdSetter()
        {
            if (_valueSetter != null)
                return _valueSetter;
            MemberExpression memberExpression;
            var expressionBody = ObjectIdSelector.Body;
            if (expressionBody.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression) expressionBody).Operand as MemberExpression;
            }
            else
            {
                memberExpression = expressionBody as MemberExpression;
            }
            if (memberExpression?.Member is PropertyInfo)
            {
                var property = (PropertyInfo) memberExpression.Member;
                if (property.CanWrite)
                {
                    _valueSetter = property.SetMethod.CompileVoidAccessor<TOptionValue, int>();
                }
                else
                {
                    throw new MemberAccessException($"Property {property} doesn't have any public setter");
                }
            }
            else
            {
                throw new MemberAccessException($"Expression {memberExpression} doesn't have property selection");
            }
            return _valueSetter;
        }

        public virtual async void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null)
        {
            await SyncCollectionsAsync(dynamics, entities, optionTypes);
        }

        public virtual async Task SyncCollectionsAsync(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null)
        {
            if (dynamics != null && dynamics.Any() && entities != null)
            {
                //Update existing
                var itemsToUpdate = dynamics.Join(entities, sd => sd.Id, s => s.Id,
                    (@dynamic, entity) =>
                    {
                        if (optionTypes != null)
                            entity.OptionTypes = optionTypes;
                        return new DynamicEntityPair<TDynamic, TEntity>(@dynamic, entity);
                    }).ToList();
                await UpdateEntityRangeAsync(itemsToUpdate);
                foreach (var item in itemsToUpdate)
                {
                    item.Entity.StatusCode = RecordStatusCode.Active;
                }

                //Delete
                var toDelete = entities.Where(e => dynamics.All(s => s.Id != e.Id));
                foreach (var paymentMethod in toDelete)
                {
                    paymentMethod.StatusCode = RecordStatusCode.Deleted;
                }

                //Insert
                var list = await ToEntityRangeAsync(dynamics.Where(s => s.Id == 0).ToList(), optionTypes);
                entities.AddRange(list);
            }
            else if (entities != null)
            {
                foreach (var paymentMethod in entities)
                {
                    paymentMethod.StatusCode = RecordStatusCode.Deleted;
                }
            }
        }

        public virtual IQueryOptionType<TOptionType> GetOptionTypeQuery()
        {
            return new OptionTypeQuery<TOptionType>();
        }

        public IEnumerable<TOptionType> FilterByType(IEnumerable<TOptionType> collection, int? objectType)
        {
            var filterFunc = GetOptionTypeQuery().WithObjectType(objectType).Query()?.Compile();
            if (filterFunc != null)
                return collection.Where(filterFunc);
            return collection;
        }

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            if (entity == null)
                return null;

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType)).Select(false);
            }

            var result = FromEntityItem(entity, withDefaults);
            FromEntityInternalAsync(result, entity, withDefaults).Wait();
            return result;
        }

        public List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<DynamicEntityPair<TDynamic, TEntity>> results;
            if (optionTypes == null)
            {
                optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                results =
                    items.Where(e => e != null).Select(
                        dynamic =>
                            new DynamicEntityPair<TDynamic, TEntity>(dynamic,
                                ToEntityItem(dynamic, FilterByType(optionTypes, dynamic.IdObjectType).ToList())))
                        .ToList();
            }
            else
            {
                results =
                    items.Where(e => e != null).Select(
                        dynamic => new DynamicEntityPair<TDynamic, TEntity>(dynamic, ToEntityItem(dynamic, optionTypes)))
                        .ToList();
            }
            ToEntityRangeInternalAsync(results).Wait();
            return results.Select(r => r.Entity).ToList();
        }

        public List<TEntity> ToEntityRange(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<TOptionType> optionTypes = null;
            foreach (var pair in items.Where(pair => pair.Value2 == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                }
                pair.Value2 = FilterByType(optionTypes, pair.Value1.IdObjectType).ToList();
            }
            var results =
                items.Where(e => e.Value1 != null).Select(
                    pair =>
                        new DynamicEntityPair<TDynamic, TEntity>(pair.Value1, ToEntityItem(pair.Value1, pair.Value2)))
                    .ToList();
            ToEntityRangeInternalAsync(results).Wait();
            return results.Select(r => r.Entity).ToList();
        }

        public List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false)
        {
            if (items == null)
                return new List<TDynamic>();

            ICollection<TOptionType> optionTypes = null;
            foreach (var entity in items.Where(pair => pair.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                }
                entity.OptionTypes = FilterByType(optionTypes, entity.IdObjectType).ToList();
            }
            List<DynamicEntityPair<TDynamic, TEntity>> results =
                items.Where(e => e != null).Select(
                    entity => new DynamicEntityPair<TDynamic, TEntity>(FromEntityItem(entity, withDefaults), entity))
                    .ToList();
            FromEntityRangeInternalAsync(results, withDefaults).Wait();
            return results.Select(r => r.Dynamic).ToList();
        }

        public async Task UpdateEntityAsync(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                return;

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType)).SelectAsync(false);
            }

            UpdateEntityItem(dynamic, entity);
            await UpdateEntityInternalAsync(dynamic, entity);
            var valueObjectIdSetter = GetValueObjectIdSetter();
            foreach (var value in entity.OptionValues)
            {
                valueObjectIdSetter(value, dynamic.Id);
            }
        }

        public TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new()
        {
            if (dynamic == null)
                return null;

            var result = new TModel();
            ToModelItem(dynamic, result, typeof (TModel), typeof (TDynamic));
            
            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(new TypePair(typeof (TModel), typeof(TDynamic)), out conv))
            {
                var converter = conv as IModelToDynamicConverter<TModel, TDynamic>;
                converter?.DynamicToModel(result, dynamic);
            }
            return result;
        }

        public TDynamic FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelItem(result, model, typeof (TModel), typeof (TDynamic));

            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(new TypePair(typeof(TModel), typeof(TDynamic)), out conv))
            {
                var converter = conv as IModelToDynamicConverter<TModel, TDynamic>;
                converter?.ModelToDynamic(model, result);
            }
            return result;
        }

        object IDynamicToModelMapper.ToModel(dynamic dynamic, Type modelType)
        {
            if (dynamic == null)
                return null;

            dynamic result = Activator.CreateInstance(modelType);
            ToModelItem(dynamic, result, modelType,
                typeof (TDynamic));

            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(new TypePair(modelType, typeof(TDynamic)), out conv))
            {
                dynamic converter = conv;
                converter?.DynamicToModel(result, dynamic);
            }

            return result;
        }

        MappedObject IDynamicToModelMapper.FromModel(Type modelType, dynamic model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelItem(result, model, modelType, typeof (TDynamic));

            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(new TypePair(modelType, typeof(TDynamic)), out conv))
            {
                dynamic converter = conv;
                converter?.ModelToDynamic(model, result);
            }
            return result;
        }

        public async void UpdateEntity(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                return;

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType)).SelectAsync(false);
            }

            UpdateEntityItem(dynamic, entity);
            await UpdateEntityInternalAsync(dynamic, entity);
            var valueObjectIdSetter = GetValueObjectIdSetter();
            foreach (var value in entity.OptionValues)
            {
                valueObjectIdSetter(value, dynamic.Id);
            }
        }

        public async void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items == null)
                return;

            ICollection<TOptionType> optionTypes = null;
            items = RemoveInvalidForUpdate(items);
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                pair.Entity.OptionTypes = FilterByType(optionTypes, pair.Dynamic.IdObjectType).ToList();
            }
            foreach (var pair in items)
            {
                UpdateEntityItem(pair);
            }
            
            await UpdateEntityRangeInternalAsync(items);
            var valueObjectIdSetter = GetValueObjectIdSetter();
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                foreach (var value in entity.OptionValues)
                {
                    valueObjectIdSetter(value, dynamic.Id);
                }
            });
        }

        private static void UpdateEntityItem(TDynamic dynamic, TEntity entity)
        {
            if (dynamic == null || entity == null)
                return;
            var optionTypesCache = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
            entity.OptionValues = new List<TOptionValue>();

            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
        }

        private static TEntity ToEntityItem(TDynamic dynamic, ICollection<TOptionType> optionTypes)
        {
            if (dynamic == null)
                return null;
            var entity = new TEntity { OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes };
            var optionTypesCache = optionTypes.ToDictionary(o => o.Name, o => o);
            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
            return entity;
        }

        private static TDynamic FromEntityItem(TEntity entity, bool withDefaults)
        {
            if (entity == null)
                return null;
            var result = new TDynamic();
            var data = result.DictionaryData;
            var optionTypes = entity.OptionTypes?.ToDictionary(o => o.Id, o => o);
            if (entity.OptionValues != null && optionTypes != null)
            {
                foreach (var value in entity.OptionValues)
                {
                    TOptionType optionType;
                    if (optionTypes.TryGetValue(value.IdOptionType, out optionType))
                    {
                        data.Add(optionType.Name,
                            MapperTypeConverter.ConvertTo<TOptionValue, TOptionType>(value,
                                (FieldType)optionType.IdFieldType));
                    }
                }
            }
            result.Id = entity.Id;
            result.DateCreated = entity.DateCreated;
            result.DateEdited = entity.DateEdited;
            result.StatusCode = entity.StatusCode;
            result.IdEditedBy = entity.IdEditedBy;
            result.IdObjectType = entity.IdObjectType;
            if (!withDefaults || entity.OptionTypes == null)
                return result;
            foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
            {
                data.Add(optionType.Name,
                    MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType)optionType.IdFieldType));
            }
            return result;
        }

        private static void UpdateEntityItem(DynamicEntityPair<TDynamic, TEntity> pair)
        {
            UpdateEntityItem(pair.Dynamic, pair.Entity);
        }

        public TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes = null)
        {
            if (dynamic == null)
                return null;

            var task = ToEntityAsync(dynamic, optionTypes);
            task.Wait();
            return task.Result;
        }

        public async Task<TEntity> ToEntityAsync(TDynamic dynamic, ICollection<TOptionType> optionTypes = null)
        {
            if (dynamic == null)
                return null;

            if (optionTypes == null)
            {
                optionTypes =
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(dynamic.IdObjectType)).SelectAsync(false);
            }
            var entity = new TEntity { OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes };
            var optionTypesCache = optionTypes.ToDictionary(o => o.Name, o => o);
            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
            await ToEntityInternalAsync(dynamic, entity);
            return entity;
        }

        public async Task<TDynamic> FromEntityAsync(TEntity entity, bool withDefaults = false)
        {
            if (entity == null)
                return null;

            if (entity.OptionTypes == null)
            {
                try
                {
                    entity.OptionTypes =
                        await
                            _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType))
                                .SelectAsync(false);
                }
                catch
                {
                    return null;
                }
            }

            var result = FromEntityItem(entity, withDefaults);
            FromEntityInternalAsync(result, entity, withDefaults).Wait();
            return result;
        }

        public async Task UpdateEntityRangeAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items == null)
                return;

            ICollection<TOptionType> optionTypes = null;
            items = RemoveInvalidForUpdate(items);
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                pair.Entity.OptionTypes = FilterByType(optionTypes, pair.Dynamic.IdObjectType).ToList();
            }
            foreach (var pair in items)
            {
                UpdateEntityItem(pair);
            }

            await UpdateEntityRangeInternalAsync(items);
            var valueObjectIdSetter = GetValueObjectIdSetter();
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                foreach (var value in entity.OptionValues)
                {
                    valueObjectIdSetter(value, dynamic.Id);
                }
            });
        }

        public async Task<List<TEntity>> ToEntityRangeAsync(ICollection<TDynamic> items,
            ICollection<TOptionType> optionTypes = null)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<DynamicEntityPair<TDynamic, TEntity>> results;
            if (optionTypes == null)
            {
                optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                results =
                    items.Where(d => d != null).Select(
                        dynamic =>
                        {
                            var entity = ToEntityItem(dynamic, FilterByType(optionTypes, dynamic.IdObjectType).ToList());
                            return new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity);
                        })
                        .ToList();
            }
            else
            {
                results =
                    items.Where(d => d != null).Select(
                        dynamic => new DynamicEntityPair<TDynamic, TEntity>(dynamic, ToEntityItem(dynamic, optionTypes)))
                        .ToList();
            }
            await ToEntityRangeInternalAsync(results);
            return results.Select(r => r.Entity).ToList();
        }

        public async Task<List<TEntity>> ToEntityRangeAsync(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<TOptionType> optionTypes = null;
            foreach (var pair in items.Where(pair => pair.Value2 == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                pair.Value2 = FilterByType(optionTypes, pair.Value1.IdObjectType).ToList();
            }
            var results =
                items.Where(d => d.Value1 != null).Select(
                    pair =>
                        new DynamicEntityPair<TDynamic, TEntity>(pair.Value1, ToEntityItem(pair.Value1, pair.Value2)))
                    .ToList();
            await ToEntityRangeInternalAsync(results);
            return results.Select(r => r.Entity).ToList();
        }

        public async Task<List<TDynamic>> FromEntityRangeAsync(ICollection<TEntity> items, bool withDefaults = false)
        {
            if (items == null)
                return new List<TDynamic>();

            ICollection<TOptionType> optionTypes = null;
            foreach (var entity in items.Where(pair => pair.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                entity.OptionTypes = FilterByType(optionTypes, entity.IdObjectType).ToList();
            }
            List<DynamicEntityPair<TDynamic, TEntity>> results =
                items.Where(e => e != null).Select(
                    entity => new DynamicEntityPair<TDynamic, TEntity>(FromEntityItem(entity, withDefaults), entity))
                    .ToList();
            await FromEntityRangeInternalAsync(results, withDefaults);
            return results.Select(r => r.Dynamic).ToList();
        }

        private void ToModelItem(MappedObject dynamic, object result,
            Type modelType, Type dynamicType)
        {
            if (dynamic == null)
                return;

            dynamic.ModelType = modelType;
            var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
            var data = dynamic.DictionaryData;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertToModelObject(pair.Value.PropertyType, dynamicProperty.Get?.Invoke(dynamic));
                    if (value != null)
                    {
                        MapperTypeConverter.ThrowIfNotValid(modelType, dynamicType, value, pair.Key, pair.Value, true);
                        pair.Value.Set?.Invoke(result, value);
                    }
                }
                else
                {
                    object dynamicValue;
                    if (data.TryGetValue(mappingName, out dynamicValue))
                    {
                        var value = _typeConverter.ConvertToModelObject(pair.Value.PropertyType, dynamicValue);
                        if (value != null)
                        {
                            MapperTypeConverter.ThrowIfNotValid(modelType, dynamicType, value, pair.Key, pair.Value,
                                true);
                            pair.Value.Set?.Invoke(result, value);
                        }
                    }
                }
            }
        }

        private void FromModelItem(MappedObject dynamic, object model,
            Type modelType, Type dynamicType)
        {
            if (dynamic == null)
                return;

            dynamic.ModelType = modelType;
            var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
            var data = dynamic.DictionaryData;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertFromModelObject(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model));
                    if (value != null)
                    {
                        MapperTypeConverter.ThrowIfNotValid(modelType, dynamicType, value, mappingName, dynamicProperty,
                            false);
                        dynamicProperty.Set?.Invoke(dynamic, value);
                    }
                }
                else
                {
                    var value = ModelTypeConverter.ConvertFromModelObject(pair.Value.PropertyType, pair.Value.Get?.Invoke(model));
                    if (value != null)
                    {
                        data.Add(mappingName, value);
                    }
                }
            }
        }

        private static void FillEntityOptions(TDynamic obj, Dictionary<string, TOptionType> optionTypesCache, TEntity entity)
        {
            foreach (var data in obj.DynamicData)
            {
                if (data.Value == null) continue;
                TOptionType optionType;

                if (!optionTypesCache.TryGetValue(data.Key, out optionType)) continue;

                var option = new TOptionValue();
                MapperTypeConverter.ConvertToOption<TOptionValue, TOptionType>(option, data.Value,
                    (FieldType)optionType.IdFieldType);
                option.IdOptionType = optionType.Id;
                entity.OptionValues.Add(option);
            }
        }

        private ICollection<DynamicEntityPair<TDynamic, TEntity>> RemoveInvalidForUpdate(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items.Any(i => i.Dynamic == null || i.Entity == null))
            {
                var intermidiate = items.Where(i => i.Dynamic != null).ToArray();
                foreach (var pair in intermidiate)
                {
                    if (pair.Entity == null)
                    {
                        pair.Entity = ToEntity(pair.Dynamic);
                    }
                }
                return intermidiate.ToList();
            }
            return items;
        }

        private async Task FromEntityInternalAsync(TDynamic dynamic, TEntity entity, bool withDefaults = false)
        {
            if (entity == null || dynamic == null)
                return;
            await
                FromEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                }, withDefaults);
        }

        private async Task UpdateEntityInternalAsync(TDynamic dynamic, TEntity entity)
        {
            if (entity == null || dynamic == null)
                return;
            await
                UpdateEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                });
        }

        private async Task ToEntityInternalAsync(TDynamic dynamic, TEntity entity)
        {
            if (entity == null || dynamic == null)
                return;
            await
                ToEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                });
        }
    }
}