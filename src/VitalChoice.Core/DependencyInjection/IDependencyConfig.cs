using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, Assembly projectAssembly);
    }
}
