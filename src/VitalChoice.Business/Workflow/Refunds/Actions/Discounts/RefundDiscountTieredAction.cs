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

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var discountableSubtotal = dataContext.Data.RefundDiscountableSubtotal;
            foreach (var tier in dataContext.Order.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && (tier.To == null || discountableSubtotal <= tier.To))
                {
                    dataContext.Order.Data.IdDiscountTier = tier.Id;
                    dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage(tier.Id);
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            return Task.FromResult<decimal>(-Math.Min(discountableSubtotal, tier.Amount ?? 0));
                        case DiscountType.PercentDiscount:
                            return Task.FromResult(-(tier.Percent ?? 0)*(decimal) discountableSubtotal/100);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            dataContext.Messages.Add(new MessageInfo
            {
                Message = "Cannot determine any tier from discountable products subtotal. Discount won't apply.",
                Field = "DiscountCode"
            });
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}