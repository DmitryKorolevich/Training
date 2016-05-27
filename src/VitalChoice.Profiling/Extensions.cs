using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
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
            services.AddScoped<IPerformanceRequest, DefaultPerformanceRequest>();
            services
                .Replace(new ServiceDescriptor(typeof(IRelationalCommandBuilderFactory), typeof(RelationalCommandBuilderFactoryProxy),
                    ServiceLifetime.Scoped));
            services
                .Replace(new ServiceDescriptor(typeof(SqlServerQuerySqlGeneratorFactory), typeof(SqlServerQuerySqlGeneratorFactoryProxy),
                    ServiceLifetime.Scoped));
            return services;
        }
    }
}