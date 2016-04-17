using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowActionResolver<TContext, TResult> :
        WorkflowExecutor<TContext, TResult>,
        IWorkflowActionResolver<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        protected WorkflowActionResolver(IWorkflowTree<TContext, TResult> tree, string actionName) : base(tree, actionName)
        {
            Actions = new Dictionary<int, string>();
            DependendActions = new List<string>();
        }

        public abstract Task<int> GetActionKeyAsync(TContext context, IWorkflowExecutionContext executionContext);

        public override async Task<TResult> ExecuteAsync(TContext context, IWorkflowExecutionContext executionContext)
        {
            TResult result;
            if (Tree.TryGetActionResult(Name, context, out result))
                return result;
            //pre-execute dependent actions, do not aggregate
            using (new ProfilingScope(Name))
            {
                foreach (var dependentActionName in DependendActions)
                {
                    context.ActionLock(dependentActionName);
                    await Tree.GetAction(dependentActionName).ExecuteAsync(context, executionContext);
                    context.ActionUnlock(dependentActionName);
                }

                var key = await GetActionKeyAsync(context, executionContext);
                if (Actions.ContainsKey(key))
                {
                    var actionName = Actions[key];
                    context.ActionLock(actionName);
                    result = await Tree.GetAction(actionName).ExecuteAsync(context, executionContext);
                    context.ActionUnlock(actionName);
                    context.ActionSetResult(Name, result);
                    return result;
                }

                context.ActionSetResult(Name, default(TResult));
                return default(TResult);
            }
        }

        public Dictionary<int, string> Actions { get; }

        public List<string> DependendActions { get; }
    }
}