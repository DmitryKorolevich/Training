using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingSurchargeOverrideAction : ComputableAction<OrderDataContext>
    {
        public ShippingSurchargeOverrideAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            decimal surchargeOverride = (decimal?)context.Order.SafeData.SurchargeOverride ?? 0;
            decimal surchargeTotal = context.AlaskaHawaiiSurcharge + context.CanadaSurcharge;
            if (surchargeOverride > surchargeTotal)
            {
                surchargeOverride = surchargeTotal;
            }
            context.SurchargeOverride = surchargeOverride;

            var perishableOverride = surchargeOverride;
            if (perishableOverride > context.SplitInfo.PerishableSurchargeOverriden)
            {
                perishableOverride = context.SplitInfo.PerishableSurchargeOverriden;
            }

            context.SplitInfo.PerishableSurchargeOverriden -= perishableOverride;
            context.SplitInfo.NonPerishableSurchargeOverriden -= surchargeOverride - perishableOverride;

            return Task.FromResult(-surchargeOverride);
        }
    }
}
