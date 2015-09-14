using System.Collections.Generic;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrderTree: ComputableTree<OrderContext>
    {
        public override decimal Execute(OrderContext context)
        {
            var result = Execute<TotalAction>(context);
            context.Total = result;
            return result;
        }

        public OrderTree(IActionItemProvider actionProvider, string treeName) : base(actionProvider, treeName)
        {
        }
    }
}