using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingOverrideAction : ComputableAction<OrderDataContext>
    {
        public ShippingOverrideAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            decimal shippingOverride = (decimal?) dataContext.Order.SafeData.ShippingOverride ?? 0;
            decimal shippingTotal = dataContext.StandardShippingCharges + dataContext.Data.ShippingUpgrade;
            if (shippingOverride > shippingTotal)
            {
                shippingOverride = shippingTotal;
            }
            dataContext.ShippingOverride = shippingOverride;
            return Task.FromResult(-shippingOverride);
        }
    }
}
