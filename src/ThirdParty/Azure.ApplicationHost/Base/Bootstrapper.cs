// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Azure.ApplicationHost.Constants;
using Azure.ApplicationHost.Services;
using Microsoft.Dnx.Runtime.Loader;
using Microsoft.Extensions.PlatformAbstractions;

namespace Azure.ApplicationHost.Base
{
    internal class Bootstrapper
    {
        private readonly IEnumerable<string> _searchPaths;

        public Bootstrapper(IEnumerable<string> searchPaths)
        {
            _searchPaths = searchPaths;
        }

        public Task<int> RunAsync(string name, IRuntimeEnvironment env, string appBase, FrameworkName targetFramework)
        {
            var accessor = LoadContextAccessor.Instance;
            var container = new LoaderContainer();
            LoadContext.InitializeDefaultContext(new DefaultLoadContext(container));

            var disposable = container.AddLoader(new PathBasedAssemblyLoader(accessor, _searchPaths));

            try
            {
                var assembly = accessor.Default.Load(name);

                if (assembly == null)
                {
                    return Task.FromResult(1);
                }

#if DNX451
                string applicationBaseDirectory = appBase;

                // Set the app domain variable so that AppContext.BaseDirectory works on .NET Framework (and hopefully mono)
                AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", applicationBaseDirectory);
#else
                var applicationBaseDirectory = AppContext.BaseDirectory;
#endif

                var configuration = Environment.GetEnvironmentVariable("TARGET_CONFIGURATION") ?? Environment.GetEnvironmentVariable(EnvironmentNames.Configuration) ?? "Debug";

                var applicationEnvironment = new HostApplicationEnvironment(applicationBaseDirectory,
                                                                        targetFramework,
                                                                        configuration,
                                                                        assembly);

                CallContextServiceLocator.Locator = new ServiceProviderLocator();
                var compilationAssembly = accessor.Default.Load("Microsoft.Dnx.Compilation");
                var serviceProviderType = compilationAssembly.GetType("Microsoft.Dnx.Runtime.Common.DependencyInjection.ServiceProvider");
                if (serviceProviderType == null)
                {
                    throw new InvalidOperationException(
                        $"Cannot find Microsoft.Dnx.Runtime.Common.DependencyInjection.ServiceProvider Type");
                }

                var serviceProvider = Activator.CreateInstance(serviceProviderType);

                var addMethod =
                    (Action<object, Type, object>)
                        serviceProviderType.GetMethod("Add", new[] {typeof (Type), typeof (object)})
                            .CreateDelegate(typeof (Action<object, Type, object>));
                addMethod(serviceProvider, typeof (IAssemblyLoaderContainer), container);
                addMethod(serviceProvider, typeof (IAssemblyLoadContextAccessor), accessor);
                addMethod(serviceProvider, typeof (IApplicationEnvironment), applicationEnvironment);
                addMethod(serviceProvider, typeof (IRuntimeEnvironment), env);

                CallContextServiceLocator.Locator.ServiceProvider = (IServiceProvider) serviceProvider;
                PlatformServices.SetDefault(
                    PlatformServices.Create(
                        basePlatformServices: null,
                        application: applicationEnvironment,
                        runtime: env,
                        container: container,
                        accessor: accessor));
                return Task.FromResult(0);
            }
            catch
            {
                disposable.Dispose();

                throw;
            }
        }
    }
}
