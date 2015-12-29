using System;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ObjectMapping
{
    public struct GenericProperty
    {
        public Type PropertyType { get; set; }
        public Func<object, object> Get { get; set; }
        public Action<object, object> Set { get; set; }
        public MapAttribute Map { get; set; }
        public ConvertWithAttribute Converter { get; set; }
    }
}