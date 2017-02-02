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
    [FraudFieldName(Name = "ZipForSearch")]
    public class ZipForSearchFraudChecker : BaseFraudChecker<string>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var zips = valueToCheck.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim());
            var shippingZip = (string) context.Order.ShippingAddress?.SafeData.Zip;
            var billingZip = (string) context.Order.PaymentMethod.Address?.SafeData.Zip;
            if (!string.IsNullOrEmpty(shippingZip) && !string.IsNullOrEmpty(billingZip))
            {
                if (zips.Any(
                    w =>
                        string.Equals(shippingZip, w, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(billingZip, w, StringComparison.OrdinalIgnoreCase)))
                {
                    return Task.FromResult(new CheckResult
                    {
                        Result = true,
                        Reason = $"Order has one of the specified zip codes: ({valueToCheck})"
                    });
                }
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, string valueToCheck, OrderReviewRuleDynamic reviewRule)
        {
            return !string.IsNullOrWhiteSpace(valueToCheck);
        }
    }
}