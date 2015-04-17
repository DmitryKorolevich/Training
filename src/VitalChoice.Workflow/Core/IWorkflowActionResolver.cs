using System.Collections.Generic;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowActionResolver<in TContext, out TResult> :
        IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        Dictionary<int, string> Actions { get; }

        int GetActionKey(TContext context);
    }
}