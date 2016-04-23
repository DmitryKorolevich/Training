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

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            var taxService = executionContext.Resolve<IAvalaraTax>();
            context.TaxTotal = (await
                Task.WhenAll(taxService.GetTax(context, TaxGetType.PerishableOnly),
                    taxService.GetTax(context, TaxGetType.NonPerishableOnly))).Sum();
            return context.TaxTotal;
        }
    }
}