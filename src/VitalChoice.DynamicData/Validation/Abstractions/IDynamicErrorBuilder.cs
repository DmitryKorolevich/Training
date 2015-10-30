using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IDynamicErrorBuilder<TProperty> : IDataContainer<TProperty>
        where TProperty : class, IModelType
    {
        IErrorResult Property(
            Expression<Func<TProperty, object>> propertyExpression);

        IErrorResult Property(string propertyName);

        IDynamicCollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression)
            where TResultProperty : class, IModelType;

        IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index)
            where TResultProperty : class, IModelType;

        IDynamicErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes)
            where TResultProperty : class, IModelType;
    }
}