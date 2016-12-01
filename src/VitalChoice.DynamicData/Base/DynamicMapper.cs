using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.DynamicData.Base
{
    public static class DynamicMapper
    {
        public static bool IsValuesMasked(MappedObject obj)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);

            if (outerCache.MaskProperties.Count == 0)
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

    public abstract class DynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> : ObjectMapper<TDynamic>,
        IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly ITypeConverter _converter;
        private readonly Dictionary<int, ICollection<TOptionType>> _optionTypesByType;
        private readonly Lazy<ICollection<TOptionType>> _optionTypes;

        public abstract Expression<Func<TOptionValue, int>> ObjectIdSelector { get; }

        protected abstract Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items,
            bool withDefaults = false);

        protected abstract Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);

        protected DynamicMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync)
            : base(converter, converterService)
        {
            _converter = converter;
            _optionTypes =
                new Lazy<ICollection<TOptionType>>(
                    () =>
                        optionTypeRepositoryAsync.Query()
                            .Include(o => o.Lookup)
                            .ThenInclude(l => l.LookupVariants)
                            .Select(false), LazyThreadSafetyMode.None);
            _optionTypesByType = new Dictionary<int, ICollection<TOptionType>>();
        }

        public virtual void SyncCollections(ICollection<TDynamic> dynamics, ICollection<TEntity> entities,
            ICollection<TOptionType> optionTypes = null)
        {
            SyncCollectionsAsync(dynamics, entities, optionTypes).GetAwaiter().GetResult();
        }

        public virtual async Task SyncCollectionsAsync<T>(ICollection<TDynamic> dynamics, ICollection<T> entities, Func<T, TEntity> selector,
            Func<TEntity, T> constructor,
            ICollection<TOptionType> optionTypes = null)
        {
            if (dynamics != null && dynamics.Count > 0 && entities != null)
            {
                //Update existing
                var itemsToUpdate = dynamics.Join(entities, sd => sd.Id, s => selector(s).Id,
                    (@dynamic, item) =>
                    {
                        var entity = selector(item);
                        if (optionTypes != null)
                            entity.OptionTypes = optionTypes;
                        return new DynamicEntityPair<TDynamic, TEntity>(@dynamic, entity);
                    }).ToList();
                await UpdateEntityRangeAsync(itemsToUpdate);

                //Delete
                var toDelete = entities.Where(e => dynamics.All(s => s.Id != selector(e).Id));
                foreach (var entity in toDelete)
                {
                    selector(entity).StatusCode = (int) RecordStatusCode.Deleted;
                }

                //Insert
                var list = await ToEntityRangeAsync(dynamics.Where(s => s.Id == 0).ToList(), optionTypes);
                entities.AddRange(list.Select(constructor));
            }
            else if (entities != null)
            {
                foreach (var entity in entities)
                {
                    selector(entity).StatusCode = (int) RecordStatusCode.Deleted;
                }
            }
        }

        public virtual async Task SyncCollectionsAsync(ICollection<TDynamic> dynamics, ICollection<TEntity> entities,
            ICollection<TOptionType> optionTypes = null)
        {
            if (dynamics != null && dynamics.Count > 0 && entities != null)
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

                //Delete
                var toDelete = entities.Where(e => dynamics.All(s => s.Id != e.Id));
                foreach (var paymentMethod in toDelete)
                {
                    paymentMethod.StatusCode = (int) RecordStatusCode.Deleted;
                }

                //Insert
                var list = await ToEntityRangeAsync(dynamics.Where(s => s.Id == 0).ToList(), optionTypes);
                entities.AddRange(list);
            }
            else if (entities != null)
            {
                foreach (var paymentMethod in entities)
                {
                    paymentMethod.StatusCode = (int) RecordStatusCode.Deleted;
                }
            }
        }

        public ICollection<TOptionType> OptionTypes => _optionTypes.Value;

        public virtual Func<TOptionType, int?, bool> FilterFunc
            => (t, type) => t.IdObjectType == type || type != null && t.IdObjectType == null;

        public ICollection<TOptionType> FilterByType(int? objectType)
        {
            var filterFunc = FilterFunc;
            if (filterFunc != null)
            {
                ICollection<TOptionType> optionTypes;
                if (_optionTypesByType.TryGetValue(objectType ?? 0, out optionTypes))
                {
                    return optionTypes;
                }
                optionTypes = OptionTypes.Where(t => filterFunc(t, objectType)).ToList();
                _optionTypesByType.Add(objectType ?? 0, optionTypes);
                return optionTypes;
            }
            return OptionTypes;
        }

        public IEnumerable<TOptionType> OptionsForType(int? objectType)
        {
            var filterFunc = FilterFunc;
            if (filterFunc != null)
                return OptionTypes.Where(t => filterFunc(t, objectType));
            return OptionTypes;
        }

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            return FromEntityAsync(entity, withDefaults).GetAwaiter().GetResult();
        }

        public List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null)
        {
            return ToEntityRangeAsync(items, optionTypes).GetAwaiter().GetResult();
        }

        public ICollection<DynamicEntityPair<TDynamic, TEntity>> ToEntityRange(
            ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items)
        {
            return ToEntityRangeAsync(items).GetAwaiter().GetResult();
        }

        public List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false)
        {
            return FromEntityRangeAsync(items, withDefaults).GetAwaiter().GetResult();
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
            return ToEntityAsync(dynamic, optionTypes).GetAwaiter().GetResult();
        }

        public async Task<TEntity> ToEntityAsync(TDynamic dynamic, ICollection<TOptionType> optionTypes = null)
        {
            if (dynamic == null)
                return null;

            if (optionTypes == null)
            {
                optionTypes = FilterByType(dynamic.IdObjectType);
            }
            var entity = new TEntity {OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes};
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
        }

        public async Task<List<TEntity>> ToEntityRangeAsync(ICollection<TDynamic> items,
            ICollection<TOptionType> optionTypes = null)
        {
            if (items == null)
                return new List<TEntity>();

            ICollection<DynamicEntityPair<TDynamic, TEntity>> results;
            DateTime now = DateTime.Now;
            if (optionTypes == null)
            {
                results =
                    items.Where(d => d != null).Select(
                        dynamic =>
                        {
                            var entity = ToEntityItem(dynamic, FilterByType(dynamic.IdObjectType), now);
                            return new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity);
                        })
                        .ToArray();
            }
            else
            {
                results =
                    items.Where(d => d != null).Select(
                        dynamic => new DynamicEntityPair<TDynamic, TEntity>(dynamic, ToEntityItem(dynamic, optionTypes, now)))
                        .ToArray();
            }
            await ToEntityRangeInternalAsync(results);
            return results.Select(r => r.Entity).ToList();
        }

        public async Task<ICollection<DynamicEntityPair<TDynamic, TEntity>>> ToEntityRangeAsync(
            ICollection<GenericObjectPair<TDynamic, ICollection<TOptionType>>> items)
        {
            if (items == null)
                return new List<DynamicEntityPair<TDynamic, TEntity>>();

            foreach (var pair in items.Where(pair => pair.Value2 == null))
            {
                pair.Value2 = FilterByType(pair.Value1.IdObjectType);
            }
            DateTime now = DateTime.Now;
            var results =
                items.Where(d => d.Value1 != null).Select(
                    pair =>
                        new DynamicEntityPair<TDynamic, TEntity>(pair.Value1, ToEntityItem(pair.Value1, pair.Value2, now)))
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

        protected override async Task FromDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType, bool caseSense, bool withNullReset = false)
        {
            await base.FromDictionaryInternal(obj, model, objectType, caseSense, withNullReset);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = model.GetType();
                var dynamicCache = DynamicTypeCache.GetTypeCache(objectType, true);
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
                        else if (withNullReset)
                        {
                            if (data.ContainsKey(pair.Key))
                            {
                                data.Remove(pair.Key);
                            }
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

        protected override async Task ToModelInternal(object obj, object result,
            Type modelType, Type objectType, bool withNullReset = false)
        {
            await base.ToModelInternal(obj, result, modelType, objectType, withNullReset);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = modelType;
                var data = dynamic.DictionaryData;
                var cache = DynamicTypeCache.GetTypeCache(modelType);
                var dynamicCache = DynamicTypeCache.GetTypeCache(objectType, true);
                foreach (var pair in cache.Properties)
                {
                    var mappingName = pair.Value.Map?.Name ?? pair.Key;
                    if (!dynamicCache.Properties.ContainsKey(mappingName))
                    {
                        object dynamicValue;
                        if (data.TryGetValue(mappingName, out dynamicValue))
                        {
                            var value =
                                await _converter.ConvertToModelAsync(dynamicValue?.GetType(), pair.Value.PropertyType, dynamicValue,
                                    pair.Value.Converter);
                            if (value != null)
                            {
                                //TypeValidator.ThrowIfNotValid(modelType, objectType, value, pair.Key, pair.Value,
                                //    true);
                                pair.Value.Set?.Invoke(result, value);
                            }
                            else if (withNullReset)
                            {
                                pair.Value.Set?.Invoke(result, pair.Value.GetDefaultValue());
                            }
                        }
                    }
                }
            }
        }

        protected override async Task FromModelInternal(object obj, object model,
            Type modelType, Type objectType, bool withNullReset = false)
        {
            await base.FromModelInternal(obj, model, modelType, objectType, withNullReset);
            var dynamic = obj as MappedObject;
            if (dynamic != null)
            {
                dynamic.ModelType = modelType;
                var cache = DynamicTypeCache.GetTypeCache(modelType);
                var dynamicCache = DynamicTypeCache.GetTypeCache(objectType, true);
                var data = dynamic.DictionaryData;
                foreach (var pair in cache.Properties)
                {
                    var mappingName = pair.Value.Map?.Name ?? pair.Key;
                    if (!dynamicCache.Properties.ContainsKey(mappingName))
                    {
                        var value = ObjectMapping.Base.TypeConverter.ConvertFromModelObject(pair.Value.PropertyType,
                            pair.Value.Get?.Invoke(model));

                        if (value == null)
                        {
                            if (withNullReset && data.ContainsKey(mappingName))
                            {
                                data.Remove(mappingName);
                            }
                            continue;
                        }

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
                var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);
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
                var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);
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

        private void UpdateEntityItem(TDynamic dynamic, TEntity entity)
        {
            if (dynamic == null || entity == null)
                return;
            if (dynamic.IdObjectType != entity.IdObjectType)
            {
                entity.OptionTypes = FilterByType(dynamic.IdObjectType);
            }
            if (entity.OptionTypes == null)
            {
                throw new ApiException($"UpdateEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
            }
            FillEntityOptions(dynamic, entity.OptionTypes, entity);
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
        }

        private static TEntity ToEntityItem(TDynamic dynamic, ICollection<TOptionType> optionTypes, DateTime now)
        {
            if (dynamic == null)
                return null;
            if (optionTypes == null)
            {
                throw new ApiException($"ToEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
            }
            var entity = new TEntity
            {
                OptionValues = new List<TOptionValue>(),
                OptionTypes = optionTypes,
                MappedObject = dynamic
            };
            FillEntityOptions(dynamic, optionTypes, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = now;
            entity.DateEdited = now;
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
            if (entity.OptionTypes == null)
            {
                throw new ApiException($"FromEntityItem<{typeof(TEntity)}> have no OptionTypes, are you forgot to pass them?");
            }
            var result = new TDynamic();
            var data = result.DictionaryData;
            if (entity.OptionValues?.Count > 0)
            {
                var optionValues = entity.OptionValues.ToDictionary(v => v.IdOptionType);
                foreach (var optionType in entity.OptionTypes)
                {
                    TOptionValue value;
                    if (optionValues.TryGetValue(optionType.Id, out value))
                    {
                        try
                        {
                            data.Add(optionType.Name,
                                MapperTypeConverter.ConvertTo<TOptionValue, TOptionType>(value,
                                    (FieldType) optionType.IdFieldType));
                        }
                        catch (Exception e) when (!(e is NotImplementedException))
                        {
                            throw new ObjectConvertException(
                                $"{optionType.Name}: \"{value}\" Cannot be converted to Type: {(FieldType) optionType.IdFieldType}",
                                e);
                        }
                    }
                }
            }
            if (withDefaults)
            {
                foreach (
                    var optionType in
                    entity.OptionTypes.Where(optionType => optionType.DefaultValue != null && !data.ContainsKey(optionType.Name)))
                {
                    try
                    {
                        data.Add(optionType.Name,
                            MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                    }
                    catch (Exception e) when (!(e is NotImplementedException))
                    {
                        throw new ObjectConvertException(
                            $"{optionType.Name}: \"{optionType.DefaultValue}\" Cannot be converted to Type: {(FieldType) optionType.IdFieldType}",
                            e);
                    }
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

        private void UpdateEntityItem(DynamicEntityPair<TDynamic, TEntity> pair)
        {
            UpdateEntityItem(pair.Dynamic, pair.Entity);
        }

        private static void FillEntityOptions(TDynamic obj, ICollection<TOptionType> optionTypes, TEntity entity)
        {
            entity.MappedObject = obj;
            var optionIdsSet = new HashSet<int>(optionTypes.Select(o => o.Id));
            entity.OptionValues.RemoveAll(o => !optionIdsSet.Contains(o.IdOptionType));
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
                        (FieldType) optionType.IdFieldType);
                }
                else
                {
                    if (value == null)
                        continue;

                    option = new TOptionValue();
                    MapperTypeConverter.ConvertToOption<TOptionValue, TOptionType>(option, value,
                        (FieldType) optionType.IdFieldType);
                    option.IdOptionType = optionType.Id;
                    entity.OptionValues.Add(option);
                }
            }
        }

        private ICollection<DynamicEntityPair<TDynamic, TEntity>> RemoveInvalidForUpdate(
            ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
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

        public virtual async Task<TDynamic> FromModelAsync<TModel>(TModel model, int idObjectType)
        {
            if (model == null)
                return null;

            var result = CreatePrototype(idObjectType);

            await FromModelInternal(result, model, typeof(TModel), typeof(TDynamic));
            await ConverterService.ModelToDynamicAsync(model, result);
            return result;
        }

        public virtual async Task UpdateObjectAsync<TModel>(TModel model, TDynamic obj, int idObjectType, bool loadDefaults = true)
            where TModel : class, new()
        {

            if (obj != null)
                obj.IdObjectType = idObjectType;

            if (loadDefaults)
            {
                UpdateObjectWithDefaults(obj, idObjectType);
            }
            await UpdateObjectAsync(model, obj);
        }

        private void UpdateObjectWithDefaults(TDynamic obj, int idObjectType)
        {
            var defaultObject = CreatePrototype(idObjectType);
            var objData = obj.DictionaryData;
            foreach (var value in defaultObject.DictionaryData)
            {
                if (objData.ContainsKey(value.Key))
                {
                    objData[value.Key] = value.Value;
                }
                else
                {
                    objData.Add(value);
                }
            }
        }

        public virtual async Task<TModel> CreatePrototypeForAsync<TModel>() where TModel : class, new()
        {
            return await ToModelAsync<TModel>(CreatePrototype());
        }

        public virtual TDynamic CreatePrototype(int idObjectType)
        {
            var result = new TDynamic {IdObjectType = idObjectType};
            var optionTypes = FilterByType(idObjectType);
            var data = result.DictionaryData;
            foreach (var optionType in optionTypes.Where(optionType => optionType.DefaultValue != null))
            {
                try
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                }
                catch (Exception e) when (!(e is NotImplementedException))
                {
                    throw new ObjectConvertException(
                        $"{optionType.Name}: \"{optionType.DefaultValue}\" Cannot be converted to Type: {(FieldType) optionType.IdFieldType}",
                        e);
                }
            }
            return result;
        }

        public virtual TDynamic CreatePrototype()
        {
            var result = new TDynamic();
            var optionTypes = FilterByType(null);
            var data = result.DictionaryData;
            foreach (var optionType in optionTypes.Where(optionType => optionType.DefaultValue != null))
            {
                try
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                }
                catch (Exception e) when (!(e is NotImplementedException))
                {
                    throw new ObjectConvertException(
                        $"{optionType.Name}: \"{optionType.DefaultValue}\" Cannot be converted to Type: {(FieldType) optionType.IdFieldType}",
                        e);
                }
            }
            return result;
        }

        public virtual TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new()
        {
            return CreatePrototypeForAsync<TModel>(idObjectType).GetAwaiter().GetResult();
        }

        public virtual TModel CreatePrototypeFor<TModel>() where TModel : class, new()
        {
            return CreatePrototypeForAsync<TModel>().GetAwaiter().GetResult();
        }

        public virtual async Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new()
        {
            return await ToModelAsync<TModel>(CreatePrototype(idObjectType));
        }
    }
}