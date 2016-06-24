using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountTieredAction : ComputableAction<OrderDataContext>
    {
        public DiscountTieredAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            context.FreeShipping = context.Order.Discount.Data.FreeShipping;
            var discountableSubtotal = (decimal) context.Data.DiscountableSubtotal;
            foreach (var tier in context.Order.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && (tier.To == null || discountableSubtotal <= tier.To))
                {
                    context.Order.Data.IdDiscountTier = tier.Id;
                    context.DiscountMessage = context.Order.Discount.GetDiscountMessage(tier.Id);
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            var totalDiscount = Math.Min(discountableSubtotal, tier.Amount ?? 0);

                            context.SplitInfo.PerishableDiscount = Math.Min(context.SplitInfo.DiscountablePerishable,
                                tier.Amount ?? 0);
                            context.SplitInfo.NonPerishableDiscount = totalDiscount - context.SplitInfo.PerishableDiscount;

                            return Task.FromResult(-totalDiscount);
                        case DiscountType.PercentDiscount:

                            var discountPercent = tier.Percent ?? 0;

                            context.SplitInfo.PerishableDiscount = discountPercent*context.SplitInfo.DiscountablePerishable/100;
                            context.SplitInfo.NonPerishableDiscount = discountPercent*context.SplitInfo.DiscountableNonPerishable/100;

                            return Task.FromResult(-discountPercent*discountableSubtotal/100);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            context.Messages.Add(new MessageInfo
            {
                Message = "Cannot determine any tier from discountable products subtotal. Discount won't apply.",
                Field = "DiscountCode"
            });
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}