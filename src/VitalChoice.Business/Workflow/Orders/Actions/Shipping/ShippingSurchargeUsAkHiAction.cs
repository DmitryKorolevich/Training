using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingSurchargeUsAkHiAction : ComputableAction<OrderDataContext>
    {
        public ShippingSurchargeUsAkHiAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            decimal result;
            decimal calculateBase;
            if (context.SplitInfo.ShouldSplit)
            {
                calculateBase = context.SplitInfo.PerishableAmount;
            }
            else
            {
                calculateBase = context.SplitInfo.PerishableCount > 0 ? context.Data.DeliveredAmount : 0;
            }
            if (calculateBase > 0)
            {
                if (calculateBase > 200)
                {
                    result = calculateBase*0.1m;
                    result = Math.Round(result, 2);
                    context.SplitInfo.PerishableSurchargeOverriden = result;
                }
                else
                {
                    result = 19.95m;
                    context.SplitInfo.PerishableSurchargeOverriden = result;
                }
            }
            else
            {
                result = 0;
            }
            context.AlaskaHawaiiSurcharge = result;
            return Task.FromResult(result);
        }
    }
}