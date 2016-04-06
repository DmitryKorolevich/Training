﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.PlatformAbstractions;

namespace VitalChoice.Core.DependencyInjection
{
    public interface IDependencyConfig
    {
		IContainer RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, Assembly projectAssembly, IApplicationEnvironment appEnv = null);
    }
}
