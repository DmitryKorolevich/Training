using System;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface ITypeConverter
    {
        object ConvertFromModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        object ConvertToModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null);
        object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone);
        object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone, Func<object, object> cloneBase);
        void CloneInto(object dest, object src, Type objectType, Type baseTypeToMemberwiseClone);
        void CopyInto(object dest, object src, Type objectType);
    }
}