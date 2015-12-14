using System;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface ITypeConverter
    {
        object ConvertFromModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        object ConvertToModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone);
    }
}