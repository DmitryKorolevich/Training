using System;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface ITypeConverter
    {
        object ConvertFromObject(Type sourceType, Type destType, object obj);
        object ConvertToObject(Type destType, object obj);
    }
}