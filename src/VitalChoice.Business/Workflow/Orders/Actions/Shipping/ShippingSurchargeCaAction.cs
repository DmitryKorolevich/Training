using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingSurchargeCaAction : ComputableAction<OrderDataContext>
    {
        public ShippingSurchargeCaAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.CanadaSurcharge = 0;
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}
