using System.Linq;
using System.Reflection;
using Autofac;
using VitalChoice.Domain.Helpers;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public static class ContentProcessorRegisterExtension
    {
        public static ContainerBuilder RegisterProcessors(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var processors = containingAssembly
                .GetExportedTypes()
                .Where(
                    t =>
                        t.IsImplement(typeof (IContentProcessor)) && !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsGenericType);
            foreach (var processorType in processors)
            {
                var processorName = processorType.GetTypeInfo().GetCustomAttribute<ProcessorNameAttribute>(false)?.Name ??
                                    processorType.Name;
                builder.RegisterType(processorType).Keyed<IContentProcessor>(processorName);
            }
            return builder;
        }
    }
}