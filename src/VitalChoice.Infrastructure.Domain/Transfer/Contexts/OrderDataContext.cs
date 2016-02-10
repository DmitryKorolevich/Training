using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public class SplitInfo
    {
        public int PerishableCount { get; set; }

        public decimal PerishableAmount { get; set; }

        public int NonPerishableCount { get; set; }

        public decimal NonPerishableAmount { get; set; }

        public int NonPerishableOrphanCount { get; set; }

        public int NonPerishableNonOrphanCount => NonPerishableCount - NonPerishableOrphanCount;

        public decimal PNpAmount => PerishableAmount + NonPerishableAmount;

        public bool SpecialSkuAdded { get; set; }

        public bool ThresholdReached { get; set; }

        public bool ShouldSplit { get; set; }

        public POrderType ProductTypes { get; set; }
    }

    public class OrderDataContext : ComputableDataContext
    {
        public List<PromotionDynamic> Promotions { get; set; }

        public OrderDataContext()
        {
            Messages = new List<MessageInfo>();
            PromoSkus = new List<PromoOrdered>();
            GcMessageInfos = new List<MessageInfo>();
            SplitInfo = new SplitInfo();
        }

        public OrderDynamic Order { get; set; }

        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public decimal? FreeShippingThresholdDifference { get; set; }

        public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

        public ICollection<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNpOptions { get; set; }

        public ICollection<MessageInfo> GcMessageInfos { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

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

        public SplitInfo SplitInfo { get; set; }

        public bool AllowHealthWise { get; set; }
    }
}