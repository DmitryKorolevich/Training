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

namespace VitalChoice.Business.Workflow.Orders.Actions.Products
{
    public class DiscountableProductsAction : ComputableAction<OrderDataContext>
    {
        public DiscountableProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            var skus = dataContext.SkuOrdereds;
            if (dataContext.Order.Discount != null)
            {
                bool includedBySku = true;
                bool includedByCategory = true;
                //if (dataContext.Order.Discount.ExcludeCategories)
                //{
                //    if ((dataContext.Order.Discount.CategoryIds?.Count ?? 0) > 0)
                //    {
                //        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                //        {
                //            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                //        }
                //        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
                //        var excludedSkus =
                //            skus.Where(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                //        foreach (var sku in excludedSkus)
                //        {
                //            sku.Messages.Add(new MessageInfo()
                //            {
                //                MessageLevel = MessageLevel.Warning,
                //                Message = "The discount for this product has been excluded by category"
                //            });
                //        }
                //        if (excludedSkus.Length > 0)
                //        {
                //            return TaskCache<decimal>.DefaultCompletedTask;
                //        }
                //    }
                //}
                //else
                {
                    if ((dataContext.Order.Discount.CategoryIds?.Count ?? 0) > 0)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
                        includedByCategory = skus.Any(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c)));
                    }
                }
                //if (dataContext.Order.Discount.ExcludeSkus)
                //{
                //    if ((dataContext.Order.Discount.SkusFilter?.Count ?? 0) > 0)
                //    {
                //        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                //        {
                //            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                //        }
                //        HashSet<int> filteredSkus =
                //            // ReSharper disable once AssignNullToNotNullAttribute
                //            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
                //        var excludedSkus =
                //            skus.Where(s => filteredSkus.Contains(s.Sku.Id)).ToArray();
                //        foreach (var sku in excludedSkus)
                //        {
                //            sku.Messages.Add(
                //                new MessageInfo()
                //                {
                //                    MessageLevel = MessageLevel.Warning,
                //                    Message = "The discount for this product has been excluded by SKU"
                //                });
                //        }
                //        if (excludedSkus.Length > 0)
                //        {
                //            return TaskCache<decimal>.DefaultCompletedTask;
                //        }
                //    }
                //}
                //else
                {
                    if ((dataContext.Order.Discount.SkusFilter?.Count ?? 0) > 0)
                    {
                        HashSet<int> filteredSkus =
                            // ReSharper disable once AssignNullToNotNullAttribute
                            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
                        includedBySku = skus.Any(s => filteredSkus.Contains(s.Sku.Id));
                    }
                }
                if (!includedByCategory || !includedBySku)
                {
                    dataContext.Messages.Add(new MessageInfo
                    {
                        Message = "Cannot apply discount. Discount criteria not met.",
                        Field = "DiscountCode"
                    });
                    return TaskCache<decimal>.DefaultCompletedTask;
                }
                if ((dataContext.Order.Discount.SkusAppliedOnlyTo?.Count ?? 0) > 0)
                {
                    var selectedSkus = skus.IntersectKeyedWith(dataContext.Order.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
                        selectedSku => selectedSku.IdSku).ToArray();
                    if (selectedSkus.Length == 0)
                    {
                        dataContext.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                        return TaskCache<decimal>.DefaultCompletedTask;
                    }
                    foreach (var sku in selectedSkus)
                    {
                        sku.Messages.Add(new MessageInfo
                        {
                            Message = "Discount applied per SKU",
                            MessageLevel = MessageLevel.Info
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
                        skus.Where(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                    
                    if (selectedSkus.Length == 0)
                    {
                        dataContext.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                        return TaskCache<decimal>.DefaultCompletedTask;
                    }
                    foreach (var sku in selectedSkus)
                    {
                        sku.Messages.Add(new MessageInfo
                        {
                            Message = "Discount applied per category",
                            MessageLevel = MessageLevel.Info
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