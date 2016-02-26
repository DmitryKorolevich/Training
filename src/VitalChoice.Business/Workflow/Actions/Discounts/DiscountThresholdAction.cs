using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountThresholdAction : ComputableAction<OrderDataContext>
    {
        public DiscountThresholdAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
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
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            var item = (SkuOrdered) dataContext.Order.Discount.Data.ThresholdSku;
            item.Quantity = 1;
            item.Amount = 0;
            dataContext.PromoSkus.Add(new PromoOrdered(item, null, true));
            return Task.FromResult((decimal)0);
        }
    }
}
