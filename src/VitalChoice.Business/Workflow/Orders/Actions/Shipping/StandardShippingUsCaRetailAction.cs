using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class StandardShippingUsCaRetailAction : ComputableAction<OrderDataContext>
    {
        public StandardShippingUsCaRetailAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            if (context.Data.PromoProducts < 99 && context.Data.DeliveredAmount > 0)
            {
                if (context.Data.PromoProducts < 99)
                {
                    context.FreeShippingThresholdDifference = 99 - context.Data.PromoProducts;
                }
                if (context.Data.PromoProducts < 50)
                {
                    context.StandardShippingCharges = (decimal) 4.95;
                    AddSplitShipping(context);
                    return Task.FromResult(context.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.FirstCost;
                }
                if (context.Data.PromoProducts < 99)
                {
                    context.StandardShippingCharges = (decimal) 9.95;
                    AddSplitShipping(context);
                    return Task.FromResult(context.StandardShippingCharges);
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }

        private static void AddSplitShipping(OrderDataContext context)
        {
            if (context.ProductSplitInfo.PerishableAmount > 0)
            {
                context.SplitInfo.PerishableShippingOveridden += context.StandardShippingCharges;
            }
            else
            {
                context.SplitInfo.NonPerishableShippingOverriden += context.StandardShippingCharges;
            }
        }
    }
}