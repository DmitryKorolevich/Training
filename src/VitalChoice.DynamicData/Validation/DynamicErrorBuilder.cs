using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class DynamicErrorBuilder<TProperty> : ErrorBuilderBase<TProperty>, IDynamicErrorBuilder<TProperty>
        where TProperty: class, IDynamicObject
    {
        public DynamicErrorBuilder(TProperty obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {

        }

        public IErrorResult<TProperty> Property(
            Expression<Func<TProperty, object>> propertyExpression)
        {
            Expression fieldSelector = propertyExpression;
            // ReSharper disable once UseNullPropagation
            if (fieldSelector is LambdaExpression)
            {
                fieldSelector = ((LambdaExpression)fieldSelector).Body;
            }
            if (fieldSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression)fieldSelector;
                var dynamicFieldName = member.Member.Name;
                var modelName = DynamicTypeCache.ModelTypeMappingCache[Data.ModelType].FirstOrDefault(
                    c => c.Value.Map.Name == dynamicFieldName).Value.Map.Name ?? dynamicFieldName;
                return new ErrorResult<TProperty>(CollectionName, Indexes, modelName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public IDynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression) 
            where TResultProperty : class, IDynamicObject
        {
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression) collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) collectionSelector;
                var dynamicCollectionName = member.Member.Name;
                Dictionary<string, GenericProperty> cache;
                if (DynamicTypeCache.ModelTypeMappingCache.TryGetValue(Data.ModelType,
                    out cache))
                {
                    var modelCollection = cache.FirstOrDefault(c => c.Value.Map.Name == dynamicCollectionName);
                    var modelCollectionName = modelCollection.Value.Map.Name ?? modelCollection.Key;
                    var innerCollectionValue =
                        DynamicTypeCache.DynamicTypeMappingCache[typeof (TProperty)][
                            dynamicCollectionName].Get?.Invoke(Data) as ICollection<TResultProperty>;
                    return new DynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty>(innerCollectionValue, modelCollectionName);
                }
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index) 
            where TResultProperty : class, IDynamicObject
        {
            return Collection(collectionExpression, new[] {index});
        }

        public IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes) 
            where TResultProperty : class, IDynamicObject
        {
            Expression collectionSelector = collectionExpression;
            // ReSharper disable once UseNullPropagation
            if (collectionSelector is LambdaExpression)
            {
                collectionSelector = ((LambdaExpression)collectionSelector).Body;
            }
            if (collectionSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression)collectionSelector;
                var dynamicCollectionName = member.Member.Name;
                Dictionary<string, GenericProperty> cache;
                if (DynamicTypeCache.ModelTypeMappingCache.TryGetValue(Data.ModelType,
                    out cache))
                {
                    var modelCollection = cache.FirstOrDefault(c => c.Value.Map.Name == dynamicCollectionName);
                    var modelCollectionName = modelCollection.Value.Map.Name ?? modelCollection.Key;
                    var innerCollectionValue =
                        DynamicTypeCache.DynamicTypeMappingCache[typeof(TProperty)][
                            dynamicCollectionName].Get?.Invoke(Data) as ICollection<TResultProperty>;
                    var itemIndexes = indexes.ToArray();
                    TResultProperty innerItem = innerCollectionValue?.Skip(itemIndexes.FirstOrDefault()).FirstOrDefault();
                    return new DynamicErrorBuilder<TResultProperty>(innerItem, modelCollectionName, itemIndexes);
                }
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}