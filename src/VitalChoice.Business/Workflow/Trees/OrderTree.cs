using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrderTree: ComputableTree<OrderDataContext>
    {
        public override async Task<decimal> Execute(OrderDataContext dataContext)
        {
            var result = await Execute<TotalAction>(dataContext);
            dataContext.Total = result;
            return result;
        }

        public OrderTree(IActionItemProvider actionProvider, string treeName) : base(actionProvider, treeName)
        {
        }
    }
}