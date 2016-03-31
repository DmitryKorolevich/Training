using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class AutoShipDiscountAction : ComputableAction<OrderDataContext>
    {
        public AutoShipDiscountAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.FreeShipping = true;

            return
                Task.FromResult(
                    -(decimal) dataContext.Order.Skus.First(s => (bool?) s.Sku.SafeData.AutoShipProduct ?? false).Sku.Data.OffPercent*
                    (decimal) dataContext.Data.DiscountableSubtotal/100);
        }
    }
}
