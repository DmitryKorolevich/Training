using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountThresholdAction : ComputableAction<OrderDataContext>
    {
        public DiscountThresholdAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            if (dataContext.Data.Products < dataContext.Order.Discount.Data.Threshold)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Products Subtotal Threshold {dataContext.Order.Discount.Data.Threshold:C} not reached",
                    Field = "DiscountCode"
                });
                return TaskCache<decimal>.DefaultCompletedTask;
            }
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            var item = (SkuOrdered) dataContext.Order.Discount.Data.ThresholdSku;
            item.Quantity = 1;
            item.Amount = 0;
            dataContext.PromoSkus.Add(new PromoOrdered(item, null, true));
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}