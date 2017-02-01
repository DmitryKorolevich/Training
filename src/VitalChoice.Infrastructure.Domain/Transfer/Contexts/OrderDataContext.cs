using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public abstract class BaseOrderContext<T> : ComputableDataContext
        where T : ItemOrdered
    {
        public abstract SplitInfoBase<T> BaseSplitInfo { get; }

        public abstract IEnumerable<T> ItemsOrdered { get; }

        public virtual OrderDynamic Order { get; set; }

        public virtual decimal DiscountTotal { get; set; }

        public virtual decimal ShippingTotal { get; set; }
    }

    public class OrderDataContext : BaseOrderContext<SkuOrdered>
    {
        public OrderStatus CombinedStatus { get; }
        public List<PromotionDynamic> Promotions { get; set; }

        public OrderDataContext(OrderStatus combinedStatus)
        {
            CombinedStatus = combinedStatus;
            Messages = new List<MessageInfo>();
            PromoSkus = new List<PromoOrdered>();
            SkuOrdereds = new List<SkuOrdered>();
            GcMessageInfos = new List<MessageInfo>();
            SplitInfo = new SplitInfo<SkuOrdered>(() => SkuOrdereds.Concat(PromoSkus));
            FraudReason = new List<ReviewReason>();
        }

        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public decimal? FreeShippingThresholdDifference { get; set; }

        public decimal? FreeShippingThresholdAmount { get; set; }

        public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

        public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNpOptions { get; set; }

        public ICollection<MessageInfo> GcMessageInfos { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public decimal GiftCertificatesSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public decimal ShippingOverride { get; set; }
        
        public decimal SurchargeOverride { get; set; }

        public bool FreeShipping { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public ICollection<SkuOrdered> SkuOrdereds { get; set; }

        public ICollection<PromoOrdered> PromoSkus { get; set; }

        public ICollection<MessageInfo> Messages { get; set; }

        public SplitInfo<SkuOrdered> SplitInfo { get; set; }

        public override IEnumerable<SkuOrdered> ItemsOrdered => SkuOrdereds.Concat(PromoSkus.Where(p => p.Enabled));

        public override SplitInfoBase<SkuOrdered> BaseSplitInfo => SplitInfo;

        public DeliveryServiceCostGroup ShippingCostGroup { get; set; }

        public decimal StandardShippingOverriden => StandardShippingCharges + Data.ShippingUpgrade + Data.ShippingOverride;

        public decimal SurchargeShippingOverriden => CanadaSurcharge + AlaskaHawaiiSurcharge + Data.SurchargeOverride;

        public bool CheckForFraud { get; set; }

        public bool IsFraud { get; set; }

        public List<ReviewReason> FraudReason { get; set; }
    }
}