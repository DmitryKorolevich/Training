using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableActionTree<TContext>: WorkflowActionTree<TContext, decimal> 
        where TContext : ComputableContext {
        protected ComputableActionTree(HashSet<ActionItem> actionMapping, string actionName) : base(actionMapping, actionName) { }

        protected ComputableActionTree(IWorkflowActionTree<TContext, decimal> tree, string actionName) : base(tree, actionName) { }

        public abstract override decimal Execute(TContext context);
    }
}