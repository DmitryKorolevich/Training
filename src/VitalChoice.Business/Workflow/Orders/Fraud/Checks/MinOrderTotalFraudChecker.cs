using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "MinOrderTotal")]
    public class MinOrderTotalFraudChecker : BaseFraudChecker<decimal>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, decimal valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var result = context.ProductsSubtotal >= valueToCheck;
            if (result)
            {
                return
                    Task.FromResult(new CheckResult()
                    {
                        Result = true,
                        Reason = $"Product subtotal: {context.ProductsSubtotal:C} is higher than set in rule: {valueToCheck:C}"
                    });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, decimal valueToCheck, OrderReviewRuleDynamic reviewRule)
        {
            return true;
        }
    }
}