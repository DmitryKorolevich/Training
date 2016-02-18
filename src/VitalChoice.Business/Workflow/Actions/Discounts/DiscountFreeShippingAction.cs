using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountFreeShippingAction : ComputableAction<OrderDataContext>
    {
        public DiscountFreeShippingAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            dataContext.FreeShipping = true;
            return Task.FromResult((decimal)0);
        }
    }
}