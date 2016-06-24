using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingOverrideAction : ComputableAction<OrderDataContext>
    {
        public ShippingOverrideAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            decimal shippingOverride = (decimal?) context.Order.SafeData.ShippingOverride ?? 0;
            decimal shippingTotal = context.StandardShippingCharges + context.Data.ShippingUpgrade;
            if (shippingOverride > shippingTotal)
            {
                shippingOverride = shippingTotal;
            }
            context.ShippingOverride = shippingOverride;

            var perishableOverride = shippingOverride;
            if (perishableOverride > context.SplitInfo.PerishableShippingOveridden)
            {
                perishableOverride = context.SplitInfo.PerishableShippingOveridden;
            }

            context.SplitInfo.PerishableShippingOveridden -= perishableOverride;
            context.SplitInfo.NonPerishableShippingOverriden -= shippingOverride - perishableOverride;

            return Task.FromResult(-shippingOverride);
        }
    }
}