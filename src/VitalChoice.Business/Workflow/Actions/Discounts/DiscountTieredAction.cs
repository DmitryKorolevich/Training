using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Exceptions;
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
                            context.DiscountMessage = $"Tiered Discount, Tier from {tier.From:C} to {tier.To:C} ({tier.Amount ?? 0:C})";
                            return -Math.Min(discountableSubtotal, tier.Amount ?? 0);
                        case DiscountType.PercentDiscount:
                            context.DiscountMessage = $"Tiered Discount, Tier from {tier.From:C} to {tier.To:C} ({(tier.Percent ?? 0) / 100:P0})";
                            return -tier.Percent ?? 0 / 100.0 * discountableSubtotal;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            context.Messages.Add(new MessageInfo
            {
                Message = "Cannot determine any tear from discountable products subtotal. Discount won't apply.",
                Field = "DiscountCode"
            });
            return 0;
        }
    }
}