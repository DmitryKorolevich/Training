using System;
using System.Collections;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public interface IGenericCollection: IEnumerable
    {
        object CollectionObject { get; }
        void CopyTo(Array array, int index);
        int Count { get; }
        int Add(object value);
        bool Contains(object value);
        void Clear();
        void Remove(object value);
        bool IsReadOnly { get; }
    }
}