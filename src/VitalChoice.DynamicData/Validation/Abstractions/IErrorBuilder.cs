using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IErrorBuilder<TProperty> : IDataContainer<TProperty>
    {
        IErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, int index);

        IErrorBuilder<TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression, IEnumerable<int> indexes);

        ICollectionErrorBuilder<ICollection<TResultProperty>, TResultProperty> Collection<TResultProperty>(
            Expression<Func<TProperty, ICollection<TResultProperty>>> collectionExpression) 
            where TResultProperty : class;

        IErrorResult<TProperty> Property(Expression<Func<TProperty, object>> propertyExpression);
    }
}