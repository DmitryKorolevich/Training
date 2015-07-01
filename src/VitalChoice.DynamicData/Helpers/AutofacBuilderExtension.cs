using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using VitalChoice.DynamicData.Services;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Helpers
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterMappers(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var mappers = containingAssembly
                .GetExportedTypes()
                .Where(t => t.IsImplementGeneric(typeof(DynamicObjectMapper<,,,>)) && !t.GetTypeInfo().IsAbstract);
            foreach (var mapper in mappers)
            {
                builder.RegisterTypes(mapper)
                    .SingleInstance();
                builder.RegisterType(mapper)
                    .Keyed<IDynamicToModelMapper>(mapper.TryGetElementType(typeof(DynamicObjectMapper<,,,>)))
                    .SingleInstance();
            }
            return builder;
        }
    }
}