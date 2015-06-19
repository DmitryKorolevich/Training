using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface ICollectionErrorBuilder<out TCollection, TProperty> : IDataContainer<TCollection> 
        where TProperty : class
        where TCollection : ICollection<TProperty>
    {
        IErrorResult<TProperty> Property<T>(
            Expression<Func<TProperty, object>> propertySelector, ICollection<T> values,
            Expression<Func<T, object>> valueSelector);
    }
}