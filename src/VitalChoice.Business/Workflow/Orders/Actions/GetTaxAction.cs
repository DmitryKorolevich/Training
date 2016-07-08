using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
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
                var pGetType = TaxGetType.Perishable;
                if (context.Order.POrderStatus.HasValue && context.Order.POrderStatus.Value == OrderStatus.Shipped)
                {
                    pGetType = pGetType | TaxGetType.Commit | TaxGetType.SavePermanent;
                }
                var npGetType = TaxGetType.NonPerishable;
                if (context.Order.NPOrderStatus.HasValue && context.Order.NPOrderStatus.Value == OrderStatus.Shipped)
                {
                    npGetType = npGetType | TaxGetType.Commit | TaxGetType.SavePermanent;
                }
                await Task.WhenAll(Task.Run(async () =>
                {
                    context.SplitInfo.PerishableTax = await taxService.GetTax(context, pGetType);
                }), Task.Run(async () =>
                {
                    context.SplitInfo.NonPerishableTax = await taxService.GetTax(context, npGetType);
                }));
                context.TaxTotal = context.SplitInfo.PerishableTax + context.SplitInfo.NonPerishableTax;
            }
            else
            {
                if (context.Order.OrderStatus.HasValue && context.Order.OrderStatus.Value == OrderStatus.Shipped)
                {
                    context.TaxTotal = await taxService.GetTax(context, TaxGetType.UseBoth | TaxGetType.Commit | TaxGetType.SavePermanent);
                }
                else
                {
                    context.TaxTotal = await taxService.GetTax(context);
                }
            }
            return context.TaxTotal;
        }
    }
}