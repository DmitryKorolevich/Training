using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class DeliveredProductsAction: ComputableAction<OrderDataContext>
    {
        public DeliveredProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            return
                Task.FromResult(dataContext.SkuOrdereds.Union(dataContext.PromoSkus).Where(s =>
                    s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable ||
                    s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable).Sum(s => s.Amount*s.Quantity));
        }
    }
}
