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
                        t.IsImplementGeneric(typeof (DynamicMapper<,,,>)) && !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsGenericType);
            foreach (var mapperType in mapperTypes)
            {
                builder.RegisterType(mapperType)
                    .As(
                        typeof (IDynamicMapper<,,,>).MakeGenericType(
                            mapperType.TryGetTypeArguments(typeof (IDynamicMapper<,,,>))))
                    .As(
                        typeof (IDynamicMapper<>).MakeGenericType(
                            mapperType.TryGetElementType(typeof (IDynamicMapper<>))))
                    .AsSelf()
                    .Keyed<IDynamicMapper>(mapperType.TryGetElementType(typeof (DynamicMapper<,,,>)));
            }
            builder.RegisterType<TypeConverter>().As<ITypeConverter>().SingleInstance();
            builder.RegisterType<ModelConverterService>().As<IModelConverterService>().SingleInstance();
            return builder;
        }

        public static ContainerBuilder RegisterModelConverters(this ContainerBuilder builder, Assembly containAssembly)
        {
            var converters =
                containAssembly.ExportedTypes.Where(
                    t => t.IsImplementGeneric(typeof (IModelConverter<,>)) && !t.GetTypeInfo().IsAbstract);
            foreach (var converter in converters)
            {
                var types = converter.TryGetTypeArguments(typeof (IModelConverter<,>));
                if (types != null && types.Length == 2)
                {
                    builder.RegisterType(converter)
                        .As(
                            typeof (IModelConverter<,>).MakeGenericType(
                                converter.TryGetTypeArguments(typeof (IModelConverter<,>))))
                        .AsSelf()
                        .Keyed<IModelConverter>(new TypePair(types[0], types[1]));
                }
            }
            return builder;
        }
    }
}