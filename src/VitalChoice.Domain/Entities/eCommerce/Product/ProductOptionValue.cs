using System;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class ProductOptionValue : OptionValue
    {
		public int ProductId { get; set; }

		public Product Product { get; set; }

	    public int SkuId { get; set; }

	    public Sku Sku { get; set; }
    }
}