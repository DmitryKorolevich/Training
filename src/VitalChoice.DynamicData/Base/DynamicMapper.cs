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
using System.Collections;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.DynamicData.Base
{
    public abstract class DynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> : ObjectMapper<TDynamic>, IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IReadRepositoryAsync<TOptionType> _optionTypeRepositoryAsync;
        private readonly ITypeConverter _typeConverter;

        protected abstract Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items, bool withDefaults = false);
        protected abstract Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        public abstract Expression<Func<TOptionValue, int?>> ObjectIdSelector { get; }
        private Action<TOptionValue, int> _valueSetter;

        protected DynamicMapper(ITypeConverter typeConverter,
            IModelConverterService converterService, 
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync) : base(typeConverter, converterService)
        {
            _optionTypeRepositoryAsync = optionTypeRepositoryAsync;
            _typeConverter = typeConverter;
        }

        public Action<TOptionValue, int> GetValueObjectIdSetter()
        {
            if (_valueSetter != null)
                return _valueSetter;
            MemberExpression memberExpression;
            var expressionBody = ObjectIdSelector.Body;
            if (expressionBody.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression)expressionBody).Operand as MemberExpression;
            }
            else
            {
                memberExpression = expressionBody as MemberExpression;
            }
            if (memberExpression?.Member is PropertyInfo)
            {
                var property = (PropertyInfo)memberExpression.Member;
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

        public virtual void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null)
        {
            var task = SyncCollectionsAsync(dynamics, entities, optionTypes);
            task.Wait();
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
                //foreach (var item in itemsToUpdate)
                //{
                //    item.Entity.StatusCode = (int)RecordStatusCode.Active;
                //}

                //Delete
                var toDelete = entities.Where(e => dynamics.All(s => s.Id != e.Id));
                foreach (var paymentMethod in toDelete)
                {
                    paymentMethod.StatusCode = (int)RecordStatusCode.Deleted;
                }

                //Insert
                var list = await ToEntityRangeAsync(dynamics.Where(s => s.Id == 0).ToList(), optionTypes);
                entities.AddRange(list);
            }
            else if (entities != null)
            {
                foreach (var paymentMethod in entities)
                {
                    paymentMethod.StatusCode = (int)RecordStatusCode.Deleted;
                }
            }
        }

        public virtual IQueryOptionType<TOptionType> GetOptionTypeQuery()
        {
            return new OptionTypeQuery<TOptionType>();
        }

        public IEnumerable<TOptionType> FilterByType(IEnumerable<TOptionType> collection, int? objectType)
        {
            var filterFunc = GetOptionTypeQuery().WithObjectType(objectType).Query()?.CacheCompile();
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
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(dynamic.IdObjectType)).SelectAsync(false);
            }

            UpdateEntityItem(dynamic, entity);
            await UpdateEntityInternalAsync(dynamic, entity);
            var valueObjectIdSetter = GetValueObjectIdSetter();
            foreach (var value in entity.OptionValues)
            {
                valueObjectIdSetter(value, dynamic.Id);
            }
        }

		public void UpdateEntity(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                return;

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType)).Select(false);
            }

            UpdateEntityItem(dynamic, entity);
            UpdateEntityInternalAsync(dynamic, entity).Wait();
            var valueObjectIdSetter = GetValueObjectIdSetter();
            foreach (var value in entity.OptionValues)
            {
                valueObjectIdSetter(value, dynamic.Id);
            }
        }

        public void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items == null)
                return;

            ICollection<TOptionType> optionTypes = null;
            items = RemoveInvalidForUpdate(items);
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                }
                pair.Entity.OptionTypes = FilterByType(optionTypes, pair.Dynamic.IdObjectType).ToList();
            }
            foreach (var pair in items)
            {
                UpdateEntityItem(pair);
            }

            UpdateEntityRangeInternalAsync(items).Wait();
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
            if (optionTypes == null)
            {
                throw new ApiException($"ToEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
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
            return entity;
        }

        private static TDynamic FromEntityItem(TEntity entity, bool withDefaults)
        {
            if (entity == null)
                return null;
            if (entity.OptionValues == null)
            {
                throw new ApiException($"FromEntityItem<{typeof(TEntity)}> have no OptionValues, are you forgot to include them in query?");
            }
            if (entity.OptionTypes == null)
            {
                throw new ApiException($"FromEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
            }
            var result = new TDynamic();
            var data = result.DictionaryData;
            if (entity.OptionValues.Any())
            {
                var optionTypes = entity.OptionTypes.ToDictionary(o => o.Id, o => o);
                foreach (var value in entity.OptionValues)
                {
                    TOptionType optionType;
                    if (optionTypes.TryGetValue(value.IdOptionType, out optionType))
                    {
                        data.Add(optionType.Name,
                            MapperTypeConverter.ConvertTo<TOptionValue, TOptionType>(value,
                                (FieldType) optionType.IdFieldType));
                    }
                }
            }
            if (withDefaults)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                }
            }
            result.Id = entity.Id;
            result.DateCreated = entity.DateCreated;
            result.DateEdited = entity.DateEdited;
            result.StatusCode = entity.StatusCode;
            result.IdEditedBy = entity.IdEditedBy;
            result.IdObjectType = entity.IdObjectType;
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
                entity.OptionTypes =
                    await
                        _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType))
                            .SelectAsync(false);
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

        public async Task<ICollection<DynamicEntityPair<TDynamic, TEntity>>> ToEntityRangeAsync(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items)
        {
            if (items == null)
                return new List<DynamicEntityPair<TDynamic, TEntity>>();

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
            return results.ToList();
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

        protected override void FromDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType, bool caseSense)
        {
            base.FromDictionaryInternal(obj, model, objectType, caseSense);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = model.GetType();
                var dynamicCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType,
                    true);
                var data = dynamic.DictionaryData;
                foreach (var pair in model)
                {
                    if (!dynamicCache.ContainsKey(pair.Key))
                    {
                        var value = TypeConverter.ConvertFromModelObject(pair.Value.GetType(), pair.Value);
                        if (value != null)
                        {
                            data.Add(pair.Key, value);
                        }
                    }
                }
            }
        }

        protected override void ToDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType)
        {
            base.ToDictionaryInternal(obj, model, objectType);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = model.GetType();
                var data = dynamic.DictionaryData;
                foreach (var pair in data)
                {
                    if (!model.ContainsKey(pair.Key))
                    {
                        model.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        protected override void ToModelInternal(object obj, object result,
            Type modelType, Type objectType)
        {
            base.ToModelInternal(obj, result, modelType, objectType);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = modelType;
                var data = dynamic.DictionaryData;
                var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
                var dynamicCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType,
                    true);
                foreach (var pair in cache)
                {
                    var mappingName = pair.Value.Map.Name ?? pair.Key;
                    if (!dynamicCache.ContainsKey(mappingName))
                    {
                        object dynamicValue;
                        if (data.TryGetValue(mappingName, out dynamicValue))
                        {
                            var value = _typeConverter.ConvertToModel(dynamicValue?.GetType(), pair.Value.PropertyType, dynamicValue);
                            if (value != null)
                            {
                                MapperTypeConverter.ThrowIfNotValid(modelType, objectType, value, pair.Key, pair.Value,
                                    true);
                                pair.Value.Set?.Invoke(result, value);
                            }
                        }
                    }
                }
            }
        }

        protected override void FromModelInternal(object obj, object model,
            Type modelType, Type objectType)
        {
            base.FromModelInternal(obj, model, modelType, objectType);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = modelType;
                var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
                var dynamicCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType,
                    true);
                var data = dynamic.DictionaryData;
                foreach (var pair in cache)
                {
                    var mappingName = pair.Value.Map.Name ?? pair.Key;
                    if (!dynamicCache.ContainsKey(mappingName))
                    {
                        var value = TypeConverter.ConvertFromModelObject(pair.Value.PropertyType,
                            pair.Value.Get?.Invoke(model));
                        if (value != null)
                        {
                            data.Add(mappingName, value);
                        }
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