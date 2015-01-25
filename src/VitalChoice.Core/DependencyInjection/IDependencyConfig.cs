using System;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services);
    }
}
