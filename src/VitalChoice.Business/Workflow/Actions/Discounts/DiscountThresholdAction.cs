using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountThresholdAction : ComputableAction<OrderContext>
    {
        public DiscountThresholdAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.FreeShipping = context.Order.Discount.Data.FreeShipping;
            if (context.ProductsSubtotal < context.Order.Discount.Data.Threshold)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Products Subtotal Threshold {context.Order.Discount.Data.Threshold:C} not reached",
                    Field = "DiscountCode"
                });
                return 0;
            }
            context.DiscountMessage = $"Threshold Discount ({context.Order.Discount.Data.ProductSKU})";
            var item = (SkuOrdered) context.Order.Discount.Data.ThresholdSku;
            item.Quantity = 1;
            item.Amount = 0;
            context.PromoSkus.Add(item);
            return 0;
        }
    }
}
