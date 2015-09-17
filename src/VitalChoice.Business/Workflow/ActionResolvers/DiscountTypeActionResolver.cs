using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public class DiscountTypeActionResolver : ComputableActionResolver<OrderContext>
    {
        public DiscountTypeActionResolver(IWorkflowTree<OrderContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (context.Order.Discount == null)
                return 0;
            if (context.Order.Discount.Data.RequireMinimumPerishable &&
                context.Data.PerishableSubtotal < context.Order.Discount.Data.RequireMinimumPerishableAmount)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message = "Minimum perishable not reached",
                    Field = "DiscountCode"
                });
                
                return 0;
            }
            return context.Order.Discount.IdObjectType ?? 0;
        }
    }
}