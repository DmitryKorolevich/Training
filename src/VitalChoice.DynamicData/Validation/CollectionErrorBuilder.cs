using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class CollectionErrorBuilder<TCollection, TProperty> : ErrorBuilderBase<TCollection>, ICollectionErrorBuilder<TCollection, TProperty>
        where TProperty : class
        where TCollection : ICollection<TProperty>
    {
        public CollectionErrorBuilder(TCollection obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
        }

        public IErrorResult<TProperty> Property<TEntity, TPropertyResult>(
            Expression<Func<TProperty, TPropertyResult>> propertySelector, ICollection<TEntity> values,
            Expression<Func<TEntity, TPropertyResult>> valueSelector)
        {
            Expression fieldSelector = propertySelector;
            // ReSharper disable once UseNullPropagation
            if (fieldSelector is LambdaExpression)
            {
                fieldSelector = ((LambdaExpression)fieldSelector).Body;
            }
            if (fieldSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression)fieldSelector;
                var dynamicFieldName = member.Member.Name;
                var indexes = CompareCollections(Data, propertySelector, values, valueSelector);
                return new ErrorResult<TProperty>(CollectionName, indexes.ToArray(), dynamicFieldName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}