using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Export
{
    public class ExportSetupPromoAction : ComputableAction<OrderDataContext>
    {
        public ExportSetupPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context,
            ITreeContext executionContext)
        {
            context.PromoSkus = context.Order.PromoSkus;
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}