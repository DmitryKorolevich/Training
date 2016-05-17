using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
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
            ProfilingScope.Enabled = true;
            return app.UseMiddleware<ProfilerMiddleware>();
        }

        public static IServiceCollection InjectProfiler(this IServiceCollection services)
        {
            return services.AddScoped<IPerformanceRequest, DefaultPerformanceRequest>();
        }

        public static EntityFrameworkServicesBuilder InjectProfiler(this EntityFrameworkServicesBuilder efServices)
        {
            efServices.GetInfrastructure()
                .Replace(new ServiceDescriptor(typeof(IRelationalCommandBuilderFactory), typeof(RelationalCommandBuilderFactoryProxy),
                    ServiceLifetime.Scoped));
            efServices.GetInfrastructure()
                .Replace(new ServiceDescriptor(typeof(IQuerySqlGenerator), typeof(QuerySqlGeneratorProxy), ServiceLifetime.Scoped));
            return efServices;
        }
    }
}