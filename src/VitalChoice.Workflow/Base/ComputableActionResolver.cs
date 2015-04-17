using System;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableActionResolver<TContext>: WorkflowActionResolver<TContext, decimal> 
        where TContext : WorkflowContext<decimal>
    {
        protected ComputableActionResolver(IWorkflowActionTree<TContext, decimal> tree) : base(tree)
        {
        }

        public abstract override int GetActionKey(TContext context);
    }
}