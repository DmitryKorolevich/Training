using System;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundDiscountPriceAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountPriceAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            return Task.FromResult<decimal>(-Math.Min(dataContext.Data.RefundProducts, dataContext.Order.Discount.Data.Amount));
        }
    }
}
