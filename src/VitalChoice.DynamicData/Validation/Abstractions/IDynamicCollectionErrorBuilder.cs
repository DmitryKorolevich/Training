using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IDynamicCollectionErrorBuilder<out TCollection, TProperty> : IDataContainer<TCollection>
        where TProperty : class, IModelType
        where TCollection : ICollection<TProperty>
    {
        IErrorResult Property<T, TPropertyResult>(
            Expression<Func<TProperty, TPropertyResult>> propertySelector, ICollection<T> values,
            Expression<Func<T, TPropertyResult>> valueSelector);
    }
}