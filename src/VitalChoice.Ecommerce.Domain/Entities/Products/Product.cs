using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class Product : DynamicDataEntity<ProductOptionValue, ProductOptionType>
    {
	    public Guid PublicId { get; set; }

	    public string Name { get; set; }
        
        public CustomerTypeCode? IdVisibility { get; set; }

        public ICollection<Sku> Skus { get; set; }

        public ICollection<ProductToCategory> ProductsToCategories { get; set; }
    }
}