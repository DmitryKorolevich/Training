using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConvertWithAttribute : Attribute
    {
        public ConvertWithAttribute(Type converterType)
        {
            ConverterType = converterType;
        }

        public ConvertWithAttribute(Type converterType, object defaultValue)
        {
            ConverterType = converterType;
            Default = defaultValue;
        }

        public Type ConverterType { get; }
        public object Default { get; }
    }
}