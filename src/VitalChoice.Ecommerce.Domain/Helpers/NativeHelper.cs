﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class NativeHelper {

#if DOTNET5_4
        private static readonly IAssemblyLoadContextAccessor LoadContextAccessor =
            (IAssemblyLoadContextAccessor)
                CallContextServiceLocator.Locator?.ServiceProvider?.GetService(typeof (IAssemblyLoadContextAccessor));
        private static readonly Func<object> GetCurrentDomain;
        private static readonly Func<object, Assembly[]> AssembliesGetter;

        static NativeHelper() {
            var type = typeof(object).GetTypeInfo().Assembly.GetType("System.AppDomain");
            if (type == null) {
                throw new InvalidOperationException("Cannot find System.AppDomain class in system library, investigate to issue and rewrite assembly list acquire");
            }
            var method = type.GetProperty("CurrentDomain", BindingFlags.Public | BindingFlags.Static).GetGetMethod();
            if (method == null) {
                throw new InvalidOperationException("Cannot find System.AppDomain.CurrentDomain Property get method in Core Mode, investigate to issue and rewrite assembly list acquire");
            }
            GetCurrentDomain = method.CompileStaticAccessor<object>();
            method = type.GetMethod("GetAssemblies", BindingFlags.Public | BindingFlags.Instance);
            AssembliesGetter = method.CompileAccessor<object, Assembly[]>();
        }

        internal static Assembly[] GetAssemblies() {
            var domain = GetCurrentDomain();
            return AssembliesGetter(domain);
        }

        internal static IAssemblyLoadContext GetAssemblyLoadContext()
        {
            return LoadContextAccessor.Default;
        }
#else

        internal static Assembly[] GetAssemblies() {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
#endif

        internal static AssemblyName GetAssemblyName(string name)
        {
            return
                GetAssemblies()
                    .Select(assembly => assembly.GetName())
                    .FirstOrDefault(assemblyName => assemblyName.Name == name);
        }
    }
}