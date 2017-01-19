using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public abstract class ItemOrdered
    {
        public virtual SkuDynamic Sku { get; set; }

        public virtual int Quantity { get; set; }

        public virtual decimal Amount { get; set; }
    }

    public class SplitInfo<T> : SplitInfoBase<T>
        where T:ItemOrdered
    {
        public SplitInfo(Func<IEnumerable<T>> getSkus): base(getSkus)
        {
        }

        public decimal PerishableTotal =>
            PerishableSubtotal - PerishableGiftCertificateAmount;

        public decimal NonPerishableTotal =>
            NonPerishableSubtotal - NonPerishableGiftCertificateAmount;

        public decimal PerishableSubtotal
            => PerishableShippingOveridden + PerishableSurchargeOverriden + PerishableTax - PerishableDiscount + PerishableAmount;

        public decimal NonPerishableSubtotal
            =>
                NonPerishableShippingOverriden + NonPerishableSurchargeOverriden + NonPerishableTax - NonPerishableDiscount +
                NonPerishableAmount +
                OtherProductsAmount;

        public int PerishableCount { get; set; }

        public decimal PerishableAmount { get; set; }

        public int NonPerishableCount { get; set; }

        public decimal NonPerishableAmount { get; set; }

        public int NonPerishableOrphanCount { get; set; }

        public decimal PerishableGiftCertificateAmount { get; set; }

        public decimal NonPerishableGiftCertificateAmount { get; set; }

        public int NonPerishableNonOrphanCount => NonPerishableCount - NonPerishableOrphanCount;

        public decimal PNpAmount => PerishableAmount + NonPerishableAmount;

        public decimal OtherProductsAmount { get; set; }

        public bool SpecialSkuAdded { get; set; }

        public bool ThresholdReached { get; set; }

        public POrderType ProductTypes { get; set; }
    }
}