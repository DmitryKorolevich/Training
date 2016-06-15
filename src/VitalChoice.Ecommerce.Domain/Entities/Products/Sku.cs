using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class Sku : DynamicDataEntity<SkuOptionValue, SkuOptionType>
    {
        public int IdProduct { get; set; }

        public Product Product { get; set; }

        private int _idObjectType;

        public override int IdObjectType
        {
            get { return Product?.IdObjectType ?? _idObjectType; }
            set { _idObjectType = value; }
        }

        public string Code { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Order { get; set; }

        public ICollection<SkuToInventorySku> SkusToInventorySkus { get; set; }
    }
}