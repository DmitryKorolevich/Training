using System;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface ITypeConverter
    {
        object ConvertFromModel(Type sourceType, Type destType, object obj);
        object ConvertToModel(Type sourceType, Type destType, object obj);
        object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone);
    }
}