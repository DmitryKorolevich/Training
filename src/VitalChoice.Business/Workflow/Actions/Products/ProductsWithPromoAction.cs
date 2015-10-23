﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.Products
{
    public class ProductsWithPromoAction : ComputableAction<OrderDataContext>
    {
        public ProductsWithPromoAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            var promoAmount = dataContext.PromoSkus.Sum(p => p.Amount*p.Quantity);
            dataContext.ProductsSubtotal = dataContext.Data.Products + promoAmount;
            return Task.FromResult(promoAmount);
        }
    }
}
