using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers {
    public class TierDiscountResolver: ComputableActionResolver<OrderContext>
    {
        public TierDiscountResolver(IWorkflowTree<OrderContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (Tree.GetActionResult("Products", context) > 100)
                return (int) TierDiscountType.ApplyDiscount;
            return (int) TierDiscountType.DoNotApply;
        }
    }
}