using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac.Features.Indexed;
using Templates.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Services
{
    public abstract class DynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IIndex<Type, IDynamicToModelMapper> _mappers;
        private readonly IModelToDynamicContainer _container;

        protected DynamicObjectMapper(IIndex<Type, IDynamicToModelMapper> mappers, IModelToDynamicContainer container)
        {
            _mappers = mappers;
            _container = container;
        }

        protected abstract void FromEntity(TDynamic dynamic, TEntity entity, bool withDefaults = false);
        protected abstract void UpdateEntityInternal(TDynamic dynamic, TEntity entity);
        protected abstract void FillNewEntity(TDynamic dynamic, TEntity entity);

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            TDynamic result = new TDynamic();
            if (entity == null)
                return null;

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
                                (FieldType) optionType.IdFieldType));
                    }
                }
            }
            result.Id = entity.Id;
            result.DateCreated = entity.DateCreated;
            result.DateEdited = entity.DateEdited;
            result.StatusCode = entity.StatusCode;
            result.IdEditedBy = entity.IdEditedBy;
            if (withDefaults && entity.OptionTypes != null)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name,
                        MapperTypeConverter.ConvertTo(optionType.DefaultValue, (FieldType) optionType.IdFieldType));
                }
            }
            FromEntity(result, entity, withDefaults);
            return result;
        }

        public TModel ToModel<TModel>(TDynamic dynamic)
            where TModel : class, new()
        {
            if (dynamic == null)
                return null;

            var result = new TModel();
            ToModelInternal(dynamic, result, typeof (TModel), typeof (TDynamic));
            var converter = _container.TryResolve<TModel, TDynamic>();
            converter?.DynamicToModel(result, dynamic);
            return result;
        }

        public TDynamic FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelInternal(result, model, typeof (TModel), typeof (TDynamic));
            var converter = _container.TryResolve<TModel, TDynamic>();
            converter?.ModelToDynamic(model, result);
            return result;
        }

        object IDynamicToModelMapper.ToModel(dynamic dynamic, Type modelType)
        {
            if (dynamic == null)
                return null;

            dynamic result = Activator.CreateInstance(modelType);
            ToModelInternal(dynamic, result, modelType,
                typeof (TDynamic));
            dynamic converter = _container.TryResolve(modelType);
            converter?.DynamicToModel(result, dynamic);
            return result;
        }

        MappedObject IDynamicToModelMapper.FromModel(Type modelType, dynamic model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelInternal(result, model, modelType, typeof (TDynamic));
            dynamic converter = _container.TryResolve(modelType);
            converter?.ModelToDynamic(model, result);
            return result;
        }

        public void UpdateEntity(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.OptionTypes == null)
                throw new ArgumentException("OptionTypes collection is null");

            var optionTypesCache = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
            entity.OptionValues = new List<TOptionValue>();

            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            UpdateEntityInternal(dynamic, entity);
        }

        public TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes)
        {
            if (dynamic == null)
                return null;

            if (optionTypes == null)
                throw new ArgumentNullException(nameof(optionTypes));

            var optionTypesCache = optionTypes.ToDictionary(o => o.Name, o => o);
            var entity = new TEntity {OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes};
            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            FillNewEntity(dynamic, entity);
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
                    var value = ConvertToModelObject(pair.Value.PropertyType, dynamicProperty.Get?.Invoke(dynamic));
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
                        var value = ConvertToModelObject(pair.Value.PropertyType, dynamicValue);
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
                    var value = ConvertFromModelObject(pair.Value.PropertyType, dynamicProperty.PropertyType,
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
                    var value = ConvertFromModelObject(pair.Value.PropertyType, pair.Value.Get?.Invoke(model));
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

        private static object ConvertFromModelObject(Type sourceType, object obj)
        {
            if (obj == null)
                return null;

            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (srcElementType != null)
            {
                IList results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(srcElementType));
                results.AddRange(obj as IEnumerable);
                return results;
            }
            return obj;
        }

        private object ConvertFromModelObject(Type sourceType, Type destType, object obj)
        {
            if (obj == null)
                return null;

            if (typeof(MappedObject).IsAssignableFrom(destType))
            {
                var mapper = _mappers[destType];
                return mapper.FromModel(sourceType, obj);
            }
            if (destType.IsInstanceOfType(obj))
            {
                return obj;
            }

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                IList results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                if (typeof(MappedObject).IsAssignableFrom(destElementType))
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        var mapper = _mappers[destElementType];
                        var dynamicObject = mapper.FromModel(srcElementType, item);
                        results.Add(dynamicObject);
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }

                return results;
            }
            return null;
        }

        private object ConvertToModelObject(Type destType, object obj)
        {
            if (obj == null)
                return null;
            var mappedObject = obj as MappedObject;
            if (mappedObject != null)
            {
                var mapper = _mappers[obj.GetType()];
                return mapper.ToModel(mappedObject, destType);
            }
            if (destType.IsInstanceOfType(obj))
            {
                return obj;
            }

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = obj.GetType().TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                var results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                if (typeof(MappedObject).IsAssignableFrom(srcElementType))
                {
                    var mapper = _mappers[srcElementType];
                    foreach (MappedObject item in (IEnumerable) obj)
                    {
                        results.Add(mapper.ToModel(item, destElementType));
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }
                return results;
            }
            return null;
        }
    }
}