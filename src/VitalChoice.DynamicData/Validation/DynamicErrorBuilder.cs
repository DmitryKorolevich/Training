using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class DynamicErrorBuilder<TProperty> : ErrorBuilderBase<TProperty>, IDynamicErrorBuilder<TProperty>
        where TProperty: class, IModelTypeContainer
    {
        public DynamicErrorBuilder(TProperty obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (obj.ModelType == null)
                throw new ArgumentException($"{obj} didn't have ModelType set, seems object was never created from model");
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
                var modelName = GetModelName(dynamicFieldName, Data.ModelType);
                return new ErrorResult<TProperty>(CollectionName, Indexes, modelName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public IDynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression) 
            where TResultProperty : class, IModelTypeContainer
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

                var modelCollectionName = GetModelName(dynamicCollectionName, Data.ModelType);
                var innerCollectionValue =
                    DynamicTypeCache.DynamicTypeMappingCache[typeof (TProperty)][
                        dynamicCollectionName].Get?.Invoke(Data) as ICollection<TResultProperty>;
                return
                    new DynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty>(
                        innerCollectionValue, modelCollectionName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }

        public IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index) 
            where TResultProperty : class, IModelTypeContainer
        {
            return Collection(collectionExpression, new[] {index});
        }

        public IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes)
            where TResultProperty : class, IModelTypeContainer
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
                var modelCollectionName = GetModelName(dynamicCollectionName, Data.ModelType);
                var innerCollectionValue =
                    DynamicTypeCache.DynamicTypeMappingCache[typeof (TProperty)][
                        dynamicCollectionName].Get?.Invoke(Data) as ICollection<TResultProperty>;
                var itemIndexes = indexes.ToArray();
                TResultProperty innerItem = innerCollectionValue?.Skip(itemIndexes.FirstOrDefault()).FirstOrDefault();
                return new DynamicErrorBuilder<TResultProperty>(innerItem, modelCollectionName, itemIndexes);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}