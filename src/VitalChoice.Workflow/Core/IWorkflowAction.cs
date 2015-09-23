using System;
using System.Collections.Generic;
using System.Threading;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowAction<in TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        List<string> DependendActions { get; }

        List<string> AggreagatedActions { get; }

        TResult AggregateResult(TResult result, TResult currentValue, string actionName);

        TResult ExecuteAction(TContext context);
    }
}