﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            IEnumerable<RefundSkuOrdered> skus = dataContext.RefundSkus;
            if (dataContext.Order.Discount != null)
            {
                if (dataContext.Order.Discount.ExcludeCategories)
                {
                    if (dataContext.Order.Discount.CategoryIds?.Any() ?? false)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
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
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                else
                {
                    if (dataContext.Order.Discount.CategoryIds?.Any() ?? false)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
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
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                if (dataContext.Order.Discount.ExcludeSkus)
                {
                    if (dataContext.Order.Discount.SkusFilter?.Any() ?? false)
                    {
                        if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                        {
                            throw new InvalidOperationException("Product doesn't have any categories set in object.");
                        }
                        HashSet<int> filteredSkus =
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
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                else
                {
                    if (dataContext.Order.Discount.SkusFilter?.Any() ?? false)
                    {
                        HashSet<int> filteredSkus =
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
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                if (dataContext.Order.Discount.SkusAppliedOnlyTo?.Any() ?? false)
                {
                    var selectedSkus = skus.IntersectKeyedWith(dataContext.Order.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
                        selectedSku => selectedSku.IdSku).ToArray();
                    if (!selectedSkus.Any())
                    {
                        dataContext.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                    }
                    skus = selectedSkus;
                }
                if (dataContext.Order.Discount.CategoryIdsAppliedOnlyTo?.Any() ?? false)
                {
                    if (skus.Any(s => s.Sku.Product.CategoryIds == null))
                    {
                        throw new InvalidOperationException("Product doesn't have any categories set in object.");
                    }
                    HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIdsAppliedOnlyTo);
                    var selectedSkus =
                        skus.Where(s => s.Sku.Product.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                    
                    if (!selectedSkus.Any())
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
            return Task.FromResult(skus.Where(s => !((bool?)s.Sku.SafeData.NonDiscountable ?? false)).Sum(s => s.RefundPrice * (decimal)s.RefundPercent / 100 * s.Quantity));
        }
    }
}