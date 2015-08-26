using System.Collections.Generic;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Contexts
{
    public class OrderContext: ComputableContext {

        public OrderDynamic Order { get; set; }

        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public IList<LookupItem<int>> ShippingUpgradePOptions { get; set; }

        public IList<LookupItem<int>> ShippingUpgradeNPOptions { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public IList<SkuOrdered> SkuOrdereds { get; set; }

        public IList<MessageInfo> Messages { get; set; }
    }
}