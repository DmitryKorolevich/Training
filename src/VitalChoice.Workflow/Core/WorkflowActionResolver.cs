using System.Collections.Generic;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowActionResolver<TContext, TResult> :
        WorkflowExecutor<TContext, TResult>,
        IWorkflowActionResolver<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowActionResolver(IWorkflowTree<TContext, TResult> tree, string actionName) : base(tree, actionName)
        {
            Actions = new Dictionary<int, string>();
            DependendActions = new List<string>();
        }

        public abstract int GetActionKey(TContext context);

        public override TResult Execute(TContext context)
        {
            //pre-execute dependent actions, do not aggregate
            foreach (var dependentActionName in DependendActions)
            {
                context.ActionLock(dependentActionName);
                Tree.GetAction(dependentActionName).Execute(context);
                context.ActionUnlock(dependentActionName);
            }

            var key = GetActionKey(context);
            if (Actions.ContainsKey(key))
            {
                var actionName = Actions[key];
                context.ActionLock(actionName);
                var result = Tree.GetAction(actionName).Execute(context);
                context.ActionUnlock(actionName);
                context.ActionSetResult(Name, result);
                return result;
            }
            context.ActionSetResult(Name, default(TResult));
            return default(TResult);
        }

        public Dictionary<int, string> Actions { get; }

        public List<string> DependendActions { get; }
    }
}