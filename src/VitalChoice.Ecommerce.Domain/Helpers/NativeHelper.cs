using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.PlatformAbstractions;
#if NETSTANDARD1_5
using System.Runtime.Loader;
#endif

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class NativeHelper {

        private static volatile DependencyContext _dependencyContext;

        private static readonly ConcurrentDictionary<string, Assembly> AssemblyCache =
            new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);

        private static readonly List<Assembly> Assemblies;

#if NETSTANDARD1_5
        internal class TemplateLoadContext : AssemblyLoadContext
        {
            protected override Assembly Load(AssemblyName assemblyName)
            {
                return Default.LoadFromAssemblyName(assemblyName);
            }

            public Assembly Load(Stream assembly, Stream assemblySymbols)
            {
                return LoadFromStream(assembly, assemblySymbols);
            }
        }
#endif
        private static void WalkReferenceAssemblies(Assembly current)
        {
            var currentInfo = current;
            if (AssemblyCache.TryAdd(current.FullName, currentInfo))
            {
                Assemblies.Add(currentInfo);
                foreach (var assemblyName in current.GetReferencedAssemblies())
                {
                    try
                    {
                        var dependent = Assembly.Load(assemblyName);
                        var dependentInfo = dependent;
                        if (AssemblyCache.TryAdd(dependent.FullName, dependentInfo))
                        {
                            Assemblies.Add(dependentInfo);
                            WalkReferenceAssemblies(dependent);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
        }

        internal static ICollection<Assembly> GetAssemblies()
        {
            return Assemblies;
        }

        private static Assembly _applicationAssembly;

        static NativeHelper()
        {
            var appEnvironment = PlatformServices.Default.Application;

            _applicationAssembly = Assembly.Load(new AssemblyName(appEnvironment.ApplicationName));
            _dependencyContext = DependencyContext.Load(_applicationAssembly);
            Assemblies = new List<Assembly>();
            WalkReferenceAssemblies(_applicationAssembly);
            WalkReferenceAssemblies(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly);
            WalkReferenceAssemblies(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).GetTypeInfo().Assembly);
            GetApplicationReferences();
        }

        private static bool _configured;

        public static void Configure(Assembly startupAssembly)
        {
            if (!_configured)
            {
                _dependencyContext = DependencyContext.Load(startupAssembly);
                _applicationAssembly = startupAssembly;
                WalkReferenceAssemblies(startupAssembly);
                GetApplicationReferences();
                _configured = true;
            }
        }

        internal static Stream GetResourceStream(Assembly assembly, string name)
        {
            return assembly.GetManifestResourceStream(name);
        }

        internal static AssemblyName GetAssemblyName(string name)
        {
            return
                GetAssemblies()
                    .Select(assembly => assembly.GetName())
                    .FirstOrDefault(assemblyName => assemblyName.Name == name);
        }

        public static void GetApplicationReferences()
        {
            if (_dependencyContext != null && !_configured)
            {
                foreach (var name in _dependencyContext.GetDefaultAssemblyNames())
                {
                    if (!AssemblyCache.ContainsKey(name.FullName))
                    {
                        var asm = Assembly.Load(name);
                        WalkReferenceAssemblies(asm);
                    }
                }
            }
        }
    }
}