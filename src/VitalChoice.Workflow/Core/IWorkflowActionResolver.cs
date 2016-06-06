using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowActionResolver<in TContext, TResult> :
        IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        List<string> DependendActions { get; }

        Dictionary<int, string> Actions { get; }

        Task<int> GetActionKeyAsync(TContext context, ITreeContext executionContext);
    }
}