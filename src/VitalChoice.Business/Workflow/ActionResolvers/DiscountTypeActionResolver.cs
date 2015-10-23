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
    public class DiscountTypeActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public DiscountTypeActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKey(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.Discount == null)
                return Task.FromResult(0);
            if (!ValidateDiscount(dataContext))
            {
                return Task.FromResult(0);
            }
            return Task.FromResult(dataContext.Order.Discount.IdObjectType ?? 0);
        }

        private static bool ValidateDiscount(OrderDataContext dataContext)
        {
            var error = true;

            if (dataContext.Order.Discount.DictionaryData.ContainsKey("RequireMinimumPerishable") &&
                dataContext.Order.Discount.Data.RequireMinimumPerishable &&
                dataContext.Data.PerishableSubtotal < dataContext.Order.Discount.Data.RequireMinimumPerishableAmount)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Minimum perishable {dataContext.Order.Discount.Data.RequireMinimumPerishableAmount:C} not reached",
                    Field = "DiscountCode"
                });
                error = false;
            }
            var now = DateTime.Now;
            if (dataContext.Order.Discount.StartDate > now)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount not started, start date: {dataContext.Order.Discount.StartDate:d}",
                    Field = "DiscountCode"
                });
                error = false;
            }
            if (dataContext.Order.Discount.ExpirationDate < now)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount expired {dataContext.Order.Discount.ExpirationDate:d}",
                    Field = "DiscountCode"
                });
                error = false;
            }
            if (dataContext.Order.Discount.Assigned.HasValue && dataContext.Order.Customer.IdObjectType.HasValue &&
                (int) dataContext.Order.Discount.Assigned.Value != dataContext.Order.Customer.IdObjectType.Value)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount could only be applied for {dataContext.Order.Discount.Assigned.Value} customer",
                    Field = "DiscountCode"
                });
                error = false;
            }
            return error;
        }
    }
}