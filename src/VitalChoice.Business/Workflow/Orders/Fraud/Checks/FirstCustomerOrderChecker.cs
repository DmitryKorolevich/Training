using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 2)]
    [FraudFieldName(Name = "FirstTimeOrder")]
    public class FirstCustomerOrderChecker : BaseFraudChecker<bool>
    {
        public override async Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, bool valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var orderService = executionContext.Resolve<IOrderService>();
            if (await orderService.GetOrderCount(context.Order.Customer.Id) == 0)
            {
                return new CheckResult
                {
                    Result = true,
                    Reason = "First Customer Order"
                };
            }
            return default(CheckResult);
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, bool valueToCheck,
            OrderReviewRuleDynamic reviewRule) => valueToCheck;
    }
}