using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.ActionResolvers;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Trees;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static void Configure(ITreeSetup<OrderContext, decimal> treeSetup)
        {
            treeSetup.Action<TotalAction>("Total", action =>
            {
                action.Dependency<ProductAction>();
                action.Dependency<DiscountTypeResolver>();
            });
            treeSetup.Action<ProductAction>("Products");
            treeSetup.Action<DiscountPercent>("PercentDiscount", action =>
            {
                action.Dependency<ProductAction>();
            });
            treeSetup.Action<DiscountPrice>("PriceDiscount", action =>
            {
                action.Dependency<ProductAction>();
            });
            treeSetup.ActionResolver<DiscountTypeResolver>("DiscountType", action =>
            {
                action.ResolvePath<DiscountPercent>((int) DiscountType.PercentDiscount, "PercentDiscount");
                action.ResolvePath<DiscountPrice>((int) DiscountType.PriceDiscount, "PriceDiscount");
            });
            treeSetup.Tree<OrdersTree>("Order", tree =>
            {
                tree.Dependency<TotalAction>();
            });
        }
    }
}