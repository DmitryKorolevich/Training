using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Helpers
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterMappers(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var mapperTypes = containingAssembly
                .GetExportedTypes()
                .Where(t => t.IsImplementGeneric(typeof(DynamicObjectMapper<,,,>)) && !t.GetTypeInfo().IsAbstract);
            foreach (var mapperType in mapperTypes)
            {
                builder.RegisterTypes(mapperType)
                    .As(typeof(IDynamicObjectMapper<,,,>).MakeGenericType(mapperType.TryGetTypeArguments(typeof(IDynamicObjectMapper<,,,>))))
                    .As(typeof(IDynamicToModelMapper<>).MakeGenericType(mapperType.TryGetElementType(typeof(IDynamicToModelMapper<>))))
                    .AsSelf()
                    .SingleInstance();
                builder.RegisterType(mapperType)
                    .Keyed<IDynamicToModelMapper>(mapperType.TryGetElementType(typeof(DynamicObjectMapper<,,,>)))
                    .SingleInstance();
            }
            return builder;
        }
    }
}