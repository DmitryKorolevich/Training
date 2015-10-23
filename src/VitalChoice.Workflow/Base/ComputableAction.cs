using System;
using System.Threading.Tasks;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableAction<TContext>: WorkflowAction<TContext, decimal> 
        where TContext : ComputableDataContext {
        protected ComputableAction(ComputableTree<TContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public sealed override decimal AggregateResult(decimal result, decimal currentValue, string actionName)
        {
            return currentValue + result;
        }

        public abstract override Task<decimal> ExecuteAction(TContext context, IWorkflowExecutionContext executionContext);
    }
}