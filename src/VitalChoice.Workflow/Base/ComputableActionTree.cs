using System;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableActionTree<TContext>: WorkflowActionTree<TContext, decimal> 
        where TContext : WorkflowContext<decimal>
    {
        public abstract override decimal Execute(TContext context);
    }
}