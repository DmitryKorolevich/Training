using System;
using Autofac;
using Autofac.Core.Lifetime;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public sealed class AutofacExecutionContext : IWorkflowExecutionContext
    {
        private static ILifetimeScope _rootScope;
        private readonly ILifetimeScope _scope;

        public static void Configure(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
            _rootScope.CurrentScopeEnding += GetNewScope;
        }

        private static void GetNewScope(object sender, LifetimeScopeEndingEventArgs e)
        {
            _rootScope.CurrentScopeEnding -= GetNewScope;
            _rootScope = new LifetimeScope(_rootScope.ComponentRegistry);
            _rootScope.CurrentScopeEnding += GetNewScope;
            throw new Exception("Application scope restarted");
        }

        public AutofacExecutionContext()
        {
            _scope = _rootScope.BeginLifetimeScope();
        }

        public T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
