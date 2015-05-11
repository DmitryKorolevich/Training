using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Business.Workflow.Actions
{
    public class TotalAction: ComputableAction<OrderContext>
    {
        public TotalAction(ComputableActionTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            return 0;
        }
    }
}