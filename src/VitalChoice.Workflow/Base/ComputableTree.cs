using System.Collections.Generic;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableTree<TContext>: WorkflowTree<TContext, decimal> 
        where TContext : ComputableContext {
        protected ComputableTree(IActionItemProvider itemProvider, string treeName) : base(itemProvider, treeName) { }
        public abstract override decimal Execute(TContext context);
    }
}