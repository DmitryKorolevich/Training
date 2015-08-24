using System.Collections.Generic;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrdersTree: ComputableTree<OrderContext>
    {
        public override decimal Execute(OrderContext context)
        {
            return Execute<TotalAction>(context);
        }

        public OrdersTree(IActionItemProvider actionProvider, string treeName) : base(actionProvider, treeName)
        {
        }
    }
}