﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Promo
{
    public class CategoryPromoAction : ComputableAction<OrderDataContext>
    {
        public CategoryPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context,
            IWorkflowExecutionContext executionContext)
        {
            IEnumerable<PromotionDynamic> eligiable;
            if (context.Order.Discount != null && context.Order.Discount.Id != 0)
            {
                eligiable = context.Promotions.Where(p => p.IdObjectType == (int)PromotionType.CategoryDiscount && (bool)p.Data.CanUseWithDiscount);
            }
            else
            {
                eligiable = context.Promotions.Where(p => p.IdObjectType == (int)PromotionType.CategoryDiscount);
            }
            foreach (var promo in eligiable)
            {
                HashSet<int> promoCategories = new HashSet<int>(promo.SelectedCategoryIds);
                foreach (var sku in context.Order.Skus)
                {
                    if (sku.Sku.Product.CategoryIds == null)
                    {
                        throw new InvalidOperationException("Product doesn't have any categories set in object.");
                    }
                    if (sku.Sku.Product.CategoryIds.Any(category => promoCategories.Contains(category)))
                    {
                        if (sku.Amount == sku.Sku.Price)
                        {
                            sku.Amount = sku.Sku.Price - sku.Sku.Price*(decimal) promo.Data.Percent/100;
                        }
                    }
                }
            }
            return Task.FromResult<decimal>(0);
        }
    }
}