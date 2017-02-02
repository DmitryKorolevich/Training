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
    [FraudFieldName(Name = "CompareAddressesType")]
    public class CompareAddressesTypeFraudChecker : BaseFraudChecker<CompareType>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, CompareType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var billingAddress2 = (string) context.Order.PaymentMethod.Address.SafeData.Address2 ?? string.Empty;
            var shippingAddress2 = (string) context.Order.ShippingAddress.SafeData.Address2 ?? string.Empty;
            switch (valueToCheck)
            {
                case CompareType.Equal:
                    if (string.Equals((string) context.Order.PaymentMethod.Address.Data.Address1,
                            (string) context.Order.ShippingAddress.Data.Address1, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(billingAddress2, shippingAddress2, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((string) context.Order.PaymentMethod.Address.Data.City,
                            (string) context.Order.ShippingAddress.Data.City, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult(new CheckResult
                        {
                            Result = true,
                            Reason = "Order has the same billing and shipping addresses"
                        });
                    }
                    return TaskCache<CheckResult>.DefaultCompletedTask;
                case CompareType.NotEqual:
                    if (!string.Equals((string) context.Order.PaymentMethod.Address.Data.Address1,
                            (string) context.Order.ShippingAddress.Data.Address1, StringComparison.OrdinalIgnoreCase) ||
                        !string.Equals(billingAddress2, shippingAddress2, StringComparison.OrdinalIgnoreCase) ||
                        !string.Equals((string) context.Order.PaymentMethod.Address.Data.City,
                            (string) context.Order.ShippingAddress.Data.City, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult(new CheckResult
                        {
                            Result = true,
                            Reason = "Order has different billing and shipping addresses"
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
            return (bool?) reviewRule.SafeData.CompareAddresses ?? false;
        }
    }
}