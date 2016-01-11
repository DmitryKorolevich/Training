using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.DynamicData.Extensions
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterDynamicsBase(this ContainerBuilder builder)
        {
            builder.RegisterType<TypeConverter>().As<ITypeConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ModelConverterService>().As<IModelConverterService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (DirectMapper<>)).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ObjectMapperFactory>().As<IObjectMapperFactory>().InstancePerLifetimeScope();
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
            builder.RegisterGeneric(typeof(ObjectMapper<>)).As(typeof(IObjectMapper<>)).InstancePerLifetimeScope();
            foreach (var mapperType in mapperTypes)
            {
                var types = mapperType.TryGetTypeArguments(typeof (IOptionTypeQueryProvider<,,>));

                if (types != null)
                {
                    builder.RegisterType(mapperType)
                        .As(
                            typeof (IDynamicMapper<,,,>).MakeGenericType(
                                mapperType.TryGetTypeArguments(typeof (IDynamicMapper<,,,>))))
                        .As(
                            typeof (IDynamicMapper<,>).MakeGenericType(mapperType.TryGetTypeArguments(typeof (IDynamicMapper<,>))))
                        .As(typeof (IObjectMapper<>).MakeGenericType(
                            mapperType.TryGetElementType(typeof (IDynamicMapper<,>))))
                        .As(typeof (IOptionTypeQueryProvider<,,>).MakeGenericType(
                            mapperType.TryGetTypeArguments(typeof (IOptionTypeQueryProvider<,,>))))
                        .AsSelf()
                        .Keyed<IObjectMapper>(mapperType.TryGetElementType(typeof (DynamicMapper<,,,>)))
                        .Keyed<IOptionTypeQueryProvider>(new GenericTypePair(types[0], types[1])).InstancePerLifetimeScope();
                }
            }
            return builder;
        }

        public static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGenericServiceDecorator(
            this ContainerBuilder builder, Type implementor, string parameterName)
        {
            return builder.RegisterGeneric(implementor).WithParameter((pi, cc) => pi.Name == parameterName,
                (pi, cc) =>
                {
                    var typeArguments = pi.ParameterType.TryGetTypeArguments(typeof (IDynamicReadServiceAsync<,>));
                    if (typeArguments == null)
                        throw new ArgumentException(
                            "No type arguments in IDynamicReadServiceAsync<,> while resolving IExtendedDynamicServiceAsync<,,,>");
                    var dynamicType = typeArguments[0];
                    var entityType = typeArguments[1];
                    var entityTypeArguments = entityType.TryGetTypeArguments(typeof (DynamicDataEntity<,>));
                    if (entityTypeArguments == null)
                        throw new ArgumentException(
                            "No type arguments in DynamicDataEntity<,> while resolving IExtendedDynamicServiceAsync<,,,>. " +
                            "Please use entity inherited from DynamicDataEntity<,>");
                    var optionValueType = entityTypeArguments[0];
                    var optionTypeType = entityTypeArguments[1];
                    return
                        cc.Resolve(typeof (IExtendedDynamicServiceAsync<,,,>).MakeGenericType(dynamicType, entityType,
                            optionTypeType, optionValueType));
                }).InstancePerLifetimeScope();
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
                        .Keyed<IModelConverter>(new TypePair(types[0], types[1])).InstancePerLifetimeScope();
                }
            }
            return builder;
        }
    }
}