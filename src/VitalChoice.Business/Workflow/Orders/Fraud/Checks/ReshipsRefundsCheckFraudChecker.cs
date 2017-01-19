using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 1)]
    [FraudFieldName(Name = "ReshipsRefundsCheckType")]
    public class ReshipsRefundsCheckFraudChecker : BaseFraudChecker<OrderType>
    {
        public override async Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext,
            OrderType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
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
                                Reason = $"Customer has {refundCount} Refund(s), Review Rule has quantity of {qty} Refund(s)"
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
                                Reason = $"Customer has {reshipCount} Reship(s), Review Rule has quantity of {qty} Reship(s)"
                            };
                        }
                        break;
                }
            }
            return default(CheckResult);
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, OrderType valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            return ((bool?) reviewRule.SafeData.ReshipsRefundsCheck ?? false) &&
                   (valueToCheck == OrderType.Refund || valueToCheck == OrderType.Reship);
        }
    }
}