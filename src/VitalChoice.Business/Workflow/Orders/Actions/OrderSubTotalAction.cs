using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class OrderSubTotalAction : ComputableAction<OrderDataContext>
    {
        public OrderSubTotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            decimal discount = 0;
            if (context.DictionaryData.Keys.Contains("Discount"))
            {
                discount = context.Data.Discount;
            }

            context.DiscountTotal = -discount;
            context.DiscountedSubtotal = context.Data.PromoProducts + discount;
            context.ShippingTotal = context.StandardShippingOverriden + context.SurchargeShippingOverriden;
            context.TotalShipping = context.StandardShippingCharges + context.Data.ShippingUpgrade;
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}