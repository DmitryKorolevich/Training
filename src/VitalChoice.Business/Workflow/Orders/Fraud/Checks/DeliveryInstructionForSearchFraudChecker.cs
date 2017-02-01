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
    [FraudFieldName(Name = "DeliveryInstructionForSearch")]
    public class DeliveryInstructionForSearchFraudChecker : BaseFraudChecker<string>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var words = valueToCheck.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim());
            var deliveryInstructions = (string) context.Order.ShippingAddress?.SafeData.DeliveryInstructions;
            if (deliveryInstructions != null &&
                words.Any(w => deliveryInstructions.IndexOf(w, StringComparison.OrdinalIgnoreCase) > -1))
            {
                return
                    Task.FromResult(new CheckResult
                    {
                        Result = true,
                        Reason = $"Delivery Instructions has one of specified phrases: ({valueToCheck})"
                    });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var deliveryInstructions = (string) context.Order.ShippingAddress?.SafeData.DeliveryInstructions;
            return !string.IsNullOrEmpty(deliveryInstructions);
        }
    }
}