using System;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeActionSetup<out TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        ITreeActionSetup<TContext, TResult> Action<T>(string actionName, Action<IActionSetup<TContext, TResult>> actions = null)
            where T : IWorkflowAction<TContext, TResult>;

        ITreeActionSetup<TContext, TResult> ActionResolver<T>(string actionName, Action<IActionResolverSetup<TContext, TResult>> actions)
            where T : IWorkflowActionResolver<TContext, TResult>;
    }
}