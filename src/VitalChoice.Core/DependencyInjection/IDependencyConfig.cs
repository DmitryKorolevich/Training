using System;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		void RegisterInfrastructure(IConfiguration configuration, IServiceCollection services);

		void Register(IServiceCollection services);
    }
}
