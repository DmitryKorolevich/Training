using System;

namespace VitalChoice.Domain.Entities.eCommerce.Products
{
    public class VSku : Entity
    {
        public int SkuId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public decimal? Price { get; set; }

        public decimal? WholesalePrice { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public RecordStatusCode? SkuStatusCode { get; set; }

        public int IdProduct { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public bool Hidden { get; set; }

        public ProductType IdProductType { get; set; }

        public string Url { get; set; }

        public string DescriptionName { get; set; }
    }
}