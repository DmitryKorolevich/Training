using System;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ObjectMapping
{
    public struct GenericProperty
    {
        public bool IsCollection;
        public Type CollectionItemType;
        public Type PropertyType;
        public Func<object> GetDefaultValue;
        public Func<object, object> Get;
        public Action<object, object> Set;
        public MapAttribute Map;
        public ConvertWithAttribute Converter;
    }
}