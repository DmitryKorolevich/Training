using System;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountPriceAction : ComputableAction<OrderDataContext>
    {
        public DiscountPriceAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.DiscountMessage = $"Price Discount ({dataContext.Order.Discount.Data.Amount:C})";
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            return Task.FromResult<decimal>(-Math.Min(dataContext.Data.DiscountableSubtotal, dataContext.Order.Discount.Data.Amount));
        }
    }
}
