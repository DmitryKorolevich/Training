using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.GiftCertificates
{
    public class GiftCertificatesBuyAction : ComputableAction<OrderDataContext>
    {
        public GiftCertificatesBuyAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
            
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            if (context.CombinedStatus == OrderStatus.Incomplete || context.CombinedStatus == OrderStatus.OnHold)
            {
                context.GeneratedGcs = new List<GeneratedGiftCertificate>();
                return 0;
            }

            var gcsOrdered = context.Order.Skus.Where(
                        s => s.Sku.IdObjectType == (int)ProductType.EGс || s.Sku.IdObjectType == (int)ProductType.Gc)
                        .ToDictionary(s => s.Sku.Id);

            if (context.Order.Id != 0)
            {
                await SyncExistingWithExist(context, executionContext, gcsOrdered);
            }
            else
            {
                context.GeneratedGcs = new List<GeneratedGiftCertificate>();
                AddAll(context, gcsOrdered);
            }
            return 0;
        }

        private static async Task SyncExistingWithExist(OrderDataContext context, IWorkflowExecutionContext executionContext, Dictionary<int, SkuOrdered> gcsOrdered)
        {
            var orderService = executionContext.Resolve<IOrderService>();
            context.GeneratedGcs = await orderService.GetGeneratedGcs(context.Order.Id);
            if (context.GeneratedGcs.Any())
            {
                var groupedBySku =
                    context.GeneratedGcs.GroupBy(g => g.Sku.Id)
                        .ToDictionary(g => g.Key, g => g.Count());

                foreach (var sku in gcsOrdered)
                {
                    int count;
                    if (groupedBySku.TryGetValue(sku.Key, out count))
                    {
                        var numberToRemove = count - sku.Value.Quantity;
                        var numberToAdd = -numberToRemove;
                        if (numberToRemove > 0)
                        {
                            context.GeneratedGcs.RemoveAll(context.GeneratedGcs.Where(g => g.Sku.Id == sku.Key).Take(numberToRemove));
                        }
                        if (numberToAdd > 0)
                        {
                            context.GeneratedGcs.AddRange(Enumerable.Repeat(new GeneratedGiftCertificate
                            {
                                Sku = sku.Value.Sku,
                                Balance =
                                    context.Order.Customer.IdObjectType == (int) CustomerType.Wholesale
                                        ? sku.Value.Sku.WholesalePrice
                                        : sku.Value.Sku.Price
                            }, numberToAdd));
                        }
                    }
                    else
                    {
                        context.GeneratedGcs.AddRange(Enumerable.Repeat(new GeneratedGiftCertificate
                        {
                            Sku = sku.Value.Sku,
                            Balance =
                                context.Order.Customer.IdObjectType == (int) CustomerType.Wholesale
                                    ? sku.Value.Sku.WholesalePrice
                                    : sku.Value.Sku.Price
                        }, sku.Value.Quantity));
                    }
                }

                foreach (var grouped in groupedBySku)
                {
                    if (!gcsOrdered.ContainsKey(grouped.Key))
                    {
                        context.GeneratedGcs.RemoveAll(g => g.Sku.Id == grouped.Key);
                    }
                }
            }
            else
            {
                AddAll(context, gcsOrdered);
            }
        }

        private static void AddAll(OrderDataContext context, Dictionary<int, SkuOrdered> gcsOrdered)
        {
            context.GeneratedGcs.AddRange(gcsOrdered.SelectMany(sku => Enumerable.Repeat(new GeneratedGiftCertificate
            {
                Sku = sku.Value.Sku,
                Balance =
                    context.Order.Customer.IdObjectType == (int) CustomerType.Wholesale
                        ? sku.Value.Sku.WholesalePrice
                        : sku.Value.Sku.Price
            }, sku.Value.Quantity)));
        }
    }
}