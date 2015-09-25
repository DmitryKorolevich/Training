using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Contexts
{
    public class SplitInfo
    {
        public int PerishableCount { get; set; }

        public int NonPerishableCount { get; set; }

        public int NonPerishableOrphanCount { get; set; }

        public int NonPerishableNonOrphanCount => NonPerishableCount - NonPerishableOrphanCount;

        public bool SpecialSkuAdded { get; set; }

        public bool ThresholdReached { get; set; }

        public bool ShouldSplit { get; set; }
    }

    public class OrderContext : ComputableContext
    {
        public Dictionary<string, int> Coutries { get; }

        public Dictionary<string, Dictionary<string, int>> States { get; }

        public OrderContext(ICollection<Country> coutries)
        {
            Coutries = coutries.ToDictionary(c => c.CountryCode, c => c.Id, StringComparer.OrdinalIgnoreCase);
            States = coutries.ToDictionary(c => c.CountryCode,
                c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
            Messages = new List<MessageInfo>();
            PromoSkus = new List<SkuOrdered>();
            SplitInfo = new SplitInfo();
        }

        public OrderDynamic Order { get; set; }

        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public IList<LookupItem<int>> ShippingUpgradePOptions { get; set; }

        public IList<LookupItem<int>> ShippingUpgradeNpOptions { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool FreeShipping { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public IList<SkuOrdered> SkuOrdereds { get; set; }

        public IList<SkuOrdered> PromoSkus { get; set; }

        public IList<MessageInfo> Messages { get; set; }

        public SplitInfo SplitInfo { get; set; }
    }
}