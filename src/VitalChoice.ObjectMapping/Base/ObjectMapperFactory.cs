using System;
using System.Linq;
using System.Reflection;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.ObjectMapping.Base
{
    public class ObjectMapperFactory : IObjectMapperFactory
    {
        private readonly ITypeConverter _typeConverter;
        private readonly IModelConverterService _converterService;

        public ObjectMapperFactory(ITypeConverter typeConverter, IModelConverterService converterService)
        {
            _typeConverter = typeConverter;
            _converterService = converterService;
        }

        public IObjectMapper CreateMapper(Type objectType)
        {
            if (!objectType.GetTypeInfo().IsClass ||
                objectType.GetConstructors().All(c => c.GetParameters().Length > 0))
            {
                throw new InvalidOperationException("Mapped object type should be a class and have parameterless constructor");
            }
            var mapperType = typeof (ObjectMapper<>).MakeGenericType(objectType);
            return (IObjectMapper) Activator.CreateInstance(mapperType, _typeConverter, _converterService);
        }

        public IObjectMapper<T> CreateMapper<T>()
            where T : class, new()
        {
            return new ObjectMapper<T>(_typeConverter, _converterService);
        }
    }
}