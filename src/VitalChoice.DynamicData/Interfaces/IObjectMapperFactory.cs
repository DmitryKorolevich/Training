using System;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IObjectMapperFactory
    {
        IObjectMapper CreateMapper(Type objectType);

        IObjectMapper<T> CreateMapper<T>()
            where T : class, new();
    }
}