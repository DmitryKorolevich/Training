using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.ActionResolvers;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Actions.Discounts;
using VitalChoice.Business.Workflow.Actions.GiftCertificates;
using VitalChoice.Business.Workflow.Actions.Products;
using VitalChoice.Business.Workflow.Actions.Shipping;
using VitalChoice.Business.Workflow.Actions.Tax;
using VitalChoice.Business.Workflow.Trees;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static void Configure(ITreeSetup<OrderDataContext, decimal> setup)
        {
            setup.Action<TotalAction>("Total", action =>
            {
                action.Aggregate<OrderSubTotalAction>();
                action.Aggregate<GiftCertificatesPaymentAction>();
                action.Aggregate<GetTaxAction>();
            });

            setup.Action<CountriesSetUpAction>("Countries");

            setup.Action<GetTaxAction>("TaxTotal", action =>
            {
                action.Dependency<CountriesSetUpAction>();
                action.Dependency<OrderSubTotalAction>();
            });

            setup.Action<OrderSubTotalAction>("SubTotal", action =>
            {
                action.Aggregate<ProductsWithPromoAction>();
                action.Aggregate<DiscountTypeActionResolver>();
                action.Aggregate<ShippingStandardResolver>();
                action.Aggregate<ShippingSurchargeResolver>();
                action.Aggregate<ShippingUpgradesActionResolver>();
                action.Aggregate<ShippingOverrideAction>();
                action.Aggregate<ShippingSurchargeOverrideAction>();
            });

            setup.Action<GiftCertificatesPaymentAction>("GiftCertificates", action =>
            {
                action.Dependency<OrderSubTotalAction>();
            });

            setup.Action<ProductAction>("Products");

            setup.Action<ProductsWithPromoAction>("PromoProducts", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();

                action.Aggregate<ProductAction>();
            });

            setup.Action<DiscountPercentAction>("PercentDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            setup.Action<DiscountPriceAction>("PriceDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            setup.Action<DiscountableProductsAction>("DiscountableSubtotal", action =>
            {
                action.Dependency<ProductAction>();
            });

            setup.Action<DiscountTieredAction>("TieredDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            setup.Action<DiscountFreeShippingAction>("FreeShippingDiscount");

            setup.Action<DiscountThresholdAction>("ThresholdDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

            setup.Action<PerishableProductsAction>("PerishableSubtotal", action =>
            {
                action.Dependency<ProductAction>();
            });

            setup.Action<DeliveredProductsAction>("DeliveredAmount", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
            });

            setup.Action<StandardShippingUsWholesaleAction>("StandardWholesaleShipping", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();
                action.Dependency<DeliveredProductsAction>();
            });

            setup.Action<StandardShippingUsCaRetailAction>("StandardRetailShipping", action =>
            {
                action.Dependency<DiscountTypeActionResolver>();
                action.Dependency<DeliveredProductsAction>();
            });

            setup.Action<ShippingSurchargeUsAkHiAction>("ShippingSurchargeUs", action =>
            {
                action.Dependency<DeliveredProductsAction>();
            });

            setup.Action<ShippingSurchargeCaAction>("ShippingSurchargeCa", action =>
            {
                action.Dependency<DeliveredProductsAction>();
            });

            setup.Action<ProductsSplitAction>("SplitAction", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
            });

            setup.Action<ShippingUpgradesUsCaAction>("ShippingUpgradeUsCa", action =>
            {
                action.Dependency<ProductsSplitAction>();
            });

            setup.ActionResolver<ShippingUpgradesActionResolver>("ShippingUpgrade", action =>
            {
                action.Dependency<CountriesSetUpAction>();
                action.ResolvePath<ShippingUpgradesUsCaAction>((int) ShippingUpgradeGroup.UsCa, "ShippingUpgradeUsCa");
            });

            setup.ActionResolver<ShippingSurchargeResolver>("ShippingSurcharge", action =>
            {
                action.Dependency<CountriesSetUpAction>();
                action.ResolvePath<ShippingSurchargeUsAkHiAction>((int) SurchargeType.AlaskaHawaii,
                    "ShippingSurchargeUs");
                action.ResolvePath<ShippingSurchargeCaAction>((int) SurchargeType.Canada,
                    "ShippingSurchargeCa");
            });

            setup.ActionResolver<DiscountTypeActionResolver>("Discount", action =>
            {
                action.Dependency<PerishableProductsAction>();

                action.ResolvePath<DiscountPercentAction>((int) DiscountType.PercentDiscount, "PercentDiscount");
                action.ResolvePath<DiscountPriceAction>((int) DiscountType.PriceDiscount, "PriceDiscount");
                action.ResolvePath<DiscountTieredAction>((int) DiscountType.Tiered, "TieredDiscount");
                action.ResolvePath<DiscountFreeShippingAction>((int) DiscountType.FreeShipping, "FreeShippingDiscount");
                action.ResolvePath<DiscountThresholdAction>((int) DiscountType.Threshold, "ThresholdDiscount");
            });

            setup.ActionResolver<ShippingStandardResolver>("StandardShipping", action =>
            {
                action.Dependency<CountriesSetUpAction>();
                action.ResolvePath<StandardShippingUsWholesaleAction>((int) CustomerType.Wholesale, "StandardWholesaleShipping");
                action.ResolvePath<StandardShippingUsCaRetailAction>((int) CustomerType.Retail, "StandardRetailShipping");
            });

            setup.Action<ShippingOverrideAction>("ShippingOverride", action =>
            {
                action.Dependency<ShippingStandardResolver>();
                action.Dependency<ShippingUpgradesActionResolver>();
            });

            setup.Action<ShippingSurchargeOverrideAction>("SurchargeOverride", action =>
            {
                action.Dependency<ShippingSurchargeResolver>();
            });

            setup.Tree<OrderTree>("Order", tree =>
            {
                tree.Action<TotalAction>();
            });
        }
    }
}