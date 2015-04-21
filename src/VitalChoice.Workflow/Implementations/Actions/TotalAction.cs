using System;
using VitalChoice.Workflow.Attributes;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.Actions
{
    [WorkflowExecutorName("Total")]
    public class TotalAction: ComputableAction<OrderContext>
    {
        public TotalAction(ComputableActionTree<OrderContext> tree) : base(tree)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return 0;
        }
    }
}