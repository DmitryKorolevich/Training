using System;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Implementations.Actions;
using VitalChoice.Workflow.Implementations.Contexts;

namespace VitalChoice.Workflow.Implementations.Trees
{
    public class OrdersTree: ComputableActionTree<OrderContext>
    {
        public override decimal Execute(OrderContext context)
        {
            return Execute<TotalAction>(context);
        }
    }
}