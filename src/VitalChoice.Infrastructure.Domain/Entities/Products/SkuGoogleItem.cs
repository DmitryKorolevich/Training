using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Products
{
    public class SkuGoogleItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public decimal RetailPrice { get; set; }

        public string Description { get; set; }

        public string Condition { get; set; }

        public string Brand { get; set; }

        public string SkuCode { get; set; }

        public string Thumbnail { get; set; }

        public string GoogleCategory { get; set; }

        public string ProductRootCategory { get; set; }

        public string Availability { get; set; }

        public string SkuCodeGroup { get; set; }

        public string MainProductImage { get; set; }

        public int? Quantity { get; set; }

        public string Manufacturer { get; set; }

        public string Seller { get; set; }
    }
}
