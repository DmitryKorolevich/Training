using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Caching.Extensions
{
    public static class AutofacBuilderExtension
    {
        public static ContainerBuilder RegisterDatabaseCache(this ContainerBuilder builder, IEnumerable<IModel> dataModels)
        {
            builder.RegisterType<InternalEntityCacheFactory>().As<IInternalEntityCacheFactory>().SingleInstance();
            builder.Register(cc => new InternalEntityInfoStorage(dataModels)).As<IInternalEntityInfoStorage>().SingleInstance();
            builder.RegisterGeneric(typeof (EntityCache<>)).As(typeof (IEntityCache<>)).SingleInstance();
            builder.RegisterType<CacheStateManager>().As<IStateManager>().InstancePerLifetimeScope();
            return builder;
        }
    }
}