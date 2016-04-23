using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class RefundGetTaxAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundGetTaxAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, IWorkflowExecutionContext executionContext)
        {
            var taxService = executionContext.Resolve<IAvalaraTax>();
            context.TaxTotal = await taxService.GetTax(context);
            return context.TaxTotal;
        }
    }
}