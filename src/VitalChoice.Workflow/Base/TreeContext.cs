using Autofac;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public class TreeContext : ITreeContext
    {
        private readonly ILifetimeScope _scope;

        public TreeContext(ILifetimeScope localScope)
        {
            _scope = localScope;
        }

        public T Resolve<T>()
        {
            return _scope.Resolve<T>();
        }

        public virtual void Dispose()
        {
        }
    }
}
