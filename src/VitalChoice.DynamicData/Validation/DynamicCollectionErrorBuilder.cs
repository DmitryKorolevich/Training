using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class DynamicCollectionErrorBuilder<TCollection, TProperty> : ErrorBuilderBase<TCollection>, IDynamicCollectionErrorBuilder<TCollection, TProperty>, IErrorResult
        where TProperty: class, IModelTypeContainer
        where TCollection : ICollection<TProperty>
    {
        public DynamicCollectionErrorBuilder(TCollection obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
        }

        public IErrorResult Property<TEntity, TPropertyResult>(
            Expression<Func<TProperty, TPropertyResult>> propertySelector, ICollection<TEntity> values,
            Expression<Func<TEntity, TPropertyResult>> valueSelector)
        {
            Expression fieldSelector = propertySelector;
            // ReSharper disable once UseNullPropagation
            if (fieldSelector is LambdaExpression)
            {
                fieldSelector = ((LambdaExpression) fieldSelector).Body;
            }
            if (fieldSelector.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression member = (MemberExpression) fieldSelector;
                var dynamicFieldName = member.Member.Name;
                var modelType = Data.FirstOrDefault()?.ModelType;
                var modelFieldName = GetModelName(dynamicFieldName, modelType);
                var indexes = CompareCollections(Data, propertySelector, values, valueSelector);
                Indexes = indexes.ToArray();
                PropertyName = modelFieldName;
                return this;
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}