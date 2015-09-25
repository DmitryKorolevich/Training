﻿using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions
{
    public class TotalAction: ComputableAction<OrderContext>
    {
        public TotalAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            context.DiscountTotal = -context.Data.Discount;
            context.DiscountedSubtotal = context.Data.Products + context.Data.Discount;
            context.ShippingTotal = context.StandardShippingCharges + context.CanadaSurcharge +
                                    context.AlaskaHawaiiSurcharge;
            return 0;
        }
    }
}