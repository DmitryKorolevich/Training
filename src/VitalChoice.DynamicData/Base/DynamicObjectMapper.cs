using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Features.Indexed;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;

namespace VitalChoice.DynamicData.Base
{
    public abstract class DynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IIndex<Type, IModelToDynamicConverter> _converters;
        private readonly IReadRepositoryAsync<TOptionType> _optionTypeRepositoryAsync;
        private readonly ModelTypeConverter _typeConverter;

        protected abstract Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items, bool withDefaults = false);
        protected abstract Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        protected abstract Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<TDynamic, TEntity>> items);
        public abstract Expression<Func<TOptionValue, int?>> ObjectIdSelector { get; }

        protected DynamicObjectMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> converters,
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync)
        {
            _converters = converters;
            _optionTypeRepositoryAsync = optionTypeRepositoryAsync;
            _typeConverter = new ModelTypeConverter(mappers);
        }

        public virtual IQueryOptionType<TOptionType> GetOptionTypeQuery()
        {
            return new OptionTypeQuery<TOptionType>();
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
            ICollection<DynamicEntityPair<TDynamic, TEntity>> results;
            if (optionTypes == null)
            {
                optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                results =
                    items.Select(
                        dynamic =>
                            new DynamicEntityPair<TDynamic, TEntity>(dynamic,
                                ToEntityItem(dynamic,
                                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(dynamic.IdObjectType).Query().Compile())
                                        .ToList())))
                        .ToList();
            }
            else
            {
                results =
                    items.Select(
                        dynamic => new DynamicEntityPair<TDynamic, TEntity>(dynamic, ToEntityItem(dynamic, optionTypes)))
                        .ToList();
            }
            ToEntityRangeInternalAsync(results).Wait();
            return results.Select(r => r.Entity).ToList();
        }

        public List<TEntity> ToEntityRange(ICollection<GenericPair<TDynamic, ICollection<TOptionType>>> items)
        {
            ICollection<TOptionType> optionTypes = null;
            foreach (var pair in items.Where(pair => pair.Value2 == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = _optionTypeRepositoryAsync.Query().Select(false);
                }
                pair.Value2 = optionTypes.Where(GetOptionTypeQuery().WithObjectType(pair.Value1.IdObjectType).Query().Compile()).ToList();
            }
            var results =
                items.Select(
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
                entity.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(entity.IdObjectType).Query().Compile()).ToList();
            }
            List<DynamicEntityPair<TDynamic, TEntity>> results =
                items.Select(
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
        }

        public TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new()
        {
            if (dynamic == null)
                return null;

            var result = new TModel();
            ToModelItem(dynamic, result, typeof (TModel), typeof (TDynamic));
            
            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(typeof (TModel), out conv))
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
            if (_converters.TryGetValue(typeof (TModel), out conv))
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
            if (_converters.TryGetValue(modelType, out conv))
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
            if (_converters.TryGetValue(modelType, out conv))
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
        }

        public async void UpdateEntityRange(ICollection<DynamicEntityPair<TDynamic, TEntity>> items)
        {
            if (items == null)
                return;

            ICollection<TOptionType> optionTypes = null;
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                pair.Entity.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(pair.Dynamic.IdObjectType).Query().Compile()).ToList();
            }
            foreach (var pair in items)
            {
                UpdateEntityItem(pair);
            }
            
            await UpdateEntityRangeInternalAsync(items);
        }

        private static void UpdateEntityItem(TDynamic dynamic, TEntity entity)
        {
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
                entity.OptionTypes =
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery().WithObjectType(entity.IdObjectType)).SelectAsync(false);
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
            foreach (var pair in items.Where(pair => pair.Entity.OptionTypes == null))
            {
                if (optionTypes == null)
                {
                    optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                }
                pair.Entity.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(pair.Dynamic.IdObjectType).Query().Compile()).ToList();
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
            if (optionTypes == null)
            {
                optionTypes = await _optionTypeRepositoryAsync.Query().SelectAsync(false);
                results =
                    items.Select(
                        dynamic =>
                            new DynamicEntityPair<TDynamic, TEntity>(dynamic,
                                ToEntityItem(dynamic,
                                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(dynamic.IdObjectType).Query().Compile())
                                        .ToList())))
                        .ToList();
            }
            else
            {
                results =
                    items.Select(
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
                pair.Value2 = optionTypes.Where(GetOptionTypeQuery().WithObjectType(pair.Value1.IdObjectType).Query().Compile()).ToList();
            }
            var results =
                items.Select(
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
                entity.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery().WithObjectType(entity.IdObjectType).Query().Compile()).ToList();
            }
            List<DynamicEntityPair<TDynamic, TEntity>> results =
                items.Select(
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

        private async Task FromEntityInternalAsync(TDynamic dynamic, TEntity entity, bool withDefaults = false)
        {
            await
                FromEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                }, withDefaults);
        }

        private async Task UpdateEntityInternalAsync(TDynamic dynamic, TEntity entity)
        {
            await
                UpdateEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                });
        }

        private async Task ToEntityInternalAsync(TDynamic dynamic, TEntity entity)
        {
            await
                ToEntityRangeInternalAsync(new List<DynamicEntityPair<TDynamic, TEntity>>
                {
                    new DynamicEntityPair<TDynamic, TEntity>(dynamic, entity)
                });
        }
    }
}