using System.Collections.Generic;
using Autofac;
using VitalChoice.Business.Workflow.Orders;
using VitalChoice.Business.Workflow.Orders.ActionResolvers;
using VitalChoice.Business.Workflow.Orders.Actions;
using VitalChoice.Business.Workflow.Orders.Actions.Discounts;
using VitalChoice.Business.Workflow.Orders.Actions.GiftCertificates;
using VitalChoice.Business.Workflow.Orders.Actions.Products;
using VitalChoice.Business.Workflow.Orders.Actions.Promo;
using VitalChoice.Business.Workflow.Orders.Actions.Shipping;
using VitalChoice.Business.Workflow.Refunds;
using VitalChoice.Business.Workflow.Refunds.ActionResolvers;
using VitalChoice.Business.Workflow.Refunds.Actions;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Core;
using ReductionType = VitalChoice.Business.Workflow.Orders.ActionResolvers.ReductionType;
using VitalChoice.Business.Workflow.Refunds.Actions.Discounts;
using VitalChoice.Business.Workflow.Refunds.Actions.Products;
using VitalChoice.Business.Workflow.Reships;
using VitalChoice.Business.Workflow.Reships.Actions;

namespace VitalChoice.Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static IEnumerable<ITreeSetup> Configure(ILifetimeScope scope)
        {
            var orderContextTreeSetup = scope.Resolve<ITreeSetup<OrderDataContext, decimal>>();
            var refundContextTreeSetup = scope.Resolve<ITreeSetup<OrderRefundDataContext, decimal>>();

            #region Normal Order

            orderContextTreeSetup.Tree<OrderTree>("Order", order =>
            {
                order.Action<TotalAction>("Total", action =>
                {
                    action.Dependency<GiftCertificatesBuyAction>();
                    action.Dependency<AffiliateCommissionAction>();

                    action.Aggregate<PayableTotalAction>();
                    action.Aggregate<GiftCertificatesPaymentAction>();
                });

                order.Action<AffiliateCommissionAction>("AffiliateCommission", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                    action.Dependency<ReductionTypeActionResolver>();
                });

                order.Action<PayableTotalAction>("PayableTotal", action =>
                {
                    action.Dependency<PerishableProductsAction>();

                    action.Aggregate<GetTaxAction>();
                    action.Aggregate<OrderSubTotalAction>();
                });

                order.Action<GetTaxAction>("TaxTotal", action =>
                {
                    action.Dependency<OrderSubTotalAction>();
                    action.Dependency<ProductsSplitAction>();
                });

                order.Action<OrderSubTotalAction>("SubTotal", action =>
                {
                    action.Aggregate<ProductsWithPromoAction>();
                    action.Aggregate<ReductionTypeActionResolver>();
                    action.Aggregate<ShippingStandardResolver>();
                    action.Aggregate<ShippingSurchargeResolver>();
                    action.Aggregate<ShippingUpgradesActionResolver>();
                    action.Aggregate<ShippingOverrideAction>();
                    action.Aggregate<ShippingSurchargeOverrideAction>();
                });

                order.Action<GiftCertificatesBuyAction>("BuyGiftCertificates", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                });

                order.Action<SetupPromoAction>("PromoSetup");
                order.Action<CategoryPromoAction>("CategoryPromotions", action =>
                {
                    action.Dependency<HealthwiseSetupAction>();
                    action.Dependency<SetupPromoAction>();
                });
                order.Action<BuyXGetYPromoAction>("PromoBuyXGetY", action =>
                {
                    action.Dependency<HealthwiseSetupAction>();
                    action.Dependency<SetupPromoAction>();
                });

                order.Action<GiftCertificatesPaymentAction>("GiftCertificates", action =>
                {
                    action.Dependency<PayableTotalAction>();
                });

                order.Action<ProductAction>("Products", action =>
                {
                    action.Dependency<CategoryPromoAction>();
                });

                order.Action<ProductsWithPromoAction>("PromoProducts", action =>
                {
                    action.Dependency<ReductionTypeActionResolver>();
                    action.Dependency<BuyXGetYPromoAction>();
                    action.Aggregate<ProductAction>();
                });

                order.Action<DiscountPercentAction>("PercentDiscount", action =>
                {
                    action.Dependency<DiscountableProductsAction>();
                });

                order.Action<DiscountPriceAction>("PriceDiscount", action =>
                {
                    action.Dependency<DiscountableProductsAction>();
                });

                order.Action<DiscountableProductsAction>("DiscountableSubtotal", action =>
                {
                    action.Dependency<ProductAction>();
                });

                order.Action<DiscountTieredAction>("TieredDiscount", action =>
                {
                    action.Dependency<DiscountableProductsAction>();
                });

                order.Action<DiscountFreeShippingAction>("FreeShippingDiscount");

                order.Action<DiscountThresholdAction>("ThresholdDiscount", action =>
                {
                    action.Dependency<DiscountableProductsAction>();
                });

                order.Action<PerishableProductsAction>("PerishableSubtotal", action =>
                {
                    action.Dependency<ProductAction>();
                });

                order.Action<DeliveredProductsAction>("DeliveredAmount", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                });

                order.Action<StandardShippingUsWholesaleAction>("StandardWholesaleShipping", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                    action.Dependency<DeliveredProductsAction>();
                });

                order.Action<StandardShippingUsCaRetailAction>("StandardRetailShipping", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                    action.Dependency<DeliveredProductsAction>();
                });

                order.Action<ShippingSurchargeUsAkHiAction>("ShippingSurchargeUs", action =>
                {
                    action.Dependency<DeliveredProductsAction>();
                });

                order.Action<ShippingSurchargeCaAction>("ShippingSurchargeCa", action =>
                {
                    action.Dependency<DeliveredProductsAction>();
                });

                order.Action<ProductsSplitAction>("SplitAction", action =>
                {
                    action.Dependency<ProductsWithPromoAction>();
                });

                order.Action<ShippingUpgradesUsCaAction>("ShippingUpgradeUsCa");

                order.ActionResolver<ShippingUpgradesActionResolver>("ShippingUpgrade", action =>
                {
                    action.Dependency<ProductsSplitAction>();

                    action.ResolvePath<ShippingUpgradesUsCaAction>((int) ShippingUpgradeGroup.UsCa, "ShippingUpgradeUsCa");
                });

                order.ActionResolver<ShippingSurchargeResolver>("ShippingSurcharge", action =>
                {
                    action.ResolvePath<ShippingSurchargeUsAkHiAction>((int) SurchargeType.AlaskaHawaii,
                        "ShippingSurchargeUs");
                    action.ResolvePath<ShippingSurchargeCaAction>((int) SurchargeType.Canada,
                        "ShippingSurchargeCa");
                });

                order.ActionResolver<ReductionTypeActionResolver>("Discount", action =>
                {
                    action.Dependency<HealthwiseSetupAction>();

                    action.ResolvePath<AutoShipDiscountAction>((int) ReductionType.AutoShip, "AutoShipDiscount");
                    action.ResolvePath<DiscountTypeActionResolver>((int) ReductionType.Discount, "NormalDiscount");
                });

                order.Action<HealthwiseSetupAction>("HealthwiseSetup");

                order.Action<AutoShipDiscountAction>("AutoShipDiscount", action =>
                {
                    action.Dependency<DiscountableProductsAction>();
                });

                order.ActionResolver<DiscountTypeActionResolver>("NormalDiscount", action =>
                {
                    action.Dependency<PerishableProductsAction>();

                    action.ResolvePath<DiscountPercentAction>((int) DiscountType.PercentDiscount, "PercentDiscount");
                    action.ResolvePath<DiscountPriceAction>((int) DiscountType.PriceDiscount, "PriceDiscount");
                    action.ResolvePath<DiscountTieredAction>((int) DiscountType.Tiered, "TieredDiscount");
                    action.ResolvePath<DiscountFreeShippingAction>((int) DiscountType.FreeShipping, "FreeShippingDiscount");
                    action.ResolvePath<DiscountThresholdAction>((int) DiscountType.Threshold, "ThresholdDiscount");
                });

                order.ActionResolver<ShippingStandardResolver>("StandardShipping", action =>
                {
                    action.Dependency<ReductionTypeActionResolver>();
                    action.ResolvePath<StandardShippingUsWholesaleAction>((int) CustomerType.Wholesale, "StandardWholesaleShipping");
                    action.ResolvePath<StandardShippingUsCaRetailAction>((int) CustomerType.Retail, "StandardRetailShipping");
                });

                order.Action<ShippingOverrideAction>("ShippingOverride", action =>
                {
                    action.Dependency<ShippingStandardResolver>();
                    action.Dependency<ShippingUpgradesActionResolver>();
                });

                order.Action<ShippingSurchargeOverrideAction>("SurchargeOverride", action =>
                {
                    action.Dependency<ShippingSurchargeResolver>();
                });
            });

            #endregion

            #region Refund

            refundContextTreeSetup.Tree<RefundTree>("Refund", refund =>
            {
                refund.Action<RefundTotal>("Total", action =>
                {
                    action.Aggregate<RefundSubtotal>();
                    action.Aggregate<RefundGiftCertificatesAction>();
                    action.Aggregate<ManualOverrideAction>();
                });

                refund.Action<RefundGiftCertificatesAction>("GiftCertificates", action =>
                {
                    action.Dependency<RefundSubtotal>();
                });

                refund.Action<RefundSubtotal>("RefundSubtotal", action =>
                {
                    action.Aggregate<RefundReductionTypeActionResolver>();
                    action.Aggregate<RefundedProductsAction>();
                    action.Aggregate<RefundGetTaxAction>();
                    action.Aggregate<RefundShippingAction>();
                });

                refund.ActionResolver<RefundReductionTypeActionResolver>("Discount", action =>
                {
                    action.ResolvePath<RefundDiscountTypeActionResolver>((int) ReductionType.Discount, "NormalDiscount");
                    action.ResolvePath<RefundAutoShipDiscountAction>((int) ReductionType.AutoShip, "AutoshipDiscount");
                });

                refund.Action<ManualOverrideAction>("ManualOverride");

                refund.Action<RefundedProductsAction>("RefundProducts");

                refund.Action<RefundGetTaxAction>("TaxTotal", action =>
                {
                    action.Dependency<RefundedProductsAction>();
                    action.Dependency<RefundReductionTypeActionResolver>();
                    action.Dependency<RefundShippingAction>();
                });

                refund.Action<RefundShippingAction>("ShippingTotal");

                refund.ActionResolver<RefundDiscountTypeActionResolver>("NormalDiscount", action =>
                {
                    action.ResolvePath<RefundDiscountPercentAction>((int) DiscountType.PercentDiscount, "PercentDiscount");
                    action.ResolvePath<RefundDiscountPriceAction>((int) DiscountType.PriceDiscount, "PriceDiscount");
                    action.ResolvePath<RefundDiscountTieredAction>((int) DiscountType.Tiered, "TieredDiscount");
                });

                refund.Action<RefundDiscountPercentAction>("PercentDiscount", action =>
                {
                    action.Dependency<RefundDiscountableProductsAction>();
                });
                refund.Action<RefundDiscountPriceAction>("PriceDiscount", action =>
                {
                    action.Dependency<RefundDiscountableProductsAction>();
                });
                refund.Action<RefundDiscountTieredAction>("TieredDiscount", action =>
                {
                    action.Dependency<RefundDiscountableProductsAction>();
                });

                refund.Action<RefundDiscountableProductsAction>("RefundDiscountableSubtotal", action =>
                {
                    action.Dependency<RefundedProductsAction>();
                });

                refund.Action<RefundAutoShipDiscountAction>("AutoshipDiscount", action =>
                {
                    action.Dependency<RefundDiscountableProductsAction>();
                });
            });

            #endregion

            #region Reship

            orderContextTreeSetup.Tree<ReshipTree>("Reship", reship =>
            {
                reship.Action<TotalAction>("Total", action =>
                {
                    action.Aggregate<PayableTotalAction>();
                });

                reship.Action<DummyPromoProductsResultAction>("PromoProducts", action =>
                {
                    action.Aggregate<ProductAction>();
                });

                reship.Action<PayableTotalAction>("PayableTotal", action =>
                {
                    action.Dependency<PerishableProductsAction>();

                    action.Aggregate<GetTaxAction>();
                    action.Aggregate<OrderSubTotalAction>();
                });

                reship.Action<GetTaxAction>("TaxTotal", action =>
                {
                    action.Dependency<OrderSubTotalAction>();
                    action.Dependency<ProductsSplitAction>();
                });

                reship.Action<OrderSubTotalAction>("SubTotal", action =>
                {
                    action.Aggregate<DummyPromoProductsResultAction>();
                    action.Aggregate<ShippingStandardResolver>();
                    action.Aggregate<ShippingSurchargeResolver>();
                    action.Aggregate<ShippingUpgradesActionResolver>();
                    action.Aggregate<ShippingOverrideAction>();
                    action.Aggregate<ShippingSurchargeOverrideAction>();
                });

                reship.Action<ProductAction>("Products");

                reship.Action<PerishableProductsAction>("PerishableSubtotal", action =>
                {
                    action.Dependency<DummyPromoProductsResultAction>();
                });

                reship.Action<DeliveredProductsAction>("DeliveredAmount", action =>
                {
                    action.Dependency<DummyPromoProductsResultAction>();
                });

                reship.Action<StandardShippingUsWholesaleAction>("StandardWholesaleShipping", action =>
                {
                    action.Dependency<DummyPromoProductsResultAction>();
                    action.Dependency<DeliveredProductsAction>();
                });

                reship.Action<StandardShippingUsCaRetailAction>("StandardRetailShipping", action =>
                {
                    action.Dependency<DummyPromoProductsResultAction>();
                    action.Dependency<DeliveredProductsAction>();
                });

                reship.Action<ShippingSurchargeUsAkHiAction>("ShippingSurchargeUs", action =>
                {
                    action.Dependency<DeliveredProductsAction>();
                });

                reship.Action<ShippingSurchargeCaAction>("ShippingSurchargeCa", action =>
                {
                    action.Dependency<DeliveredProductsAction>();
                });

                reship.Action<ProductsSplitAction>("SplitAction", action =>
                {
                    action.Dependency<DummyPromoProductsResultAction>();
                });

                reship.Action<ShippingUpgradesUsCaAction>("ShippingUpgradeUsCa", action =>
                {
                });

                reship.ActionResolver<ShippingUpgradesActionResolver>("ShippingUpgrade", action =>
                {
                    action.Dependency<ProductsSplitAction>();

                    action.ResolvePath<ShippingUpgradesUsCaAction>((int) ShippingUpgradeGroup.UsCa, "ShippingUpgradeUsCa");
                });

                reship.ActionResolver<ShippingSurchargeResolver>("ShippingSurcharge", action =>
                {
                    action.ResolvePath<ShippingSurchargeUsAkHiAction>((int) SurchargeType.AlaskaHawaii,
                        "ShippingSurchargeUs");
                    action.ResolvePath<ShippingSurchargeCaAction>((int) SurchargeType.Canada,
                        "ShippingSurchargeCa");
                });

                reship.ActionResolver<ShippingStandardResolver>("StandardShipping", action =>
                {
                    action.ResolvePath<StandardShippingUsWholesaleAction>((int) CustomerType.Wholesale, "StandardWholesaleShipping");
                    action.ResolvePath<StandardShippingUsCaRetailAction>((int) CustomerType.Retail, "StandardRetailShipping");
                });

                reship.Action<ShippingOverrideAction>("ShippingOverride", action =>
                {
                    action.Dependency<ShippingStandardResolver>();
                    action.Dependency<ShippingUpgradesActionResolver>();
                });

                reship.Action<ShippingSurchargeOverrideAction>("SurchargeOverride", action =>
                {
                    action.Dependency<ShippingSurchargeResolver>();
                });
            });

            #endregion

            yield return orderContextTreeSetup;
            yield return refundContextTreeSetup;
        }
    }
}