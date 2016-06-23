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
            if (context.Data.DeliveredAmount > 200)
            {
                result = context.Data.DeliveredAmount * 0.1m;
                result = Math.Round(result, 2);
                context.SplitInfo.PerishableSurchargeOverriden = context.ProductSplitInfo.PerishableAmount*0.1m;
                context.SplitInfo.NonPerishableSurchargeOverriden = context.ProductSplitInfo.NonPerishableAmount*0.1m;
            }
            else
            {
                result = 19.95m;
            }
            context.AlaskaHawaiiSurcharge = result;
            return Task.FromResult(result);
        }
    }
}