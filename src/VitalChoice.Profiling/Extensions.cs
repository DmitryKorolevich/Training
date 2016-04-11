using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.Sql.Internal;
using Microsoft.Data.Entity.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Profiling.Base;
using VitalChoice.Profiling.Interfaces;

namespace VitalChoice.Profiling
{
    public static class Extensions
    {
        public static IApplicationBuilder InjectProfiler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ProfilerMiddleware>();
        }

        public static IServiceCollection InjectProfiler(this IServiceCollection services)
        {
            return services.AddScoped<IPerformanceRequest, DefaultPerformanceRequest>();
        }

        public static EntityFrameworkServicesBuilder InjectProfiler(this EntityFrameworkServicesBuilder efServices)
        {
            efServices.GetInfrastructure()
                .Replace(new ServiceDescriptor(typeof (SqlServerQuerySqlGeneratorFactory), typeof (SqlServerQuerySqlGeneratorFactoryProxy),
                    ServiceLifetime.Scoped));
            return efServices;
        }
    }
}