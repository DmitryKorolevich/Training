using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class AutoShipDiscountAction : ComputableAction<OrderDataContext>
    {
        public AutoShipDiscountAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            context.FreeShipping = true;

            var discountPercent =
                (decimal?) context.SkuOrdereds.FirstOrDefault(s => (bool?) s.Sku.SafeData.AutoShipProduct ?? false)?.Sku.Data.OffPercent ??
                0;

            context.SplitInfo.PerishableDiscount = discountPercent*context.SplitInfo.DiscountablePerishable / 100;
            context.SplitInfo.NonPerishableDiscount = discountPercent*context.SplitInfo.DiscountableNonPerishable / 100;

            return Task.FromResult(-discountPercent*(decimal) context.Data.DiscountableSubtotal/100);
        }
    }
}