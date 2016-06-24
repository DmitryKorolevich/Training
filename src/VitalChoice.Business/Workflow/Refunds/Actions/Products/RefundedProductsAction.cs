using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Products
{
    public class RefundedProductsAction:ComputableAction<OrderRefundDataContext>
    {
        public RefundedProductsAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            decimal productsSubtotal = 0;
            context.RefundSkus = context.RefundOrder.RefundSkus.ToList();
            foreach (var refundSku in context.RefundSkus)
            {
                refundSku.RefundValue = refundSku.RefundPrice*(decimal) refundSku.RefundPercent/100*refundSku.Quantity;
                productsSubtotal += refundSku.RefundValue;
            }
            context.RefundSkus = context.RefundSkus.ToList();
            context.ProductsSubtotal = productsSubtotal;

            context.SplitInfo.ShouldSplit = context.Order.NPOrderStatus != null &&
                                            context.Order.POrderStatus != null;

            return Task.FromResult(productsSubtotal);
        }
    }
}