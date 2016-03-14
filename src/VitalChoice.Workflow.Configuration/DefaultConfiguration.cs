using VitalChoice.Business.Workflow.ActionResolvers;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Business.Workflow.Actions.Discounts;
using VitalChoice.Business.Workflow.Actions.GiftCertificates;
using VitalChoice.Business.Workflow.Actions.Products;
using VitalChoice.Business.Workflow.Actions.Promo;
using VitalChoice.Business.Workflow.Actions.Shipping;
using VitalChoice.Business.Workflow.Actions.Tax;
using VitalChoice.Business.Workflow.Trees;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static void Configure(ITreeSetup<OrderDataContext, decimal> setup)
        {
            setup.Action<TotalAction>("Total", action =>
            {
                action.Dependency<GiftCertificatesBuyAction>();
                action.Dependency<PayableTotalAction>();

                action.Aggregate<PayableTotalAction>();
                action.Aggregate<GiftCertificatesPaymentAction>();
            });

            setup.Action<PayableTotalAction>("PayableTotal", action =>
            {
                action.Dependency<PerishableProductsAction>();

                action.Aggregate<GetTaxAction>();
                action.Aggregate<OrderSubTotalAction>();
            });

            setup.Action<GetTaxAction>("TaxTotal", action =>
            {
                action.Dependency<OrderSubTotalAction>();
            });

            setup.Action<OrderSubTotalAction>("SubTotal", action =>
            {
                action.Aggregate<ProductsWithPromoAction>();
                action.Aggregate<ReductionTypeActionResolver>();
                action.Aggregate<ShippingStandardResolver>();
                action.Aggregate<ShippingSurchargeResolver>();
                action.Aggregate<ShippingUpgradesActionResolver>();
                action.Aggregate<ShippingOverrideAction>();
                action.Aggregate<ShippingSurchargeOverrideAction>();
            });

            setup.Action<GiftCertificatesBuyAction>("BuyGiftCertificates", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
            });

            setup.Action<SetupPromoAction>("PromoSetup");
            setup.Action<CategoryPromoAction>("CategoryPromotions", action =>
            {
                action.Dependency<SetupPromoAction>();
            });
            setup.Action<BuyXGetYPromoAction>("PromoBuyXGetY", action =>
            {
                action.Dependency<SetupPromoAction>();
            });

            setup.Action<GiftCertificatesPaymentAction>("GiftCertificates", action =>
            {
                action.Dependency<OrderSubTotalAction>();
            });

            setup.Action<ProductAction>("Products", action =>
            {
                action.Dependency<CategoryPromoAction>();
            });

            setup.Action<ProductsWithPromoAction>("PromoProducts", action =>
            {
                action.Dependency<ReductionTypeActionResolver>();
                action.Dependency<BuyXGetYPromoAction>();
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
                action.Dependency<ProductsWithPromoAction>();
                action.Dependency<DeliveredProductsAction>();
            });

            setup.Action<StandardShippingUsCaRetailAction>("StandardRetailShipping", action =>
            {
                action.Dependency<ProductsWithPromoAction>();
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
            });

            setup.ActionResolver<ShippingUpgradesActionResolver>("ShippingUpgrade", action =>
            {
                action.Dependency<ProductsSplitAction>();

                action.ResolvePath<ShippingUpgradesUsCaAction>((int) ShippingUpgradeGroup.UsCa, "ShippingUpgradeUsCa");
            });

            setup.ActionResolver<ShippingSurchargeResolver>("ShippingSurcharge", action =>
            {
                action.ResolvePath<ShippingSurchargeUsAkHiAction>((int) SurchargeType.AlaskaHawaii,
                    "ShippingSurchargeUs");
                action.ResolvePath<ShippingSurchargeCaAction>((int) SurchargeType.Canada,
                    "ShippingSurchargeCa");
            });

			setup.ActionResolver<ReductionTypeActionResolver>("Reduction", action =>
			{
				action.ResolvePath<AutoShipAction>((int)ReductionType.AutoShip, "AutoShip");
				action.ResolvePath<DiscountTypeActionResolver>((int)ReductionType.Discount, "Discount");
			});

			setup.Action<AutoShipAction>("AutoShip", action =>
			{
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
                action.Dependency<ReductionTypeActionResolver>();
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