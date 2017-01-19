using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Fraud
{
    public struct CheckResult
    {
        public bool Result { get; set; }
        public string Reason { get; set; }

        public static implicit operator bool(CheckResult checkResult) => checkResult.Result;
    }

    public interface IFraudChecker
    {
        Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, object valueToCheck,
            OrderReviewRuleDynamic reviewRule);
    }

    public interface IFraudChecker<in T> : IFraudChecker
    {
        Task<CheckResult> CheckCondition(OrderDataContext context, ITreeContext executionContext, T valueToCheck, OrderReviewRuleDynamic reviewRule);
    }
}