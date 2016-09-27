using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class ProductAction : ComputableAction<OrderDataContext>
    {
        public ProductAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.SkuOrdereds = dataContext.Order.Skus.Where(s => s.Quantity > 0).ToList();
            foreach (var sku in dataContext.SkuOrdereds)
            {
                if (!(sku.Sku.SafeData.DisregardStock ?? true))
                {
                    if (sku.Sku.SafeData.Stock < sku.Quantity)
                    {
                        sku.Messages.Add(
                            new MessageInfo
                            {
                                MessageLevel = MessageLevel.Error,
                                Message = "Сurrently out of stock. Please remove to continue."
                            });
                    }
                }
                if (sku.Sku.Product?.StatusCode == (int) RecordStatusCode.NotActive ||
                    sku.Sku.StatusCode == (int) RecordStatusCode.NotActive)
                {
                    sku.Messages.Add(
                        new MessageInfo
                        {
                            MessageLevel = MessageLevel.Error,
                            Message = "Currently not active. Please remove to continue."
                        });
                }
            }
            return Task.FromResult(dataContext.SkuOrdereds.Sum(s => s.Amount*s.Quantity));
        }
    }
}