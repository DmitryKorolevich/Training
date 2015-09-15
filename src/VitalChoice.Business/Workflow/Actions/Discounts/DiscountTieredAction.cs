using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountTieredAction: ComputableAction<OrderContext>
    {
        public DiscountTieredAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            var discountableSubtotal = context.Data.DiscountableSubtotal;
            foreach (var tier in context.Order.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && discountableSubtotal <= tier.To)
                {
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            return -Math.Min(discountableSubtotal, tier.Amount ?? 0);
                        case DiscountType.PercentDiscount:
                            return -tier.Percent ?? 0 / 100.0 * discountableSubtotal;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            context.DiscountMessage = "Cannot determine any tear from discountable products subtotal. Discount won't apply.";
            return 0;
        }
    }
}