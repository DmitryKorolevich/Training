﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductsSplitAction : ComputableAction<OrderDataContext>
    {
        public ProductsSplitAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var products = dataContext.SkuOrdereds.Union(dataContext.PromoSkus).ToArray();
            var perishableProducts =
                products.Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.Perishable).ToArray();
            var nonPerishableProducts =
                products.Where(s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable).ToArray();
            dataContext.SplitInfo.PerishableCount = perishableProducts.Length;
            dataContext.SplitInfo.NonPerishableCount = nonPerishableProducts.Length;
            dataContext.SplitInfo.PerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            dataContext.SplitInfo.NonPerishableAmount = perishableProducts.Sum(p => p.Amount*p.Quantity);
            dataContext.SplitInfo.NonPerishableOrphanCount =
                products.Count(
                    s => s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType);
            dataContext.SplitInfo.ThresholdReached =
                products.Any(
                    s =>
                        s.ProductWithoutSkus.IdObjectType == (int) ProductType.NonPerishable && s.Sku.Data.OrphanType &&
                        s.Sku.Data.QTYThreshold >= s.Quantity);
            dataContext.SplitInfo.SpecialSkuAdded = products.Any(s => s.Sku.Code.ToLowerInvariant() == "emp");

            //TODO: move NonPerishableOrphanCount threshold to global admin config
            dataContext.SplitInfo.ShouldSplit = (dataContext.SplitInfo.PerishableCount > 0 &&
                                             dataContext.SplitInfo.NonPerishableOrphanCount > 4
                                             ||
                                             dataContext.SplitInfo.PerishableCount > 0 && dataContext.SplitInfo.ThresholdReached
                                             ||
                                             dataContext.SplitInfo.NonPerishableNonOrphanCount > 0 &&
                                             dataContext.SplitInfo.PerishableCount > 0)
                                            && !dataContext.SplitInfo.SpecialSkuAdded;
            if (dataContext.SplitInfo.ShouldSplit)
                dataContext.SplitInfo.ProductTypes = POrderType.PNP;
            else
            {
                if (dataContext.SplitInfo.PerishableCount > 0)
                    dataContext.SplitInfo.ProductTypes = POrderType.P;
                else if (dataContext.SplitInfo.NonPerishableCount > 0)
                    dataContext.SplitInfo.ProductTypes = POrderType.NP;
                else
                    dataContext.SplitInfo.ProductTypes = POrderType.Other;
            }
            return Task.FromResult<decimal>(0);
        }
    }
}
