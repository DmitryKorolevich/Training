using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public struct GenericCollection<T>: IGenericCollection
    {
        public object CollectionObject => _obj;

        private readonly ICollection<T> _obj;

        public GenericCollection(object collection)
        {
            _obj = (ICollection<T>) collection;
        }

        public IEnumerator GetEnumerator()
        {
            return _obj.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            _obj.CopyTo((T[]) array, index);
        }

        public int Count => _obj.Count;

        public int Add(object value)
        {
            _obj.Add((T) value);
            return _obj.Count - 1;
        }

        public bool Contains(object value)
        {
            return _obj.Contains((T) value);
        }

        public void Clear()
        {
            _obj.Clear();
        }

        public void Remove(object value)
        {
            _obj.Remove((T) value);
        }

        public bool IsReadOnly => _obj.IsReadOnly;
    }
}
