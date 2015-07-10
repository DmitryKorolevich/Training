using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Domain.Exceptions;
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

        public IErrorResult Property<TEntity, TPropertyResult>(
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
                Indexes = CompareCollections(Data, propertySelector, values, valueSelector).ToArray();
                PropertyName = dynamicFieldName;
                return this;
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}