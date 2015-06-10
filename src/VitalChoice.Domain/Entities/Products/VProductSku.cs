﻿using System;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.Products
{
    public class VProductSku: Entity
    {
        public int? SkuId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public decimal? Price { get; set; }

        public decimal? WholesalePrice { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int IdProduct { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public string Url { get; set; }

        public string Thumbnail { get; set; }
    }
}