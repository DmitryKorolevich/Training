using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.ActionResolvers;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Actions.Discounts;
using VitalChoice.Business.Workflow.Actions.Products;
using VitalChoice.Business.Workflow.Trees;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static void Configure(ITreeSetup<OrderContext, decimal> treeSetup)
        {
            treeSetup.Action<TotalAction>("Total", action =>
            {
                action.Dependency<ProductAction>();
                action.Dependency<DiscountTypeActionResolver>();
            });
            treeSetup.Action<ProductAction>("Products");
            treeSetup.Action<DiscountPercentAction>("PercentDiscount");
            treeSetup.Action<DiscountPriceAction>("PriceDiscount");
            treeSetup.Action<DiscountableProductsAction>("DiscountableSubtotal");
            treeSetup.Action<DiscountTieredAction>("TieredDiscount");
            treeSetup.Action<PerishableProductsAction>("PerishableSubtotal");
            treeSetup.ActionResolver<DiscountTypeActionResolver>("Discount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
                action.Dependency<PerishableProductsAction>();

                action.ResolvePath<DiscountPercentAction>((int) DiscountType.PercentDiscount, "PercentDiscount");
                action.ResolvePath<DiscountPriceAction>((int) DiscountType.PriceDiscount, "PriceDiscount");
                action.ResolvePath<DiscountTieredAction>((int)DiscountType.Tiered, "TieredDiscount");
            });
            treeSetup.Tree<OrderTree>("Order", tree =>
            {
                tree.Dependency<TotalAction>();
            });
        }
    }
}