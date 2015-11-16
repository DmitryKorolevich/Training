using System;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeSetup<TContext, TResult> 
        where TContext : WorkflowDataContext<TResult>
    {
        ITreeSetup<TContext, TResult> Tree<T>(string treeName, Action<ITreeActionSetup<TContext, TResult>> actions)
            where T: IWorkflowTree<TContext, TResult>;
        ITreeSetup<TContext, TResult> Action<T>(string actionName, Action<IActionSetup<TContext, TResult>> actions = null)
            where T : IWorkflowAction<TContext, TResult>;
        ITreeSetup<TContext, TResult> ActionResolver<T>(string actionName, Action<IActionResolverSetup<TContext, TResult>> actions)
            where T : IWorkflowActionResolver<TContext, TResult>;
        Task<bool> UpdateAsync();
    }
}