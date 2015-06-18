using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicInterfaces;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.DynamicData.Helpers
{
    //public class ErrorBuilder<TDynamic>
    //    where TDynamic: class, IDynamicObject
    //{
    //    public ErrorBuilder()
    //    {
            
    //    }

    //    private object obj;

    //    public ErrorBuilder<TCollectionItem> Collection<TEntity, TOptionValue, TOptionType, TCollectionItem>(
    //        ErrorBuilder<TDynamic> builder,
    //        Expression<Func<TDynamic, ICollection<TCollectionItem>>> collectionExpression,
    //        Expression<Func<TCollectionItem, object>> fieldExpression, int index, string error)
    //        where TOptionType : OptionType, new()
    //        where TOptionValue : OptionValue<TOptionType>, new()
    //        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //        where TCollectionItem : class, IDynamicObject
    //    {
            
    //    }
    //}

    //public static class RaiseErrorExtension
    //{
    //    public static void RaiseValidationError<TDynamic, TEntity, TOptionValue, TOptionType, TCollectionItem>(
    //        this TDynamic obj,
    //        Expression<Func<TDynamic, ICollection<TCollectionItem>>> collectionExpression,
    //        Expression<Func<TCollectionItem, object>> fieldExpression, int index, string error)
    //        where TDynamic : DynamicObject<TEntity, TOptionValue, TOptionType>
    //        where TOptionType : OptionType, new()
    //        where TOptionValue : OptionValue<TOptionType>, new()
    //        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //        where TCollectionItem: class, IDynamicObject
    //    {
    //        if (obj == null)
    //            throw new ArgumentNullException(nameof(obj));
    //        if (obj.ModelType == null)
    //            throw new ArgumentException("Dynamic object never constructed from model or converted onto it");

    //        Expression collectionSelector = collectionExpression;
    //        string modelCollectionName = string.Empty;
    //        string dynamicCollectionName = string.Empty;
    //        // ReSharper disable once UseNullPropagation
    //        if (collectionSelector is LambdaExpression)
    //        {
    //            collectionSelector = ((LambdaExpression) collectionSelector).Body;
    //        }
    //        if (collectionSelector.NodeType == ExpressionType.MemberAccess)
    //        {
    //            MemberExpression member = (MemberExpression) collectionSelector;
    //            dynamicCollectionName = member.Member.Name;
    //            Dictionary<string, GenericProperty> cache;
    //            if (DynamicObject<TEntity, TOptionValue, TOptionType>.ModelTypeMappingCache.TryGetValue(obj.ModelType,
    //                out cache))
    //            {
    //                var modelCollection = cache.FirstOrDefault(c => c.Value.Map.Name == dynamicCollectionName);
    //                modelCollectionName = modelCollection.Value.Map.Name ?? modelCollection.Key;
    //            }
    //        }
    //        Expression fieldSelector = fieldExpression;
    //        // ReSharper disable once UseNullPropagation
    //        if (fieldSelector is LambdaExpression)
    //        {
    //            fieldSelector = ((LambdaExpression)fieldSelector).Body;
    //        }
    //        if (fieldSelector.NodeType == ExpressionType.MemberAccess && !string.IsNullOrEmpty(dynamicCollectionName))
    //        {
    //            MemberExpression member = (MemberExpression)fieldSelector;
    //            var dynamicFieldName = member.Member.Name;
    //            var innerCollectionValue =
    //                DynamicObject<TEntity, TOptionValue, TOptionType>.DynamicTypeMappingCache[typeof (TDynamic)][
    //                    dynamicCollectionName].Get?.Invoke(obj) as ICollection<TCollectionItem>;
    //            dynamic innerItem = innerCollectionValue?.Skip(index).FirstOrDefault();
    //            if (innerItem != null)
    //            {
    //                Dictionary<string, GenericProperty> cache;
    //                if (
    //                    DynamicObject<TEntity, TOptionValue, TOptionType>.ModelTypeMappingCache.TryGetValue(
    //                        innerItem.ModelType,
    //                        out cache))
    //                {
    //                    var modelField = cache.FirstOrDefault(c => c.Value.Map.Name == dynamicFieldName);
    //                    var fieldName = modelField.Value.Map.Name ?? modelField.Key;
    //                    throw new ApiValidationException(CollectionFormProperty.GetFullName(modelCollectionName, index, fieldName), error);
    //                }
    //            }
    //        }
    //    }

    //    public static void RaiseValidationError<TEntity, TOptionValue, TOptionType, TCollectionItem>(
    //        this DynamicObject<TEntity, TOptionValue, TOptionType> obj,
    //        Expression<Func<DynamicObject<TEntity, TOptionValue, TOptionType>, ICollection<TCollectionItem>>>
    //            collectionExpression, IEnumerable<int> indexes, Expression<Func<TCollectionItem, object>> fieldExpression, string error)
    //        where TOptionType : OptionType, new()
    //        where TOptionValue : OptionValue<TOptionType>, new()
    //        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static void RaiseValidationError<TEntity, TOptionValue, TOptionType>(
    //        this DynamicObject<TEntity, TOptionValue, TOptionType> obj,
    //        string modelCollectionName, int index, string modelFieldName, string error)
    //        where TOptionType : OptionType, new()
    //        where TOptionValue : OptionValue<TOptionType>, new()
    //        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static void RaiseValidationError<TEntity, TOptionValue, TOptionType>(
    //        this DynamicObject<TEntity, TOptionValue, TOptionType> obj,
    //        string modelCollectionName, IEnumerable<int> indexes, string modelFieldName, string error)
    //        where TOptionType : OptionType, new()
    //        where TOptionValue : OptionValue<TOptionType>, new()
    //        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}