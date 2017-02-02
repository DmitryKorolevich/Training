using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud.Checks
{
    [FraudPriority(Priority = 0)]
    [FraudFieldName(Name = "Guest")]
    public class GuestFraudChecker : BaseFraudChecker<bool>
    {
        public override Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, bool valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            if (((bool?) context.Order.Customer?.SafeData.Guest ?? false) &&
                (int?) context.Order.SafeData.OrderType == (int) SourceOrderType.Web)
            {
                return
                    Task.FromResult(new CheckResult
                    {
                        Result = true,
                        Reason = "Guest checkout"
                    });
            }
            return TaskCache<CheckResult>.DefaultCompletedTask;
        }

        public override bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, bool valueToCheck,
            OrderReviewRuleDynamic reviewRule) => valueToCheck;
    }
}