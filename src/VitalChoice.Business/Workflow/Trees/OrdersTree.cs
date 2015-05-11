using System.Collections.Generic;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Contexts;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrdersTree: ComputableActionTree<OrderContext>
    {
        public override decimal Execute(OrderContext context)
        {
            return Execute<TotalAction>(context);
        }

        public OrdersTree(HashSet<ActionItem> actionMapping, string actionName) : base(actionMapping, actionName)
        {
        }

        public OrdersTree(ComputableActionTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }
    }
}