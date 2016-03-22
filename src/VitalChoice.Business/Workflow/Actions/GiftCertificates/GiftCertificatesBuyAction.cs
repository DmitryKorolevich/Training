﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
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
                return 0;
            }

            var gcService = executionContext.Resolve<IGcService>();
            var orderService = executionContext.Resolve<IOrderService>();

            if (context.Order.Id != 0)
            {
                await SyncExistingWithExist(context, gcService, orderService);
            }
            else
            {
                await AddAll(context, gcService);
            }
            return 0;
        }

        private static async Task SyncExistingWithExist(OrderDataContext context, IGcService gcService, IOrderService orderService)
        {
            var ordereds = await orderService.GetGeneratedGcs(context.Order.Id);
            if (ordereds.Any())
            {
                var groupedBySku =
                    ordereds.ToDictionary(g => g.Sku.Id);

                var gcsOrdered =
                    context.SkuOrdereds.Where(s => s.Sku.IdObjectType == (int) ProductType.EGс || s.Sku.IdObjectType == (int) ProductType.Gc);

                //Merge on existing skus in old and new order instance, removed tracked automatically
                foreach (var sku in gcsOrdered)
                {
                    if (sku.GcsGenerated == null)
                    {
                        sku.GcsGenerated = new List<GiftCertificate>();
                    }
                    SkuOrdered ordered;
                    if (groupedBySku.TryGetValue(sku.Sku.Id, out ordered))
                    {
                        sku.GcsGenerated = ordered.GcsGenerated ?? new List<GiftCertificate>();
                        var numberToRemove = sku.GcsGenerated.Count - sku.Quantity;
                        var numberToAdd = -numberToRemove;
                        if (numberToRemove > 0)
                        {
                            sku.GcsGenerated.RemoveAll(sku.GcsGenerated.Take(numberToRemove).ToList());
                        }
                        if (numberToAdd > 0)
                        {
                            sku.GcsGenerated.AddRange(await CreateGiftCertificates(sku, numberToAdd, context, gcService));
                        }
                    }
                    else
                    {
                        sku.GcsGenerated.AddRange(await CreateGiftCertificates(sku, sku.Quantity, context, gcService));
                    }
                }
            }
            else
            {
                await AddAll(context, gcService);
            }
        }

        private static async Task<List<GiftCertificate>> CreateGiftCertificates(SkuOrdered sku, int count, OrderDataContext context,
            IGcService gcService)
        {
            List<GiftCertificate> result = new List<GiftCertificate>();
            for (int i = 0; i < count; i++)
            {
                result.Add(
                    new GiftCertificate
                    {
                        IdSku = sku.Sku.Id,
                        Balance = context.Order.Customer.IdObjectType == (int) CustomerType.Wholesale
                            ? sku.Sku.WholesalePrice
                            : sku.Sku.Price,
                        Code = await gcService.GenerateGCCode(),
                        Email = context.Order.Customer?.Email,
                        FirstName = context.Order.Customer?.ProfileAddress.SafeData.FirstName,
                        LastName = context.Order.Customer?.ProfileAddress.SafeData.LastName,
                        IdOrder = context.Order.Id == 0 ? null : (int?) context.Order.Id,
                        GCType = sku.Sku.IdObjectType == (int) ProductType.EGс ? GCType.EGC : GCType.GC,
                        PublicId = Guid.NewGuid(),
                        Created = DateTime.Now,
                        StatusCode = RecordStatusCode.Active,
                        UserId = context.Order.Customer?.Id
                    });
            }
            return result;
        }

        private static async Task AddAll(OrderDataContext context, IGcService gcService)
        {
            await context.SkuOrdereds.Where(s => s.Sku.IdObjectType == (int) ProductType.EGс || s.Sku.IdObjectType == (int) ProductType.Gc)
                .ForEachAsync(async sku =>
                {
                    sku.GcsGenerated = await CreateGiftCertificates(sku, sku.Quantity, context, gcService);
                });
        }
    }
}