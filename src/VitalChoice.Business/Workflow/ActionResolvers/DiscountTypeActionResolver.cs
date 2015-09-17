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
            if (!ValidateDiscount(context))
            {
                return 0;
            }
            return context.Order.Discount.IdObjectType ?? 0;
        }

        private static bool ValidateDiscount(OrderContext context)
        {
            var error = true;

            if (context.Order.Discount.DictionaryData.ContainsKey("RequireMinimumPerishable") &&
                context.Order.Discount.Data.RequireMinimumPerishable &&
                context.Data.PerishableSubtotal < context.Order.Discount.Data.RequireMinimumPerishableAmount)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Minimum perishable {context.Order.Discount.Data.RequireMinimumPerishableAmount:C} not reached",
                    Field = "DiscountCode"
                });
                error = false;
            }
            var now = DateTime.Now;
            if (context.Order.Discount.StartDate > now)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message = $"Discount not started, start date: {context.Order.Discount.StartDate:d}",
                    Field = "DiscountCode"
                });
                error = false;
            }
            if (context.Order.Discount.ExpirationDate < now)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message = $"Discount expired {context.Order.Discount.ExpirationDate:d}",
                    Field = "DiscountCode"
                });
                error = false;
            }
            if (context.Order.Discount.Assigned.HasValue && context.Order.Customer.IdObjectType.HasValue &&
                (int) context.Order.Discount.Assigned.Value != context.Order.Customer.IdObjectType.Value)
            {
                context.Messages.Add(new MessageInfo
                {
                    Message = $"Discount could only be applied for {context.Order.Discount.Assigned.Value} customer",
                    Field = "DiscountCode"
                });
                error = false;
            }
            return error;
        }
    }
}