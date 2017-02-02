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
    public class ReshipsRefundsCheck : BaseFraudChecker<OrderType>
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
                        if (refundCount >= qty)
                        {
                            return new CheckResult
                            {
                                Result = true,
                                Reason =
                                    $"Customer has {refundCount} Refund{(refundCount > 1 ? "s" : string.Empty)} for the last {monthCount} month{(monthCount > 1 ? "s" : string.Empty)}, Review Rule has quantity of {qty} Refund{(qty > 1 ? "s" : string.Empty)}"
                            };
                        }
                        break;
                    case OrderType.Reship:
                        var orderService = executionContext.Resolve<IOrderService>();
                        var reshipCount = await orderService.GetReshipCount(monthCount, context.Order.Customer.Id);
                        if (reshipCount >= qty)
                        {
                            return new CheckResult
                            {
                                Result = true,
                                Reason =
                                    $"Customer has {reshipCount} Reship{(reshipCount > 1 ? "s" : string.Empty)} for the last {monthCount} month{(monthCount > 1 ? "s" : string.Empty)}, Review Rule has quantity of {qty} Reship{(qty > 1 ? "s" : string.Empty)}"
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