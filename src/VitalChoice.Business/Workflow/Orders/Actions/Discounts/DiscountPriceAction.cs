using System;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountPriceAction : ComputableAction<OrderDataContext>
    {
        public DiscountPriceAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            return Task.FromResult<decimal>(-Math.Min(dataContext.Data.DiscountableSubtotal, dataContext.Order.Discount.Data.Amount));
        }
    }
}
