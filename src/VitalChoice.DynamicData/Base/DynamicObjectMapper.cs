using System;
using System.Collections.Generic;
using System.Linq;
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

        protected DynamicObjectMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> converters,
            IReadRepositoryAsync<TOptionType> optionTypeRepositoryAsync)
        {
            _converters = converters;
            _optionTypeRepositoryAsync = optionTypeRepositoryAsync;
            _typeConverter = new ModelTypeConverter(mappers);
        }

        public abstract IQueryObject<TOptionType> GetOptionTypeQuery(int? idType);
        protected abstract void FromEntityInternal(TDynamic dynamic, TEntity entity, bool withDefaults = false);
        protected abstract void UpdateEntityInternal(TDynamic dynamic, TEntity entity);
        protected abstract void ToEntityInternal(TDynamic dynamic, TEntity entity);

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            TDynamic result = new TDynamic();
            if (entity == null)
                return null;

            var data = result.DictionaryData;
            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    _optionTypeRepositoryAsync.Query(GetOptionTypeQuery(entity.IdObjectType)).Select(false);
            }
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
                                (FieldType) optionType.IdFieldType));
                    }
                }
            }
            result.Id = entity.Id;
            result.DateCreated = entity.DateCreated;
            result.DateEdited = entity.DateEdited;
            result.StatusCode = entity.StatusCode;
            result.IdEditedBy = entity.IdEditedBy;
            result.IdObjectType = entity.IdObjectType;
            if (withDefaults && entity.OptionTypes != null)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                }
            }
            FromEntityInternal(result, entity, withDefaults);
            return result;
        }

        public void UpdateEntityRange(ICollection<Pair<TDynamic, TEntity>> items)
        {
            foreach (var item in items)
            {
                UpdateEntity(item.FirstValue, item.SecondValue);
            }
        }

        public List<TEntity> ToEntityRange(ICollection<TDynamic> items, ICollection<TOptionType> optionTypes = null)
        {
            return items.Select(i => ToEntity(i, optionTypes)).ToList();
        }

        public List<TDynamic> FromEntityRange(ICollection<TEntity> items, bool withDefaults = false)
        {
            return items.Select(i => FromEntity(i, withDefaults)).ToList();
        }

        public TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new()
        {
            if (dynamic == null)
                return null;

            var result = new TModel();
            ToModelInternal(dynamic, result, typeof (TModel), typeof (TDynamic));
            
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
            FromModelInternal(result, model, typeof (TModel), typeof (TDynamic));

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
            ToModelInternal(dynamic, result, modelType,
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
            FromModelInternal(result, model, modelType, typeof (TDynamic));

            IModelToDynamicConverter conv;
            if (_converters.TryGetValue(modelType, out conv))
            {
                dynamic converter = conv;
                converter?.ModelToDynamic(model, result);
            }
            return result;
        }

        public void UpdateEntity(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.OptionTypes == null)
            {
                entity.OptionTypes =
                    _optionTypeRepositoryAsync.Query(GetOptionTypeQuery(entity.IdObjectType)).Select(false);
            }

            var optionTypesCache = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
            entity.OptionValues = new List<TOptionValue>();

            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            entity.IdObjectType = dynamic.IdObjectType;
            UpdateEntityInternal(dynamic, entity);
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
                    await _optionTypeRepositoryAsync.Query(GetOptionTypeQuery(dynamic.IdObjectType)).SelectAsync(false);
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
            ToEntityInternal(dynamic, entity);
            return entity;
        }

        private void ToModelInternal(MappedObject dynamic, object result,
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

        private void FromModelInternal(MappedObject dynamic, object model,
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
    }
}