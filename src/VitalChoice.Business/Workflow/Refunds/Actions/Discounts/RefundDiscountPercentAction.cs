using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundDiscountPercentAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountPercentAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            return
                Task.FromResult<decimal>(-dataContext.Order.Discount.Data.Percent*
                                         (decimal) dataContext.Data.RefundDiscountableSubtotal / 100);
        }
    }
}