using System;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountTieredAction: ComputableAction<OrderDataContext>
    {
        public DiscountTieredAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            var discountableSubtotal = (decimal)dataContext.Data.DiscountableSubtotal;
            foreach (var tier in dataContext.Order.Discount.DiscountTiers)
            {
                if (discountableSubtotal >= tier.From && (tier.To == null || discountableSubtotal <= tier.To))
                {
                    dataContext.Order.Data.IdDiscountTier = tier.Id;
                    dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage(tier.Id);
                    switch (tier.IdDiscountType)
                    {
                        case DiscountType.PriceDiscount:
                            return Task.FromResult(-Math.Min(discountableSubtotal, tier.Amount ?? 0));
                        case DiscountType.PercentDiscount:
                            return Task.FromResult(-(tier.Percent ?? 0)*discountableSubtotal/100);
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
            return Task.FromResult((decimal)0);
        }
    }
}