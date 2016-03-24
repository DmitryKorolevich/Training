using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Discounts
{
    public class AutoShipAction : ComputableAction<OrderDataContext>
    {
        public AutoShipAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.FreeShipping = true;

			return Task.FromResult<decimal>(-(decimal)dataContext.Order.Skus.First().Sku.Data.OffPercent *
										(decimal)dataContext.Data.DiscountableSubtotal / 100);
		}
    }
}
