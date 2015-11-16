using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class DiscountPercentAction: ComputableAction<OrderDataContext>
    {
        public DiscountPercentAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.DiscountMessage = $"Percent Discount ({(decimal)dataContext.Order.Discount.Data.Percent / 100:P0})";
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            return
                Task.FromResult<decimal>(-dataContext.Order.Discount.Data.Percent*
                                         (decimal) dataContext.Data.DiscountableSubtotal/100);
        }
    }
}