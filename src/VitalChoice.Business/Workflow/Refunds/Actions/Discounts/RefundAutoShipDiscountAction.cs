using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Discounts
{
    public class RefundAutoShipDiscountAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundAutoShipDiscountAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return
                Task.FromResult(
                    -(decimal)
                        (dataContext.RefundSkus.FirstOrDefault(s => (bool?) s.Sku.SafeData.AutoShipProduct ?? false)?.Sku.Data.OffPercent ??
                         0)*
                    (decimal) dataContext.Data.DiscountableSubtotal/100);
        }
    }
}
