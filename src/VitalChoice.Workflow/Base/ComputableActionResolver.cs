using System;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableActionResolver<TContext>: WorkflowActionResolver<TContext, decimal> 
        where TContext : ComputableContext {
        protected ComputableActionResolver(IWorkflowActionTree<TContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public abstract override int GetActionKey(TContext context);
    }
}