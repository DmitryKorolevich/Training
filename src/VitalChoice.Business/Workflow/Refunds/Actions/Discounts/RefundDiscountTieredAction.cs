using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundDiscountTieredAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountTieredAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            decimal discountableSubtotal = context.Data.RefundDiscountableSubtotal;
            foreach (var tier in context.RefundOrder.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && (tier.To == null || discountableSubtotal <= tier.To))
                {
                    context.RefundOrder.Data.IdDiscountTier = tier.Id;
                    context.DiscountMessage = BusinessHelper.GetDiscountMessage(context.RefundOrder.Discount, tier.Id);
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            decimal totalDiscount = Math.Min(discountableSubtotal, tier.Amount ?? 0);
                            context.SplitInfo.PerishableDiscount = Math.Min(context.SplitInfo.DiscountablePerishable, totalDiscount);
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