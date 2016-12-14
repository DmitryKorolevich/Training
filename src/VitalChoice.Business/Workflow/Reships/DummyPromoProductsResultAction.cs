using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Reships
{
    public class DummyPromoProductsResultAction : ComputableAction<OrderDataContext>
    {
        public DummyPromoProductsResultAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}