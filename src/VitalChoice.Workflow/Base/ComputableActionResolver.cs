using System;
using System.Threading.Tasks;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableActionResolver<TContext>: WorkflowActionResolver<TContext, decimal> 
        where TContext : ComputableDataContext {
        protected ComputableActionResolver(IWorkflowTree<TContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public abstract override Task<int> GetActionKeyAsync(TContext context, IWorkflowExecutionContext executionContext);
    }
}