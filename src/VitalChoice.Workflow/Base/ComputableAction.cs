using System;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableAction<TContext>: WorkflowAction<TContext, decimal> 
        where TContext : ComputableContext {
        protected ComputableAction(ComputableActionTree<TContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public sealed override decimal AggregateResult(decimal result, decimal currentValue, string actionName)
        {
            return currentValue + result;
        }

        public abstract override decimal ExecuteAction(TContext context);
    }
}