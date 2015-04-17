using System;
using VitalChoice.Workflow.Attributes;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;
using VitalChoice.Workflow.Implementations.Actions;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.ActionResolvers
{
    [WorkflowExecutorName("TierDiscount")]
    public class TierDiscountResolver: ComputableActionResolver<OrderContext>
    {
        public TierDiscountResolver(IWorkflowActionTree<OrderContext, decimal> tree) : base(tree)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (ActionTree.GetActionResult<ProductAction>(context) > 100)
                return (int) TierDiscountType.ApplyDiscount;
            return (int) TierDiscountType.DoNotApply;
        }
    }
}