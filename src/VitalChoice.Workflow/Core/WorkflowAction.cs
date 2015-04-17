using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.Workflow.Attributes;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowAction<TContext, TResult> : WorkflowExecutor<TContext, TResult>, IWorkflowAction<TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowAction(IWorkflowActionTree<TContext, TResult> tree) : base(tree)
        {
            DependendActions = new List<string>();
        }

        public abstract TResult AggregateResult(TResult result, TResult currentValue, string actionName);
        public abstract TResult ExecuteAction(TContext context);

        public override TResult Execute(TContext context)
        {
            TResult result;
            if (ActionTree.TryGetActionResult(Name, context, out result))
                return result;
            foreach (var actionName in DependendActions)
            {
                context.ActionLock(actionName);
                result = AggregateResult(ActionTree.GetAction(actionName).Execute(context), result, actionName);
                context.ActionUnlock(actionName);
            }
            result = AggregateResult(ExecuteAction(context), result, null);
            context.ActionSetResult(Name, result);
            return result;
        }

        public List<string> DependendActions { get; }
    }
}