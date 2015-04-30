using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;
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

        public OrdersTree(HashSet<ActionItem> actionMapping, string actionName) : base(actionMapping, actionName)
        {
        }

        public OrdersTree(IWorkflowActionTree<OrderContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }
    }
}