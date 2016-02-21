using System;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface ITypeConverter
    {
        object ConvertFromModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        object ConvertToModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
    }
}