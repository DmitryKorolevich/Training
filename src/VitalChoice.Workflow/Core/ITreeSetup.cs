using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeSetup
    {
        Task<bool> CreateTreesAsync();

        Dictionary<Type, WorkflowTreeDefinition> Trees { get; }
    }

    public interface ITreeSetup<TContext, TResult> : ITreeSetup
        where TContext : WorkflowDataContext<TResult>
    {
        ITreeSetup<TContext, TResult> Tree<T>(string treeName, Action<ITreeActionSetup<TContext, TResult>> actions)
            where T: IWorkflowTree<TContext, TResult>;
    }
}