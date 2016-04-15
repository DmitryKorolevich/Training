using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class VSku : Entity
    {
        public int SkuId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string SubTitle { get; set; }

        public decimal? Price { get; set; }

        public decimal? WholesalePrice { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public RecordStatusCode? SkuStatusCode { get; set; }

        public int IdProduct { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public bool Hidden { get; set; }

        public bool ProductHidden { get; set; }

        public ProductType IdProductType { get; set; }

        public string DescriptionName { get; set; }

        public bool AutoShipProduct { get; set; }

        public bool AutoShipFrequency1 { get; set; }

        public bool AutoShipFrequency2 { get; set; }

        public bool AutoShipFrequency3 { get; set; }

        public bool AutoShipFrequency6 { get; set; }

        public bool DisregardStock { get; set; }

        public int Stock { get; set; }
    }
}