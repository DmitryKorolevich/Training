using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "CompareNamesType")]
    public class CompareNamesTypeFraudChecker : BaseFraudChecker<CompareType>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, CompareType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            switch (valueToCheck)
            {
                case CompareType.Equal:
                    if (string.Equals((string) context.Order.PaymentMethod.Address.Data.FirstName,
                            (string) context.Order.ShippingAddress.Data.FirstName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((string) context.Order.PaymentMethod.Address.Data.LastName,
                            (string) context.Order.ShippingAddress.Data.LastName, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult(new CheckResult
                        {
                            Result = true,
                            Reason = "Order has the same billing and shipping names"
                        });
                    }
                    return TaskCache<CheckResult>.DefaultCompletedTask;
                case CompareType.NotEqual:
                    if (!string.Equals((string) context.Order.PaymentMethod.Address.Data.FirstName,
                            (string) context.Order.ShippingAddress.Data.FirstName, StringComparison.OrdinalIgnoreCase) ||
                        !string.Equals((string) context.Order.PaymentMethod.Address.Data.LastName,
                            (string) context.Order.ShippingAddress.Data.LastName, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult(new CheckResult
                        {
                            Result = true,
                            Reason = "Order has different billing and shipping names"
                        });
                    }
                    return TaskCache<CheckResult>.DefaultCompletedTask;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueToCheck), valueToCheck, null);
            }
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, CompareType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            return (bool?) reviewRule.SafeData.CompareNames ?? false;
        }
    }
}