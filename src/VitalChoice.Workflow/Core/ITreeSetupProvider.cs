using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Helpers;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeSetupProvider
    {
        ITreeSetupProvider Tree<T>(string treeName, Action<ITreeSetupProvider> actions);
        ITreeSetupProvider Action<T>(string actionName, Action<ITreeSetupProvider> actions);
    }

    public class TreeSetupProvider : ITreeSetupProvider
    {

        public ITreeSetupProvider Tree<T>(string treeName, Action<ITreeSetupProvider> actions)
        {
            if (!typeof (T).IsImplementGeneric(typeof (IWorkflowTree<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowTree<TContext, TResult>");
            }
            throw new NotImplementedException();
        }

        public ITreeSetupProvider Action<T>(string actionName, Action<ITreeSetupProvider> actions)
        {
            throw new NotImplementedException();
        }
    }
}