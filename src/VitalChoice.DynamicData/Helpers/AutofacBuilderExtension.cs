using System;
using System.Linq;
using System.Reflection;
using Autofac;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.DynamicData.Helpers
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterMappers(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var mapperTypes = containingAssembly
                .GetExportedTypes()
                .Where(
                    t =>
                        t.IsImplementGeneric(typeof (DynamicObjectMapper<,,,>)) && !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsGenericType);
            foreach (var mapperType in mapperTypes)
            {
                builder.RegisterType(mapperType)
                    .As(
                        typeof (IDynamicObjectMapper<,,,>).MakeGenericType(
                            mapperType.TryGetTypeArguments(typeof (IDynamicObjectMapper<,,,>))))
                    .As(
                        typeof (IDynamicToModelMapper<>).MakeGenericType(
                            mapperType.TryGetElementType(typeof (IDynamicToModelMapper<>))))
                    .AsSelf()
                    .Keyed<IDynamicToModelMapper>(mapperType.TryGetElementType(typeof (DynamicObjectMapper<,,,>)));
            }
            return builder;
        }

        public static ContainerBuilder RegisterModelConverters(this ContainerBuilder builder, Assembly containAssembly)
        {
            var converters =
                containAssembly.ExportedTypes.Where(
                    t => t.IsImplementGeneric(typeof (IModelToDynamicConverter<,>)) && !t.GetTypeInfo().IsAbstract);
            foreach (var converter in converters)
            {
                var types = converter.TryGetTypeArguments(typeof (IModelToDynamicConverter<,>));
                if (types != null && types.Length == 2)
                {
                    builder.RegisterType(converter)
                        .As(
                            typeof (IModelToDynamicConverter<,>).MakeGenericType(
                                converter.TryGetTypeArguments(typeof (IModelToDynamicConverter<,>))))
                        .AsSelf()
                        .Keyed<IModelToDynamicConverter>(new TypePair(types[0], types[1]));
                }
            }
            return builder;
        }
    }
}