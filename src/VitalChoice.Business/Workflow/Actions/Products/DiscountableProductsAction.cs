using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class DiscountableProductsAction : ComputableAction<OrderDataContext>
    {
        public DiscountableProductsAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            IEnumerable<SkuOrdered> skus = dataContext.SkuOrdereds;
            if (dataContext.Order.Discount != null)
            {
                if (dataContext.Order.Discount.ExcludeCategories)
                {
                    if (dataContext.Order.Discount.CategoryIds.Any())
                    {
                        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
                        var excludedSkus =
                            skus.Where(s => s.ProductWithoutSkus.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add("The discount for this product has been excluded by category");
                        }
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                else
                {
                    if (dataContext.Order.Discount.CategoryIds.Any())
                    {
                        HashSet<int> categories = new HashSet<int>(dataContext.Order.Discount.CategoryIds);
                        var excludedSkus =
                            skus.Where(s => s.ProductWithoutSkus.CategoryIds.Any(c => !categories.Contains(c))).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add("The discount for this product has been excluded by category");
                        }
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                if (dataContext.Order.Discount.ExcludeSkus)
                {
                    if (dataContext.Order.Discount.SkusFilter.Any())
                    {
                        HashSet<int> filteredSkus =
                            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
                        var excludedSkus =
                            skus.Where(s => filteredSkus.Contains(s.Sku.Id)).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add("The discount for this product has been excluded by SKU");
                        }
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                else
                {
                    if (dataContext.Order.Discount.SkusFilter.Any())
                    {
                        HashSet<int> filteredSkus =
                            new HashSet<int>(dataContext.Order.Discount.SkusFilter.Select(s => s.IdSku));
                        var excludedSkus =
                            skus.Where(s => !filteredSkus.Contains(s.Sku.Id)).ToArray();
                        foreach (var sku in excludedSkus)
                        {
                            sku.Messages.Add("The discount for this product has been excluded by SKU");
                        }
                        if (excludedSkus.Any())
                        {
                            return Task.FromResult<decimal>(0);
                        }
                    }
                }
                if (dataContext.Order.Discount.SkusAppliedOnlyTo.Any())
                {
                    var selectedSkus = skus.IntersectKeyedWith(dataContext.Order.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
                        selectedSku => selectedSku.IdSku);
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
            return Task.FromResult(skus.Where(s => !s.Sku.Data.NonDiscountable).Sum(s => s.Amount*s.Quantity));
        }
    }
}