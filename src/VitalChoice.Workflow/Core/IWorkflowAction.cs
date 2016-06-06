using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowAction<in TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        List<string> DependendActions { get; }

        List<string> AggreagatedActions { get; }

        TResult AggregateResult(TResult result, TResult currentValue, string actionName);

        Task<TResult> ExecuteActionAsync(TContext context, ITreeContext executionContext);
    }
}