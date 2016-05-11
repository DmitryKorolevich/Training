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

namespace VitalChoice.Workflow.Configuration
{
    public static class DefaultConfiguration
    {
        public static IEnumerable<ITreeSetup> Configure(ILifetimeScope scope)
        {
            var order = scope.Resolve<ITreeSetup<OrderDataContext, decimal>>();
            var refund = scope.Resolve<ITreeSetup<OrderRefundDataContext, decimal>>();

            #region Normal Order

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
                action.Dependency<SetupPromoAction>();
            });
            order.Action<BuyXGetYPromoAction>("PromoBuyXGetY", action =>
            {
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

            order.Action<ShippingUpgradesUsCaAction>("ShippingUpgradeUsCa", action =>
            {
            });

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
                action.ResolvePath<HealthwiseDiscountAction>((int)ReductionType.HealthWise, "HealthwiseDiscount");
                action.ResolvePath<AutoShipDiscountAction>((int) ReductionType.AutoShip, "AutoShipDiscount");
                action.ResolvePath<DiscountTypeActionResolver>((int) ReductionType.Discount, "NormalDiscount");
            });

            order.Action<HealthwiseDiscountAction>("HealthwiseDiscount", action =>
            {
                action.Dependency<DiscountableProductsAction>();
            });

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

            order.Tree<OrderTree>("Order", startup =>
            {
                startup.Action<TotalAction>();
            });

            #endregion

            yield return order;

            #region Refund

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

            refund.Tree<RefundTree>("Refund", startup =>
            {
                startup.Action<RefundTotal>();
            });

            #endregion

            yield return refund;
        }
    }
}