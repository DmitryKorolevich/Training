﻿using System;
using System.Threading.Tasks;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Discounts
{
    public class DiscountPriceAction : ComputableAction<OrderDataContext>
    {
        public DiscountPriceAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            context.DiscountMessage = BusinessHelper.GetDiscountMessage(context.Order.Discount);
            context.FreeShipping = context.Order.Discount.Data.FreeShipping;

            decimal totalDiscount = Math.Min(context.Data.DiscountableSubtotal, context.Order.Discount.Data.Amount);

            context.SplitInfo.PerishableDiscount = Math.Min(context.SplitInfo.DiscountablePerishable, context.Order.Discount.Data.Amount);
            context.SplitInfo.NonPerishableDiscount = totalDiscount - context.SplitInfo.PerishableDiscount;

            return Task.FromResult(-totalDiscount);
        }
    }
}
