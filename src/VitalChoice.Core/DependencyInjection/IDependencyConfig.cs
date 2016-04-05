using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, Assembly projectAssembly, IApplicationEnvironment appEnv = null);
    }
}
