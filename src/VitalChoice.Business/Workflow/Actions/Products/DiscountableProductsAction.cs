using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class DiscountableProductsAction : ComputableAction<OrderContext>
    {
        public DiscountableProductsAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            IEnumerable<SkuOrdered> skus = context.SkuOrdereds ?? context.Order.Skus;
            if (context.Order.Discount != null)
            {
                if (context.Order.Discount.ExcludeCategories)
                {
                    HashSet<int> categories = new HashSet<int>(context.Order.Discount.CategoryIds);
                    var excludedSkus =
                        skus.Where(s => s.ProductWithoutSkus.CategoryIds.Any(c => categories.Contains(c))).ToArray();
                    foreach (var sku in excludedSkus)
                    {
                        sku.Messages.Add("The discount for this product has been excluded by category");
                    }
                    if (excludedSkus.Any())
                    {
                        return 0;
                    }
                }
                if (context.Order.Discount.ExcludeSkus)
                {
                    HashSet<int> filteredSkus = new HashSet<int>(context.Order.Discount.SkusFilter.Select(s => s.IdSku));
                    var excludedSkus =
                        skus.Where(s => filteredSkus.Contains(s.Sku.Id)).ToArray();
                    foreach (var sku in excludedSkus)
                    {
                        sku.Messages.Add("The discount for this product has been excluded by SKU");
                    }
                    if (excludedSkus.Any())
                    {
                        return 0;
                    }
                }
                if (context.Order.Discount.SkusAppliedOnlyTo.Any())
                {
                    var selectedSkus = skus.IntersectKeyedWith(context.Order.Discount.SkusAppliedOnlyTo, sku => sku.Sku.Id,
                        selectedSku => selectedSku.IdSku);
                    if (!selectedSkus.Any())
                    {
                        context.Messages.Add(new MessageInfo
                        {
                            Message = "Cannot apply discount. Discountable products not found.",
                            Field = "DiscountCode"
                        });
                    }
                    skus = selectedSkus;
                }
            }
            return skus.Where(s => !s.Sku.Data.NonDiscountable).Sum(s => s.Amount * s.Quantity);
        }
    }
}