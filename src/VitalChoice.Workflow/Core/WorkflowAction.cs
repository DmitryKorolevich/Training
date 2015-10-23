using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowAction<TContext, TResult> : WorkflowExecutor<TContext, TResult>, IWorkflowAction<TContext, TResult> 
        where TContext : WorkflowDataContext<TResult>
    {
        protected WorkflowAction(IWorkflowTree<TContext, TResult> tree, string actionName) : base(tree, actionName)
        {
            DependendActions = new List<string>();
            AggreagatedActions = new List<string>();
        }

        public abstract TResult AggregateResult(TResult result, TResult currentValue, string actionName);
        public abstract Task<TResult> ExecuteAction(TContext context, IWorkflowExecutionContext executionContext);

        public override async Task<TResult> Execute(TContext context, IWorkflowExecutionContext executionContext)
        {
            TResult result;
            if (Tree.TryGetActionResult(Name, context, out result))
                return result;
            foreach (var actionName in DependendActions)
            {
                context.ActionLock(actionName);
                await Tree.GetAction(actionName).Execute(context, executionContext);
                context.ActionUnlock(actionName);
            }
            foreach (var actionName in AggreagatedActions)
            {
                context.ActionLock(actionName);
                result = AggregateResult(await Tree.GetAction(actionName).Execute(context, executionContext), result, actionName);
                context.ActionUnlock(actionName);
            }
            result = AggregateResult(await ExecuteAction(context, executionContext), result, null);
            context.ActionSetResult(Name, result);
            return result;
        }

        public List<string> DependendActions { get; }
        public List<string> AggreagatedActions { get; }
    }
}