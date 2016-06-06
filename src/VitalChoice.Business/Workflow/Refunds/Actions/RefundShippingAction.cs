using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class RefundShippingAction:ComputableAction<OrderRefundDataContext>
    {
        public RefundShippingAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            context.ShippingRefunded = context.Order.Data.ShippingRefunded;
            context.ManualShippingTotal = context.Order.Data.ManualShippingTotal;
            if (context.ShippingRefunded)
            {
                context.ShippingTotal = context.ManualShippingTotal;
                return Task.FromResult(context.ManualShippingTotal);
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}
