using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IDynamicCollectionErrorBuilder<out TCollection, TProperty> : IDataContainer<TCollection>
        where TProperty : class, IDynamicObject
        where TCollection : ICollection<TProperty>
    {
        IErrorResult<TProperty> Property<T>(
            Expression<Func<TProperty, object>> propertySelector, ICollection<T> values,
            Expression<Func<T, object>> valueSelector);
    }
}