﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.ActionResolvers;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Actions.Discounts;
using VitalChoice.Business.Workflow.Actions.Products;
using VitalChoice.Business.Workflow.Actions.Shipping;
using VitalChoice.Business.Workflow.Trees;
using VitalChoice.Domain.Entities.eCommerce.Customers;
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
                action.Aggregate<ProductsWithPromoAction>();
                action.Aggregate<DiscountTypeActionResolver>();
                action.Aggregate<ShippingStandardResolver>();
                action.Aggregate<ShippingSurchargeResolver>();
                action.Aggregate<ShippingUpgradesActionResolver>();
                action.Aggregate<ShippingOverrideAction>();
                action.Aggregate<ShippingSurchargeOverrideAction>();
            });

            treeSetup.Action<ProductAction>("Products");

            treeSetup.Action<ProductsWithPromoAction>("PromoProducts", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();

                action.Aggregate<ProductAction>();
            });

            treeSetup.Action<DiscountPercentAction>("PercentDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            treeSetup.Action<DiscountPriceAction>("PriceDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            treeSetup.Action<DiscountableProductsAction>("DiscountableSubtotal", action =>
            {
                action.Dependency<ProductAction>();
            });

            treeSetup.Action<DiscountTieredAction>("TieredDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            treeSetup.Action<DiscountFreeShippingAction>("FreeShippingDiscount");

            treeSetup.Action<DiscountThresholdAction>("ThresholdDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            treeSetup.Action<PerishableProductsAction>("PerishableSubtotal", action =>
            {
                action.Dependency<ProductAction>();
            });

            treeSetup.Action<DeliveredProductsAction>("DeliveredAmount", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
            });

            treeSetup.Action<StandardShippingUsWholesaleAction>("StandardWholesaleShipping", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();
                action.Dependency<DeliveredProductsAction>();
            });

            treeSetup.Action<StandardShippingUsCaRetailAction>("StandardRetailShipping", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();
                action.Dependency<DeliveredProductsAction>();
            });

            treeSetup.Action<ShippingSurchargeUsAkHiAction>("ShippingSurchargeUs", action =>
            {
                action.Dependency<DeliveredProductsAction>();
            });

            treeSetup.Action<ShippingSurchargeCaAction>("ShippingSurchargeCa", action =>
            {
                action.Dependency<DeliveredProductsAction>();
            });

            treeSetup.Action<ProductsSplitAction>("SplitAction", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
            });

            treeSetup.Action<ShippingUpgradesUsCaAction>("ShippingUpgradeUsCa", action =>
            {
                action.Dependency<ProductsSplitAction>();
            });

            treeSetup.ActionResolver<ShippingUpgradesActionResolver>("ShippingUpgrade", action =>
            {
                action.ResolvePath<ShippingUpgradesUsCaAction>((int) ShippingUpgradeGroup.UsCa, "ShippingUpgradeUsCa");
            });

            treeSetup.ActionResolver<ShippingSurchargeResolver>("ShippingSurcharge", action =>
            {
                action.ResolvePath<ShippingSurchargeUsAkHiAction>((int) SurchargeType.AlaskaHawaii,
                    "ShippingSurchargeUs");
                action.ResolvePath<ShippingSurchargeCaAction>((int) SurchargeType.Canada,
                    "ShippingSurchargeCa");
            });

            treeSetup.ActionResolver<DiscountTypeActionResolver>("Discount", action =>
            {
                action.Dependency<PerishableProductsAction>();

                action.ResolvePath<DiscountPercentAction>((int) DiscountType.PercentDiscount, "PercentDiscount");
                action.ResolvePath<DiscountPriceAction>((int) DiscountType.PriceDiscount, "PriceDiscount");
                action.ResolvePath<DiscountTieredAction>((int) DiscountType.Tiered, "TieredDiscount");
                action.ResolvePath<DiscountFreeShippingAction>((int) DiscountType.FreeShipping, "FreeShippingDiscount");
                action.ResolvePath<DiscountThresholdAction>((int) DiscountType.Threshold, "ThresholdDiscount");
            });

            treeSetup.ActionResolver<ShippingStandardResolver>("StandardShipping", action =>
            {
                action.ResolvePath<StandardShippingUsWholesaleAction>((int) CustomerType.Wholesale, "StandardWholesaleShipping");
                action.ResolvePath<StandardShippingUsCaRetailAction>((int) CustomerType.Retail, "StandardRetailShipping");
            });

            treeSetup.Action<ShippingOverrideAction>("ShippingOverride", action =>
            {
                action.Dependency<ShippingStandardResolver>();
                action.Dependency<ShippingUpgradesActionResolver>();
            });

            treeSetup.Action<ShippingSurchargeOverrideAction>("SurchargeOverride", action =>
            {
                action.Dependency<ShippingSurchargeResolver>();
            });

            treeSetup.Tree<OrderTree>("Order", tree =>
            {
                tree.Dependency<TotalAction>();
            });
        }
    }
}