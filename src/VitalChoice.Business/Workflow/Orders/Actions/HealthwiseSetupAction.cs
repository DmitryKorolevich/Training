using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class HealthwiseSetupAction : ComputableAction<OrderDataContext>
    {
        public HealthwiseSetupAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            if ((int?) context.Order.SafeData.OrderType == (int) SourceOrderType.Web)
            {
                if (context.Order.Discount?.Code.ToLower() == ProductConstants.HEALTHWISE_DISCOUNT_CODE ||
                    ((bool?) context.Order.Customer?.SafeData.HasHealthwiseOrders ?? false))
                {
                    context.Order.Data.IsHealthwise = true;
                }
            }
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}