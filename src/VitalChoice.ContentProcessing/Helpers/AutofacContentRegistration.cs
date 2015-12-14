using Autofac;
using System.Linq;
using System.Reflection;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.ContentProcessing.Helpers
{
    public static class AutofacContentRegistration
    {
        public static ContainerBuilder RegisterContentBase(this ContainerBuilder builder)
        {
            builder.RegisterType<ContentProcessorService>().As<IContentProcessorService>();
            return builder;
        }

        public static ContainerBuilder RegisterProcessors(this ContainerBuilder builder, Assembly containingAssembly)
        {
            var processors = containingAssembly
                .GetExportedTypes()
                .Where(
                    t =>
                        t.IsImplement(typeof(IContentProcessor)) && !t.GetTypeInfo().IsAbstract &&
                        !t.GetTypeInfo().IsGenericType);
            foreach (var processorType in processors)
            {
                var processorName = processorType.GetTypeInfo().GetCustomAttribute<ProcessorNameAttribute>(false)?.Name ??
                                    processorType.Name;
                builder.RegisterType(processorType).Keyed<IContentProcessor>(processorName).AsSelf();
            }
            return builder;
        }
    }
}