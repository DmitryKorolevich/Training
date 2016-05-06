using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Orders.ActionResolvers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Avatax;
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
            var orderRep = executionContext.Resolve<IEcommerceRepositoryAsync<Order>>();
            var shouldSplit =
                await
                    orderRep.Query(o => o.Id == context.Order.IdOrderSource && o.POrderStatus != null && o.NPOrderStatus != null)
                        .SelectAnyAsync();
            if (shouldSplit)
            {
                context.TaxTotal = (await
                    Task.WhenAll(taxService.GetTax(context, TaxGetType.Perishable),
                        taxService.GetTax(context, TaxGetType.NonPerishable))).Sum();
            }
            else
            {
                context.TaxTotal = await taxService.GetTax(context);
            }
            return context.TaxTotal;
        }
    }
}