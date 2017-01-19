using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud
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
    }

    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "Guest")]
    public class GuestFraudChecker : BaseFraudChecker<bool>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, bool valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            if (((bool?) context.Order.Customer?.SafeData.Guest ?? false) == valueToCheck)
            {
                return
                    Task.FromResult(new CheckResult
                    {
                        Result = true,
                        Reason = $"Customer is {(valueToCheck ? "Guest" : "Not Guest")}"
                    });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "DeliveryInstructionForSearch")]
    public class DeliveryInstructionForSearchFraudChecker : BaseFraudChecker<string>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, string valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            var words = valueToCheck.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim());
            var deliveryInstructions = (string) context.Order.ShippingAddress?.SafeData.DeliveryInstructions;
            if (!string.IsNullOrEmpty(deliveryInstructions) &&
                words.Any(w => deliveryInstructions.IndexOf(w, StringComparison.OrdinalIgnoreCase) > -1))
            {
                return
                    Task.FromResult(new CheckResult
                    {
                        Result = true,
                        Reason = $"Delivery Instructions has one of specified words: ({valueToCheck})"
                    });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "CompareNamesType")]
    public class CompareNamesTypeFraudChecker : BaseFraudChecker<CompareType>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, CompareType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            if ((bool?) reviewRule.SafeData.CompareNames ?? false)
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
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "CompareAddressesType")]
    public class CompareAddressesTypeFraudChecker : BaseFraudChecker<CompareType>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, CompareType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            if ((bool?) reviewRule.SafeData.CompareAddresses ?? false)
            {
                switch (valueToCheck)
                {
                    case CompareType.Equal:
                        if (string.Equals((string) context.Order.PaymentMethod.Address.Data.Address1,
                                (string) context.Order.ShippingAddress.Data.Address1, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals((string) context.Order.PaymentMethod.Address.Data.Address2,
                                (string) context.Order.ShippingAddress.Data.Address2, StringComparison.OrdinalIgnoreCase) &&
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
                            !string.Equals((string) context.Order.PaymentMethod.Address.Data.Address2,
                                (string) context.Order.ShippingAddress.Data.Address2, StringComparison.OrdinalIgnoreCase) ||
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
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

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
                        Reason = $"Order has one of the specified zip codes: {valueToCheck}"
                    });
                }
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

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
                    Reason = $"Order has one of the specified skus/code parts: {valueToCheck}"
                });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }
    }

    [FraudPriority(Priority = 1)]
    [FraudFieldName(Name = "ReshipsRefundsCheckType")]
    public class ReshipsRefundsCheckFraudChecker : BaseFraudChecker<OrderType>
    {
        public override async Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, OrderType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            if ((bool?) reviewRule.SafeData.ReshipsRefundsCheck ?? false)
            {
                var qty = (int?) reviewRule.SafeData.ReshipsRefundsQTY ?? 0;
                var monthCount = (int?) reviewRule.SafeData.ReshipsRefundsMonthCount ?? 0;
                if (qty > 0 && monthCount > 0)
                {
                    switch (valueToCheck)
                    {
                        case OrderType.Refund:
                            var refundService = executionContext.Resolve<IOrderRefundService>();
                            var refundCount = await refundService.GetRefundCount(monthCount, context.Order.Customer.Id);
                            if (qty >= refundCount)
                            {
                                return new CheckResult
                                {
                                    Result = true,
                                    Reason = $"Customer has {refundCount} Refunds but allowed only {qty}"
                                };
                            }
                            break;
                        case OrderType.Reship:
                            var orderService = executionContext.Resolve<IOrderService>();
                            var reshipCount = await orderService.GetReshipCount(monthCount, context.Order.Customer.Id);
                            if (qty >= reshipCount)
                            {
                                return new CheckResult
                                {
                                    Result = true,
                                    Reason = $"Customer has {reshipCount} Reships but allowed only {qty}"
                                };
                            }
                            break;
                    }
                }
            }
            return default(CheckResult);
        }
    }
}