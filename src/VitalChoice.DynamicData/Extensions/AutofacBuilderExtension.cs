using System.Linq;
using System.Reflection;
using Autofac;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Extensions
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterDynamicsBase(this ContainerBuilder builder)
        {
            builder.RegisterType<TypeConverter>().As<ITypeConverter>();
            builder.RegisterType<ModelConverterService>().As<IModelConverterService>();
            builder.RegisterGeneric(typeof (DirectMapper<>)).AsSelf();
            builder.RegisterType<DynamicExpressionVisitor>().AsSelf();
            return builder;
        }

        public static ContainerBuilder RegisterMappers(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var mapperTypes = containingAssembly
                .GetExportedTypes()
                .Where(
                    t =>
                        t.IsImplementGeneric(typeof (DynamicMapper<,,,>)) && !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsGenericType);
            builder.RegisterGeneric(typeof(ObjectMapper<>)).As(typeof(IObjectMapper<>));
            foreach (var mapperType in mapperTypes)
            {
                var types = mapperType.TryGetTypeArguments(typeof (IOptionTypeQueryProvider<,>));

                if (types != null && types.Length == 2)
                {
                    builder.RegisterType(mapperType)
                        .As(
                            typeof (IDynamicMapper<,,,>).MakeGenericType(
                                mapperType.TryGetTypeArguments(typeof (IDynamicMapper<,,,>))))
                        .As(
                            typeof (IDynamicMapper<>).MakeGenericType(
                                mapperType.TryGetElementType(typeof (IDynamicMapper<>))))
                        .As(typeof (IObjectMapper<>).MakeGenericType(
                            mapperType.TryGetElementType(typeof (IDynamicMapper<>))))
                        .As(typeof (IOptionTypeQueryProvider<,>).MakeGenericType(
                            mapperType.TryGetTypeArguments(typeof (IOptionTypeQueryProvider<,>))))
                        .AsSelf()
                        .Keyed<IObjectMapper>(mapperType.TryGetElementType(typeof (DynamicMapper<,,,>)))
                        .Keyed<IOptionTypeQueryProvider>(new GenericTypePair(types[0], types[1]));
                }
            }
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