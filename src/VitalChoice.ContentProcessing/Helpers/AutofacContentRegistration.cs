using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Business.Services;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.ContentProcessing.Helpers
{
    public static class AutofacContentRegistration
    {
        public static ContainerBuilder RegisterContentBase(this ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(GetContentProcessor<,>)).As(typeof(IContentProcessor<,>));
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
