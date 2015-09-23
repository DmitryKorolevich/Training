using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowAction<TContext, TResult> : WorkflowExecutor<TContext, TResult>, IWorkflowAction<TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowAction(IWorkflowTree<TContext, TResult> tree, string actionName) : base(tree, actionName)
        {
            DependendActions = new List<string>();
            AggreagatedActions = new List<string>();
        }

        public abstract TResult AggregateResult(TResult result, TResult currentValue, string actionName);
        public abstract TResult ExecuteAction(TContext context);

        public override TResult Execute(TContext context)
        {
            TResult result;
            if (Tree.TryGetActionResult(Name, context, out result))
                return result;
            foreach (var actionName in DependendActions)
            {
                context.ActionLock(actionName);
                Tree.GetAction(actionName).Execute(context);
                context.ActionUnlock(actionName);
            }
            foreach (var actionName in AggreagatedActions)
            {
                context.ActionLock(actionName);
                result = AggregateResult(Tree.GetAction(actionName).Execute(context), result, actionName);
                context.ActionUnlock(actionName);
            }
            result = AggregateResult(ExecuteAction(context), result, null);
            context.ActionSetResult(Name, result);
            return result;
        }

        public List<string> DependendActions { get; }
        public List<string> AggreagatedActions { get; }
    }
}