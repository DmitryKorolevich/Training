using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface ICollectionErrorBuilder<out TCollection, TProperty> : IDataContainer<TCollection> 
        where TProperty : class
        where TCollection : ICollection<TProperty>
    {
        IErrorResult<TProperty> Property<T, TPropertyResult>(
            Expression<Func<TProperty, TPropertyResult>> propertySelector, ICollection<T> values,
            Expression<Func<T, TPropertyResult>> valueSelector);
    }
}