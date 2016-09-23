using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class StandardShippingDropship : ComputableAction<OrderDataContext>
    {
        public StandardShippingDropship(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            if (context.Data.PromoProducts < 100 && context.Data.DeliveredAmount > 0)
            {
                context.StandardShippingCharges = (decimal) 4.95;
                AddSplitShipping(context, DeliveryServiceCostGroup.FirstCost);
                return Task.FromResult(context.StandardShippingCharges);
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }

        private static void AddSplitShipping(OrderDataContext context, DeliveryServiceCostGroup costGroup)
        {
            context.ShippingCostGroup = costGroup;
            if (context.SplitInfo.PerishableAmount > 0)
            {
                context.SplitInfo.PerishableShippingOveridden += context.StandardShippingCharges;
                context.SplitInfo.PerishableCostGroup = costGroup;
            }
            else
            {
                context.SplitInfo.NonPerishableShippingOverriden += context.StandardShippingCharges;
                context.SplitInfo.NonPerishableCostGroup = costGroup;
            }
        }
    }
}
