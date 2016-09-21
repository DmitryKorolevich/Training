using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions.Products
{
    public class RefundDiscountableProductsAction : ComputableAction<OrderRefundDataContext>
    {
        public RefundDiscountableProductsAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, ITreeContext executionContext)
        {
            List<RefundSkuOrdered> skus = new List<RefundSkuOrdered>();
            var originalPromos = dataContext.Order.PromoSkus.ToDictionary(p => p.Sku.Id);
            foreach (var refundSku in dataContext.RefundOrder.RefundSkus)
            {
                PromoOrdered promo;
                if (originalPromos.TryGetValue(refundSku.Sku.Id, out promo))
                {
                    var newQuantity = refundSku.Quantity - promo.Quantity;
                    if (newQuantity > 0)
                    {
                        skus.Add(new RefundSkuOrdered
                        {
                            RefundValue = refundSku.RefundValue,
                            RefundPercent = refundSku.RefundPercent,
                            Quantity = newQuantity,
                            Sku = refundSku.Sku,
                            Redeem = refundSku.Redeem,
                            Messages = refundSku.Messages,
                            RefundPrice = refundSku.RefundPrice
                        });
                    }
                }
                else
                {
                    skus.Add(refundSku);
                }
            }
            if (dataContext.RefundOrder.Discount != null)
            {
                if (dataContext.RefundOrder.Discount.ExcludeCategories)
                {
                    if ((dataContext.RefundOrder.Discount.CategoryIds?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        // ReSharper disable once AssignNullToNotNullAttribute
                        HashSet<int> categories = new HashSet<int>(dataContext.RefundOrder.Discount.CategoryIds);
                        var excludedSkus =
                            skus.Where(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add(new MessageInfo()
                            {
                                MessageLevel = MessageLevel.Warning,
                                Message = "The discount for this product has been excluded by category"
                            });
                        }
                        if (excludedSkus.Length > 0)
                        {
                            return TaskCache<decimal>.DefaultCompletedTask;
                        }
                    }
                }
                else
                {
                    if ((dataContext.RefundOrder.Discount.CategoryIds?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        // ReSharper disable once AssignNullToNotNullAttribute
                        HashSet<int> categories = new HashSet<int>(dataContext.RefundOrder.Discount.CategoryIds);
                        var excludedSkus =
                            skus.Where(s => s.Sku.Product.CategoryIds.Any(c => !categories.Contains(c))).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add(
                                new MessageInfo()
                                {
                                    MessageLevel = MessageLevel.Warning,
                                    Message = "The discount for this product has been excluded by category"
                                });
                        }
                        if (excludedSkus.Length > 0)
                        {
                            return TaskCache<decimal>.DefaultCompletedTask;
                        }
                    }
                }
                if (dataContext.RefundOrder.Discount.ExcludeSkus)
                {
                    if ((dataContext.RefundOrder.Discount.SkusFilter?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        HashSet<int> filteredSkus =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            new HashSet<int>(dataContext.RefundOrder.Discount.SkusFilter.Select(s => s.IdSku));
                        var excludedSkus =
                            skus.Where(s => filteredSkus.Contains(s.Sku.Id)).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add(
                                new MessageInfo()
                                {
                                    MessageLevel = MessageLevel.Warning,
                                    Message = "The discount for this product has been excluded by SKU"
                                });
                        }
                        if (excludedSkus.Length > 0)
                        {
                            return TaskCache<decimal>.DefaultCompletedTask;
                        }
                    }
                }
                else
                {
                    if ((dataContext.RefundOrder.Discount.SkusFilter?.Count ?? 0) > 0)
                    {
                        HashSet<int> filteredSkus =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            new HashSet<int>(dataContext.RefundOrder.Discount.SkusFilter.Select(s => s.IdSku));
                        var excludedSkus =
                            skus.Where(s => !filteredSkus.Contains(s.Sku.Id)).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add(
                                new MessageInfo()
                                {
                                    MessageLevel = MessageLevel.Warning,
                                    Message = "The discount for this product has been excluded by SKU"
                                });
                        }
                        if (excludedSkus.Length > 0)
                        {
                            return TaskCache<decimal>.DefaultCompletedTask;
                        }
                    }
                }
                if ((dataContext.RefundOrder.Discount.SkusAppliedOnlyTo?.Count ?? 0) > 0)
                {
                    var selectedSkus = skus.IntersectKeyedWith(dataContext.RefundOrder.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
                        selectedSku => selectedSku.IdSku).ToList();
                    if (selectedSkus.Count == 0)
                    {
                        dataContext.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                    }
                    skus = selectedSkus;
                }
                if ((dataContext.RefundOrder.Discount.CategoryIdsAppliedOnlyTo?.Count ?? 0) > 0)
                {
                    if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                    {
                        throw new InvalidOperationException("Product doesn't have any categories set in object.");
                    }
                    // ReSharper disable once AssignNullToNotNullAttribute
                    HashSet<int> categories = new HashSet<int>(dataContext.RefundOrder.Discount.CategoryIdsAppliedOnlyTo);
                    var selectedSkus =
                        skus.Where(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c))).ToList();

                    if (selectedSkus.Count == 0)
                    {
                        dataContext.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                    }
                    skus = selectedSkus;
                }
            }
            var discountableProducts = skus.Where(s => !((bool?) s.Sku.SafeData.NonDiscountable ?? false)).ToArray();

            dataContext.SplitInfo.DiscountablePerishable =
                discountableProducts.Where(p => p.Sku.IdObjectType == (int) ProductType.Perishable).Sum(p => p.Amount*p.Quantity);

            dataContext.SplitInfo.DiscountableNonPerishable =
                discountableProducts.Where(p => p.Sku.IdObjectType == (int) ProductType.NonPerishable).Sum(p => p.Amount*p.Quantity);

            return Task.FromResult(discountableProducts.Sum(s => s.Amount*s.Quantity));
        }
    }
}