using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountFreeShippingAction : ComputableAction<OrderDataContext>
    {
        public DiscountFreeShippingAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.DiscountMessage =BusinessHelper.GetDiscountMessage(dataContext.Order.Discount);
            dataContext.FreeShipping = true;
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}