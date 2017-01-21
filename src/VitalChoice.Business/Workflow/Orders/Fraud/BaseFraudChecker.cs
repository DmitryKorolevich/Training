using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud
{
    public abstract class BaseFraudChecker<T> : IFraudChecker<T>
    {
        public abstract Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, T valueToCheck,
            OrderReviewRuleDynamic reviewRule);

        public Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, object valueToCheck,
            OrderReviewRuleDynamic reviewRule)
            =>
                valueToCheck == null
                    ? TaskCache<CheckResult>.DefaultCompletedTask
                    : CheckCondition(context, executionContext, (T) valueToCheck, reviewRule);

        public bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, object valueToCheck,
            OrderReviewRuleDynamic reviewRule)
        {
            return valueToCheck != null && ShouldCheck(context, executionContext, (T) valueToCheck, reviewRule);
        }

        public abstract bool ShouldCheck(OrderDataContext context, ITreeContext executionContext, T valueToCheck,
            OrderReviewRuleDynamic reviewRule);
    }
}