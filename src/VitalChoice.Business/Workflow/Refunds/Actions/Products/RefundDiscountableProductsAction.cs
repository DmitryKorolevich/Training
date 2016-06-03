using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
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

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            List<RefundSkuOrdered> skus = new List<RefundSkuOrdered>();
            var originalPromos = dataContext.Order.OriginalOrder.PromoSkus.ToDictionary(p => p.Sku.Id);
            foreach (var refundSku in dataContext.Order.RefundSkus)
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
            if (dataContext.Order.Discount != null)
            {
                if (dataContext.Order.Discount.ExcludeCategories)
                {
                    if ((dataContext.Order.Discount.CategoryIds?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        // ReSharper disable once AssignNullToNotNullAttribute
                        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
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
                    if ((dataContext.Order.Discount.CategoryIds?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        // ReSharper disable once AssignNullToNotNullAttribute
                        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
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
                if (dataContext.Order.Discount.ExcludeSkus)
                {
                    if ((dataContext.Order.Discount.SkusFilter?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        HashSet<int> filteredSkus =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
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
                    if ((dataContext.Order.Discount.SkusFilter?.Count ?? 0) > 0)
                    {
                        HashSet<int> filteredSkus =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
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
                if ((dataContext.Order.Discount.SkusAppliedOnlyTo?.Count ?? 0) > 0)
                {
                    var selectedSkus = skus.IntersectKeyedWith(dataContext.Order.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
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
                if ((dataContext.Order.Discount.CategoryIdsAppliedOnlyTo?.Count ?? 0) > 0)
                {
                    if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                    {
                        throw new InvalidOperationException("Product doesn't have any categories set in object.");
                    }
                    // ReSharper disable once AssignNullToNotNullAttribute
                    HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIdsAppliedOnlyTo);
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
            return
                Task.FromResult(
                    skus.Where(s => !((bool?) s.Sku.SafeData.NonDiscountable ?? false))
                        .Sum(s => s.RefundPrice*(decimal) s.RefundPercent/100*s.Quantity));
        }
    }
}