// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Azure.ApplicationHost.Base
{
    internal class LoaderContainer : IAssemblyLoaderContainer
    {
        private readonly Stack<IAssemblyLoader> _loaders = new Stack<IAssemblyLoader>();

        public IDisposable AddLoader(IAssemblyLoader loader)
        {
            _loaders.Push(loader);

            return new DisposableAction(() =>
            {
                var removed = _loaders.Pop();
                if (!ReferenceEquals(loader, removed))
                {
                    throw new InvalidOperationException("TODO: Loader scopes being disposed in wrong order");
                }
            });
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            var sw = Stopwatch.StartNew();

            foreach (var loader in _loaders.Reverse())
            {
                var assembly = loader.Load(assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }
            }

            return null;
        }

        public IntPtr LoadUnmanagedLibrary(string name)
        {
            var sw = Stopwatch.StartNew();

            foreach (var loader in _loaders.Reverse())
            {
                var handle = loader.LoadUnmanagedLibrary(name);
                if (handle != IntPtr.Zero)
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;
            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}