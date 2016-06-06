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

        public override async Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            var taxService = executionContext.Resolve<IAvalaraTax>();
            if (context.Order.OriginalOrder.NPOrderStatus != null && context.Order.OriginalOrder.POrderStatus != null)
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