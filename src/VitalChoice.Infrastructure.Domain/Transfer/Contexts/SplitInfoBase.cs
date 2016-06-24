using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public class SplitInfoBase<T>
        where T : ItemOrdered
    {
        protected readonly Func<IEnumerable<T>> GetSkus;

        public SplitInfoBase(Func<IEnumerable<T>> getSkus)
        {
            GetSkus = getSkus;
        }

        public virtual IEnumerable<T> GetPerishablePartProducts()
        {
            return GetSkus().Where(p => p.Sku.IdObjectType == (int)ProductType.Perishable);
        }

        public virtual IEnumerable<T> GetNonPerishablePartProducts()
        {
            return GetSkus().Where(p => p.Sku.IdObjectType != (int)ProductType.Perishable);
        }

        public decimal PerishableShippingOveridden { get; set; }

        public decimal NonPerishableShippingOverriden { get; set; }

        public decimal PerishableSurchargeOverriden { get; set; }

        public decimal NonPerishableSurchargeOverriden { get; set; }

        public decimal PerishableTax { get; set; }

        public decimal NonPerishableTax { get; set; }

        public decimal PerishableDiscount { get; set; }

        public decimal NonPerishableDiscount { get; set; }

        public decimal DiscountablePerishable { get; set; }

        public decimal DiscountableNonPerishable { get; set; }

        public bool ShouldSplit { get; set; }
    }
}