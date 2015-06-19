using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public class DynamicCollectionErrorBuilder<TCollection, TProperty> : ErrorBuilderBase<TCollection>, IDynamicCollectionErrorBuilder<TCollection, TProperty> 
        where TProperty: class, IDynamicObject
        where TCollection : ICollection<TProperty>
    {
        public DynamicCollectionErrorBuilder(TCollection obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null) : base(obj, collectionName, indexes, propertyName, error)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
        }

        public IErrorResult<TProperty> Property<TEntity>(
            Expression<Func<TProperty, object>> propertySelector, ICollection<TEntity> values,
            Expression<Func<TEntity, object>> valueSelector)
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
                if (modelType == null)
                    throw new ArgumentException("There are no objects to enumerate or object didn't ModelType");

                var modelFieldName = GetModelName(dynamicFieldName, modelType);
                var getter = propertySelector.Compile();
                var entityGetter = valueSelector.Compile();
                var valueSet = new HashSet<object>(values.Select(v => entityGetter.Invoke(v)));
                int index = 0;
                List<int> indexes = new List<int>();
                foreach (var item in Data)
                {
                    if (valueSet.Contains(getter.Invoke(item)))
                    {
                        indexes.Add(index);
                    }
                    index++;
                }
                return new ErrorResult<TProperty>(CollectionName, indexes.ToArray(), modelFieldName);
            }
            throw new ArgumentException("collectionExpression should contain member access expression");
        }
    }
}