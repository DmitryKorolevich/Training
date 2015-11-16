using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountTieredAction: ComputableAction<OrderDataContext>
    {
        public DiscountTieredAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            var discountableSubtotal = dataContext.Data.DiscountableSubtotal;
            foreach (var tier in dataContext.Order.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && tier.To == null || discountableSubtotal <= tier.To)
                {
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            dataContext.DiscountMessage = $"Tiered Discount, Tier from {tier.From:C} to {tier.To:C} ({tier.Amount ?? 0:C})";
                            return Task.FromResult<decimal>(-Math.Min(discountableSubtotal, tier.Amount ?? 0));
                        case DiscountType.PercentDiscount:
                            dataContext.DiscountMessage = $"Tiered Discount, Tier from {tier.From:C} to {tier.To:C} ({(tier.Percent ?? 0) / 100:P0})";
                            return Task.FromResult(-tier.Percent ?? 0*(decimal) discountableSubtotal/100);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            dataContext.Messages.Add(new MessageInfo
            {
                Message = "Cannot determine any tear from discountable products subtotal. Discount won't apply.",
                Field = "DiscountCode"
            });
            return Task.FromResult((decimal)0);
        }
    }
}