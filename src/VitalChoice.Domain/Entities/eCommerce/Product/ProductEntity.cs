﻿using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Product
{
    public class ProductEntity : DynamicDataEntity<ProductOptionValue, ProductOptionType>
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool Hidden { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public ProductType IdProductType { get; set; }

        public int? IdExternal { get; set; }

        public ICollection<Sku> Skus { get; set; }

        public ICollection<ProductToCategory> ProductsToCategories { get; set; }
    }
}