using System;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, string appPath);
    }
}
