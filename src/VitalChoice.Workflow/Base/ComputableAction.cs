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
            return Math.Round(currentValue + result, 2);
        }

        public abstract override Task<decimal> ExecuteActionAsync(TContext context, ITreeContext executionContext);
    }
}