using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IDynamicErrorBuilder<TProperty> : IDataContainer<TProperty>
        where TProperty : class, IDynamicObject
    {
        IErrorResult<TProperty> Property(
            Expression<Func<TProperty, object>> propertyExpression);

        IDynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression)
            where TResultProperty : class, IDynamicObject;

        IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index)
            where TResultProperty : class, IDynamicObject;

        IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes)
            where TResultProperty : class, IDynamicObject;
    }
}