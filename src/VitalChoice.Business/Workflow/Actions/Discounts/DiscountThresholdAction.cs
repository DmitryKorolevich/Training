using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountThresholdAction : ComputableAction<OrderDataContext>
    {
        public DiscountThresholdAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            if (dataContext.ProductsSubtotal < dataContext.Order.Discount.Data.Threshold)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Products Subtotal Threshold {dataContext.Order.Discount.Data.Threshold:C} not reached",
                    Field = "DiscountCode"
                });
                return Task.FromResult((decimal)0);
            }
            dataContext.DiscountMessage = $"Threshold Discount ({dataContext.Order.Discount.Data.ProductSKU})";
            var item = (SkuOrdered) dataContext.Order.Discount.Data.ThresholdSku;
            item.Quantity = 1;
            item.Amount = 0;
            dataContext.PromoSkus.Add(item);
            return Task.FromResult((decimal)0);
        }
    }
}
