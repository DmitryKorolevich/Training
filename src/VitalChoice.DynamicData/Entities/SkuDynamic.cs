using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class SkuDynamic : MappedObject
    {
        public string Code { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public int Order { get; set; }

        public int IdProduct { get; set; }
    }
}