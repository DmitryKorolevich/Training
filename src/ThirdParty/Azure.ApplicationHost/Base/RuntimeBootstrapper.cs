// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Azure.ApplicationHost.Base
{
    internal static class RuntimeBootstrapper
    {
        private static readonly char[] LibPathSeparator = { ';' };

        public static int Execute(string[] args, BootstrapperContext bootstrapperContext)
        {
            try
            {
                return ExecuteAsync(args, bootstrapperContext).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                PrintErrors(ex);
                return 1;
            }
        }

        private static void PrintErrors(Exception ex)
        {
            while (ex != null)
            {
                var suppressStackTrace = (ex.Data["suppressStackTrace"] as bool? == true);

                if (ex is TargetInvocationException ||
                    ex is AggregateException)
                {
                    // Skip these exception messages as they are
                    // generic
                }
                else if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;

                    foreach (var loaderException in typeLoadException.LoaderExceptions)
                    {
                        Console.Error.WriteLine(suppressStackTrace ? $"Error: {loaderException.Message}" : loaderException.ToString());
                    }
                }
                else
                {
                    Console.Error.WriteLine(suppressStackTrace ? $"Error: {ex.Message}" : ex.ToString());
                }

                ex = ex.InnerException;
            }
        }

        public static Task<int> ExecuteAsync(string[] args, BootstrapperContext bootstrapperContext)
        {
            var env = new RuntimeEnvironment(bootstrapperContext);

            // Some options should be forwarded to Microsoft.Dnx.ApplicationHost
            IEnumerable<string> searchPaths =
                ResolveSearchPaths(bootstrapperContext.RuntimeDirectory, new List<string>(), new List<string>());

            var bootstrapper = new Bootstrapper(searchPaths);

            return bootstrapper.RunAsync("Microsoft.Dnx.ApplicationHost", env, bootstrapperContext.ApplicationBase,
                bootstrapperContext.TargetFramework);
        }

        private static IEnumerable<string> ResolveSearchPaths(string defaultLibPath, List<string> libPaths, List<string> remainingArgs)
        {
            var searchPaths = new List<string>();

            if (!string.IsNullOrEmpty(defaultLibPath))
            {
                // Add the default lib folder if specified
                ExpandSearchPath(defaultLibPath, searchPaths);
            }

            // Add the expanded search libs to the list of paths
            foreach (var libPath in libPaths)
            {
                ExpandSearchPath(libPath, searchPaths);
            }

            // If a .dll or .exe is specified then turn this into
            // --lib {path to dll/exe} [dll/exe name]
            if (remainingArgs.Count > 0)
            {
                var application = remainingArgs[0];
                var extension = Path.GetExtension(application);

                if (!string.IsNullOrEmpty(extension) && (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) ||
                                          extension.Equals(".exe", StringComparison.OrdinalIgnoreCase)))
                {
                    // Add the directory to the list of search paths
                    searchPaths.Add(Path.GetDirectoryName(Path.GetFullPath(application)));

                    // Modify the argument to be the dll/exe name
                    remainingArgs[0] = Path.GetFileNameWithoutExtension(application);
                }
            }

            return searchPaths;
        }

        private static void ExpandSearchPath(string libPath, List<string> searchPaths)
        {
            if (libPath.IndexOf(';') >= 0)
            {
                searchPaths.AddRange(
                    libPath.Split(LibPathSeparator, StringSplitOptions.RemoveEmptyEntries).Select(Path.GetFullPath));
            }
            else
            {
                searchPaths.Add(libPath);
            }
        }
    }
}
