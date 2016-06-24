using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundAutoShipDiscountAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundAutoShipDiscountAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            var discountPercent =
                (decimal?) context.RefundSkus.FirstOrDefault(s => (bool?) s.Sku.SafeData.AutoShipProduct ?? false)?.Sku.Data.OffPercent ?? 0;

            context.SplitInfo.PerishableDiscount = discountPercent*context.SplitInfo.DiscountablePerishable/100;
            context.SplitInfo.NonPerishableDiscount = discountPercent*context.SplitInfo.DiscountableNonPerishable/100;

            return Task.FromResult(-discountPercent*(decimal) context.Data.RefundDiscountableSubtotal / 100);
        }
    }
}
