﻿using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class ProductOptionValue : OptionValue<ProductOptionType>
    {
        public int IdProduct { get; set; }
    }
}