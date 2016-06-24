using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class GetTaxAction : ComputableAction<OrderDataContext>
    {
        public GetTaxAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            var taxService = executionContext.Resolve<IAvalaraTax>();
            if (context.SplitInfo.ShouldSplit)
            {
                context.SplitInfo.PerishableTax = await taxService.GetTax(context, TaxGetType.Perishable);
                context.SplitInfo.NonPerishableTax = await taxService.GetTax(context, TaxGetType.NonPerishable);
                context.TaxTotal = context.SplitInfo.PerishableTax + context.SplitInfo.NonPerishableTax;
            }
            else
            {
                context.TaxTotal = await taxService.GetTax(context);
            }
            return context.TaxTotal;
        }
    }
}