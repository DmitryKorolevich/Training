using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using System.Threading;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public static class DynamicMapper
    {
        public static bool IsValuesMasked(MappedObject obj)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);

            if (!outerCache.MaskProperties.Any())
                return false;

            foreach (var masker in outerCache.MaskProperties)
            {
                object value;
                if (obj.DictionaryData.TryGetValue(masker.Key, out value))
                {
                    if (!masker.Value.IsMasked(value as string))
                    {
                        return false;
                    }
                }
            }
            return ObjectMapper.IsValuesMasked(obj);
        }
    }

    public abstract class DynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> : ObjectMapper<TDynamic>, IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly ITypeConverter _typeConverter;

        protected abstract Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items, bool withDefaults = false);
        protected abstract Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Expression<Func<TOptionValue, int>> ObjectIdReferenceSelector { get; }
        private Action<TOptionValue, int> _valueSetter;
        private Func<TOptionValue, int> _valueGetter;
        private static ICollection<TOptionType> _optionTypes;

        protected DynamicMapper(ITypeConverter typeConverter,
            IModelConverterService converterService,
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync) : base(typeConverter, converterService)
        {
            _typeConverter = typeConverter;
            if (OptionTypes == null)
            {
                Interlocked.CompareExchange(ref _optionTypes,
                    optionTypeRepositoryAsync.Query().Include(o => o.Lookup).ThenInclude(l => l.LookupVariants).Select(false), null);
            }
        }

        public virtual void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities, ICollection<TOptionType> optionTypes = null)
        {
            SyncCollectionsAsync(dynamics, entities, optionTypes).Wait();
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

        public ICollection<TOptionType> OptionTypes => _optionTypes;

        public Action<TOptionValue, int> SetObjectReferenceId
        {
            get
            {
                if (_valueSetter == null)
                {
                    Interlocked.CompareExchange(ref _valueSetter, GetValueObjectIdSetter(ObjectIdReferenceSelector), null);
                }
                return _valueSetter;
            }
        }

        public Func<TOptionValue, int> GetObjectReferenceId
        {
            get
            {
                if (_valueGetter == null)
                {
                    Interlocked.CompareExchange(ref _valueGetter, ObjectIdReferenceSelector.CacheCompile(), null);
                }
                return _valueGetter;
            }
        }

        public Expression<Func<TOptionValue, int>> ObjectReferenceExpression => ObjectIdReferenceSelector;

        public ICollection<TOptionType> FilterByType(int? objectType)
        {
            var filterFunc = GetOptionTypeQuery().WithObjectType(objectType).Query()?.CacheCompile();
            if (filterFunc != null)
                return OptionTypes.Where(filterFunc).ToArray();
            return OptionTypes.ToArray();
        }

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            return FromEntityAsync(entity, withDefaults).Result;
        }

        public List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null)
        {
            return ToEntityRangeAsync(items, optionTypes).Result;
        }

        public ICollection<DynamicEntityPair<TDynamic, TEntity>> ToEntityRange(ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items)
        {
            return ToEntityRangeAsync(items).Result;
        }

        public List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false)
        {
            return FromEntityRangeAsync(items, withDefaults).Result;
        }

        public async Task UpdateEntityAsync(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                return;

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes = FilterByType(dynamic.IdObjectType);
            }

            UpdateEntityItem(dynamic, entity);
            await UpdateEntityInternalAsync(dynamic, entity);
            //if (dynamic.Id != 0)
            //{
            //    foreach (var value in entity.OptionValues)
            //    {
            //        SetObjectReferenceId(value, dynamic.Id);
            //    }
            //}
        }

		public void UpdateEntity(TDynamic dynamic, TEntity entity)
		{
		    UpdateEntityAsync(dynamic, entity).Wait();
		}

        public void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            UpdateEntityRangeAsync(items).Wait();
        }

        public TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes = null)
        {
            return ToEntityAsync(dynamic, optionTypes).Result;
        }

        public async Task<TEntity> ToEntityAsync(TDynamic dynamic, ICollection<TOptionType> optionTypes = null)
        {
            if (dynamic == null)
                return null;

            if (optionTypes == null)
            {
                optionTypes = FilterByType(dynamic.IdObjectType);
            }
            var entity = new TEntity { OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes };
            FillEntityOptions(dynamic, optionTypes, entity);
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
                entity.OptionTypes = FilterByType(entity.IdObjectType);
            }

            var result = FromEntityItem(entity, withDefaults);
            await FromEntityInternalAsync(result, entity, withDefaults);
            return result;
        }

        public async Task UpdateEntityRangeAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items == null)
                return;

            items = RemoveInvalidForUpdate(items);
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                pair.Entity.OptionTypes = FilterByType(pair.Dynamic.IdObjectType);
            }
            foreach (var pair in items)
            {
                UpdateEntityItem(pair);
            }

            await UpdateEntityRangeInternalAsync(items);
            //items.ForEach(item =>
            //{
            //    var entity = item.Entity;
            //    var dynamic = item.Dynamic;
            //    if (dynamic.Id != 0)
            //    {
            //        foreach (var value in entity.OptionValues)
            //        {
            //            SetObjectReferenceId(value, dynamic.Id);
            //        }
            //    }
            //});
        }

        public async Task<List<TEntity>> ToEntityRangeAsync(ICollection<TDynamic> items,
            ICollection<TOptionType> optionTypes = null)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<DynamicEntityPair<TDynamic, TEntity>> results;
            if (optionTypes == null)
            {
                results =
                    items.Where(d => d != null).Select(
                        dynamic =>
                        {
                            var entity = ToEntityItem(dynamic, FilterByType(dynamic.IdObjectType));
                            return new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity);
                        })
                        .ToArray();
            }
            else
            {
                results =
                    items.Where(d => d != null).Select(
                        dynamic => new DynamicEntityPair<TDynamic, TEntity>(dynamic, ToEntityItem(dynamic, optionTypes)))
                        .ToArray();
            }
            await ToEntityRangeInternalAsync(results);
            return results.Select(r => r.Entity).ToList();
        }

        public async Task<ICollection<DynamicEntityPair<TDynamic, TEntity>>> ToEntityRangeAsync(ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items)
        {
            if (items == null)
                return new List<DynamicEntityPair<TDynamic, TEntity>>();

            foreach (var pair in items.Where(pair => pair.Value2 == null))
            {
                pair.Value2 = FilterByType(pair.Value1.IdObjectType);
            }
            var results =
                items.Where(d => d.Value1 != null).Select(
                    pair =>
                        new DynamicEntityPair<TDynamic, TEntity>(pair.Value1, ToEntityItem(pair.Value1, pair.Value2)))
                    .ToArray();
            await ToEntityRangeInternalAsync(results);
            return results;
        }

        public async Task<List<TDynamic>> FromEntityRangeAsync(ICollection<TEntity> items, bool withDefaults = false)
        {
            if (items == null)
                return new List<TDynamic>();

            foreach (var entity in items.Where(pair => pair.OptionTypes == null))
            {
                entity.OptionTypes = FilterByType(entity.IdObjectType);
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
                    if (!dynamicCache.Properties.ContainsKey(pair.Key))
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
                foreach (var pair in cache.Properties)
                {
                    var mappingName = pair.Value.Map?.Name ?? pair.Key;
                    if (!dynamicCache.Properties.ContainsKey(mappingName))
                    {
                        object dynamicValue;
                        if (data.TryGetValue(mappingName, out dynamicValue))
                        {
                            var value = _typeConverter.ConvertToModel(dynamicValue?.GetType(), pair.Value.PropertyType, dynamicValue, pair.Value.Converter);
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
                foreach (var pair in cache.Properties)
                {
                    var mappingName = pair.Value.Map?.Name ?? pair.Key;
                    if (!dynamicCache.Properties.ContainsKey(mappingName))
                    {
                        var value = TypeConverter.ConvertFromModelObject(pair.Value.PropertyType,
                            pair.Value.Get?.Invoke(model));

                        if (value == null)
                            continue;

                        if (!data.ContainsKey(mappingName))
                        {
                            data.Add(mappingName, value);
                        }
                        else
                        {
                            data[mappingName] = value;
                        }
                    }
                }
            }
        }

        protected override bool IsObjectSecuredInternal(object obj, HashSet<object> processedObjectsSet)
        {
            MappedObject mappedObj = obj as MappedObject;
            if (mappedObj != null)
            {
                var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);
                foreach (var masker in outerCache.MaskProperties)
                {
                    object value;
                    if (mappedObj.DictionaryData.TryGetValue(masker.Key, out value))
                    {
                        if (!masker.Value.IsMasked(value as string))
                        {
                            return false;
                        }
                    }
                }
            }
            return base.IsObjectSecuredInternal(obj, processedObjectsSet);
        }

        protected override void SecureObjectInternal(object obj, HashSet<object> processedObjectsSet)
        {
            MappedObject mappedObj = obj as MappedObject;
            if (mappedObj != null)
            {
                var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);
                foreach (var masker in outerCache.MaskProperties)
                {
                    object value;
                    if (mappedObj.DictionaryData.TryGetValue(masker.Key, out value))
                    {
                        var maskedValue = masker.Value.MaskValue(value as string);
                        if (maskedValue != null)
                            mappedObj.DictionaryData[masker.Key] = maskedValue;
                    }
                }
            }
            base.SecureObjectInternal(obj, processedObjectsSet);
        }

        private static void UpdateEntityItem(TDynamic dynamic, TEntity entity)
        {
            if (dynamic == null || entity == null)
                return;
            if (entity.OptionTypes == null)
            {
                throw new ApiException($"UpdateEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
            }
            FillEntityOptions(dynamic, entity.OptionTypes, entity);
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
            FillEntityOptions(dynamic, optionTypes, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
            entity.OptionTypes = optionTypes;
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
                                (FieldType)optionType.IdFieldType));
                    }
                }
            }
            if (withDefaults)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType)optionType.IdFieldType));
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

        private static void FillEntityOptions(TDynamic obj, ICollection<TOptionType> optionTypes, TEntity entity)
        {
            HashSet<int> optionTypeIds = new HashSet<int>(optionTypes.Select(o => o.Id));
            entity.OptionValues.RemoveAll(o => !optionTypeIds.Contains(o.IdOptionType));
            Dictionary<int, TOptionValue> optionValues = entity.OptionValues.ToDictionary(v => v.IdOptionType);
            foreach (var optionType in optionTypes)
            {
                object value;
                obj.DictionaryData.TryGetValue(optionType.Name, out value);
                TOptionValue option;
                if (optionValues.TryGetValue(optionType.Id, out option))
                {
                    if (value == null)
                    {
                        entity.OptionValues.Remove(option);
                        continue;
                    }
                    MapperTypeConverter.ConvertToOption<TOptionValue, TOptionType>(option, value,
                        (FieldType)optionType.IdFieldType);
                }
                else
                {
                    if (value == null)
                        continue;

                    option = new TOptionValue();
                    MapperTypeConverter.ConvertToOption<TOptionValue, TOptionType>(option, value,
                        (FieldType)optionType.IdFieldType);
                    option.IdOptionType = optionType.Id;
                    entity.OptionValues.Add(option);
                }
            }
        }

        private static Action<TOptionValue, int> GetValueObjectIdSetter(Expression<Func<TOptionValue, int>> selector)
        {
            MemberExpression memberExpression;
            var expressionBody = selector.Body;
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
                    return property.SetMethod.CompileVoidAccessor<TOptionValue, int>();
                }
                throw new MemberAccessException($"Property {property} doesn't have any public setter");
            }
            throw new MemberAccessException($"Expression {memberExpression} doesn't have property selection");
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