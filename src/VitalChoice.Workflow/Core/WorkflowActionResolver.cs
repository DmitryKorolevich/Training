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
        }

        public abstract int GetActionKey(TContext context);

        public override TResult Execute(TContext context)
        {
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
            return default(TResult);
        }

        public Dictionary<int, string> Actions { get; }
    }
}