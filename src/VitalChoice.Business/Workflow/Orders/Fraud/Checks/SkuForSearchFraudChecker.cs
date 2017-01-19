using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "SkuForSearch")]
    public class SkuForSearchFraudChecker : BaseFraudChecker<string>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var skus = valueToCheck.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim());
            if (skus.Any(sku => context.Order.Skus.Any(s => s.Sku.Code.StartsWith(sku, StringComparison.OrdinalIgnoreCase))))
            {
                return Task.FromResult(new CheckResult
                {
                    Result = true,
                    Reason = $"Order has one of the specified skus/code parts: ({valueToCheck})"
                });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            return !string.IsNullOrWhiteSpace(valueToCheck);
        }
    }
}