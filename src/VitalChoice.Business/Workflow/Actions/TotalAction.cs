using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions
{
    public class PayableTotalAction : ComputableAction<OrderDataContext>
    {
        public PayableTotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return Task.FromResult<decimal>(0);
        }
    }

    public class TotalAction : PayableTotalAction
    {
        public TotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }
    }
}
